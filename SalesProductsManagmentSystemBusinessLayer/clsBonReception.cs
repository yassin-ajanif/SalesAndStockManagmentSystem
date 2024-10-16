using SalesProductsManagmentSystemDataLayer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesProductsManagmentSystemBusinessLayer
{
    public class clsBonReception
    {

        public static SqlDataReader RetrieveBonReceptions(
            DateTime operationTimeStartDate,  // Required start date first
            DateTime operationTimeEndDate,    // Required end date second
            string supplierBLNumber = null,
            string supplierName = null,
            long? productID = null,
            string productName = null,
            string operationTypeName = null,
            decimal? costProduct = null,
            decimal? minTotalPrice = null,
            decimal? maxTotalPrice = null)
        {
            // Call the data layer function
            return clsDataLayerBonReception.GetBonReceptionsData(
                operationTimeStartDate,  // Pass non-nullable start date
                operationTimeEndDate,    // Pass non-nullable end date
                supplierBLNumber,
                supplierName,
                productID,
                productName,
                operationTypeName,
                costProduct,
                minTotalPrice,
                maxTotalPrice);
        }


    }

}

