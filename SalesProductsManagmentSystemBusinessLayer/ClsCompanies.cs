using SalesProductsManagmentSystemDataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesProductsManagmentSystemBusinessLayer
{
    public static class ClsCompanies
    {
        public static void AddOrUpdateCompany(int companyId, byte[] companyLogo, string companyName, string companyLocation,
                                            string ice, string ifs, string email, string patente, string rc, string cnss)
        {
            clsDataLayerCompanies.AddOrUpdateCompany(companyId, companyLogo, companyName, companyLocation, ice, ifs, email, patente, rc, cnss);
        }
    }
}
