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
            return string.Empty;
        }

        try
        {
            // Find the index of the '-' character
            int dashIndex = input.IndexOf('-');

            // Check if the '-' character is found and it's not the last character
            if (dashIndex == -1 || dashIndex == input.Length - 1)
            {
                return string.Empty; // Return empty string if the format is incorrect
            }

            // Extract the phone number, which is the part after the '-'
            string phoneNumber = input.Substring(dashIndex + 1).Trim();

            return phoneNumber;
        }
        catch (Exception ex)
        {
            // Log or handle the exception if necessary
            Console.WriteLine($"Error: {ex.Message}");
            return string.Empty;
        }
    }

}
