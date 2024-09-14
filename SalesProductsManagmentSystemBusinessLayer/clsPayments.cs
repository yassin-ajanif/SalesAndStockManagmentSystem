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
    }
}
