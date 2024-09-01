using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Transactions;
using SalesProductsManagmentSystemBusinessLayer;
using System.Data.SqlClient;
using GetStartedApp.Helpers;
using Avalonia.Media.Imaging;
using System.IO;
using Avalonia;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Security.Cryptography;
using GetStartedApp.ViewModels;


namespace GetStartedApp.Models
{
    public static class AccessToClassLibraryBackendProject
    {

        public static Bitmap? noImageImage { get; set; }

        public static bool InsertFirstRunDateAndSetTrialOrPaidMode(bool isPaid)
        {
            return SalesProductsManagmentSystemBusinessLayer.ClsLoginManager.InsertFirstRunDateAndSetTrialOrPaidMode(isPaid);
        }

        public static DateTime? GetFirstRunDate()
        {
            return SalesProductsManagmentSystemBusinessLayer.ClsLoginManager.GetFirstRunDate();
        }

        public static bool IsApplicationUsersInPaidMode()
        {
            return SalesProductsManagmentSystemBusinessLayer.ClsLoginManager.IsApplicationUsersInPaidMode();
        }
        public static List<String> GetProductsCategoryFromDatabase()
        {

            return SalesProductsManagmentSystemBusinessLayer.ClsProductManager.getProductsCategories();

        }

        public static bool IsThisProductIdAlreadyExist(long productID)
        {
            return SalesProductsManagmentSystemBusinessLayer.ClsProductManager.IsProductID_AlreadyExisting(productID);
        }

        public static bool AddProductToDataBase(ProductInfo AddedProduct)
        {

            // we converted selected image product from avalonia image form to byte[] as binary form to store it into database

            return SalesProductsManagmentSystemBusinessLayer.ClsProductManager.
            AddProduct(AddedProduct.id, AddedProduct.name,
                       AddedProduct.description, AddedProduct.price,
                       AddedProduct.cost, AddedProduct.StockQuantity,
                       AddedProduct.selectedCategory,
                    ImageConverter.BitmapToByteArray(AddedProduct.SelectedProductImage)
                    );

        }

        public static bool IsValidImage(byte[] imageData)
        {
            try
            {
                using (var ms = new MemoryStream(imageData))
                {
                    var bitmap = new Bitmap(ms);
                }
                return true;
            }
            catch (Exception)
            {
                // If an exception is thrown, the byte array is not a valid image
                return false;
            }
        }

        // this empty image is actually an image with small size that indicate a user didint sotre any image of a given product
        public static Bitmap EmptyImage()
        {

            return ImageHelper.LoadFromResource(new Uri("avares://GetStartedApp/Assets/Icons/NoImageImage.png"));

        }
        private static Bitmap ReadDbImageAndConvertItToBitmap(SqlDataReader reader, string imageDataColumn)
        {
            Bitmap bitmap = EmptyImage();

            if (!reader.IsDBNull(reader.GetOrdinal(imageDataColumn)))
            {
                var imageData = (byte[])reader[imageDataColumn];

                if (IsValidImage(imageData)) bitmap = ImageConverter.ByteArrayToBitmap(imageData);

                return bitmap;
            }

            return bitmap;
        }

        /*
         * in all proudct list retrival i made image loades as null to speed the loading time
         * this approach it help me to only load the image i need which are the PRODUCT image a user 
         * will be hovering over
         * for this reason i made this method below that load the image separetly 
         * but to use the image loaded from database we need to convert it to bitmap
         */


        public static Bitmap getImageOfProductFromDatabaseById(long id)
        {
            var ImageLoadedFromDatabase = SalesProductsManagmentSystemBusinessLayer.ClsProductManager.GetImageOfProductById(id);

            if (ImageLoadedFromDatabase == null || ImageConverter.ByteArrayToBitmap(ImageLoadedFromDatabase) == null) return EmptyImage();

            return ImageConverter.ByteArrayToBitmap(ImageLoadedFromDatabase);
        }

        public static List<ProductInfo> GetProductsInfoList()
        {
            var products = new List<ProductInfo>();

            // this is a table of productinfo readed or retrived from database
            SqlDataReader reader = SalesProductsManagmentSystemBusinessLayer.ClsProductManager.GetProductsInfoReader();

            while (reader.Read())
            {
                long id = (long)reader.GetInt64(reader.GetOrdinal("ProductID"));
                var name = reader.GetString(reader.GetOrdinal("ProductName"));
                var description = reader.GetString(reader.GetOrdinal("Description"));
                var stockQuantity = reader.GetInt32(reader.GetOrdinal("QuantityInStock"));
                var price = (float)reader.GetDouble(reader.GetOrdinal("Price"));
                var cost = (float)reader.GetDouble(reader.GetOrdinal("Cost"));
                var selectedCategory = reader.GetString(reader.GetOrdinal("CategoryName"));
                // byte[] imageData = reader["ImageData"] as byte[];
                Bitmap imageData = null;
                var product = new ProductInfo(id, name, description, stockQuantity, price, cost, imageData, selectedCategory);
                products.Add(product);
            }



            reader.Close();

            return products;
        }

