using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesProductsManagmentSystemDataLayer
{
    public static class clsDataLayerPayments
    {
        private static string connectionString;
        private static SqlConnection connection;

        static clsDataLayerPayments()
        {
            connectionString = ClsKeyConnection.connectionKey;
            connection = new SqlConnection(connectionString);
        }
        public static List<string> GetPaymentTypes()
        {
            var paymentTypes = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("GetPaymentTypes", connection);
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        paymentTypes.Add(reader["PaymentType"].ToString());
                    }
                }
            }

            return paymentTypes;
        }

        public static int GetPaymentTypeIdFromName(string paymentType)
        {
            int paymentTypeID = -1;  // Default value if not found

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetPaymentTypeId_FromItsName", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Input parameter
                    command.Parameters.AddWithValue("@PaymentType", paymentType);

                    // Output parameter
                    SqlParameter outputParam = new SqlParameter("@PaymentTypeID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputParam);

                    connection.Open();
                    command.ExecuteNonQuery();

                    // Retrieve the value of the output parameter
                    paymentTypeID = (int)command.Parameters["@PaymentTypeID"].Value;
                }
            }

            return paymentTypeID;
        }


        public static bool ProcessPayment(
         decimal? depositAmount,
         int saleId,
         string selectedPaymentMethod,
         decimal? checkAmount,
         string checkNumber,
         DateTime? checkDate)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("ProcessPayment", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@DepositAmount", depositAmount ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@SaleID", saleId);
                        command.Parameters.AddWithValue("@SelectedPaymentMethod", selectedPaymentMethod);
                        command.Parameters.AddWithValue("@CheckAmount", checkAmount ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CheckNumber", checkNumber ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CheckDate", checkDate ?? (object)DBNull.Value);

                        // Execute the stored procedure
                        command.ExecuteNonQuery();
                    }
                }
                return true; // Return true if the operation was successful
            }
            catch (Exception ex)
            {
                // Log the exception (if necessary)
                Console.WriteLine("Error processing payment: " + ex.Message);
                return false; // Return false if an exception occurs
            }
        }
    }
}
