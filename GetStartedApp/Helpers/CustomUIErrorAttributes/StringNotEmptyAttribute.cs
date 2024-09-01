using System;
using System.ComponentModel.DataAnnotations;

public class StringNotEmptyAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value == null) return false;

        string strValue = value.ToString();
        return !string.IsNullOrWhiteSpace(strValue);
    }
}
