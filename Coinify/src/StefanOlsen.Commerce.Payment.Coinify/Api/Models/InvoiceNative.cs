﻿using Newtonsoft.Json;

namespace StefanOlsen.Commerce.Payment.Coinify.Api.Models
{
    public class InvoiceNative
    {
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }
}
