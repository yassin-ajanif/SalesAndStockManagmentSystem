using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesProductsManagmentSystemDataLayer
{
    public static class clsDataDeposits
    {

        private static string connectionString;
        private static SqlConnection connection;

        static clsDataDeposits()
        {
            connectionString = ClsKeyConnection.connectionKey;
            connection = new SqlConnection(connectionString);
        }
        public static decimal GetDepositAmountBySaleID(int saleID)
        {
            decimal depositAmount = -1;

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand("GetDepositAmountBySaleID", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@SaleID", saleID);

                var depositAmountParam = new SqlParameter("@DepositAmount", SqlDbType.Decimal)
                {
                    Direction = ParameterDirection.Output,
                    Precision = 10,
                    Scale = 2
                };
                command.Parameters.Add(depositAmountParam);

                connection.Open();
                command.ExecuteNonQuery();

                depositAmount = depositAmountParam.Value != DBNull.Value ? (decimal)depositAmountParam.Value : -1;
            }

            return depositAmount;
        }
    }
}
