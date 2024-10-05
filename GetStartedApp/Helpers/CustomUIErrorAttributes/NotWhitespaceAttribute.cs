using System;
using System.ComponentModel.DataAnnotations;

public class NotWhitespaceAttribute : ValidationAttribute
{
    public NotWhitespaceAttribute()
    {
        // No default error message, it will be passed by the user if needed
    }

    public override bool IsValid(object value)
    {
        if (value == null) return false; // Null values are not allowed

        // Convert to string and check if it's empty or just whitespace
        string strValue = value as string;
        return !string.IsNullOrWhiteSpace(strValue);
    }
}
