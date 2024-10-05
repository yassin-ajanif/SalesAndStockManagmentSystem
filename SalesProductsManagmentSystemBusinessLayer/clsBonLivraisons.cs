using SalesProductsManagmentSystemDataLayer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesProductsManagmentSystemBusinessLayer
{
    public static class clsBonLivraisons
    {
        public static SqlDataReader GetSalesForClientsOrCompanies(
     string clientOrCompany,      // Changed parameter name from entityType to clientOrCompany
     DateTime startDate,
     DateTime endDate,
     int paymentType,
     decimal? minAmount = null,
     decimal? maxAmount = null,
     long? productID = null,
     string productName = null,
     int? saleID = null,           // SaleID parameter
     int? clientID = null,         // ClientID parameter
     int? companyID = null          // CompanyID parameter
 )
        {
            // Input validation for clientOrCompany
            if (string.IsNullOrWhiteSpace(clientOrCompany) ||
               (clientOrCompany != "Client" && clientOrCompany != "Company"))
            {
                throw new ArgumentException("Invalid entity type. It should be either 'Client' or 'Company'.");
            }

            // Call the data layer function and return the SqlDataReader
            return clsDataBonLivraisonsLayer.GetAllSalesForClientsOrCompanies(
                clientOrCompany,
                startDate,
                endDate,
                paymentType,
                minAmount,
                maxAmount,
                productID,
                productName,
                saleID,
                clientID,
                companyID
            );
        }


    }
}
