using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using Mediachase.Commerce.Catalog;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Configurator;

namespace StefanOlsen.Commerce.Payment.Coinify
{
    [InitializableModule]
    public class MetadataInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            var metadataContext = CatalogContext.MetaDataContext;

            var paymentClass = CreateOrGetMetaClass(
                metadataContext,
                Constants.MetaClassNamespaceOrderUser,
                Constants.MetaClassNameCoinifyPayment,
                "Coinify Payment",
                Constants.MetaClassTableNameCoinifyPayment,
                Constants.MetaClassOrderFormPayment);

            CreateMetaField(
                metadataContext,
                paymentClass,
                Constants.MetaClassNamespaceOrder,
                Constants.MetaFieldCoinifyBitcoinAmount,
                "Bitcoin amount",
                MetaDataType.Decimal);

            CreateMetaField(
                metadataContext,
                paymentClass,
                Constants.MetaClassNamespaceOrder,
                Constants.MetaFieldCoinifyPaymentInvoiceId,
                "Invoice ID",
                MetaDataType.BigInt);

            CreateMetaField(
                metadataContext,
                paymentClass,
                Constants.MetaClassNamespaceOrder,
                Constants.MetaFieldCoinifyReturnAddress,
                "Bitcoin return address",
                MetaDataType.ShortString);
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        private static MetaClass CreateOrGetMetaClass(MetaDataContext mdContext, string metaDataNamespace,
            string className, string friendlyName, string tableName, string parentClassName)
        {
            var metaClass = MetaClass.Load(mdContext, className);
            if (metaClass != null)
            {
                return metaClass;
            }

            var parent = MetaClass.Load(mdContext, parentClassName);
            return MetaClass.Create(mdContext, metaDataNamespace, className, friendlyName, tableName, parent, false, string.Empty);
        }

        private static void CreateMetaField(MetaDataContext mdContext, MetaClass metaClass, string metaDataNamespace, string fieldName, string friendlyName, MetaDataType type)
        {
            var metaField = MetaField.Load(mdContext, fieldName) ??
                    MetaField.Create(mdContext, metaDataNamespace, fieldName, friendlyName, string.Empty, type, 0, true, false, false, false);

            JoinField(metaField, metaClass);
        }

        private static void JoinField(MetaField metaField, MetaClass metaClass)
        {
            if (metaClass == null ||
                metaClass.MetaFields.Contains(metaField))
            {
                return;
            }

            metaClass.AddField(metaField);
        }
    }
}