using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesProductsManagmentSystemDataLayer
{
    public static class clsDataLayerCompanies
    {
        private static string connectionString;
        private static SqlConnection connection;

        static clsDataLayerCompanies()
        {
            connectionString = ClsKeyConnection.connectionKey;
            connection = new SqlConnection(connectionString);
        }

        public static void AddOrUpdateCompany(int companyId, byte[] companyLogo, string companyName, string companyLocation,
                                            string ice, string ifs, string email, string patente, string rc, string cnss)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("AddOrUpdateCompany", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@CompanyID", companyId);
                        command.Parameters.AddWithValue("@CompanyLogo", (object)companyLogo ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CompanyName", companyName);
                        command.Parameters.AddWithValue("@CompanyLocation", companyLocation);
                        command.Parameters.AddWithValue("@ICE", (object)ice ?? DBNull.Value);
                        command.Parameters.AddWithValue("@IFs", (object)ifs ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Email", (object)email ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Patente", (object)patente ?? DBNull.Value);
                        command.Parameters.AddWithValue("@RC", (object)rc ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Cnss", (object)cnss ?? DBNull.Value);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }



