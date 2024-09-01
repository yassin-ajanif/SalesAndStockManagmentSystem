using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesProductsManagmentSystemBusinessLayer
{
    public static class ClsProgramPrameters
    {
        public static int GetMinimalStockValue()
        {
            return ClsDataAccessLayer.GetMinimalStockValue();
        }

        public static bool UpdateMinimalStockValue(int valueToUpdate)
        {
            return ClsDataAccessLayer.UpdateMinimalStockValue(valueToUpdate);
        }
    }
}
