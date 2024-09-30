using SalesProductsManagmentSystemDataLayer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesProductsManagmentSystemBusinessLayer
{
    public static class ClsCompanies
    {
        public static bool AddOrUpdateCompany(int companyId, byte[] companyLogo, string companyName, string companyLocation,
                                            string ice, string ifs, string email, string patente, string rc, string cnss, string phoneNumber, string city)
        {
           return clsDataLayerCompanies.AddOrUpdateCompany(companyId, companyLogo, companyName, companyLocation, ice, ifs, email, patente, rc, cnss, phoneNumber, city);
        }

       
            public static SqlDataReader RetrieveCompanyInfo(int companyId)
            {
                   // Call the DAL function and return the SqlDataReader
                    return clsDataLayerCompanies.GetCompanyInfo(companyId);
                
            }


        public static Dictionary<string, int> GetAllCompanyNames_And_Ids()
        {
            // Business logic could be added here if necessary
            return clsDataLayerCompanies.GetAllCompanyNames();
        }

    }

    }

