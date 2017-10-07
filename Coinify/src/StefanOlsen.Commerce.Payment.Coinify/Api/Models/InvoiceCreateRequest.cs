using Newtonsoft.Json;

namespace StefanOlsen.Commerce.Payment.Coinify.Api.Models
{
    public class InvoiceCreateRequest
    {
        public InvoiceCreateRequest()
        {
            PluginName = "EPiServer Commerce";
            PluginVersion = "1.0";
        }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("plugin_name")]
        public string PluginName { get; private set; }

        [JsonProperty("plugin_version")]
        public string PluginVersion { get; private set; }

        [JsonProperty("custom")]
        public InvoiceCustom CustomData { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("callback_url")]
        public string CallbackUrl { get; set; }

        [JsonProperty("return_url")]
        public string ReturnUrl { get; set; }

        [JsonProperty("cancel_url")]
        public string CancelUrl { get; set; }
    }
}
