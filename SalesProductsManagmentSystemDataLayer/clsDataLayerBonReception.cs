using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesProductsManagmentSystemDataLayer
{
    public class clsDataLayerBonReception
    {


        private static string connectionString;
        private static SqlConnection connection;

        static clsDataLayerBonReception()
        {
            connectionString = ClsKeyConnection.connectionKey;
            connection = new SqlConnection(connectionString);
        }

        public static SqlDataReader GetBonReceptionsData(
           DateTime operationTimeStartDate, // These parameters are required, not nullable
           DateTime operationTimeEndDate,
           string supplierBLNumber = null,
           string supplierName = null,
           long? productID = null,
           string productName = null,
           string operationTypeName = null,
           decimal? costProduct = null,
           decimal? minTotalPrice = null,
           decimal? maxTotalPrice = null
       )
        {
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("GetBonReceptions", conn)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            // Add parameters
            cmd.Parameters.AddWithValue("@SupplierBLNumber", (object)supplierBLNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SupplierName", (object)supplierName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ProductID", (object)productID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ProductName", (object)productName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@OperationTypeName", (object)operationTypeName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CostProduct", (object)costProduct ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@OperationTimeStartDate", operationTimeStartDate);  // Directly pass the non-nullable value
            cmd.Parameters.AddWithValue("@OperationTimeEndDate", operationTimeEndDate);      // Directly pass the non-nullable value
            cmd.Parameters.AddWithValue("@MinTotalPrice", (object)minTotalPrice ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MaxTotalPrice", (object)maxTotalPrice ?? DBNull.Value);

            conn.Open();

            // Execute and return SqlDataReader
            return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }

    }
}
