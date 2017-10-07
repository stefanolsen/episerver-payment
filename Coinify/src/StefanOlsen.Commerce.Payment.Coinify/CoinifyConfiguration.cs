using System;
using System.Collections.Generic;
using System.Linq;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;

namespace StefanOlsen.Commerce.Payment.Coinify
{
    public class CoinifyConfiguration
    {
        public const string ParameterNameApiKey = "APIKey";
        public const string ParameterNameApiSecret = "APISecret";
        public const string ParameterNameHashSecret = "HashSecret";
        public const string ParameterNameSandboxMode = "SandboxMode";
        public const string ParameterNameCancelUrl = "CancelUrl";
        public const string ParameterNameReturnUrl = "ReturnUrl";
        public const string ParameterNameSuccessUrl = "SuccessUrl";

        public const string SystemNameCoinify = "Coinify";

        private IDictionary<string, string> _settings;
        private PaymentMethodDto _paymentMethodDto;

        public Guid PaymentMethodId { get; protected set; }

        public string ApiKey => GetStringValue(ParameterNameApiKey);

        public string ApiSecret => GetStringValue(ParameterNameApiSecret);

        public string HashSecret => GetStringValue(ParameterNameHashSecret);

        public bool SandboxMode => GetBooleanValue(ParameterNameSandboxMode);

        public string CancelUrl => GetStringValue(ParameterNameCancelUrl);

        public string ReturnUrl => GetStringValue(ParameterNameReturnUrl);

        public string SuccessUrl => GetStringValue(ParameterNameSuccessUrl);

        public CoinifyConfiguration() : this(null)
        {
        }

        public CoinifyConfiguration(IDictionary<string, string> settings)
        {
            Initialize(settings);
        }

        protected void Initialize(IDictionary<string, string> settings)
        {
            _paymentMethodDto = GetCoinifyPaymentMethod();
            PaymentMethodId = GetPaymentMethodId();

            _settings = settings ?? GetSettings();
        }

        private string GetStringValue(string parameterName)
        {
            return _settings.TryGetValue(parameterName, out string result)
                ? result
                : null;
        }

        private bool GetBooleanValue(string parameterName)
        {
            string value = GetStringValue(parameterName);

            return bool.TryParse(value, out bool result) && result;
        }

        private Guid GetPaymentMethodId()
        {
            var paymentMethodRow = _paymentMethodDto.PaymentMethod.Rows[0] as PaymentMethodDto.PaymentMethodRow;
            return paymentMethodRow?.PaymentMethodId ?? Guid.Empty;
        }

        private IDictionary<string, string> GetSettings()
        {
            return _paymentMethodDto.PaymentMethod
                .FirstOrDefault()
                ?.GetPaymentMethodParameterRows()
                ?.ToDictionary(row => row.Parameter, row => row.Value);
        }

        public static PaymentMethodDto.PaymentMethodParameterRow GetParameterByName(PaymentMethodDto paymentMethodDto, string parameterName)
        {
            var rowArray = (PaymentMethodDto.PaymentMethodParameterRow[])paymentMethodDto.PaymentMethodParameter.Select(string.Format("Parameter = '{0}'", parameterName));

            return rowArray.Length > 0 ? rowArray[0] : null;
        }

        public static PaymentMethodDto GetCoinifyPaymentMethod()
        {
            return PaymentManager.GetPaymentMethodBySystemName(SystemNameCoinify, SiteContext.Current.LanguageName);
        }
    }
}
