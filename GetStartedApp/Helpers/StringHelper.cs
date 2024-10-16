using System.Text.RegularExpressions;

public static class StringHelper
{
    public static string ExtractNameFrom_Combo_NamePhoneNumber(string input)
    {
        // Regular expression to match the desired pattern
        if(input==null) return null;

        var match = Regex.Match(input, @"^(.*?)<");

        if (match.Success)
        {
            // Return the captured group, which is everything before '<'
            return match.Groups[1].Value.Trim();
        }

        // Return the input if no match is found
        return input.Trim();
    }
}