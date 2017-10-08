# Coinify payment plugin for EPiServer Commerce
This solution folder contains the source code for a payment provider for EPiServer Commerce, integrating with the [Coinify payment provider](https://www.coinify.com/).
## What is Coinify
Coinify is a danish payment provider, making it possible for merchants to accept bitcoins and altcoins (other cryptocurrencies, like litecoin and namecoin). Their solution works exactly like a payment provider accepting payment cards.
When a customer is sent from the Commerce site to the Coinify payment page, the following is presented:
- a payment amount in Bitcoin (the customer can select an altcoin),
- a payment address,
- a QR code (embedding address and amount),
- a button sending the address to a wallet app.

When the customer pays, using any of the three methods, the payment is immediately settled and the customer is returned to the Commerce website where the usual processes is executed (converting cart to purchase order etc).
As the amount is settled, the amount is available on the Coinify account within 10 minutes.
Merchants can setup their Coinify accounts as a bitcoin account or as a regular fiat currency account. When the account is a fiat currency account, any received amounts are immediately converted from cryptocoins to fiat currency amounts. This way merchants can accept cryptocoins, but receive regular fiat money.

## Setup guide
### Setup Coinify
Setting up a Coinify account is somewhat easy. Sign up at [Coinify](https://www.coinify.com/). After signing up the merchant will need to provide a few documents verifying ownership. At some point in time, a pay out account should also be registered.
For development and testing, a separate account can be created in the [Coinify Sandbox](https://www.sandbox.coinify.com/). The Coinify Sandbox is a complete copy of the platform, except that it does not integrate with the real Bitcoin network, but the Bitcoin Testnet.

### Installation
The code is not yet published as a NuGet package. For now, the following is needed to install the code:
- Download the repository, or just the solution in this folder. 
- Build the solution.
- From the project, `StefanOlsen.Commerce.Payment.Coinify`:
  - Copy the binary file, `StefanOlsen.Commerce.Payment.Coinify.dll` from the bin folder to some location in your project and add it as a reference in your projects (at least in the website and the Commerce Manager projects).
  - Copy the folder `lang` to your website project.
  - Copy the Razor view files from the folder `Views`, to your website project. Edit them or leave them out as appropriate (they are made for the Quicksilver demo site).
- From the project, `StefanOlsen.Commerce.Payment.Coinify.Manager`:
  - Copy the file, ConfigurePayment.ascx, to your Commerce Manager project. It should be placed in this path `Apps/Order/Payments/Plugins/Coinify` (create the Coinify folder).
- Copy the methods from the file `CheckoutController.cs`. It is located in the folder `sample` next to the folder `src`. Add the methods to a MVC controller in your own website project (in a Quicksilver project, add it to the file `CheckoutController.cs`).

### Commerce Manager
The following settings need to be added in Commerce Manager.
Go to the Commerce Manager, open the "Administration section, then select "Order System" and "Payments". Click "New" and fill out these fields in the first tab:
- __Name__: (choose something)
- __System Keyword__: Coinify
- __Class Name__: Select StefanOlsen.Commerce.Payment.Coinify.CoinifyPaymentGateway
- __Payment Class__: Select StefanOlsen.Commerce.Payment.Coinify.CoinifyPayment
- __Is Active__: Yes
- __Chosen shipping methods__: (select those that applies)

On the next tab, Parameters, add the following plugin-specific settings:
- __API Key__: (enter the API Key from the Coinify account)
- __API Secret__: (enter the API Secret from the Coinify account)
- __Hash Secret__: (generate a random key and enter it here)
- __Sandbox Mode?__: (check this, if the API key and API secret belongs to a sandbox account)
- __Return URL__: (enter the URL for the page, on which you added the payment callback method)
- __Cancel URL__: (enter the URL for the page, on which you added the payment callback method)
- __Success URL__: (enter the URL for the page, on which you added the payment success method)

On the last tab, Markets, select the markets for which this payment method should be available.

### Testing
When you are ready to test out the payment plugin, you will need a Bitcoin wallet, such as the [CoPay wallet app](https://copay.io/) (which supports both Bitcoin and Bitcoin Testnet).
To add bitcoins to your Testnet wallet, go to a web faucet ([see a list here](https://en.bitcoin.it/wiki/Testnet#Faucets)) and transfer some bitcoins to your wallet.