        public static List<ProductInfo> GetProductsInfoListByCategories(string selectedCategoryByUser)
        {
            var products = new List<ProductInfo>();

            // this is a table of productinfo readed or retrived from database
            SqlDataReader reader = SalesProductsManagmentSystemBusinessLayer.ClsProductManager.GetProductsInfoReaderByCategory(selectedCategoryByUser);

            while (reader.Read())
            {
                long id = (long)reader.GetInt64(reader.GetOrdinal("ProductID"));
                var name = reader.GetString(reader.GetOrdinal("ProductName"));
                var description = reader.GetString(reader.GetOrdinal("Description"));
                var stockQuantity = reader.GetInt32(reader.GetOrdinal("QuantityInStock"));
                var price = (float)reader.GetDouble(reader.GetOrdinal("Price"));
                var cost = (float)reader.GetDouble(reader.GetOrdinal("Cost"));
                var selectedCategory = reader.GetString(reader.GetOrdinal("CategoryName"));
                //var DatabaseImageConvertedToBitmp = ReadDbImageAndConvertItToBitmap(reader, "ImageData");
                Bitmap imageData = null;


                var product = new ProductInfo(id, name, description, stockQuantity, price, cost, imageData, selectedCategory);
                products.Add(product);
            }

            reader.Close();

            return products;
        }

        public static List<ProductInfo> GetProductsInfoListBy_CategoryName_And_SearchProductName(string SelectedCategory, string searchTerm)
        {
            var products = new List<ProductInfo>();

            // this is a table of productinfo readed or retrived from database
            SqlDataReader reader = SalesProductsManagmentSystemBusinessLayer.ClsProductManager.
                GetProductsInfoListBy_CategoryName_And_SearchProductName(SelectedCategory, searchTerm);

            while (reader.Read() && searchTerm != null)
            {
                long id = (long)reader.GetInt64(reader.GetOrdinal("ProductID"));
                var name = reader.GetString(reader.GetOrdinal("ProductName"));
                var description = reader.GetString(reader.GetOrdinal("Description"));
                var stockQuantity = reader.GetInt32(reader.GetOrdinal("QuantityInStock"));
                var price = (float)reader.GetDouble(reader.GetOrdinal("Price"));
                var cost = (float)reader.GetDouble(reader.GetOrdinal("Cost"));
                var selectedCategory = reader.GetString(reader.GetOrdinal("CategoryName"));
                Bitmap imageData = null;
                // var DatabaseImageConvertedToBitmp = ReadDbImageAndConvertItToBitmap(reader, "ImageData");


                var product = new ProductInfo(id, name, description, stockQuantity, price, cost, imageData, selectedCategory);
                products.Add(product);
            }

            reader.Close();

            return products;
        }


        public static ProductInfo GetProductInfoByBarCode(long scannedBarCode)
        {
            // in our case a barcode is a productid name

            using (SqlDataReader reader = SalesProductsManagmentSystemBusinessLayer.ClsProductManager.GetProductById(scannedBarCode))
            {


                if (reader.Read())
                {
                    long id = (long)reader.GetInt64(reader.GetOrdinal("ProductID"));
                    var name = reader.GetString(reader.GetOrdinal("ProductName"));
                    var description = reader.GetString(reader.GetOrdinal("Description"));
                    var stockQuantity = reader.GetInt32(reader.GetOrdinal("QuantityInStock"));
                    var price = (float)reader.GetDouble(reader.GetOrdinal("Price"));
                    var cost = (float)reader.GetDouble(reader.GetOrdinal("Cost"));
                    var selectedCategory = reader.GetString(reader.GetOrdinal("CategoryName")).ToString();
                    var DatabaseImageConvertedToBitmp = ReadDbImageAndConvertItToBitmap(reader, "ImageData");


                    var product = new ProductInfo(id, name, description, stockQuantity, price, cost, DatabaseImageConvertedToBitmp, selectedCategory);

                    reader.Close();
                    return product;
                }



                else
                {

                    return null; // Or throw an exception if preferred
                }
            }
        }

