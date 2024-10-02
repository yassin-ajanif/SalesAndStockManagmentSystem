using SalesProductsManagmentSystemDataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SalesProductsManagmentSystemDataLayer.ClsDataInvoicesLayer;

namespace SalesProductsManagmentSystemBusinessLayer
{
    public class ClsInvoices
    {
        public static int AddInvoiceIfNotExists(int saleID)
        {
            // Call the data access method to add a new invoice if it doesn't exist
            int invoiceID = ClsDataInvoicesLayer.AddNewInvoiceIfNotExisting(saleID);

            // You can add additional business logic here if needed

            return invoiceID;  // Return the InvoiceID or -1 if the SaleID exists
        }

        public static int GetInvoiceIDBySaleID(int saleID)
        {
            // Call the data access method to retrieve the InvoiceID by SaleID
            int invoiceID = ClsDataInvoicesLayer.GetInvoiceIDBySaleID(saleID);

            // You can add additional business logic here if needed

            return invoiceID;  // Return the InvoiceID or -1 if not found
        }
    }
}
