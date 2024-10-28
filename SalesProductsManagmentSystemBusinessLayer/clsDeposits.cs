using SalesProductsManagmentSystemDataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesProductsManagmentSystemBusinessLayer
{
    public static class clsDeposits
    {
        public static decimal RetrieveDepositAmountBySaleID(int saleID)
        {
            // Call the data layer method
            decimal depositAmount = clsDataDeposits.GetDepositAmountBySaleID(saleID);

            // Additional business logic can go here if needed

            return depositAmount;
        }
    }
}
