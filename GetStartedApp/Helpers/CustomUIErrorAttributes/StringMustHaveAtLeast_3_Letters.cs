using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace GetStartedApp.Helpers.CustomUIErrorAttributes
{
    public class StringMustHaveAtLeast_3_LettersAttribute : ValidationAttribute
    {
        public StringMustHaveAtLeast_3_LettersAttribute()
        {
            
        }

        public override bool IsValid(object value)
        {
            string stringValue = value as string;

            if (string.IsNullOrEmpty(stringValue)) { return true; }
            // Regular expression to match at least three letters in English or Arabic
            var match = Regex.Match(stringValue, @"\p{L}", RegexOptions.IgnoreCase);
            int letterCount = 0;

            while (match.Success)
            {
                letterCount++;
                match = match.NextMatch();
            }

            return letterCount >= 3;
        }
    }
}
