using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer;
using EPiServer.Commerce.Order;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Data.Provider;
using StefanOlsen.Commerce.Payment.Coinify.Api;
using StefanOlsen.Commerce.Payment.Coinify.Api.Models;

namespace StefanOlsen.Commerce.Payment.Coinify
{
    public class CoinifyPaymentGateway : IPaymentPlugin
    {
        private readonly ILogger _logger;
        private readonly IInventoryProcessor _inventoryProcessor;
        private readonly IOrderRepository _orderRepository;
        private CoinifyConfiguration _coinifyConfiguration;

        public CoinifyPaymentGateway()
            : this(
                  ServiceLocator.Current.GetInstance<ILogger>(),
                  ServiceLocator.Current.GetInstance<IInventoryProcessor>(),
                  ServiceLocator.Current.GetInstance<IOrderRepository>())
        {
        }

        public CoinifyPaymentGateway(
            ILogger logger,
            IInventoryProcessor inventoryProcessor,
            IOrderRepository orderRepository)
        {
            _logger = logger;
            _inventoryProcessor = inventoryProcessor;
            _orderRepository = orderRepository;
        }

        public IDictionary<string, string> Settings { get; set; }

        public PaymentProcessingResult ProcessPayment(IOrderGroup orderGroup, IPayment payment)
        {
            if (HttpContext.Current == null)
            {
                return PaymentProcessingResult.CreateSuccessfulResult(Utilities.Translate("ProcessPaymentNullHttpContext"));
            }

            if (payment == null)
            {
                return PaymentProcessingResult.CreateUnsuccessfulResult(Utilities.Translate("PaymentNotSpecified"));
            }

            var orderForm = orderGroup.Forms.FirstOrDefault(f => f.Payments.Contains(payment));
            if (orderForm == null)
            {
                return PaymentProcessingResult.CreateUnsuccessfulResult(Utilities.Translate("PaymentNotAssociatedOrderForm"));
            }

            _coinifyConfiguration = new CoinifyConfiguration(Settings);
            var httpRequest = HttpContext.Current.Request;

            if (orderGroup is IPurchaseOrder purchaseOrder)
            {
                // TODO
                //if (payment.TransactionType == TransactionType.Credit.ToString())
                //{
                //    return ProcessPaymentRefund(purchaseOrder, payment);
                //}
            }

            var cart = orderGroup as ICart;
            if (cart != null)
            {
                if (cart.OrderStatus == OrderStatus.Completed)
                {
                    return PaymentProcessingResult.CreateSuccessfulResult(Utilities.Translate("ProcessPaymentStatusCompleted"));
                }

                if (payment.Status == PaymentStatus.Pending.ToString() &&
                    payment.Properties.ContainsKey(Constants.MetaFieldCoinifyPaymentInvoiceId) &&
                    httpRequest.QueryString["hash"] != null)
                {
                    return ProcessPaymentReturn(orderGroup, payment);
                }
            }

            return ProcessPaymentCheckout(cart, payment);
        }

        private PaymentProcessingResult ProcessPaymentReturn(IOrderGroup orderGroup, IPayment payment)
        {
            var cart = orderGroup as ICart;
            if (cart == null)
            {
                return PaymentProcessingResult.CreateUnsuccessfulResult(Utilities.Translate("PaymentNotSpecified"));
            }

            var httpRequest = HttpContext.Current.Request;
            string remoteHash = httpRequest.QueryString["hash"];
            if (string.IsNullOrWhiteSpace(remoteHash))
            {
                return PaymentProcessingResult.CreateUnsuccessfulResult(Utilities.Translate("CommitTranErrorInvalidReturn"));
            }

            int orderGroupId = cart.OrderLink.OrderGroupId;
            string calculatedHash = Utilities.GetHMAC(
                _coinifyConfiguration.HashSecret, $"{orderGroupId}_return");
            if (calculatedHash != remoteHash)
            {
                return PaymentProcessingResult.CreateUnsuccessfulResult(Utilities.Translate("CommitTranErrorInvalidReturn"));
            }

            long invoiceId = (long)payment.Properties[Constants.MetaFieldCoinifyPaymentInvoiceId];
            var coinifyClient = new CoinifyClient(_coinifyConfiguration);
            var invoice = coinifyClient.InvoiceGet(invoiceId);
            if (invoice == null)
            {
                return PaymentProcessingResult.CreateUnsuccessfulResult(Utilities.Translate("CommitTranErrorInvalidReturn"));
            }

            if (orderGroupId != invoice.CustomData.OrderId)
            {
                _logger.Error("The order id of the cart did not match the order id of the invoice.");
                return PaymentProcessingResult.CreateUnsuccessfulResult(Utilities.Translate("CommitTranErrorInvalidReturn"));
            }

            switch (invoice.State)
            {
                case InvoiceState.Complete:
                case InvoiceState.Paid:
                    _logger.Information("Payment completed. About to process successful transaction.");
                    return ProcessSuccessfulTransaction(cart, payment);
                default:
                    _logger.Information("Payment has expired.");
                    return ProcessUnsuccessfulTransaction(Utilities.Translate("CancelMessage"));
            }
        }

