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
                int? bonReceptionID = null,
                string supplierName = null,
                long? productID = null,
                string productName = null,
                string operationTypeName = null,
                decimal? costProduct = null,
                DateTime? operationTime = null,
                decimal? minTotalPrice = null,
                decimal? maxTotalPrice = null)
            {
                // Call the data layer function
                return clsDataLayerBonReception.GetBonReceptionsData(
                    bonReceptionID,
                    supplierName,
                    productID,
                    productName,
                    operationTypeName,
                    costProduct,
                    operationTime,
                    minTotalPrice,
                    maxTotalPrice);
            }
        }

    }

