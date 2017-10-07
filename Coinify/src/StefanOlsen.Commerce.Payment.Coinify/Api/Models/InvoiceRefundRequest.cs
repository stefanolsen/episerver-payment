using Newtonsoft.Json;

namespace StefanOlsen.Commerce.Payment.Coinify.Api.Models
{
    public class InvoiceRefundRequest
    {
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("email_address")]
        public string EmailAddress { get; set; }

        [JsonProperty("btc_address")]
        public string BtcAddress { get; set; }

        [JsonProperty("use_payment_protocol_refund_address")]
        public bool UsePaymentProtocolRefundAddress { get; set; }
    }
}
