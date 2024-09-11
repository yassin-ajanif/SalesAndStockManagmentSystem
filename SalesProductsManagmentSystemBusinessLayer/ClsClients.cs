using System;
using System.Collections.Generic;
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

            else if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
            {
                Console.WriteLine("Invalid email address.");
                return false;
            }

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

        public static List<string> GetClientNames()
        {
            return ClsDataAccessLayer.GetClientNames();
        }

        public static bool UpdateClient(string clientName,string oldPhoneNumber, string NewphoneNumber, string email)
        {
            // Perform validation if needed
            if (string.IsNullOrWhiteSpace(clientName) || string.IsNullOrWhiteSpace(NewphoneNumber) || string.IsNullOrWhiteSpace(email))
            {
                return false; // Return false if any required field is empty
            }
            bool PhoneNumberIsChanged = oldPhoneNumber != NewphoneNumber;

             if(PhoneNumberIsChanged && ClsDataAccessLayer.IsPhoneNumberExists(NewphoneNumber)) { return false; }

            // Call the data layer to update the client information
            return ClsDataAccessLayer.UpdateClientInfo( clientName, oldPhoneNumber, NewphoneNumber, email);
        }

        public static bool DeleteClient(string phoneNumber)
        {
            // Optionally, add any business rules or validations here

            // Call the data access method
            return ClsDataAccessLayer.DeleteClientByPhoneNumber(phoneNumber);
        }
       
        public static bool GetClientInfo(string phoneNumber, ref string clientName , ref string email)
        {
           
            return ClsDataAccessLayer.GetClientInfoByPhoneNumber(phoneNumber, ref clientName, ref email);

          
        }

    }
}
