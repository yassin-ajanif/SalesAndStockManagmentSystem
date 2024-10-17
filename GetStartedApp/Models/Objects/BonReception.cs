using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetStartedApp.Helpers;

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
        public string OperationTime { get; set; }
        public int AddedStock1 { get; set; }
        public int AddedStock2 { get; set; }
        public int AddedStock3 { get; set; }
        public decimal TotalPrice { get; set; }
        public int TotalStock { get; set; } 
        public int ProductsNumberPerReception { get; set; }
        public string PaymentType { get; set; }



        // Parameterized constructor for easy instantiation
        public BonReception(string supplierBlNumber, long productID, string productName,
                           string supplierName, string operationTypeName,
                           decimal costProduct, DateTime operationTime,
                           int addedStock1, int addedStock2,
                           int addedStock3, decimal totalPrice, int ProductsNumberPerReception,string PaymentType)
        {
            SupplierBlNumber = supplierBlNumber ?? "غير معرف";
            SupplierName = supplierName ?? "غير معرف";
            ProductID = productID;
            ProductName = productName;
            OperationTypeName = operationTypeName == "added" ? "اضافة" : operationTypeName == "updated" ? "تحديت" : operationTypeName;
            CostProduct = costProduct;
            OperationTime = TimeHelper.FormatDateWithDayInArabic(operationTime);
            AddedStock1 = addedStock1;
            AddedStock2 = addedStock2;
            AddedStock3 = addedStock3;
            TotalPrice = totalPrice;
            this.ProductsNumberPerReception = ProductsNumberPerReception;
            this.TotalStock = addedStock1+addedStock2+addedStock3;
            this.PaymentType = PaymentType;
        }

       
    }

}
