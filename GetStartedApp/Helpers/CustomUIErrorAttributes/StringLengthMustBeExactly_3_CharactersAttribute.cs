using System;
using System.ComponentModel.DataAnnotations;

namespace GetStartedApp.Helpers.CustomUIErrorAttributes
{
    public class StringLengthMustBeExactly_3_CharactersAttribute : ValidationAttribute
    {
        public StringLengthMustBeExactly_3_CharactersAttribute()
        {
            // Set the default error message
            ErrorMessage = "The string length must be exactly 3 characters.";
        }

        public override bool IsValid(object value)
        {
            // Convert the value to a string
            string stringValue = value as string;

            // If the string is null or empty, return true (consider it valid)
            if (string.IsNullOrEmpty(stringValue))
            {
                return true; // You may want to change this behavior based on your requirements
            }

            // Check the length of the string
            return stringValue.Length == 3;
        }
    }
}
