using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesProductsManagmentSystemDataLayer
{
    public static class clsDataChecks
    {
        private static string connectionString;
        private static SqlConnection connection;

        static clsDataChecks()
        {
            connectionString = ClsKeyConnection.connectionKey;
            connection = new SqlConnection(connectionString);
        }

        public static long GetCustomerChequeIDBySaleID(long saleID)
        {
            long customerChequeID = -1; // Default return value if not found


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetCustomerChequeIDBySaleID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@SaleID", saleID);

                    var chequeIDParam = new SqlParameter("@CustomerChequeID", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                    command.Parameters.Add(chequeIDParam);

                    connection.Open();
                    command.ExecuteNonQuery();

                    customerChequeID = (long)chequeIDParam.Value; // This will return -1 if not found, as set in the procedure
                }


                return customerChequeID; // This will return -1 if no ID is found


            }
        }
    }
}



