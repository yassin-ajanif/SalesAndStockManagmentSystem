using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace SalesProductsManagmentSystemBusinessLayer
{
    public class ClsBackup
    {
       
            private static string backupDirectory;
            private static string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Integrated Security=True;Connect Timeout=30;";

            public static void DoDailyBackup()
            {
                // Load configuration
                LoadConfiguration();

                string executablePath = Assembly.GetExecutingAssembly().Location;
                string parentDirectory = Directory.GetParent(executablePath).FullName;

                string databaseFilePath = Path.Combine(parentDirectory, "SalesAndStockManagmentSystem.mdf");
                string logFilePath = Path.Combine(parentDirectory, "SalesAndStockManagmentSystem_log.ldf");

            //    if (IsDatabaseBackupLoadedToday(backupDirectory, "SalesAndStockManagmentSystem.bak")) return;

                if (!IsDatabaseAttached("SalesAndStockManagmentSystem"))
                {
                    AttachDatabase("SalesAndStockManagmentSystem", databaseFilePath, logFilePath);
                }

                // Perform full backup first
                BackupDatabase_FullMode("SalesAndStockManagmentSystem");

                CloseConnection(new SqlConnection(connectionString));

               // Console.ReadKey();
            }

            public static bool IsDatabaseBackupLoadedToday(string directoryPath, string fileNamePattern)
        {
            // Get today's date formatted as yyyy-MM-dd
            string todayDate = DateTime.Now.ToString("yyyy-MM-dd");

            // Replace placeholder in the file name pattern with today's date
            string fileName = fileNamePattern.Replace("{date}", todayDate);

            // Combine the directory path with the file name
            string filePath = Path.Combine(directoryPath, fileName);

            // Check if the file exists
            if (File.Exists(filePath))
            {
                // Get the file creation or last write time
                DateTime fileCreationTime = File.GetCreationTime(filePath);
                DateTime fileLastWriteTime = File.GetLastWriteTime(filePath);

                // Check if the file's creation or last write time is today
                DateTime today = DateTime.Now.Date;
                if (fileCreationTime.Date == today || fileLastWriteTime.Date == today)
                {
                    return true;
                }
            }

            // If the file does not exist or was not created/modified today, return false
            return false;
        }
            private static void CloseConnection(SqlConnection connection)
            {
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    try
                    {
                        connection.Close();
                        Console.WriteLine("Database connection closed successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred while closing the database connection: {ex.Message}");
                    }
                }
            }

            private static void LoadConfiguration()
            {
                string configFilePath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");

                Console.WriteLine(configFilePath);

                if (File.Exists(configFilePath))
                {
                    try
                    {
                        string json = File.ReadAllText(configFilePath);
                        dynamic config = JsonConvert.DeserializeObject(json);
                        backupDirectory = config.backupDirectory;
                        if (!Directory.Exists(backupDirectory))
                        {
                            Console.WriteLine($"Backup directory does not exist. Creating directory: {backupDirectory}");
                            Directory.CreateDirectory(backupDirectory);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred while loading the configuration: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Configuration file not found.");
                 backupDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "");
                // Default value
            }
        }

            private static bool IsDatabaseAttached(string databaseName)
            {
                string checkDbCommand = $@"
            SELECT COUNT(*)
            FROM sys.databases
            WHERE name = '{databaseName}'";

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(checkDbCommand, connection))
                        {
                            int dbCount = (int)command.ExecuteScalar();
                            return dbCount > 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while checking if the database is attached: {ex.Message}");
                    return false;
                }
            }

            private static void AttachDatabase(string databaseName, string mdfFilePath, string ldfFilePath)
            {
                string attachDbCommand = $@"
            CREATE DATABASE [{databaseName}]
            ON (FILENAME = '{mdfFilePath}'),
               (FILENAME = '{ldfFilePath}')
            FOR ATTACH";

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(attachDbCommand, connection))
                        {
                            command.ExecuteNonQuery();
                            Console.WriteLine($"Database {databaseName} attached successfully.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while attaching the database: {ex.Message}");
                }
            }
        private static void LogToFile(string message)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string logFilePath = Path.Combine(desktopPath, "BackupLog.txt");

            // Append the message to the log file with a timestamp
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"{DateTime.Now}: {message}");
                }
            }
            catch (Exception ex)
            {
                // If logging fails, you might want to handle it differently, e.g., write to event log
                Console.WriteLine($"Failed to write to log file: {ex.Message}");
            }
        }
        
        private static void BackupDatabase_FullMode(string databaseName)
        {
            // Ensure the backup directory exists
            if (!Directory.Exists(backupDirectory))
            {
                LogToFile($"Backup directory '{backupDirectory}' does not exist.");
                return;
            }

            string backupFilePath = Path.Combine(backupDirectory, $"{databaseName}.bak");
            string backupCommand = $@"
            BACKUP DATABASE [{databaseName}]
            TO DISK = @BackupFilePath
            WITH FORMAT, INIT, NAME = 'Full Backup of {databaseName}'";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(backupCommand, connection))
                    {
                        // Use parameterized query to safely handle the file path
                        command.Parameters.AddWithValue("@BackupFilePath", backupFilePath);

                        command.ExecuteNonQuery();
                        LogToFile($"Full database backup of '{databaseName}' created successfully at '{backupFilePath}'.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception details for debugging
                LogToFile($"An error occurred while backing up the database '{databaseName}': {ex.Message}");
            }
        }

        private static void BackupDatabase_DifferentialMode(string databaseName)
            {
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string backupFilePath = Path.Combine(backupDirectory, $"{databaseName}_Differential_{timestamp}.bak");

                string backupCommand = $@"
            BACKUP DATABASE [{databaseName}]
            TO DISK = '{backupFilePath}'
            WITH DIFFERENTIAL, INIT, NAME = 'Differential Backup of {databaseName}'";

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(backupCommand, connection))
                        {
                            command.ExecuteNonQuery();
                            Console.WriteLine("Differential database backup created successfully.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while creating the differential backup: {ex.Message}");
                }
            }
        }
    }



