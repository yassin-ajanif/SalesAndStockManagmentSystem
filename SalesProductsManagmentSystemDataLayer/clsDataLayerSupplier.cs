using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SalesProductsManagmentSystemDataLayer
{
    public static class clsDataLayerSupplier
    {
        private static string connectionString;
        private static SqlConnection connection;

        static clsDataLayerSupplier()
        {
            connectionString = ClsKeyConnection.connectionKey;
            connection = new SqlConnection(connectionString);
        }

        public static List<string> GetSupplierNamePhoneNumberCombo()
        {
            var supplierCombos = new List<string>();

            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand("GetAllSuppliers", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var supplierName = reader.GetString(reader.GetOrdinal("SupplierName"));
                            var phoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber"));
                            // there is a bug in avalonia rendrer when the string ends with charcther like > at the mode of rightTOleft for arabic lanauge this 
                            // character i mean ">" in this case dosent show up in it exact location
                            // so to fix that issue we must add a letter after that character which i stored width this variable
                            string letterThatFixesAvaloniaBug_And_IndicateIfItsClientOrSupplier = "f";
                            var combo = $"{supplierName}<{phoneNumber}>{letterThatFixesAvaloniaBug_And_IndicateIfItsClientOrSupplier}";

                            supplierCombos.Add(combo);
                        }
                    }
                }
            }

            return supplierCombos;
        }

        public static bool AddSupplier(
    string supplierName,
    string fiscalIdentifier,
    string patented,
    string rc,
    string cnss,
    string ice,
    string bankAccountNumber,
    string phoneNumber,
    string address)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("AddSupplier", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Adding parameters
                        command.Parameters.AddWithValue("@SupplierName", supplierName);
                        command.Parameters.AddWithValue("@FiscalIdentifier", (object)fiscalIdentifier ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Patented", (object)patented ?? DBNull.Value);
                        command.Parameters.AddWithValue("@RC", (object)rc ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CNSS", (object)cnss ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ICE", (object)ice ?? DBNull.Value);
                        command.Parameters.AddWithValue("@BankAccountNumber", (object)bankAccountNumber ?? DBNull.Value);
                        command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        command.Parameters.AddWithValue("@Address", (object)address ?? DBNull.Value);

                        // Execute the insert command
                        command.ExecuteNonQuery();

                        // Return true if at least one row is affected
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception if necessary
                Console.WriteLine($"Error: {ex.Message}");
                return false; // Return false in case of any errors
            }
        }



        public static bool UpdateSupplierByPhoneNumber(
      string oldPhoneNumber, // Old phone number used to find the supplier
      string newPhoneNumber, // New phone number to update if changed
      string supplierName,
      string fiscalIdentifier,
      string patented,
      string rc,
      string cnss,
      string ice,
      string accountNumber,
      string address)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("UpdateSupplierByPhoneNumber", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Adding parameters
                        command.Parameters.AddWithValue("@OldPhoneNumber", oldPhoneNumber); // old phone number
                        command.Parameters.AddWithValue("@NewPhoneNumber", newPhoneNumber); // new phone number
                        command.Parameters.AddWithValue("@SupplierName", supplierName);
                        command.Parameters.AddWithValue("@FiscalIdentifier", (object)fiscalIdentifier ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Patented", (object)patented ?? DBNull.Value);
                        command.Parameters.AddWithValue("@RC", (object)rc ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CNSS", (object)cnss ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ICE", (object)ice ?? DBNull.Value);
                        command.Parameters.AddWithValue("@AccountNumber", (Object)accountNumber ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Address", (object)address ?? DBNull.Value);

                        // Execute the update command
                        command.ExecuteNonQuery();

                        // Return true if at least one row is affected
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception if necessary
                Console.WriteLine($"Error: {ex.Message}");
                return false; // Return false in case of any errors
            }
        }


        public static bool DeleteSupplierByPhoneNumber(string phoneNumber)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("DeleteSupplierByPhoneNumber", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add the phone number parameter
                        command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

                        // Execute the delete command
                        command.ExecuteNonQuery();

                        // Return true if at least one row is affected
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception if necessary
                Console.WriteLine($"Error: {ex.Message}");
                return false; // Return false in case of any errors
            }
        }

        public static bool CheckIfSupplierFieldInfoIsAlreadyExisting(string fieldName, string fieldValue)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("CheckIfSupplierFieldInfoIsAlreadyExisting", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@FieldName", fieldName);
                        command.Parameters.AddWithValue("@FieldValue", fieldValue ?? (object)DBNull.Value);

                        // Execute the command and get the result
                        var result = (int)command.ExecuteScalar();

                        // Return true if conflict exists
                        return result == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception if necessary
                return false; // Return false in case of any errors
            }
        }


        public static bool GetSupplierInfoByPhoneNumber(
       string phoneNumber,
       ref string supplierName,
       ref string bankAccount,
       ref string fiscalIdentifier,
       ref string rc,
       ref string ice,
       ref string patented,
       ref string cnss,
       ref string address) // Added parameter
        {
            bool isSupplierFound = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("GetSupplierInfoByPhoneNumber", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                supplierName = reader["SupplierName"] as string;
                                bankAccount = reader["BankAccountNumber"] as string;
                                fiscalIdentifier = reader["FiscalIdentifier"] as string;
                                rc = reader["RC"] as string;
                                ice = reader["ICE"] as string;
                                patented = reader["Patented"] as string;
                                cnss = reader["CNSS"] as string;
                                address = reader["Address"] as string; // Retrieve address

                                isSupplierFound = true;
                            }
                            else
                            {
                                // Set default values if supplier is not found
                                supplierName = null;
                                bankAccount = null;
                                fiscalIdentifier = null;
                                rc = null;
                                ice = null;
                                patented = null;
                                cnss = null;
                                address = null; // Default value for address
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (logging, rethrowing, etc.)
                Console.WriteLine($"Error: {ex.Message}");
            }

            return isSupplierFound;
        }


        public static bool CheckSupplierBLNumberExists(string supplierName, string supplierBLNumber)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("CheckSupplierBLNumberExists", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SupplierName", supplierName);
                    command.Parameters.AddWithValue("@SupplierBLNumber", supplierBLNumber);

                    SqlParameter existsParam = new SqlParameter("@Exists", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(existsParam);

                    connection.Open();
                    command.ExecuteNonQuery();
                    return (bool)existsParam.Value;
                }
            }
        }

    }
}
