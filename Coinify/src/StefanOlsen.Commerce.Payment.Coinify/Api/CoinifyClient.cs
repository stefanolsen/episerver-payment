using System;
using System.Net;
using EPiServer.Logging;
using Newtonsoft.Json;
using StefanOlsen.Commerce.Payment.Coinify.Api.Models;

namespace StefanOlsen.Commerce.Payment.Coinify.Api
{
    public class CoinifyClient
    {
        private const string ApiBaseUrlProduction = "https://api.coinify.com/v3";
        private const string ApiBaseUrlSandbox = "https://api.sandbox.coinify.com/v3";

        private readonly CoinifyConfiguration _configuration;
        private readonly ILogger _logger = LogManager.GetLogger(typeof(CoinifyClient));

        public CoinifyClient(CoinifyConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Invoice InvoiceCreate(InvoiceCreateRequest invoiceCreateRequest)
        {
            if (invoiceCreateRequest == null)
            {
                throw new ArgumentNullException(nameof(invoiceCreateRequest));
            }

            string json = JsonConvert.SerializeObject(invoiceCreateRequest);

            string result = PostAuthenticated("invoices", json);
            if (result == null)
            {
                return null;
            }

            var invoice = JsonConvert.DeserializeObject<InvoiceCreateResponse>(result);
            
            return invoice.Invoice;
        }

        public Invoice InvoiceGet(long invoiceId)
        {
            if (invoiceId == default(long))
            {
                throw new ArgumentException("Default value is not valid.", nameof(invoiceId));
            }

            string result = GetAuthenticated($"invoices/{invoiceId}");
            if (result == null)
            {
                return null;
            }

            var invoice = JsonConvert.DeserializeObject<InvoiceCreateResponse>(result);

            return invoice.Invoice;
        }

        //public void InvoiceRefund(IOrderGroup orderGroup, IPayment payment)
        //{
        //    var request = new InvoiceRefundRequest()
        //    {
        //        Amount = payment.Amount,
        //        Currency = orderGroup.Currency.CurrencyCode,
        //        //EmailAddress = payment["BtcEmailAddress"]
        //    };

        //    string invoiceId = payment.ProviderTransactionID;

        //    string json = JsonConvert.SerializeObject(request);

        //    string result = CallAuthenticated($"invoices/{invoiceId}/refund", "POST", json);

        //    //var invoice = JsonConvert.DeserializeObject<InvoiceCreateResponse>(result);
        //}

        private string GetAuthenticated(string endpoint)
        {
            var client = new WebClient();
            try
            {
                string authHeader = GenerateAuthorizationHeader(_configuration.ApiKey, _configuration.ApiSecret);
                client.Headers.Add(HttpRequestHeader.Authorization, authHeader);
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");

                string baseUrl = _configuration.SandboxMode ? ApiBaseUrlSandbox : ApiBaseUrlProduction;
                string result = client.DownloadString($"{baseUrl}/{endpoint}");

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error occurred while getting data from {endpoint}.", ex);

                return null;
            }
            finally
            {
                client.Dispose();
            }
        }

        private string PostAuthenticated(string endpoint)
        {
            return PostAuthenticated(endpoint, string.Empty);
        }

        private string PostAuthenticated(string endpoint, string json)
        {
            var client = new WebClient();
            try
            {
                string authHeader = GenerateAuthorizationHeader(_configuration.ApiKey, _configuration.ApiSecret);
                client.Headers.Add(HttpRequestHeader.Authorization, authHeader);
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");

                string baseUrl = _configuration.SandboxMode ? ApiBaseUrlSandbox : ApiBaseUrlProduction;
                string result = client.UploadString($"{baseUrl}/{endpoint}", "POST", json);

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error occurred while posting data to {endpoint}.", ex);

                return null;
            }
            finally
            {
                client.Dispose();
            }
        }

        private static string GenerateAuthorizationHeader(string apiKey, string apiSecret)
        {
            long ticks = DateTime.Now.Ticks;
            string nonce = ticks.ToString();
            string message = nonce + apiKey;
            string signature = Utilities.GetHMAC(apiSecret, message);

            string headerContent = $"Coinify apikey=\"{apiKey}\", nonce=\"{nonce}\", signature=\"{signature}\"";

            return headerContent;
        }
    }
}
