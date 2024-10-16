using SalesProductsManagmentSystemDataLayer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;

namespace SalesProductsManagmentSystemBusinessLayer
{
    public static class ClsSupplier
    {

        public static bool AddSupplier(
       string supplierName,
       string fiscalIdentifier,
       string patented,
       string rc,
       string cnss,
       string ice,
       string bankAccountNumber,
       string phoneNumber,
       string address)
        {
            // Call the Data Access Layer function
            return clsDataLayerSupplier.AddSupplier(
                supplierName,
                fiscalIdentifier,
                patented,
                rc,
                cnss,
                ice,
                bankAccountNumber,
                phoneNumber,
                address);
        }

        public static bool UpdateSupplierByPhoneNumber(
           string oldPhoneNumber,
           string newPhoneNumber,
           string supplierName,
           string fiscalIdentifier,
           string patented,
           string rc,
           string cnss,
           string ice,
           string accountNumber,
           string address)
        {
            // Validate inputs (you can add more validations as needed)
            if (string.IsNullOrEmpty(oldPhoneNumber) || string.IsNullOrEmpty(newPhoneNumber))
            {
                throw new ArgumentException("Phone numbers cannot be null or empty.");
            }

            // Call the data layer function to update supplier info
            return clsDataLayerSupplier.UpdateSupplierByPhoneNumber(
                oldPhoneNumber,
                newPhoneNumber,
                supplierName,
                fiscalIdentifier,
                patented,
                rc,
                cnss,
                ice,
                accountNumber,
                address
            );
        }


        public static List<string> GetSupplierNamePhoneNumberCombo()
        {
            // Get the list of supplier name and phone number combos
            return clsDataLayerSupplier.GetSupplierNamePhoneNumberCombo();
        }

        public static bool DeleteSupplierByPhoneNumber(string phoneNumber)
      {
          // Call the Data Access Layer function
          return clsDataLayerSupplier.DeleteSupplierByPhoneNumber(phoneNumber);
      }


        public static bool CheckIfSupplierFieldInfoIsAlreadyExisting(string fieldName, string fieldValue)
        {
           return clsDataLayerSupplier.CheckIfSupplierFieldInfoIsAlreadyExisting(fieldName,fieldValue);
        }

        public static void getSupplierInfo( 
                      string phoneNumber,
                      ref string supplierName,
                      ref string bankAccount,
                      ref string fiscalIdentifier,
                      ref string rc,
                      ref string ice,
                      ref string patented,
                      ref string cnss,
                      ref string address)
        {
            clsDataLayerSupplier.GetSupplierInfoByPhoneNumber(phoneNumber, ref supplierName, ref bankAccount, ref fiscalIdentifier, ref rc, ref ice, ref patented, ref cnss,ref address);
        }

   
            public static bool IsSupplierBLNumberAlreadyExisitg_For_ThisSupplierName(string supplierName, string supplierBLNumber)
            {
                // Call the data layer function
                return clsDataLayerSupplier.CheckSupplierBLNumberExists(supplierName, supplierBLNumber);
            }
        

    }
}
