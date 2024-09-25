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

        public static bool AddOrUpdateCompany(int companyId, byte[] companyLogo, string companyName, string companyLocation,
                                         string ice, string ifs, string email, string patente, string rc, string cnss)
        {
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(connectionString);
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

                // Return true if the operation was successful
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                Console.WriteLine($"Error: {ex.Message}");
                return false; // Return false if an exception occurred
            }
            finally
            {
                // Ensure the connection is closed
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }



        public static SqlDataReader GetCompanyInfo(int companyId)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand("GetCompanyInfo", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@CompanyID", companyId);

            connection.Open();
            return command.ExecuteReader(CommandBehavior.CloseConnection); // Ensure connection closes when reader is closed
        }

        public static Dictionary<string, int> GetAllCompanyNames()
        {
            Dictionary<string, int> companies = new Dictionary<string, int>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetAllCompanyNames", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string companyName = reader["CompanyName"].ToString();
                        int companyID = Convert.ToInt32(reader["CompanyID"]);

                        // Add company name as key and company ID as value
                        if (!companies.ContainsKey(companyName)) // Prevent duplicate keys
                        {
                            companies.Add(companyName, companyID);
                        }
                    }
                }
            }

            return companies;
        }




    }
}



