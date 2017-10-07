using System;
using System.IO;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;

namespace StefanOlsen.Commerce.Payment.Coinify
{
    internal static class Utilities
    {
        private static Injected<LocalizationService> _localizationService = default(Injected<LocalizationService>);

        public static string GetName<TSource, TField>(this Expression<Func<TSource, TField>> expression)
        {
            var body = expression.Body as MemberExpression;
            if (body == null)
            {
                UnaryExpression ubody = (UnaryExpression)expression.Body;
                body = ubody.Operand as MemberExpression;
            }

            return body?.Member.Name;
        }

        public static string GetPostData(Stream inputStream)
        {
            inputStream.Position = 0;
            using (var reader = new StreamReader(inputStream))
            {
                string body = reader.ReadToEnd();

                return body;
            }
        }

        public static string Translate(string languageKey)
        {
            return GetLocalizationMessage("/Commerce/Checkout/PayPal/" + languageKey);
        }

        public static string GetLocalizationMessage(string path)
        {
            return _localizationService.Service.GetString(path);
        }

        public static string GetHMAC(string secret, string message)
        {
            Encoding encoding = Encoding.UTF8;

            byte[] secretBytes = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);

            using (var hmac = new HMACSHA256(secretBytes))
            {
                byte[] signatureBytes = hmac.ComputeHash(messageBytes);

                var sb = new StringBuilder();
                foreach (byte b in signatureBytes)
                {
                    sb.Append(b.ToString("x2").ToLower());
                }

                var signature = sb.ToString();

                return signature;
            }
        }
    }
}
