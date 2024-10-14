﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics;
using ZXing;
using SkiaSharp;
using System.Runtime.InteropServices.WindowsRuntime;

namespace SalesProductsManagmentSystemBusinessLayer
{
    public class ClsProductManager
    {

        private static bool IsConnectionClosed()
        {
            // Console.WriteLine("there is no connection to database");
            if (!ClsDataAccessLayer.IsConnectionOpen())
            {
                Console.WriteLine("there is no connection to database");
                return true;
            }

            return false;

        }

        public static bool IsConnectionOpen()
        {
            return ClsDataAccessLayer.IsConnectionOpen();
        }

        public static bool IsProductID_AlreadyExisting(long productID)
        {

            if(ClsDataAccessLayer.IsProductIDExists(productID))
            {
                 return true;
            }

            return false;
        }

        // i commented this function until later to see its importance 
     //   public static bool IsProductAlreadyExistingInSalesDbTable(int productID)
     //   {
     //       if (IsProductID_AlreadyExisting(productID)) return false;
     //
     //       Console.WriteLine("product id is not existing before");
     //       
     //       return true;
     //   }
        public static bool IsProcudtName_AlreadyExisting(string procName)
        {
            if (ClsDataAccessLayer.IsProductNameExists(procName)) {

                 return true;
            }
            return false;
        
    }
        public static bool AreNegative(float price, float cost, int quantityInStock,int quantityInStock2,int quantityInStock3)
        {

            if (price < 0 || cost < 0 || quantityInStock < 0 || quantityInStock2 < 0 || quantityInStock3 < 0)
            {

                Console.WriteLine("price, cost or quantityInstock is negative or all them");
                return true;
            }

            else return false;
        }

        public static bool AddProduct(long productID, string productName, string description,
     float price, float cost, int quantityInStock,int quantityInStock2,int quantityInStock3, string categorySelected, byte[] selectedProductImage)
        {
            if (IsConnectionClosed())
            {
                return false;
            }

            if (AreNegative(price, cost, quantityInStock,quantityInStock2,quantityInStock3))
            {
                return false;
            }

            else if (IsProductID_AlreadyExisting(productID))
            {
                return false;
            }

            int TheCategoryID_oF_ThisCategorySelected;
            
                // Attempt to get the category ID
                TheCategoryID_oF_ThisCategorySelected = getProductCategoryIdFromProductCategoryName(categorySelected);

            // if something wrong happens like error thrown the value returned will be -1 so we should disable the operation
            if (TheCategoryID_oF_ThisCategorySelected == -1) return false;

            return ClsDataAccessLayer.AddProduct(
                productID, productName, description, price, cost, quantityInStock, quantityInStock2, quantityInStock3,TheCategoryID_oF_ThisCategorySelected, selectedProductImage);
        }


        public static bool UpdateProduct
            (long productID, string productName, string description,
            float price, float cost, int quantityInStock,int quantityInStock2,int quantityInstock3, string categorySelected, byte[] selectedProductImage)
        {

            if (IsConnectionClosed())
            {

                return false;
            }

            if (AreNegative(price, cost, quantityInStock,quantityInStock2,quantityInstock3))
            {

                return false;
            }

            int TheCategoryID_oF_ThisCategorySelected = getProductCategoryIdFromProductCategoryName(categorySelected);

            return ClsDataAccessLayer.UpdateProduct
                (productID, productName, description, price, cost, quantityInStock,quantityInStock2,quantityInstock3, TheCategoryID_oF_ThisCategorySelected,selectedProductImage);
        }
   
        public static bool UpdateProductQuantity(long productId, int newQuantityOfProduct_StockQuantity, int newQuantityOfProduct_StockQuantity2, int newQuantityOfProduct_StockQuantity3)
        {
            return ClsDataAccessLayer.UpdateProductQuantity(productId, newQuantityOfProduct_StockQuantity, newQuantityOfProduct_StockQuantity2, newQuantityOfProduct_StockQuantity3);
        }

        public static bool UpdateProductPriceOrCost(long productID, float coast, float price)
        {
            return ClsDataAccessLayer.UpdateProductPriceAndCost(productID, coast, price);
        }
        public static bool DeleteProduct(long productId)
        {
 
            if (IsConnectionClosed() || IsProductId_ExistingIn_SalesItemTable(productId))
            {

                return false;
            }

            return ClsDataAccessLayer.DeleteProduct(productId);
        }
    

        private static int getProductCategoryIdFromProductCategoryName(string productCategoryName)
        {
            return ClsDataAccessLayer.getProductCategoryIdFromProductCategoryName(productCategoryName);
        }

