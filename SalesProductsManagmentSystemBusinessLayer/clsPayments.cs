using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalesProductsManagmentSystemDataLayer;

namespace SalesProductsManagmentSystemBusinessLayer
{
    public static class clsPayments
    {
        public static List<string> GetPaymentTypes()
        {
              return clsDataLayerPayments.GetPaymentTypes();
        }

        public static int GetPaymentTypeID(string paymentType)
        {
            // Directly calling the static method from the data access layer
            return clsDataLayerPayments.GetPaymentTypeIdFromName(paymentType);
        }


          public static bool ExecuteProcessPayment(
       decimal? depositAmount,
       int saleId,
       string selectedPaymentMethod,
       decimal? checkAmount,
       string checkNumber,
       DateTime? checkDate)
          {
              try
              {
                  return clsDataLayerPayments.ProcessPayment(
                      depositAmount,
                      saleId,
                      selectedPaymentMethod,
                      checkAmount,
                      checkNumber,
                      checkDate);
              }
              catch (Exception ex)
              {
                  // Log the exception (if necessary)
                  Console.WriteLine("Error in ExecuteProcessPayment: " + ex.Message);
                  return false; // Return false if an exception occurs
              }
          }

    }
}