        private PaymentProcessingResult ProcessSuccessfulTransaction(IOrderGroup orderGroup, IPayment payment)
        {
            var cart = orderGroup as ICart;
            if (cart == null)
            {
                return ProcessUnsuccessfulTransaction(Utilities.Translate("CommitTranErrorCartNull"));
            }

            using (var scope = new TransactionScope())
            {
                var errorMessages = new List<string>();
                var cartCompleted = DoCompletingCart(cart, errorMessages);
                if (!cartCompleted)
                {
                    string cancelUrl = _coinifyConfiguration.CancelUrl;
                    cancelUrl = UriSupport.AddQueryString(cancelUrl, "message", string.Join(";", errorMessages.Distinct().ToArray()));

                    return PaymentProcessingResult.CreateSuccessfulResult("", cancelUrl);
                }

                var purchaseOrder = MakePurchaseOrder(cart, payment);

                string redirectUrl = CreateRedirectionUrl(purchaseOrder);

                scope.Complete();

                _logger.Information($"Transaction completed. Redirecting to {redirectUrl}.");
                return PaymentProcessingResult.CreateSuccessfulResult("", redirectUrl);
            }
        }

        private PaymentProcessingResult ProcessUnsuccessfulTransaction(string errorMessage)
        {
            string cancelUrl = _coinifyConfiguration.CancelUrl;

            _logger.Error($"Transaction failed [{errorMessage}].");
            return PaymentProcessingResult.CreateUnsuccessfulResult(cancelUrl);
        }

        private PaymentProcessingResult ProcessPaymentCheckout(ICart cart, IPayment payment)
        {
            var invoiceRequest = CreateInvoiceRequest(cart, payment);
            var coinifyClient = new CoinifyClient(_coinifyConfiguration);
            Invoice invoice = coinifyClient.InvoiceCreate(invoiceRequest);
            if (invoice == null)
            {
                return PaymentProcessingResult.CreateUnsuccessfulResult("No invoice created.");
            }

            payment.Properties[Constants.MetaFieldCoinifyPaymentInvoiceId] = invoice.Id;
            payment.Properties[Constants.MetaFieldCoinifyBitcoinAmount] = invoice.Bitcoin.Amount;

            _orderRepository.Save(cart);

            return PaymentProcessingResult.CreateSuccessfulResult(string.Empty, invoice.PaymentUrl);
        }

        private PaymentProcessingResult ProcessPaymentRefund(IPurchaseOrder purchaseOrder, IPayment payment)
        {
            string invoiceId = payment.ProviderTransactionID;
            if (string.IsNullOrEmpty(invoiceId) || invoiceId.Equals("0"))
            {
                return PaymentProcessingResult.CreateUnsuccessfulResult("TransactionID is not valid or the current payment method does not support this order type.");
            }

            //_coinifyClient.InvoiceRefund(purchaseOrder, payment);

            return PaymentProcessingResult.CreateSuccessfulResult(string.Empty);
        }

