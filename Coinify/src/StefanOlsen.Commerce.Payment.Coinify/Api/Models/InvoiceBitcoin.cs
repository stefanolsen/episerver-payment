using Newtonsoft.Json;

namespace StefanOlsen.Commerce.Payment.Coinify.Api.Models
{
    public class InvoiceBitcoin
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("amount_due")]
        public decimal AmountDue { get; set; }

        [JsonProperty("amount_paid")]
        public decimal AmountPaid { get; set; }
    }
}
