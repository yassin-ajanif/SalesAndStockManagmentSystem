using System;

public static class PhoneNumberExtractor
{
    /// <summary>
    /// Extracts the phone number from a string formatted as 'name-"0611662541"'.
    /// The phone number is always after the sign and enclosed in quotes.
    /// </summary>
    /// <param name="input">The input string containing the phone number.</param>
    /// <returns>The extracted phone number, or an empty string if extraction fails.</returns>
    public static string ExtractPhoneNumber(string input)
    {
        // Check if the input is null or empty
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentException("Input cannot be null or empty.");
        }

        // Find the index of the '<' character
        int startIndex = input.IndexOf('<');
        // Find the index of the '>' character
        int endIndex = input.IndexOf('>');

        // Check if both characters are found and the indices are valid
        if (startIndex == -1 || endIndex == -1 || startIndex >= endIndex)
        {
            throw new FormatException("The input string is not in the correct format. Expected format: 'name<phoneNumber>'");
        }

        // Extract the phone number between '<' and '>'
        string phoneNumber = input.Substring(startIndex + 1, endIndex - startIndex - 1).Trim();

        // Ensure the phone number is not empty after extraction
        if (string.IsNullOrEmpty(phoneNumber))
        {
            throw new FormatException("No phone number found between the angle brackets.");
        }

        return phoneNumber;
    }


}
