using System;
using System.Runtime.Serialization;
using Mediachase.Commerce.Orders;
using Mediachase.MetaDataPlus.Configurator;

namespace StefanOlsen.Commerce.Payment.Coinify
{
    [Serializable]
    public class CoinifyPayment : Mediachase.Commerce.Orders.Payment
    {
        private static MetaClass _metaClass;

        public CoinifyPayment()
            :base(CoinifyPaymentMetaClass)
        {
            ImplementationClass = GetType().AssemblyQualifiedName;
            PaymentType = PaymentType.Other;
        }

        protected CoinifyPayment(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ImplementationClass = GetType().AssemblyQualifiedName;
            PaymentType = PaymentType.Other;
        }

        public static MetaClass CoinifyPaymentMetaClass =>
            _metaClass ??
            (_metaClass = Mediachase.MetaDataPlus.Configurator.MetaClass.Load(OrderContext.MetaDataContext, Constants.MetaClassNameCoinifyPayment));

        public long CoinifyInvoiceId
        {
            get => (long) this[Constants.MetaFieldCoinifyPaymentInvoiceId];
            set => this[Constants.MetaFieldCoinifyPaymentInvoiceId] = value;
        }

        public decimal CoinifyBitcoinAmount
        {
            get => GetDecimal(Constants.MetaFieldCoinifyBitcoinAmount);
            set => this[Constants.MetaFieldCoinifyBitcoinAmount] = value;
        }

        public string ReturnAddress
        {
            get => GetString(Constants.MetaFieldCoinifyReturnAddress);
            set => this[Constants.MetaFieldCoinifyReturnAddress] = value;
        }
    }
}