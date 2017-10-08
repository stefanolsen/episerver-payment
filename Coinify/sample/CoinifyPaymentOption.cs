[ServiceConfiguration(typeof(IPaymentOption))]
public class CoinifyPaymentOption : PaymentOptionBase
{
	public override string SystemKeyword => "Coinify";

	public CoinifyPaymentOption() 
		: this(LocalizationService.Current, ServiceLocator.Current.GetInstance<IOrderGroupFactory>(), ServiceLocator.Current.GetInstance<ICurrentMarket>(), ServiceLocator.Current.GetInstance<LanguageService>(), ServiceLocator.Current.GetInstance<IPaymentService>())
	{
	}

	public CoinifyPaymentOption(LocalizationService localizationService,
		IOrderGroupFactory orderGroupFactory,
		ICurrentMarket currentMarket,
		LanguageService languageService,
		IPaymentService paymentService)
		: base(localizationService, orderGroupFactory, currentMarket, languageService, paymentService)
	{
	}

	public override bool ValidateData()
	{
		// Nothing to validate before the payment page.
		return true;
	}

	public override IPayment CreatePayment(decimal amount, IOrderGroup orderGroup)
	{
		var payment = orderGroup.CreatePayment(OrderGroupFactory, typeof(CoinifyPayment));
		payment.PaymentMethodId = PaymentMethodId;
		payment.PaymentMethodName = SystemKeyword;
		payment.PaymentType = PaymentType.Other;
		payment.Amount = amount;
		payment.Status = PaymentStatus.Pending.ToString();
		payment.TransactionType = TransactionType.Sale.ToString();

		return payment;
	}
}