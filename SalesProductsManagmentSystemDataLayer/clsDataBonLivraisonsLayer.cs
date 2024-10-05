using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesProductsManagmentSystemDataLayer
{
    public static  class clsDataBonLivraisonsLayer
    {
             
            private static string connectionString;
            private static SqlConnection connection;

            static clsDataBonLivraisonsLayer()
            {
                connectionString = ClsKeyConnection.connectionKey;
                connection = new SqlConnection(connectionString);
            }

        public static SqlDataReader GetAllSalesForClientsOrCompanies
 (
     string clientOrCompany,      // Changed parameter name from entityType to clientOrCompany
     DateTime startDate,
     DateTime endDate,
     int paymentType,
     decimal? minAmount = null,
     decimal? maxAmount = null,
     long? productID = null,
     string productName = null,
     int? saleID = null,           // SaleID parameter
     int? clientID = null,         // ClientID parameter
     int? companyID = null          // CompanyID parameter
 )
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand("getAllSalesForClients_Or_Companies", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Add parameters
            command.Parameters.AddWithValue("@EntityType", clientOrCompany);  // Updated parameter name
            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);
            command.Parameters.AddWithValue("@PaymentType", paymentType);
            command.Parameters.AddWithValue("@MinAmount", (object)minAmount ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaxAmount", (object)maxAmount ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProductID", (object)productID ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProductName", (object)productName ?? DBNull.Value);
            command.Parameters.AddWithValue("@SaleID", (object)saleID ?? DBNull.Value);            // SaleID parameter
            command.Parameters.AddWithValue("@ClientID", (object)clientID ?? DBNull.Value);      // ClientID parameter
            command.Parameters.AddWithValue("@CompanyID", (object)companyID ?? DBNull.Value);    // CompanyID parameter

            connection.Open();
            // Return SqlDataReader directly, so the caller is responsible for handling the reader and closing the connection
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }




    }
}



