[HttpGet]
public ActionResult PaymentSuccess(CheckoutPage currentPage, int orderNumber)
{
	var viewModel = CreateCheckoutViewModel(currentPage);

	var purchaseOrder = _orderRepository.Load<IPurchaseOrder>(orderNumber);

	return Redirect(_checkoutService.BuildRedirectionUrl(viewModel, purchaseOrder, true));
}

[AllowDBWrite]
[HttpGet]
public ActionResult PaymentCallback(CheckoutPage currentPage, string hash)
{
	string cancelUrl = _urlResolver.GetUrl(currentPage.ContentLink);
	cancelUrl = UriSupport.AddQueryString(cancelUrl, "success", "false");

	// Load the cart right away.
	// The individual payment plugins should then validate/reject the hash.
	var cart = _orderRepository.LoadCart<ICart>(
		PrincipalInfo.CurrentPrincipal.GetContactId(), _cartService.DefaultCartName);
	if (cart == null)
	{
		return Redirect(cancelUrl);
	}

	var firstResult = cart.ProcessPayments().FirstOrDefault();
	if (!string.IsNullOrWhiteSpace(firstResult?.RedirectUrl))
	{
		return Redirect(firstResult.RedirectUrl);
	}

	return Redirect(cancelUrl);
}