using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Web.Console.Interfaces;

namespace StefanOlsen.Commerce.Payment.Coinify.Manager
{
    public partial class ConfigurePayment : UserControl, IGatewayControl
    {
        private PaymentMethodDto _paymentMethodDto;

        /// <summary>
        /// Gets or sets the validation group.
        /// </summary>
        /// <value>The validation group.</value>
        public string ValidationGroup { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurePayment"/> class.
        /// </summary>
        public ConfigurePayment()
        {
            ValidationGroup = string.Empty;
            _paymentMethodDto = null;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            BindData();
        }

        /// <summary>
        /// Loads the object.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public void LoadObject(object dto)
        {
            _paymentMethodDto = dto as PaymentMethodDto;
        }

        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public void SaveChanges(object dto)
        {
            if (!Visible)
            {
                return;
            }

            _paymentMethodDto = dto as PaymentMethodDto;
            if (_paymentMethodDto?.PaymentMethodParameter == null)
            {
                return;
            }

            var paymentMethodId = _paymentMethodDto.PaymentMethod.Count > 0 ? _paymentMethodDto.PaymentMethod[0].PaymentMethodId : Guid.Empty;

            UpdateOrCreateParameter(CoinifyConfiguration.ParameterNameApiKey, ApiKey, paymentMethodId);
            UpdateOrCreateParameter(CoinifyConfiguration.ParameterNameApiSecret, ApiSecret, paymentMethodId);
            UpdateOrCreateParameter(CoinifyConfiguration.ParameterNameHashSecret, HashSecret, paymentMethodId);
            UpdateOrCreateParameter(CoinifyConfiguration.ParameterNameSandboxMode, SandboxMode, paymentMethodId);
            UpdateOrCreateParameter(CoinifyConfiguration.ParameterNameCancelUrl, CancelUrl, paymentMethodId);
            UpdateOrCreateParameter(CoinifyConfiguration.ParameterNameReturnUrl, ReturnUrl, paymentMethodId);
            UpdateOrCreateParameter(CoinifyConfiguration.ParameterNameSuccessUrl, SuccessUrl, paymentMethodId);
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            if (_paymentMethodDto?.PaymentMethodParameter == null)
            {
                Visible = false;
                return;
            }

            BindParameterData(CoinifyConfiguration.ParameterNameApiKey, ApiKey);
            BindParameterData(CoinifyConfiguration.ParameterNameApiSecret, ApiSecret);
            BindParameterData(CoinifyConfiguration.ParameterNameHashSecret, HashSecret);
            BindParameterData(CoinifyConfiguration.ParameterNameSandboxMode, SandboxMode);
            BindParameterData(CoinifyConfiguration.ParameterNameCancelUrl, CancelUrl);
            BindParameterData(CoinifyConfiguration.ParameterNameReturnUrl, ReturnUrl);
            BindParameterData(CoinifyConfiguration.ParameterNameSuccessUrl, SuccessUrl);
        }

        private void UpdateOrCreateParameter(string parameterName, CheckBox parameterControl, Guid paymentMethodId)
        {
            UpdateOrCreateParameter(parameterName, parameterControl.Checked.ToString(), paymentMethodId);
        }

        private void UpdateOrCreateParameter(string parameterName, TextBox parameterControl, Guid paymentMethodId)
        {
            UpdateOrCreateParameter(parameterName, parameterControl.Text, paymentMethodId);
        }

        private void UpdateOrCreateParameter(string parameterName, string parameterValue, Guid paymentMethodId)
        {
            var parameter = GetParameterByName(parameterName);
            if (parameter != null)
            {
                parameter.Value = parameterValue;
            }
            else
            {
                var row = _paymentMethodDto.PaymentMethodParameter.NewPaymentMethodParameterRow();
                row.PaymentMethodId = paymentMethodId;
                row.Parameter = parameterName;
                row.Value = parameterValue;
                _paymentMethodDto.PaymentMethodParameter.Rows.Add(row);
            }
        }

        private void BindParameterData(string parameterName, CheckBox parameterControl)
        {
            var parameterByName = GetParameterByName(parameterName);
            if (parameterByName == null)
            {
                return;
            }

            parameterControl.Checked = bool.TryParse(parameterByName.Value, out bool isChecked) && isChecked;
        }

        private void BindParameterData(string parameterName, TextBox parameterControl)
        {
            var parameterByName = GetParameterByName(parameterName);
            if (parameterByName == null)
            {
                return;
            }

            parameterControl.Text = parameterByName.Value;
        }

        private PaymentMethodDto.PaymentMethodParameterRow GetParameterByName(string name)
        {
            return CoinifyConfiguration.GetParameterByName(_paymentMethodDto, name);
        }
    }
}