        private bool DoCompletingCart(ICart cart, IList<string> errorMessages)
        {
            // Change status of payments to processed. 
            // It must be done before execute workflow to ensure payments which should mark as processed.
            // To avoid get errors when executed workflow.
            foreach (IPayment p in cart.Forms.SelectMany(f => f.Payments).Where(p => p != null))
            {
                PaymentStatusManager.ProcessPayment(p);
            }

            var validationIssues = new Dictionary<ILineItem, IList<ValidationIssue>>();
            cart.AdjustInventoryOrRemoveLineItems((item, issue) => AddValidationIssues(validationIssues, item, issue), _inventoryProcessor);

            var isSuccess = !validationIssues.Any();

            foreach (var issue in validationIssues.Values.SelectMany(x => x).Distinct())
            {
                errorMessages.Add(
                    issue == ValidationIssue.RejectedInventoryRequestDueToInsufficientQuantity
                        ? Utilities.Translate("NotEnoughStockWarning")
                        : Utilities.Translate("CartValidationWarning"));
            }

            return isSuccess;
        }

        private IPurchaseOrder MakePurchaseOrder(ICart cart, IPayment payment)
        {
            var orderReference = _orderRepository.SaveAsPurchaseOrder(cart);
            var purchaseOrder = _orderRepository.Load<IPurchaseOrder>(orderReference.OrderGroupId);

            long invoiceId = (long)payment.Properties[Constants.MetaFieldCoinifyPaymentInvoiceId];
            UpdateTransactionIdOfPaymentMethod(purchaseOrder, invoiceId.ToString());

            var contact = CustomerContext.Current.CurrentContact;
            if (contact != null)
            {
                contact.LastOrder = purchaseOrder.Created;
                contact.SaveChanges();
            }

            //AddNoteToPurchaseOrder($"New order placed by {PrincipalInfo.CurrentPrincipal.Identity.Name} in Public site", purchaseOrder);

            // Remove old cart
            _orderRepository.Delete(cart.OrderLink);
            purchaseOrder.OrderStatus = OrderStatus.InProgress;

            _orderRepository.Save(purchaseOrder);

            return purchaseOrder;
        }

        private InvoiceCreateRequest CreateInvoiceRequest(ICart cart, IPayment payment)
        {
            var request = new InvoiceCreateRequest();
            request.Amount = payment.Amount;
            request.Currency = cart.Currency.CurrencyCode;
            request.CustomData = new InvoiceCustom { OrderId = cart.OrderLink.OrderGroupId };

            string cancelUrl = _coinifyConfiguration.CancelUrl;
            string returnUrl = _coinifyConfiguration.ReturnUrl;

            int orderGroupId = cart.OrderLink.OrderGroupId;
            cancelUrl = UriSupport.AddQueryString(cancelUrl, "hash", Utilities.GetHMAC(_coinifyConfiguration.HashSecret, $"{orderGroupId}_cancel"));
            returnUrl = UriSupport.AddQueryString(returnUrl, "hash", Utilities.GetHMAC(_coinifyConfiguration.HashSecret, $"{orderGroupId}_return"));

            request.CancelUrl = UriSupport.AbsoluteUrlBySettings(cancelUrl);
            request.ReturnUrl = UriSupport.AbsoluteUrlBySettings(returnUrl);

            return request;
        }

        private string CreateRedirectionUrl(IPurchaseOrder purchaseOrder)
        {
            string successUrl = _coinifyConfiguration.SuccessUrl;

            string redirectionUrl = UriSupport.AddQueryString(successUrl, "success", "true");
            redirectionUrl = UriSupport.AddQueryString(redirectionUrl, "orderNumber", purchaseOrder.OrderLink.OrderGroupId.ToString());

            return redirectionUrl;
        }

        private static void AddValidationIssues(IDictionary<ILineItem, IList<ValidationIssue>> issues, ILineItem lineItem, ValidationIssue issue)
        {
            if (!issues.ContainsKey(lineItem))
            {
                issues.Add(lineItem, new List<ValidationIssue>());
            }

            if (!issues[lineItem].Contains(issue))
            {
                issues[lineItem].Add(issue);
            }
        }

        private static void UpdateTransactionIdOfPaymentMethod(IPurchaseOrder purchaseOrder, string paymentGatewayTransactionId)
        {
            foreach (var payment in purchaseOrder.Forms
                .SelectMany(form => form.Payments)
                .OfType<CoinifyPayment>())
            {
                payment.TransactionID = paymentGatewayTransactionId;
                payment.ProviderTransactionID = paymentGatewayTransactionId;
            }
        }
    }
}
