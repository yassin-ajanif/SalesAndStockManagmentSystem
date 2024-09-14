using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GetStartedApp.Helpers.WordTranslation;

namespace GetStartedApp.Helpers
{
    public static class WordTranslation
    {
        public struct Cash
        {
            public string enArabic => "نقدا";
            public string enEnglish => "cash";
            public string enFrench => "Espèces";
        }

        public struct Credit
        {
            public string enArabic => "دين";
            public string enEnglish => "Credit";
            public string enFrench => "Crédit";
        }

        public struct Tpe
        {
            public string enArabic => "tpe";
            public string enEnglish => "tpe";
            public string enFrench => "tpe";
        }

        public struct Check
        {
            public string enArabic => "شيك";
            public string enEnglish => "Check";
            public string enFrench => "Chèque";
        }

        public static readonly object[] PaymentTypes = new object[]
        {
        new Cash(),
        new Credit(),
        new Tpe(),
        new Check()
        };

        public static string TranslatePaymentIntoTargetedLanguage(string paymentTerm, string targetLanguagePrefix)
        {
            foreach (var payment in PaymentTypes)
            {
                string arabicValue = payment.GetType().GetProperty("enArabic")?.GetValue(payment)?.ToString();
                string englishValue = payment.GetType().GetProperty("enEnglish")?.GetValue(payment)?.ToString();
                string frenchValue = payment.GetType().GetProperty("enFrench")?.GetValue(payment)?.ToString();

                // Check if the term matches in any language
                if (paymentTerm.ToLower() == arabicValue?.ToLower() ||
                    paymentTerm.ToLower() == englishValue?.ToLower() ||
                    paymentTerm.ToLower() == frenchValue?.ToLower())
                {
                    // Return the translation based on the target language
                    if (targetLanguagePrefix == "ar")
                    {
                        return arabicValue;
                    }
                    else if (targetLanguagePrefix == "en")
                    {
                        return englishValue;
                    }
                    else if (targetLanguagePrefix == "fr")
                    {
                        return frenchValue;
                    }
                    else
                    {
                        throw new Exception("Invalid language prefix");
                    }
                }
            }

            // If no match found, throw an exception or return null
            throw new Exception($"Payment method '{paymentTerm}' not found.");
        }


    }
}
