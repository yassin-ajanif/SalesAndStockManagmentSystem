using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesProductsManagmentSystemDataLayer
{
    public static class ClsDataInvoicesLayer
    {
        private static string connectionString;
        private static SqlConnection connection;

        static ClsDataInvoicesLayer()
        {
            connectionString = ClsKeyConnection.connectionKey;
            connection = new SqlConnection(connectionString);
        }

            public static int AddNewInvoiceIfNotExisting(int saleID)
            {
                int invoiceID;  // Declare the variable without an initial value

              
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("AddNewInvoice_IfNotExisting", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Input parameter for SaleID
                        command.Parameters.AddWithValue("@SaleID", saleID);

                        // Output parameter for InvoiceID
                        SqlParameter outputInvoiceID = new SqlParameter("@InvoiceID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(outputInvoiceID);

                        connection.Open();
                        command.ExecuteNonQuery();

                        // Get the value of the output parameter
                        invoiceID = Convert.ToInt32(outputInvoiceID.Value);
                    }
                }

                return invoiceID;  // Returns the InvoiceID or -1 if the SaleID already exists
            }
        
            public static int GetInvoiceIDBySaleID(int saleID)
        {
            int invoiceID;  // Declare without an initial value

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetInvoiceIDBySaleID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Input parameter for SaleID
                    command.Parameters.AddWithValue("@SaleID", saleID);

                    // Output parameter for InvoiceID
                    SqlParameter outputInvoiceID = new SqlParameter("@InvoiceID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputInvoiceID);

                    connection.Open();
                    command.ExecuteNonQuery();

                    // Get the value of the output parameter
                    invoiceID = Convert.ToInt32(outputInvoiceID.Value);
                }
            }

            return invoiceID;  // Returns the InvoiceID or -1 if not found
        }
  
    }
}
