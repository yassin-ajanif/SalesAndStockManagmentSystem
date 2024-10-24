using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesProductsManagmentSystemDataLayer
{
    public class clsDataLayerReturnProducts
    {

        private static string connectionString;
        private static SqlConnection connection;

        static clsDataLayerReturnProducts()
        {
            connectionString = ClsKeyConnection.connectionKey;
            connection = new SqlConnection(connectionString);
        }
        public static bool ReturnProductsBySaleID(int saleID, DataTable products)
        {
            bool success = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("ReturnProductsBy_SaleID_ProductsTable", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters
                    command.Parameters.AddWithValue("@SaleID", saleID);

                    // Set the products parameter (TVP)
                    SqlParameter productsParam = command.Parameters.AddWithValue("@Products", products);
                    productsParam.SqlDbType = SqlDbType.Structured;
                    productsParam.TypeName = "dbo.ProductToReturnInfo"; // TVP type in SQL

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        success = true; // If no exception, assume success
                    }
                    catch (SqlException ex)
                    {
                        // Log exception if necessary
                        Console.WriteLine("Error executing stored procedure: " + ex.Message);
                    }
                }
            }

            return success;
        }
    }
}
