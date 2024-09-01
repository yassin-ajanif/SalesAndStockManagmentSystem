using System;
using System.ComponentModel.DataAnnotations;

namespace GetStartedApp.Helpers.CustomUIErrorAttributes
{
    public class MaxStringLengthAttribute_IS : ValidationAttribute
    {
        public int MaxLength { get; }
        private string ErrorToShowWhenStringIsTooLong_ToshowToUser;
        public MaxStringLengthAttribute_IS(int maxLength,string ErrorToShowWhenStringIsTooLong)
        {
            MaxLength = maxLength;
            ErrorToShowWhenStringIsTooLong_ToshowToUser = ErrorToShowWhenStringIsTooLong;
        }

        public override bool IsValid(object value)
        {
            string stringValue = value as string;

            if (string.IsNullOrEmpty(stringValue))
            {
                return true;
            }

            return stringValue.Length <= MaxLength;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{ErrorToShowWhenStringIsTooLong_ToshowToUser}";
        }
    }
}

