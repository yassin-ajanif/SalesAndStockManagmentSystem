using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace SalesProductsManagmentSystemDataLayer
{
    public class ClsKeyConnection
    {
        private static string EnterpriseDbConnectionString= "Server=.;Database=SalesAndStockManagmentSystem;User Id=sa;Password=123456;";
        private static string ExpressDbConnectionString= "Server=.\\SQLEXPRESS;Database=SalesAndStockManagmentSystem;Trusted_Connection=True";
        //  private static string localDbConnectionString = @"Server=(LocalDB)\MSSQLLocalDB;Integrated Security=true;AttachDbFileName=|DataDirectory|\SalesAndStockManagmentSystem.mdf;";
      
        private static string LocalDbConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;" +
            "AttachDbFilename=C:\\Users\\yassin\\AppData\\Local\\Microsoft\\Microsoft SQL Server Local DB\\Instances\\MSSQLLocalDB\\salesAndStockManagementDb.mdf;" +
            "Integrated Security=True;Connect Timeout=30;" +
            "TrustServerCertificate=true;";

        public static readonly string BinProjectLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);

        public static readonly string databaseProjectPath = Path.Combine(BinProjectLocation, "SalesAndStockManagmentSystem.mdf");

        private static string LocalDbConnectionStringProduction = "Data Source=(LocalDB)\\MSSQLLocalDB;" +
            "AttachDbFilename=" + databaseProjectPath + ";" +
            "Integrated Security=True;Connect Timeout=30;" +
            "TrustServerCertificate=true;";


    //   private static string LocalDbConnectionStringProduction1 = "Data Source=(LocalDB)\\MSSQLLocalDB;" +
    //       "AttachDbFilename=C:\\Users\\yassin\\source\\repos\\GetStartedApp\\salesAndStockManagementDb.mdf;" +
    //       "Integrated Security=True;Connect Timeout=30;" +
    //       "TrustServerCertificate=true;";


        public static string connectionKey = LocalDbConnectionStringProduction;
    }
}
