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

        public static (bool Success, int SalesId) SaveNewSaleOperationToDatabase(
            DateTime SaleDateTime,
            float TotalPrice,
            DataTable SoldProductList,
            string clientNameAndPhoneNumberOrNormal,
            string selectedPaymentMethod,
            long? chequeNumber = null,
            decimal? amount = null,
            DateTime? chequeDate = null)
        {
            // Call the data access layer method
            return ClsDataAccessLayer.SaveNewSaleOperationToDatabase(
                SaleDateTime,
                TotalPrice,
                SoldProductList,
                clientNameAndPhoneNumberOrNormal,
                selectedPaymentMethod,
                chequeNumber,
                amount,
                chequeDate);
        }


        public static (bool isSuccess, int bonLivraisonNumber) SaveNewSaleOperationToDatabase_ForCompanies(
      DateTime SaleDateTime,
      float TotalPrice,
      DataTable SoldProductList,
      int companyID,
      string selectedPaymentMethod,
      long? chequeNumber = null,
      decimal? amount = null,
      DateTime? chequeDate = null)
        {
            // Call the data layer function and return its result as a tuple
            return ClsDataAccessLayer.SaveNewSaleOperationToDatabase_ForCompanies(
                SaleDateTime,
                TotalPrice,
                SoldProductList,
                companyID,
                selectedPaymentMethod,
                chequeNumber,
                amount,
                chequeDate);
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
