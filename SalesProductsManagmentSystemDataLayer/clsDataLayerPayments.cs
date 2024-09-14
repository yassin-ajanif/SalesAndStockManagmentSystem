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


    }
}
