    using System;
using System.Linq;
using EPiServer.Commerce.Order;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;

namespace StefanOlsen.Commerce.Payment.Coinify
{
    [ServiceConfiguration(typeof(IPaymentOption))]
    public class CoinifyPaymentOption : IPaymentOption
    {
        private readonly IOrderGroupFactory _orderGroupFactory;
        private readonly PaymentMethodDto.PaymentMethodRow _paymentMethod;

        public Guid PaymentMethodId { get; }
        public string SystemKeyword { get; }
        public string Name { get; }
        public string Description { get; }

        public CoinifyPaymentOption()
            : this(ServiceLocator.Current.GetInstance<IOrderGroupFactory>())
        {
        }

        public CoinifyPaymentOption(IOrderGroupFactory orderGroupFactory)
        {
            _orderGroupFactory = orderGroupFactory;
            _paymentMethod = CoinifyConfiguration.GetCoinifyPaymentMethod()?.PaymentMethod?.FirstOrDefault();

            if (_paymentMethod == null)
            {
                return;
            }

            PaymentMethodId = _paymentMethod.PaymentMethodId;
            SystemKeyword = _paymentMethod.SystemKeyword;
            Name = _paymentMethod.Name;
            Description = _paymentMethod.Description;
        }

        public bool ValidateData()
        {
            // Nothing to validate before the payment page.
            return true;
        }

        public IPayment CreatePayment(decimal amount, IOrderGroup orderGroup)
        {
            var payment = orderGroup.CreatePayment(_orderGroupFactory, typeof(CoinifyPayment));

            payment.PaymentMethodId = PaymentMethodId;
            payment.PaymentMethodName = _paymentMethod.Name;
            payment.Amount = amount;
            payment.Status = PaymentStatus.Pending.ToString();
            payment.TransactionType = TransactionType.Sale.ToString();

            return payment;
        }
    }
}
