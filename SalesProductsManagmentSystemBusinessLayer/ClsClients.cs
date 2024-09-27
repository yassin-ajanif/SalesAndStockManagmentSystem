using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SalesProductsManagmentSystemBusinessLayer
{
    public static class ClsClients
    {
        public static bool AddClient(string clientName, string phoneNumber, string email)
        {
            // Perform basic validations before calling the data layer
            if (string.IsNullOrWhiteSpace(clientName))
            {
                Console.WriteLine("Client name is required.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(phoneNumber) || !IsValidPhoneNumber(phoneNumber))
            {
                Console.WriteLine("Invalid phone number.");
                return false;
            }

            // else if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
            // {
            //     Console.WriteLine("Invalid email address.");
            //     return false;
            // }

            else if (ClsDataAccessLayer.IsPhoneNumberExists(phoneNumber)) { return false; }

            // Call the data layer to add the client
            return ClsDataAccessLayer.AddNewClient(clientName, phoneNumber, email);
        }

        // Validation helper method for phone number
        private static bool IsValidPhoneNumber(string phoneNumber)
        {
            // Simple phone number validation (you can adjust this based on your requirements)
            return Regex.IsMatch(phoneNumber, @"^[0-9\-\+]{9,15}$");
        }

        // Validation helper method for email
        private static bool IsValidEmail(string email)
        {
            // Simple email validation
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public static List<string> GetClientNames_And_Their_Phones_As_String()
        {
            return ClsDataAccessLayer.GetClientNames_And_Their_Phones_As_String();
        }


        public static bool UpdateClient(string clientName, string oldPhoneNumber, string NewphoneNumber, string email)
        {
            // Perform validation if needed
            if (string.IsNullOrWhiteSpace(clientName) || string.IsNullOrWhiteSpace(NewphoneNumber))
            {
                return false; // Return false if any required field is empty
            }
            bool PhoneNumberIsChanged = oldPhoneNumber != NewphoneNumber;

            if (PhoneNumberIsChanged && ClsDataAccessLayer.IsPhoneNumberExists(NewphoneNumber)) { return false; }

            // Call the data layer to update the client information
            return ClsDataAccessLayer.UpdateClientInfo(clientName, oldPhoneNumber, NewphoneNumber, email);
        }

        public static bool DeleteClient(string phoneNumber)
        {
            // Optionally, add any business rules or validations here

            // Call the data access method
            return ClsDataAccessLayer.DeleteClientByPhoneNumber(phoneNumber);
        }

        public static bool GetClientInfo(ref int ClientID ,string phoneNumber, ref string clientName, ref string email)
        {

            return ClsDataAccessLayer.GetClientInfoByPhoneNumber(ref ClientID,phoneNumber, ref clientName, ref email);


        }

        public static void GetLastSaleClientId_And_Name(ref int clientID, ref string clientName)
        {
            ClsDataAccessLayer.GetLastSaleClientInfo(ref clientID, ref clientName);
        }


            // Function to retrieve client information by ID
            public static bool GetClientInfoById(int clientId, ref string clientName, ref string phoneNumber, ref string email)
            {
                // Initialize out parameters with default values
           

                // Call the data layer function to retrieve client information
                bool isClientFound = ClsDataAccessLayer.GetClientInfoById(
                    clientId,
                    ref clientName,
                    ref phoneNumber,
                    ref email
                );

                // Optional: Add any business-specific logic here, such as logging or additional validation
                if (isClientFound)
                {
                    // Business logic: Maybe log the success or format the data, etc.
                    Console.WriteLine("Client information retrieved successfully.");
                }
                else
                {
                    // Business logic: Handle the case where the client is not found
                    Console.WriteLine("Client not found.");
                }

                return isClientFound;
            }
        }


    

}
