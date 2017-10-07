using System;
using Newtonsoft.Json;

namespace StefanOlsen.Commerce.Payment.Coinify.Api.Models
{
    public class Invoice
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("create_time")]
        public DateTime CreateTime { get; set; }

        [JsonProperty("expire_time")]
        public DateTime ExpireTime { get; set; }

        [JsonProperty("state")]
        public InvoiceState State { get; set; }

        [JsonProperty("custom")]
        public InvoiceCustom CustomData { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("payment_url")]
        public string PaymentUrl { get; set; }

        [JsonProperty("bitcoin")]
        public InvoiceBitcoin Bitcoin { get; set; }

        [JsonProperty("native")]
        public InvoiceNative Native { get; set; }

        [JsonProperty("transfer")]
        public InvoiceTransfer Transfer { get; set; }
    }
}
