using Newtonsoft.Json;

namespace StefanOlsen.Commerce.Payment.Coinify.Api.Models
{
    public class InvoiceCreateResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public Invoice Invoice { get; set; }
    }
}
