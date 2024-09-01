using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesProductsManagmentSystemBusinessLayer
{
    public class ClsSalesManager
    {

        public static bool SaveNewSaleOperationToDatabase(DateTime SaleDateTime,float TotalPrice, DataTable SoldProductList)
        {
          
            return ClsDataAccessLayer.SaveNewSaleOperationToDatabase(SaleDateTime, TotalPrice, SoldProductList);

        }


        public static decimal GetTotalProfit(DateTime startTime, DateTime endTime)
        {
            return ClsDataAccessLayer.GetTotalProfit(startTime, endTime);
        }

        public static decimal GetTotalRevenue(DateTime startTime, DateTime endTime)
        {   
            return ClsDataAccessLayer.GetTotalRevenue(startTime, endTime);

            
        }

        public static SqlDataReader GetSoldItems(DateTime startDate, DateTime endDate)
        {
            return ClsDataAccessLayer.GetSoldItems(startDate, endDate);
        }

        public static SqlDataReader GetReturnedProducts(DateTime startDate, DateTime endDate)
        {
            return ClsDataAccessLayer.GetReturnedProducts(startDate, endDate);
        }
    }
}
