using SalesProductsManagmentSystemDataLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesProductsManagmentSystemBusinessLayer
{
    public class ClsReturnedProduct
    {

        public static bool ProcessProductReturns(int saleID, DataTable productsToReturn)
        {
            // Call the data layer function
            bool isSuccess = clsDataLayerReturnProducts.ReturnProductsBySaleID(saleID, productsToReturn);

            if (!isSuccess)
            {
                // Optionally handle logging or error handling here
                Console.WriteLine($"Product return process failed for SaleID: {saleID}");
            }

            return isSuccess;
        }
    }
}
