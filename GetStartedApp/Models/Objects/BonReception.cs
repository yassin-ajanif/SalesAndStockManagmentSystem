using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStartedApp.Models.Objects
{
    public class BonReception
    {
        public string SupplierBlNumber { get; set; }
        public long ProductID { get; set; }
        public string ProductName { get; set; }
        public string SupplierName { get; set; }
        public string OperationTypeName { get; set; }
        public decimal CostProduct { get; set; }
        public DateTime OperationTime { get; set; }
        public int AddedStock1 { get; set; }
        public int AddedStock2 { get; set; }
        public int AddedStock3 { get; set; }
        public decimal TotalPrice { get; set; }

  

        // Parameterized constructor for easy instantiation
        public BonReception(string supplierBlNumber, long productID, string productName,
                           string supplierName, string operationTypeName,
                           decimal costProduct, DateTime operationTime,
                           int addedStock1, int addedStock2,
                           int addedStock3, decimal totalPrice)
        {
            if(supplierBlNumber== null) SupplierBlNumber= string.Empty;
            else SupplierBlNumber= supplierBlNumber;
            ProductID = productID;
            ProductName = productName;
            SupplierName = supplierName;
            OperationTypeName = operationTypeName;
            CostProduct = costProduct;
            OperationTime = operationTime;
            AddedStock1 = addedStock1;
            AddedStock2 = addedStock2;
            AddedStock3 = addedStock3;
            TotalPrice = totalPrice;
        }
    }

}