        public static List<String> getProductsCategories()
        {

            // this function is connection open check if the connection is open it isn't it try to connect 
            // if it couldn't it returns false which means it couldn't reconnect 
            // so in tis case we return an empty list of string to ui
          
            if(IsConnectionOpen()) return ClsDataAccessLayer.getProductsCategories();

            return new List<string> { };
        }

        public static SqlDataReader GetProductsInfoReader()
        {
            return ClsDataAccessLayer.GetProductsInfoReader();
        }

        public static SqlDataReader GetProductsInfoReaderByCategory(string selectedCategory) {

            //int TheCategoryID_oF_ThisCategorySelected = getProductCategoryIdFromProductCategoryName(selectedCategory);

            return ClsDataAccessLayer.GetProductsInfoReaderByCategory(selectedCategory);
        }

        public static SqlDataReader GetProductsInfoListBy_CategoryName_And_SearchProductName(string selectedCategory,string searchTerm)
        {
            return ClsDataAccessLayer.GetProductsInfoListReaderBy_CategoryName_And_SearchProductName(selectedCategory,searchTerm);
        }

        public static SqlDataReader GetProductsInfoListReaderBy_CategoryName_And_SearchProductID(string selectedCategory, decimal searchNumber)
        {
            return ClsDataAccessLayer.GetProductsInfoListReaderBy_CategoryName_And_ProductID(selectedCategory, searchNumber);
        }

        public static bool IsProductId_ExistingIn_SalesItemTable(long productId)
        {
            return ClsDataAccessLayer.IsProductIdExistingInSalesItemTable(productId);
        }

        public static long GetNewProductID()
        {
            return ClsDataAccessLayer.GetNewProductID();
        }

        public static SqlDataReader GetProductById(long productId)
        {
            return ClsDataAccessLayer.GetProductById(productId);
        }

        public static byte[] GetImageOfProductById(long id)
        {
            return ClsDataAccessLayer.GetImageOfProductById(id);
        }

       public static bool InsertIntoReturnedProducts(long productId,string productName,int quantity,float sellingPrice,float profit)
        {
            return ClsDataAccessLayer.InsertIntoReturnedProducts(productId, productName, quantity, sellingPrice, profit);
        }

        public static bool IsLowStock()
        {
            return ClsDataAccessLayer.IsLowStock();
        }

        public static Dictionary<string, long> GetProductsByStartingLetter(string prefix)
        {
            // You can call the data access method to get products starting with the specified prefix
            return ClsDataAccessLayer.GetProductsByStartingLetter(prefix);
        }

        public static bool DoesProductNameAlreadyExist(string productName, int mode, long currentProductId)
        {
            // Call the data access method to check if the product name already exists
            return ClsDataAccessLayer.DoesProductNameAlreadyExist(productName, mode, currentProductId);
        }

        public static long GetProductIDFromProductName(string productName)
        {
            // Call the data access method to get the ProductID from the product name
            return ClsDataAccessLayer.GetProductIDFromProductName(productName);
        }

        public static bool AddOrUpdateProducts(DataTable productTable)
        {
            // Validate the DataTable before proceeding
            if (productTable == null || productTable.Rows.Count == 0)
            {
                Console.WriteLine("Error: The product table is null or empty.");
                return false;
            }

            foreach (DataRow row in productTable.Rows)
            {
                // Perform validation for each row
                if (row["Name"] == DBNull.Value || string.IsNullOrWhiteSpace(row["Name"].ToString()))
                {
                    Console.WriteLine("Error: Product Name is required.");
                    return false;
                }

                if (row["Price"] == DBNull.Value || Convert.ToDecimal(row["Price"]) < 0)
                {
                    Console.WriteLine("Error: Product Price must be a non-negative value.");
                    return false;
                }

                if (row["StockQuantity"] == DBNull.Value || Convert.ToInt32(row["StockQuantity"]) < 0)
                {
                    Console.WriteLine("Error: Stock Quantity must be a non-negative value.");
                    return false;
                }

                // Additional validations for other columns can be added as needed
                // For example, if SelectedCategory should be an integer, you might want to validate it as well
                if (row["SelectedCategory"] == DBNull.Value || string.IsNullOrWhiteSpace(row["SelectedCategory"].ToString()))
                {
                    Console.WriteLine("Error: Selected Category is required.");
                    return false;
                }
            }

            // Call the data layer function if all validations pass
            bool result = ClsDataAccessLayer.AddOrUpdateProducts(productTable);

            if (result)
            {
                Console.WriteLine("Products added/updated successfully.");
            }
            else
            {
                Console.WriteLine("Failed to add/update products.");
            }

            return result;
        }
    }


}

