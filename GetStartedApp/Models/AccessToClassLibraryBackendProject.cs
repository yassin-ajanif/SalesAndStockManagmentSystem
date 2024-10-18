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
using SalesProductsManagmentSystemDataLayer;
using System.ComponentModel.Design;
using GetStartedApp.Models.Objects;
using Splat;


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
                       AddedProduct.cost, AddedProduct.StockQuantity, AddedProduct.StockQuantity2, AddedProduct.StockQuantity3,
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
                var stockQuantity2 = reader.GetInt32(reader.GetOrdinal("QuantityInStock2"));
                var stockQuantity3 = reader.GetInt32(reader.GetOrdinal("QuantityInStock3"));
                var price = (float)reader.GetDouble(reader.GetOrdinal("Price"));
                var cost = (float)reader.GetDouble(reader.GetOrdinal("Cost"));
                var selectedCategory = reader.GetString(reader.GetOrdinal("CategoryName"));
                // byte[] imageData = reader["ImageData"] as byte[];
                Bitmap imageData = null;
                var product = new ProductInfo(id, name, description, stockQuantity, stockQuantity2, stockQuantity3, price, cost, imageData, selectedCategory);
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
                var stockQuantity2 = reader.GetInt32(reader.GetOrdinal("QuantityInStock2"));
                var stockQuantity3 = reader.GetInt32(reader.GetOrdinal("QuantityInStock3"));
                var price = (float)reader.GetDouble(reader.GetOrdinal("Price"));
                var cost = (float)reader.GetDouble(reader.GetOrdinal("Cost"));
                var selectedCategory = reader.GetString(reader.GetOrdinal("CategoryName"));
                //var DatabaseImageConvertedToBitmp = ReadDbImageAndConvertItToBitmap(reader, "ImageData");
                Bitmap imageData = null;


                var product = new ProductInfo(id, name, description, stockQuantity, stockQuantity2, stockQuantity3, price, cost, imageData, selectedCategory);
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
                var stockQuantity2 = reader.GetInt32(reader.GetOrdinal("QuantityInStock2"));
                var stockQuantity3 = reader.GetInt32(reader.GetOrdinal("QuantityInStock3"));
                var price = (float)reader.GetDouble(reader.GetOrdinal("Price"));
                var cost = (float)reader.GetDouble(reader.GetOrdinal("Cost"));
                var selectedCategory = reader.GetString(reader.GetOrdinal("CategoryName"));
                Bitmap imageData = null;
                // var DatabaseImageConvertedToBitmp = ReadDbImageAndConvertItToBitmap(reader, "ImageData");


                var product = new ProductInfo(id, name, description, stockQuantity, stockQuantity2, stockQuantity3, price, cost, imageData, selectedCategory);
                products.Add(product);
            }

            reader.Close();

            return products;
        }

        public static List<ProductInfo> GetProductsInfoListBy_CategoryName_And_SearchProductID(string selectedCategory, decimal searchNumber)
        {
            var products = new List<ProductInfo>();

            // Retrieve data from the database using a function that calls the table-valued function
            SqlDataReader reader = SalesProductsManagmentSystemBusinessLayer.ClsProductManager.
                GetProductsInfoListReaderBy_CategoryName_And_SearchProductID(selectedCategory, searchNumber);

            while (reader.Read())
            {
                long id = reader.GetInt64(reader.GetOrdinal("ProductID"));
                var name = reader.GetString(reader.GetOrdinal("ProductName"));
                var description = reader.GetString(reader.GetOrdinal("Description"));
                var stockQuantity = reader.GetInt32(reader.GetOrdinal("QuantityInStock"));
                var stockQuantity2 = reader.GetInt32(reader.GetOrdinal("QuantityInStock2"));
                var stockQuantity3 = reader.GetInt32(reader.GetOrdinal("QuantityInStock3"));
                var price = (float)reader.GetDouble(reader.GetOrdinal("Price"));
                var cost = (float)reader.GetDouble(reader.GetOrdinal("Cost"));
                var category = reader.GetString(reader.GetOrdinal("CategoryName"));
                Bitmap imageData = null;

                // Optionally convert database image data to Bitmap
                // imageData = ReadDbImageAndConvertItToBitmap(reader, "ImageData");

                var product = new ProductInfo(id, name, description, stockQuantity, stockQuantity2, stockQuantity3, price, cost, imageData, category);
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
                    var stockQuantity2 = reader.GetInt32(reader.GetOrdinal("QuantityInStock2"));
                    var stockQuantity3 = reader.GetInt32(reader.GetOrdinal("QuantityInStock3"));
                    var price = (float)reader.GetDouble(reader.GetOrdinal("Price"));
                    var cost = (float)reader.GetDouble(reader.GetOrdinal("Cost"));
                    var selectedCategory = reader.GetString(reader.GetOrdinal("CategoryName")).ToString();
                    var DatabaseImageConvertedToBitmp = ReadDbImageAndConvertItToBitmap(reader, "ImageData");


                    var product = new ProductInfo(id, name, description, stockQuantity, stockQuantity2, stockQuantity3, price, cost, DatabaseImageConvertedToBitmp, selectedCategory);

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
                         EditedProduct.cost, EditedProduct.StockQuantity, EditedProduct.StockQuantity2, EditedProduct.StockQuantity3,
                         EditedProduct.selectedCategory,
                      ImageConverter.BitmapToByteArray(EditedProduct.SelectedProductImage)
                      );

        }
        public static bool UpdateProductQuantity(long productId, int newQuantityOfProduct_StockQuantity, int newQuantityOfProduct_StockQuantity2, int newQuantityOfProduct_StockQuantity3)
        {
            return SalesProductsManagmentSystemBusinessLayer.ClsProductManager.UpdateProductQuantity
                (productId, newQuantityOfProduct_StockQuantity, newQuantityOfProduct_StockQuantity2, newQuantityOfProduct_StockQuantity3);
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

        public static (bool Success, int SalesId) AddNewSaleToDatabase(
       DateTime SaleDateTime,
       float TotalPrice,
       DataTable SoldProductList,
       string clientNameAndPhoneNumberOrNormal,
       string selectedPaymentMethod,
       ChequeInfo userChequeInfo)
        {
            // Default values to be used if userChequeInfo is null
            long? chequeNumber = null;
            decimal? amount = null;
            DateTime? chequeDate = null;

            if (userChequeInfo != null)
            {
                chequeNumber = userChequeInfo.ChequeNumber;
                amount = userChequeInfo.Amount;
                chequeDate = userChequeInfo.ChequeDate; // Convert DateTime to DateTimeOffset
            }

            // Call the business layer method and return the result
            return SalesProductsManagmentSystemBusinessLayer.ClsSalesManager.SaveNewSaleOperationToDatabase(
                SaleDateTime,
                TotalPrice,
                SoldProductList,
                clientNameAndPhoneNumberOrNormal,
                selectedPaymentMethod,
                chequeNumber,
                amount,
                chequeDate
            );
        }


        public static (bool isSuccess, int bonLivraisonNumber) AddNewSaleToDatabase_ForCompanies(
     DateTime SaleDateTime,
     float TotalPrice,
     DataTable SoldProductList,
     int CompanyID,
     string selectedPaymentMethod,
     ChequeInfo userChequeInfo)
        {
            // Default values to be used if userChequeInfo is null
            long? chequeNumber = null;
            decimal? amount = null;
            DateTime? chequeDate = null;

            // Populate cheque details if provided
            if (userChequeInfo != null)
            {
                chequeNumber = userChequeInfo.ChequeNumber;
                amount = userChequeInfo.Amount;
                chequeDate = userChequeInfo.ChequeDate;
            }

            // Call the updated method from the business layer and return the result
            return SalesProductsManagmentSystemBusinessLayer.ClsSalesManager.SaveNewSaleOperationToDatabase_ForCompanies(
                SaleDateTime,
                TotalPrice,
                SoldProductList,
                CompanyID,
                selectedPaymentMethod,
                chequeNumber,
                amount,
                chequeDate
            );
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

        public static async Task<bool> UpdateCompanyDetailsAsync(byte[] companyLogo, string companyName, string companyLocation, string ICE, string TaxProfeesionalID, string TaxID)
        {
            return await SalesProductsManagmentSystemBusinessLayer.ClsLoginManager.UpdateCompanyDetailsAsync(companyLogo, companyName, companyLocation, ICE, TaxProfeesionalID, TaxID);
        }

        public static void GenerateBls
            (DataTable ProductSoldTable, string companyName, byte[] companyLogo, string companyLocation,
            string ICE, string ProfessionalTaxID, string TaxID, int lastSaleClientID, string lastSaleClientName)
        {
            SalesProductsManagmentSystemBusinessLayer.ClsPdfGenerator.BlsPdf.
                GenerateBls(ProductSoldTable, companyName, companyLogo, companyLocation, ICE, ProfessionalTaxID, TaxID, lastSaleClientID, lastSaleClientName);
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

        public static bool AddClient(string clientName, string phoneNumber, string email)
        {
            return SalesProductsManagmentSystemBusinessLayer.ClsClients.AddClient(clientName, phoneNumber, email);
        }

        public static List<string> GetClientNames_And_Their_Phones_As_String()
        {
            return SalesProductsManagmentSystemBusinessLayer.ClsClients.GetClientNames_And_Their_Phones_As_String();
        }

        public static bool UpdateClient(string clientName, string oldPhoneNumber, string NewPhoneNumber, string email)
        {
            return SalesProductsManagmentSystemBusinessLayer.ClsClients.UpdateClient(clientName, oldPhoneNumber, NewPhoneNumber, email);
        }

        public static bool DeleteClient(string phoneNumber)
        {
            // Optionally, add any business rules or validations here

            // Call the data access method
            return SalesProductsManagmentSystemBusinessLayer.ClsClients.DeleteClient(phoneNumber);
        }

        public static bool GetClientInfo(ref int ClientID, string phoneNumber, ref string clientName, ref string email)
        {

            return SalesProductsManagmentSystemBusinessLayer.ClsClients.GetClientInfo(ref ClientID, phoneNumber, ref clientName, ref email);


        }


        public static bool AddNewSupplierToDb(string supplierName,
       string fiscalIdentifier,
       string patented,
       string rc,
       string cnss,
       string ice,
       string bankAccountNumber,
       string phoneNumber,
       string address)
        {
            return SalesProductsManagmentSystemBusinessLayer.ClsSupplier.
               AddSupplier
               (supplierName,
                fiscalIdentifier,
                patented,
                rc,
                cnss,
                ice,
                bankAccountNumber,
                phoneNumber,
                address);
        }

        public static List<string> GetSupplierNamePhoneNumberCombo()
        {
            // Get the list of supplier name and phone number combos
            return SalesProductsManagmentSystemBusinessLayer.ClsSupplier.GetSupplierNamePhoneNumberCombo();
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
            clsDataLayerSupplier.GetSupplierInfoByPhoneNumber(phoneNumber, ref supplierName, ref bankAccount, ref fiscalIdentifier, ref rc, ref ice, ref patented, ref cnss, ref address);
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
            return SalesProductsManagmentSystemBusinessLayer.ClsSupplier.UpdateSupplierByPhoneNumber
                (oldPhoneNumber, newPhoneNumber, supplierName, fiscalIdentifier, patented, rc, cnss, ice, accountNumber, address);
        }

        public static bool DeleteSupplierByPhoneNumber(string phoneNumber)
        {
            // Call the Data Access Layer function
            return SalesProductsManagmentSystemBusinessLayer.ClsSupplier.DeleteSupplierByPhoneNumber(phoneNumber);
        }

        public static void GetLastSaleClientId_And_Name(ref int clientID, ref string clientName)
        {
            SalesProductsManagmentSystemBusinessLayer.ClsClients.GetLastSaleClientId_And_Name(ref clientID, ref clientName);
        }

        public static List<string> GetPaymentTypes()
        {
            return SalesProductsManagmentSystemBusinessLayer.clsPayments.GetPaymentTypes();
        }

        public static Dictionary<string, long> GetProductsByStartingLetter(string prefix)
        {
            // You can call the data access method to get products starting with the specified prefix
            return SalesProductsManagmentSystemBusinessLayer.ClsProductManager.GetProductsByStartingLetter(prefix);
        }

        public static bool DoesProductNameAlreadyExist(string productName, int mode, long currentProductId)
        {
            // Call the data access method to check if the product name already exists
            return SalesProductsManagmentSystemBusinessLayer.ClsProductManager.DoesProductNameAlreadyExist(productName, mode, currentProductId);
        }

        public static long GetProductIDFromProductName(string productName)
        {
            // Call the data access method to get the ProductID from the product name
            return SalesProductsManagmentSystemBusinessLayer.ClsProductManager.GetProductIDFromProductName(productName);
        }

        public static bool AddOrUpdateCompany(int companyId, byte[] companyLogo, string companyName, string companyLocation,
                                           string ice, string ifs, string email, string patente, string rc, string cnss,string phoneNumber, string city)
        {

            return SalesProductsManagmentSystemBusinessLayer.ClsCompanies.AddOrUpdateCompany(companyId, companyLogo, companyName, companyLocation, ice, ifs, email, patente, rc, cnss,phoneNumber,city);
        }

        public static CompanyInfo LoadCompanyInfo(int companyId)
        {
            // Call the business layer to retrieve the SqlDataReader
            SqlDataReader reader = null;
            CompanyInfo companyInfo = null;

            try
            {
                // Call the business layer function to get the SqlDataReader
                reader = SalesProductsManagmentSystemBusinessLayer.ClsCompanies.RetrieveCompanyInfo(companyId); // Assuming the business layer provides the reader

                // Check if the reader has any rows (data)
                if (reader.Read())
                {
                    // Load data into the CompanyInfo object
                    companyInfo = new CompanyInfo(
                        reader.GetInt32(reader.GetOrdinal("CompanyID")),
                        reader["CompanyLogo"] != DBNull.Value ? (byte[])reader["CompanyLogo"] : null,
                        reader.GetString(reader.GetOrdinal("CompanyName")),
                        reader.GetString(reader.GetOrdinal("CompanyLocation")),
                        reader["ICE"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("ICE")) : null,
                        reader["IFs"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("IFs")) : null,
                        reader["Email"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("Email")) : null,
                        reader["Patente"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("Patente")) : null,
                        reader["RC"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("RC")) : null,
                        reader["CNSS"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("CNSS")) : null,
                        reader.GetString(reader.GetOrdinal("PhoneNumber")),
                        reader.GetString(reader.GetOrdinal("City"))
                    );
                }
            }
            finally
            {
                // Ensure the reader is closed even in case of an exception
                reader?.Close();
            }

            // Return the populated CompanyInfo object, or null if no data was found
            return companyInfo;
        }

        public static Dictionary<string, int> GetAllCompanyNames_And_IDs()
        {
            // Business logic could be added here if necessary
            return SalesProductsManagmentSystemBusinessLayer.ClsCompanies.GetAllCompanyNames_And_Ids();
        }

        public static void GenerateDevis_ForClient(DataTable products, int clientID,string SelectedPaymentMethodInFrench,decimal TVA)
        {
            SalesProductsManagmentSystemBusinessLayer.ClsDevisGenerator devisGenerator = new ClsDevisGenerator(products, clientID, SelectedPaymentMethodInFrench,TVA);
            devisGenerator.GenerateDevis_ForClient();
        }

        public static void GenerateDevis_ForCompany( int companyID, DataTable products, string SelectedPaymentMethodInFrench,decimal TVA)
        {
            SalesProductsManagmentSystemBusinessLayer.ClsDevisGenerator devisGenerator = new ClsDevisGenerator(companyID, products, SelectedPaymentMethodInFrench, TVA);
            devisGenerator.GenerateDevis_ForCompany();
        }

        public static void GenerateBonLivraison_ForClient(DataTable products, int clientID, string SelectedPaymentMethodInFrench, decimal TVA, int SaleID,DateTime SalesTime)
        {
            SalesProductsManagmentSystemBusinessLayer.ClsBonLivraisonGenerator BlGenerator 
                = new ClsBonLivraisonGenerator(products, clientID, SelectedPaymentMethodInFrench, TVA, SaleID, SalesTime);

            BlGenerator.GenerateBlivraison_ForClient();
        }

       public static void GenerateBonLivraison_ForCompany(int companyID, DataTable products, string SelectedPaymentMethodInFrench, decimal TVA,int SaleID, DateTime SalesTime)
       {
           SalesProductsManagmentSystemBusinessLayer.ClsBonLivraisonGenerator BlGenerator = 
                new ClsBonLivraisonGenerator(companyID, products, SelectedPaymentMethodInFrench, TVA, SaleID, SalesTime);
    
           // Call further methods on devisGenerator if needed to generate the devis for the company
           BlGenerator.GenerateBlivraison_ForCompany();
       }

        public static void GenerateInvoice_ForClient(DataTable products, int clientID, string selectedPaymentMethodInFrench, decimal tva, int saleID, int invoiceID, DateTime SalesTime, 
            bool DoesUserHasPrintedInvoiceBefore)
        {
            // Create an instance of ClsInvoiceGenerator
            SalesProductsManagmentSystemBusinessLayer.ClsInvoiceGenerator invoiceGenerator
                = new ClsInvoiceGenerator(products, clientID, selectedPaymentMethodInFrench, tva, saleID,invoiceID, SalesTime, DoesUserHasPrintedInvoiceBefore);

            // Generate the invoice for the client
            invoiceGenerator.GenerateInvoice_ForClient();
        }

        public static void GenerateInvoice_ForCompany(int companyID, DataTable products, string selectedPaymentMethodInFrench, decimal tva, int saleID, int invoiceID, DateTime SalesTime, 
            bool DoesUserHasPrintedInvoiceBefore)
        {
            // Create an instance of ClsInvoiceGenerator
            SalesProductsManagmentSystemBusinessLayer.ClsInvoiceGenerator invoiceGenerator
                = new ClsInvoiceGenerator(companyID, products, selectedPaymentMethodInFrench, tva, saleID, invoiceID, SalesTime, DoesUserHasPrintedInvoiceBefore) ;

            // Generate the invoice for the company
            invoiceGenerator.GenerateInvoice_ForCompany();
        }

        public static void GenerateBonCommand(
       int companyID,
       DataTable products,
       string selectedPaymentMethodInFrench,
       decimal tva,
       int saleID,
       int invoiceID,
       DateTime salesTime,
       string supplierName,
       string phoneNumber,
       string email,
       string bankAccount,
       string fiscalIdentifier,
       string rc,
       string ice,
       string patented,
       string cnss,
       string address)
        {

            SalesProductsManagmentSystemBusinessLayer.clsBonCommandGenerator bonCommandGenerator =
                new clsBonCommandGenerator(
                    companyID,products,  selectedPaymentMethodInFrench, tva,saleID,salesTime, 
                    supplierName, phoneNumber,email,bankAccount,fiscalIdentifier,rc,ice,patented,cnss,address
                );

            bonCommandGenerator.GenerateBonCommand();
        }




        public static bool GetClientInfoById(int clientId, ref string clientName, ref string phoneNumber, ref string email)
        {
            return SalesProductsManagmentSystemBusinessLayer.ClsClients.GetClientInfoById(clientId, ref clientName, ref phoneNumber, ref email);
        }

        public static int AddInvoiceIfNotExists(int saleID)
        {

            return SalesProductsManagmentSystemBusinessLayer.ClsInvoices.AddInvoiceIfNotExists(saleID); 
        }

        public static int GetInvoiceIDBySaleID(int saleID)
        {
            return ClsInvoices.GetInvoiceIDBySaleID(saleID);
        }

        public static List<ClientOrCompanySaleInfo> LoadSalesForClientOrCompany(
    string clientOrCompany,
    DateTime startDate,
    DateTime endDate,
    int paymentType,
    decimal? minAmount = null,
    decimal? maxAmount = null,
    long? productID = null,
    string productName = null,
    int? saleID = null,
    int? clientID = null,
    int? companyID = null)
        {
            // List to hold the sales information
            List<ClientOrCompanySaleInfo> salesList = new List<ClientOrCompanySaleInfo>();

       

            // Get the data using the SqlDataReader
            using (SqlDataReader reader = clsBonLivraisons.GetSalesForClientsOrCompanies(
                clientOrCompany,
                startDate,
                endDate,
                paymentType,
                minAmount,
                maxAmount,
                productID,
                productName,
                saleID,
                clientID,
                companyID))
            {
                // Read the data from the reader
                while (reader.Read())
                {
                    // Create a unified ClientOrCompanySaleInfo object
                    ClientOrCompanySaleInfo saleInfo = new ClientOrCompanySaleInfo(
                        saleID: reader.GetInt32(reader.GetOrdinal("SaleID")),
                        timeOfOperation: reader.GetDateTime(reader.GetOrdinal("SaleDateTime")),
                        totalPrice: reader.GetDecimal(reader.GetOrdinal("TotalPrice")),
                        paymentID: reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                        paymentName: reader.GetString(reader.GetOrdinal("PaymentType")),
                        clientOrCompanyID: clientOrCompany == "Client"
                            ? reader.GetInt32(reader.GetOrdinal("ClientID"))
                            : reader.GetInt32(reader.GetOrdinal("CompanyID")),
                        clientOrCompanyName: clientOrCompany == "Client"
                            ? reader.GetString(reader.GetOrdinal("ClientName"))
                            : reader.GetString(reader.GetOrdinal("CompanyName"))
                    );

                    // Add the sale info to the list
                    salesList.Add(saleInfo);
                }
            }

            return salesList;
        }


        public static int GetPaymentTypeID(string paymentType)
        {
            // Directly call the static method from the business layer
            return SalesProductsManagmentSystemBusinessLayer.clsPayments.GetPaymentTypeID(paymentType);
        }

        public static ProductSoldInfos LoadProductSoldInfoFromReader(int saleID)
        {
            // Retrieve the SqlDataReader from the data access layer
            using (SqlDataReader reader = clsDataBonLivraisonsLayer.GetSoldItemInfoBySaleID(saleID))
            {
                if (reader == null || !reader.HasRows)
                {
                    throw new InvalidOperationException("No data found for the specified SaleID.");
                }

                // Create a DataTable to hold product information with all required columns
                DataTable productsDataTable = new DataTable();
                productsDataTable.Columns.Add("SoldItemID", typeof(int));
                productsDataTable.Columns.Add("ProductID", typeof(long));
                productsDataTable.Columns.Add("ProductName", typeof(string));
                productsDataTable.Columns.Add("QuantitySold", typeof(int));
                productsDataTable.Columns.Add("QuantitySold2", typeof(int));
                productsDataTable.Columns.Add("QuantitySold3", typeof(int));
                productsDataTable.Columns.Add("UnitPrice", typeof(decimal));
                productsDataTable.Columns.Add("UnitSoldPrice", typeof(decimal));
                productsDataTable.Columns.Add("Profit", typeof(double));
                productsDataTable.Columns.Add("InsertionTime", typeof(DateTime));
                productsDataTable.Columns.Add("Tva", typeof(decimal));
                productsDataTable.Columns.Add("TotalPrice", typeof(decimal));

                // Variables to hold other properties
                int? clientID = null; // Default to null
                int? companyID = null; // Default to null
                string selectedPaymentMethodInEnglish = string.Empty;

                // Read the data from the SqlDataReader
                while (reader.Read())
                {
                    // Load product data into DataTable
                    DataRow row = productsDataTable.NewRow();
                    row["SoldItemID"] = reader.GetInt32(reader.GetOrdinal("SoldItemID"));
                    row["ProductID"] = reader.GetInt64(reader.GetOrdinal("ProductID"));
                    row["ProductName"] = reader.GetString(reader.GetOrdinal("ProductName"));
                    row["QuantitySold"] = reader.GetInt32(reader.GetOrdinal("QuantitySold"));
                    row["QuantitySold2"] = reader.GetInt32(reader.GetOrdinal("QuantitySold2"));
                    row["QuantitySold3"] = reader.GetInt32(reader.GetOrdinal("QuantitySold3"));
                    row["UnitPrice"] = reader.GetDecimal(reader.GetOrdinal("UnitPrice"));
                    row["UnitSoldPrice"] = reader.GetDecimal(reader.GetOrdinal("UnitSoldPrice"));
                    row["Profit"] = reader.GetDouble(reader.GetOrdinal("Profit"));
                    row["InsertionTime"] = reader.GetDateTime(reader.GetOrdinal("InsertionTime"));
                    row["Tva"] = reader.GetDecimal(reader.GetOrdinal("Tva"));
                    row["TotalPrice"] = reader.GetDecimal(reader.GetOrdinal("TotalPrice"));
                    productsDataTable.Rows.Add(row);

                    // Read ClientID and CompanyID; they may be null
                    if (!reader.IsDBNull(reader.GetOrdinal("ClientID")))
                    {
                        clientID = reader.GetInt32(reader.GetOrdinal("ClientID"));
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("CompanyID")))
                    {
                        companyID = reader.GetInt32(reader.GetOrdinal("CompanyID"));
                    }

                    // Read the selected payment method
                    selectedPaymentMethodInEnglish = reader.GetString(reader.GetOrdinal("PaymentType"));
                }

                // Create an instance of ProductSoldInfo
                var productSoldInfo = new ProductSoldInfos(
                    saleID: saleID, // Pass the saleID
                    clientID: clientID, // Nullable ClientID
                    companyID: companyID, // Nullable CompanyID
                    productsBoughtInThisOperation: productsDataTable,
                    selectedPaymentMethodInEnglish: selectedPaymentMethodInEnglish,
                    userChequeInfo: null // Handle this separately if needed
                );

                return productSoldInfo;
            }
        }

       public static bool AddOrUpdateProducts(DataTable productsTable_To_Add_Or_Update,string supplierName,string bonReceptionNumber,string paymentType)
        {
            return SalesProductsManagmentSystemBusinessLayer.ClsProductManager.AddOrUpdateProducts(productsTable_To_Add_Or_Update, supplierName, bonReceptionNumber,paymentType);
        }

        public static List<BonReception> RetrieveBonReceptions(
       DateTime operationTimeStartDate,  // Non-nullable parameters first
       DateTime operationTimeEndDate,
       string supplierBLNumber = null,
       string supplierName = null,
       long? productID = null,
       string productName = null,
       string operationTypeName = null,
       decimal? costProduct = null,
       decimal? minTotalPrice = null,
       decimal? maxTotalPrice = null,
       string PaymentType = null)
        {
            List<BonReception> bonReceptions = new List<BonReception>();

            using (SqlDataReader reader = SalesProductsManagmentSystemBusinessLayer.clsBonReception.RetrieveBonReceptions(
                operationTimeStartDate,
                operationTimeEndDate,
                supplierBLNumber,
                supplierName,
                productID,
                productName,
                operationTypeName,
                costProduct,
                minTotalPrice,
                maxTotalPrice,
                PaymentType))
            {
                while (reader.Read())
                {
                    BonReception bonReception = new BonReception(
                        reader.IsDBNull(reader.GetOrdinal("SupplierBLNumber")) ? null : reader.GetString(reader.GetOrdinal("SupplierBLNumber")),
                        reader.GetInt64(reader.GetOrdinal("ProductID")),
                        reader.GetString(reader.GetOrdinal("ProductName")),
                        reader.IsDBNull(reader.GetOrdinal("SupplierBLNumber")) ? null : reader.GetString(reader.GetOrdinal("SupplierName")),
                        reader.GetString(reader.GetOrdinal("OperationTypeName")),
                        reader.GetDecimal(reader.GetOrdinal("CostProduct")),
                        reader.GetDateTime(reader.GetOrdinal("OperationTime")),
                        reader.GetInt32(reader.GetOrdinal("Added_Stock1")),
                        reader.GetInt32(reader.GetOrdinal("Added_Stock2")),
                        reader.GetInt32(reader.GetOrdinal("Added_Stock3")),
                        reader.GetDecimal(reader.GetOrdinal("TotalPrice")),
                        reader.GetInt32(reader.GetOrdinal("ProductsNumber")),
                        reader.GetString(reader.GetOrdinal("PaymentType"))
                    );

                    bonReceptions.Add(bonReception);
                }
            }

            return bonReceptions;
        }



        public static bool IsSupplierBLNumberAlreadyExisitg_For_ThisSupplierName(string supplierName, string supplierBLNumber)
        {
            // Call the data layer function
            return SalesProductsManagmentSystemBusinessLayer.ClsSupplier.IsSupplierBLNumberAlreadyExisitg_For_ThisSupplierName(supplierName, supplierBLNumber);
        }
    }
}