        public static List<ProductSold> GetSoldProductsList(DateTime startDate, DateTime endDate)
        {
            var products = new List<ProductSold>();

            // Get data reader from the business layer
            SqlDataReader reader = SalesProductsManagmentSystemBusinessLayer.ClsSalesManager.GetSoldItems(startDate, endDate);

            while (reader.Read())
            {
                long productId = (long)reader.GetInt64(reader.GetOrdinal("ProductSoldId"));
                string productName = reader.GetString(reader.GetOrdinal("ProductSoldName"));
                float originalPrice = (float)reader.GetDouble(reader.GetOrdinal("OriginalPrice"));
                float soldPrice = (float)reader.GetDecimal(reader.GetOrdinal("SoldPrice"));
                int quantity = reader.GetInt32(reader.GetOrdinal("Quantity"));
                Bitmap image = null; // we don't return the iamge becuase its takes memory i made a system that once a user hover to the product an image is loaded from the database

                // Create ProductSold object and add to list
                var product = new ProductSold(productId, productName, originalPrice, soldPrice, quantity, image);
                products.Add(product);
            }

            reader.Close();

            return products;
        }

        public static List<ReturnedProduct> GetReturnedProductsList(DateTime startDate, DateTime endDate)
        {
            var products = new List<ReturnedProduct>();

            // Get data reader from the business layer
            SqlDataReader reader = SalesProductsManagmentSystemBusinessLayer.ClsSalesManager.GetReturnedProducts(startDate, endDate);

            while (reader.Read())
            {
                // Create ProductSold object
                long productId = (long)reader.GetInt64(reader.GetOrdinal("ProductReturnedId"));
                string productName = reader.GetString(reader.GetOrdinal("ProductReturnedName"));
                float originalPrice = (float)reader.GetDouble(reader.GetOrdinal("OriginalPrice"));
                float returnedPrice = (float)reader.GetDecimal(reader.GetOrdinal("ReturnedPrice"));
                int quantity = reader.GetInt32(reader.GetOrdinal("Quantity"));
                Bitmap image = null; // we don't return the iamge becuase its takes memory i made a system that once a user hover to the product an image is loaded from the database

                var returnedProduct = new ReturnedProduct(productId, productName, originalPrice, returnedPrice, quantity, image);


                products.Add(returnedProduct);
            }

            reader.Close();

            return products;
        }

        public static bool UpdateAllInfoProduct(ProductInfo EditedProduct)
        {
            return SalesProductsManagmentSystemBusinessLayer.ClsProductManager.
                  UpdateProduct(EditedProduct.id, EditedProduct.name,
                         EditedProduct.description, EditedProduct.price,
                         EditedProduct.cost, EditedProduct.StockQuantity,
                         EditedProduct.selectedCategory,
                      ImageConverter.BitmapToByteArray(EditedProduct.SelectedProductImage)
                      );

        }
        public static bool UpdateProductQuantity(long productId, int newQuantityOfProduct)
        {
            return SalesProductsManagmentSystemBusinessLayer.ClsProductManager.UpdateProductQuantity(productId, newQuantityOfProduct);
        }

        public static bool UpdateProductPriceOrCost(long productID, float coast, float price)
        {
            return SalesProductsManagmentSystemBusinessLayer.ClsProductManager.UpdateProductPriceOrCost(productID, coast, price);
        }

        public static bool DeleteProduct(long productId)
        {
            return SalesProductsManagmentSystemBusinessLayer.ClsProductManager.DeleteProduct(productId);
        }

        public static bool InsertNewCategoryOfProduct(string newProductCategory)
        {
            return SalesProductsManagmentSystemBusinessLayer.ClsCategoryProductManager.InsertNewCategory(newProductCategory);
        }

        public static bool EditCategoryOfProduct(string previousCategoryName, string newCategoryName)
        {
            return SalesProductsManagmentSystemBusinessLayer.ClsCategoryProductManager.updateCategoryName(previousCategoryName, newCategoryName);
        }

        public static bool DeleteCategory(string categoryName)
        {
            return SalesProductsManagmentSystemBusinessLayer.ClsCategoryProductManager.DeleteCategory(categoryName);
        }

        public static long GetNewProductIDFromDatabase()
        {
            return SalesProductsManagmentSystemBusinessLayer.ClsProductManager.GetNewProductID();
        }

        public static bool AddNewSaleToDatabase(DateTime SaleDateTime, float TotalPrice, DataTable SoldProductList)
        {
            return SalesProductsManagmentSystemBusinessLayer.ClsSalesManager.SaveNewSaleOperationToDatabase(SaleDateTime, TotalPrice, SoldProductList);
        }


