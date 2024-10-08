using System;
using System.ComponentModel.DataAnnotations;

public class NotWhitespaceAttribute : ValidationAttribute
{

    public override bool IsValid(object value)
    {
        // Allow null values and check if the value is a string that is not just whitespace
        if (value == null|| value == string.Empty)
        {
            return true; // Allow null values
        }

        string strValue = value as string;
        return  !string.IsNullOrWhiteSpace(strValue);
    }

}
