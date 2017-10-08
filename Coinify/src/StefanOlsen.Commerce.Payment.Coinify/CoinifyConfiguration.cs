using System.Collections.Generic;

namespace StefanOlsen.Commerce.Payment.Coinify
{
    public class CoinifyConfiguration
    {
        public const string SystemNameCoinify = "Coinify";

        private IDictionary<string, string> _settings;

        public string ApiKey => GetStringValue(Constants.SettingsKeyApiKey);

        public string ApiSecret => GetStringValue(Constants.SettingsKeyApiSecret);

        public string HashSecret => GetStringValue(Constants.SettingsKeyHashSecret);

        public bool SandboxMode => GetBooleanValue(Constants.SettingsKeySandboxMode);

        public string CancelUrl => GetStringValue(Constants.SettingsKeyCancelUrl);

        public string ReturnUrl => GetStringValue(Constants.SettingsKeyReturnUrl);

        public string SuccessUrl => GetStringValue(Constants.SettingsKeySuccessUrl);

        public CoinifyConfiguration(IDictionary<string, string> settings)
        {
            Initialize(settings);
        }

        protected void Initialize(IDictionary<string, string> settings)
        {
            _settings = settings;
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
    }
}
