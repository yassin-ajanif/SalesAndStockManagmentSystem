using SalesProductsManagmentSystemDataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesProductsManagmentSystemBusinessLayer
{
    public class clsChecks
    {

       public static long GetCustomerChequeIDBySaleID(int saleID)
        {
            return clsDataChecks.GetCustomerChequeIDBySaleID(saleID);
        }
    }
}
