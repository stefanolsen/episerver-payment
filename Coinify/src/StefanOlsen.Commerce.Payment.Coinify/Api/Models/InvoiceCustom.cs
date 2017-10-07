using Newtonsoft.Json;

namespace StefanOlsen.Commerce.Payment.Coinify.Api.Models
{
    public class InvoiceCustom
    {
        [JsonProperty("order_id")]
        public int OrderId { get; set; }
    }
}
