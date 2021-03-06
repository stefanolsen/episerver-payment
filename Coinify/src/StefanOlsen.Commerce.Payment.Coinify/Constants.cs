﻿namespace StefanOlsen.Commerce.Payment.Coinify
{
    public static class Constants
    {
        public const string SettingsKeyApiKey = "APIKey";
        public const string SettingsKeyApiSecret = "APISecret";
        public const string SettingsKeyHashSecret = "HashSecret";
        public const string SettingsKeySandboxMode = "SandboxMode";
        public const string SettingsKeyCancelUrl = "CancelUrl";
        public const string SettingsKeyReturnUrl = "ReturnUrl";
        public const string SettingsKeySuccessUrl = "SuccessUrl";

        public const string MetaClassNamespaceOrder = "Mediachase.Commerce.Orders";
        public const string MetaClassNamespaceOrderUser = "Mediachase.Commerce.Orders.User";

        public const string MetaClassOrderFormPayment = "OrderFormPayment";
        public const string MetaClassNameCoinifyPayment = "CoinifyPayment";
        public const string MetaClassTableNameCoinifyPayment = "OrderFormPaymentEx_CoinifyPayment";

        public const string MetaFieldCoinifyBitcoinAmount = "CoinifyBitcoinAmount";
        public const string MetaFieldCoinifyPaymentInvoiceId = "CoinifyInvoiceId";
        public const string MetaFieldCoinifyReturnAddress = "CoinifyReturnAddress";
    }
}
