using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace GetStartedApp.Helpers.CustomUIErrorAttributes
{
    public class CheckForInvalidCharacters : ValidationAttribute
    {
        private static readonly string InvalidCharactersPattern = @"['""$%&/\\;!?|<>+=\[\]{}:,'\\.*^`#~]";

        public CheckForInvalidCharacters()
        {
        }

        public override bool IsValid(object value)
        {
            string stringValue = value as string;

            if (string.IsNullOrEmpty(stringValue))
            {
                return true;
            }

            // Check for invalid SQL characters using regular expression
            var match = Regex.Match(stringValue, InvalidCharactersPattern);
            if (match.Success)
            {
                // Store the invalid character in the ErrorMessage property
                ErrorMessage = $" هناك رموز ممنوعة قم بإزالة هذاالرمز  {match.Value}.";
                return false;
            }

            return true;
        }

     
    }
}