        public static bool InsertIntoReturnedProducts(long productId, string productName, int quantity, float sellingPrice, float profit)
        {
            return SalesProductsManagmentSystemBusinessLayer.
                  ClsProductManager.InsertIntoReturnedProducts
                  (productId, productName, quantity, sellingPrice, profit);
        }

        public static decimal GetTotalProfit(DateTime startTime, DateTime endTime)
        {
            return SalesProductsManagmentSystemBusinessLayer.ClsSalesManager.GetTotalProfit(startTime, endTime);
        }

        public static decimal GetTotalRevenue(DateTime startTime, DateTime endTime)
        {
            return SalesProductsManagmentSystemBusinessLayer.ClsSalesManager.GetTotalRevenue(startTime, endTime);
        }

        // we check if the admin has signed before if yes it will be directed to a login page
        // if not it will directed to sing up page 
        public static bool CheckIfAdminHasSignedBefore()
        {

            return SalesProductsManagmentSystemBusinessLayer.ClsLoginManager.CheckIfLogNamesIncludeAdminAndUser();

        }

        public static bool InsertWhenAdminDidintSignedOut_Or_UpdateWhenAdminHasAlreadySignedOut_Logins(string logNameAdmin, string passwordAdmin, string nationalIDOFAdmin,
        string logNameUser, string passwordUser, string nationalIDOFUser)
        {
            return SalesProductsManagmentSystemBusinessLayer.ClsLoginManager.
                InsertOrUpdateAdminAndUserLogins(logNameAdmin, passwordAdmin, nationalIDOFAdmin, logNameUser, passwordUser, nationalIDOFUser);
        }

        public static bool isThisNationalidBelongstoadmin(string nationalIDOfAdmin)
        {

            return SalesProductsManagmentSystemBusinessLayer.ClsLoginManager.isThisNationalidBelongstoadmin(nationalIDOfAdmin);
        }

        public static bool CheckIfUserOrAdminPasswordIsCorrect(string userType, string password)
        {


            return SalesProductsManagmentSystemBusinessLayer.
                ClsLoginManager.CheckIfUserOrAdminPasswordIsCorrect(userType, password);

        }

        public static bool CheckIfProductAlreadyExistingInSoldItemsList(long productid)
        {
            return SalesProductsManagmentSystemBusinessLayer.ClsLoginManager.CheckIfProductAlreadyExistingInSoldItemsList(productid);
        }

        // if the in the database one of the proudct the product found has a quanity less than 10 this function will return true
        public static bool IsLowStock()
        {
            return ClsDataAccessLayer.IsLowStock();
        }

        public static bool IsCompanyLogoExisting()
        {
            return SalesProductsManagmentSystemBusinessLayer.ClsLoginManager.IsCompanyLogoExisting();
        }

        public static DataTable GetCompanyDetails()
        {
            return SalesProductsManagmentSystemBusinessLayer.ClsLoginManager.GetCompanyDetails();
        }

        public static async Task<bool> UpdateCompanyDetailsAsync(byte[] companyLogo, string companyName, string companyLocation,string ICE, string TaxProfeesionalID, string TaxID)
        {
            return await SalesProductsManagmentSystemBusinessLayer.ClsLoginManager.UpdateCompanyDetailsAsync(companyLogo, companyName, companyLocation,ICE,TaxProfeesionalID,TaxID);
        }

        public static void GenerateBls(DataTable ProductSoldTable, string companyName, byte[] companyLogo, string companyLocation,string ICE, string ProfessionalTaxID, string TaxID)
        {
          SalesProductsManagmentSystemBusinessLayer.ClsPdfGenerator.BlsPdf.GenerateBls(ProductSoldTable, companyName, companyLogo, companyLocation,ICE,ProfessionalTaxID,TaxID);
        }

        public static int GetMinimalStockValue()
        {
            return SalesProductsManagmentSystemBusinessLayer.ClsProgramPrameters.GetMinimalStockValue();
        }

        public static bool UpdateMinimalStockValue(int valueToUpdate)
        {
            return SalesProductsManagmentSystemBusinessLayer.ClsProgramPrameters.UpdateMinimalStockValue(valueToUpdate);
        }

        // this function check if there is no backup for today then create a full backup and store it into one drive location mentioned into json config file
        public static void DoDailyBackup()
        {
            SalesProductsManagmentSystemBusinessLayer.ClsBackup.DoDailyBackup();
        }
    }
}
