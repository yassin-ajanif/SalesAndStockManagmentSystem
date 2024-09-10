using SalesProductsManagmentSystemDataLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SkiaSharp;
using System.Diagnostics;
//using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Globalization;

public class ClsDataAccessLayer
{
    private static string connectionString;
    private static SqlConnection connection;

    static ClsDataAccessLayer()
    {
        connectionString = ClsKeyConnection.connectionKey;
        connection = new SqlConnection(connectionString);
    }
    public static bool IsConnectionOpen()
    {
        if (connection.State == ConnectionState.Open)
        {
            return true;
        }
        else
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (SqlException)
            {
                // Handle connection error
                return false;
            }
        }
    }

    public static bool InsertFirstRunDateAndSetTrialOrPaidMode(bool isPaid)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("InsertFirstRunDate_And_Set_Trial_Or_Paid_Mode", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add the IsPaid parameter
                    command.Parameters.Add(new SqlParameter("@IsPaid", SqlDbType.Bit)
                    {
                        Value = isPaid
                    });

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            return true; // Indicate success
        }
        catch (Exception ex)
        {
            // Handle or log the exception as needed
            Console.WriteLine(ex.Message);
            return false; // Indicate failure
        }
    }

    public static DateTime? GetFirstRunDate()
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT dbo.GetFirstRunDate()", connection))
                {
                    var result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return (DateTime)result; // Direct cast, no boxing
                    }
                    else
                    {
                        return null; // No date found or null result
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Log the exception (not shown here for brevity)
            Console.WriteLine(ex.Message); // Replace with proper logging
            return null;
        }
    }

    public static bool IsApplicationUsersInPaidMode()
    {
        bool isInPaidMode = false;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand("IsApplicationUsersInPaidMode", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();
                object result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    isInPaidMode = (bool)result;
                }
            }
        }

        return isInPaidMode;
    }
    public static int getProductCategoryIdFromProductCategoryName(string categoryName)
    {
        
        SqlConnection connection = null;
        try
        {
            connection = new SqlConnection(connectionString);
            connection.Open();

            using (SqlCommand command = new SqlCommand("GetProductCategoryId_FromProductCategoryName", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Add the CategoryName parameter
                command.Parameters.Add(new SqlParameter("@CategoryName", categoryName));

                // Add the CategoryId parameter
                SqlParameter categoryIdParam = new SqlParameter("@CategoryId", SqlDbType.Int);
                categoryIdParam.Direction = ParameterDirection.Output;
                command.Parameters.Add(categoryIdParam);

                // Execute the stored procedure
                command.ExecuteNonQuery();

                // Get the value of the output parameter
                int categoryId = (int)categoryIdParam.Value;

                return categoryId;
            }
        }
        catch (Exception ex)
        {
            // Handle any errors that may have occurred
            // For example, you can log the error message: ex.Message
            return -1;
        }
        finally
        {
            // Ensure the connection is closed
            if (connection != null)
            {
                connection.Close();
            }
        }
    }

    private static bool IsAnEmptyString(string variable)
    {
        return variable == string.Empty;
    }

    private static bool IsThisNumberOutOfRange(int number)
    {
        // Check if the number is outside the range of int.MinValue to int.MaxValue
        return (number <= int.MinValue || number >= int.MaxValue);
    }

    private static bool IsThisNumberOutOfRange(long number)
    {
        // Check if the number is outside the range of int.MinValue to int.MaxValue
        return (number <= long.MinValue || number >= long.MaxValue);
    }
    private static bool IsThisFloatOutOfRange(float number)
    {
        // Check if the number is outside the range of float.MinValue to float.MaxValue
        return (number <= float.MinValue || number >= float.MaxValue);
    }

    public static bool IsStringLengthHigherThan(string str, int maxLength)
    {
        if (str == null) return false;
        return str.Length >= maxLength;
    }

    public static bool AddProduct(long productID, string productName,
        string description, float price, float cost, int quantityInStock, int categoryID, byte[] selectedProductImage)
    {
        bool operationHasSucceded = false;

        if (productID < 0) return false;

        if (IsAnEmptyString(productName) || productName==null) return false;

        if(IsThisNumberOutOfRange(productID)) return false;

        if (IsThisFloatOutOfRange(price)) return false;

        if (IsThisFloatOutOfRange(cost)) return false;

        if (IsThisFloatOutOfRange(quantityInStock)) return false;

        if (IsStringLengthHigherThan(productName, 50) || IsStringLengthHigherThan(description, 255)) return false;


        using (SqlCommand command = new SqlCommand("AddProduct", connection))
        {
            command.CommandType = CommandType.StoredProcedure;

            // Add parameters
            command.Parameters.AddWithValue("@ProductID", productID);
            command.Parameters.AddWithValue("@ProductName", productName);
            command.Parameters.AddWithValue("@Description", description);
            command.Parameters.AddWithValue("@Price", price);
            command.Parameters.AddWithValue("@Cost", cost);
            command.Parameters.AddWithValue("@QuantityInStock", quantityInStock);
            command.Parameters.AddWithValue("@CategoryID", categoryID);
            command.Parameters.AddWithValue("@selectedProductImage", selectedProductImage);

            try
            {
                bool ProductIdIsNotExistingBefore = !IsProductIDExists(productID);
                command.ExecuteNonQuery();
                bool QueryIsExecutedSucceffully = true;

                operationHasSucceded = ProductIdIsNotExistingBefore && QueryIsExecutedSucceffully;

            }
            catch (SqlException ex)
            {
                // Handle SQL exception
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                connection.Close(); // Close the connection

            }

            return operationHasSucceded;
        }

    }

    public static bool UpdateProduct(long productID, string productName,
    string description, float price, float cost, int quantityInStock, int categoryID, byte[] selectedProductImage)
    {
        bool operationHasSucceeded = false;

        if (productID < 0) return false;

        if (IsAnEmptyString(productName)) return false;

        if (IsThisNumberOutOfRange(productID)) return false;

        if (IsThisFloatOutOfRange(price)) return false;

        if (IsThisFloatOutOfRange(cost)) return false;

        if (IsThisNumberOutOfRange(quantityInStock)) return false;

        if (!IsProductIDExists(productID)) return false;

        if (IsStringLengthHigherThan(productName, 255) || IsStringLengthHigherThan(description, 1000)) return false;

        using (SqlCommand command = new SqlCommand("UpdateProduct", connection))
        {
            command.CommandType = CommandType.StoredProcedure;

            // Add parameters
            command.Parameters.AddWithValue("@ProductID", productID);
            command.Parameters.AddWithValue("@ProductName", productName);
            command.Parameters.AddWithValue("@Description", description);
            command.Parameters.AddWithValue("@Price", price);
            command.Parameters.AddWithValue("@Cost", cost);
            command.Parameters.AddWithValue("@QuantityInStock", quantityInStock);
            command.Parameters.AddWithValue("@CategoryID", categoryID);
            command.Parameters.AddWithValue("@selectedProductImage", selectedProductImage);

            try
            {

                command.ExecuteNonQuery();
                operationHasSucceeded = true;

            }
            catch (SqlException ex)
            {
                // Handle SQL exception
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                connection.Close(); // Close the connection
            }

            return operationHasSucceeded;
        }

    }

    public static bool UpdateProductQuantity(long productId, int quantityInStock)
    {
        bool isSuccessful = false;
        SqlConnection connection = null;

        if (productId <= 0 || quantityInStock<0 ) return false;

        if (IsThisNumberOutOfRange(productId)) return false;

        if (IsThisNumberOutOfRange(quantityInStock)) return false;

        if (!IsProductIDExists(productId)) return false;

        try
        {
            connection = new SqlConnection(connectionString);
            connection.Open();

            using (SqlCommand command = new SqlCommand("UpdateProductQuantity", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Add parameters
                command.Parameters.Add(new SqlParameter("@productID", productId));
                command.Parameters.Add(new SqlParameter("@quantityInStock", quantityInStock));

                // Execute the stored procedure
                int rowsAffected = command.ExecuteNonQuery();

                // Set isSuccessful based on the number of rows affected
                isSuccessful = rowsAffected > 0;
            }
        }
        catch (Exception ex)
        {
            // Log or handle the exception as per your application's requirements
            Console.WriteLine($"Exception occurred in UpdateProductQuantity: {ex.Message}");
            // You can throw the exception further if needed:
            // throw;
        }
        finally
        {
            // Ensure connection is closed even if an exception occurs
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        return isSuccessful;
    }


    public static bool UpdateProductPriceAndCost(long productId, float cost, float price)
    {
        bool isSuccessful = false;

        if (productId < 0 || cost < 0 || price < 0) return false;

        if (IsThisNumberOutOfRange(productId)) return false;

        if (IsThisFloatOutOfRange(price)) return false;

        if (IsThisFloatOutOfRange(cost)) return false;

        if (!IsProductIDExists(productId)) return false;

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("UpdateProductPriceAndCost", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@productID", productId));
                    command.Parameters.Add(new SqlParameter("@price", price));
                    command.Parameters.Add(new SqlParameter("@cost", cost));

                    int rowsAffected = command.ExecuteNonQuery();
                    isSuccessful = rowsAffected > 0;
                }
            }
        }
        catch (Exception ex)
        {
            // Handle the exception here, such as logging or throwing a custom exception
            Console.WriteLine($"An error occurred while updating product price and cost: {ex.Message}");
            // Optionally, you can throw the exception again to propagate it to the caller
            throw;
        }
        finally
        {
            // Ensure connection is always closed, even if an exception occurs
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        return isSuccessful;
    }


    public static bool DeleteProduct(long productID)
    {

        if (productID < 0) return false;

        if (IsThisNumberOutOfRange(productID)) return false;

        if (!IsProductIDExists(productID)) return false;

        using (SqlCommand command = new SqlCommand("DeleteProduct", connection))
        {
            command.CommandType = CommandType.StoredProcedure;

            // Add parameters
            command.Parameters.AddWithValue("@ProductID", productID);

            try
            {
                command.ExecuteNonQuery();
                return true;
            }
            catch (SqlException ex)
            {
                // Handle SQL exception
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }

            finally
            {
                connection.Close(); // Close the connection
            }
        }

    }

    public static bool IsProductIDExists(long productID)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("CheckProductIDExists", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter productIdParameter = new SqlParameter("@ProductID", SqlDbType.Decimal)
                {
                    Value = (decimal)productID
                };
                command.Parameters.Add(productIdParameter);

                // Output parameter
                SqlParameter existsParameter = new SqlParameter("@Exists", SqlDbType.Bit);
                existsParameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(existsParameter);

                // Execute the stored procedure
                command.ExecuteNonQuery();

                // Get the value of the output parameter
                bool exists = (bool)existsParameter.Value;

                connection.Close();

                return exists;
            }
        }
    }


    public static bool IsProductNameExists(string productName)
    {
        if (productName == null) return false;

        if(productName==string.Empty) return false;

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("CheckProductNameExists", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Input parameter
                    command.Parameters.AddWithValue("@ProductName", productName);

                    // Output parameter
                    SqlParameter existsParameter = new SqlParameter("@Exists", SqlDbType.Bit);
                    existsParameter.Direction = ParameterDirection.Output;
                    command.Parameters.Add(existsParameter);

                    // Execute the stored procedure
                    command.ExecuteNonQuery();

                    // Get the value of the output parameter
                    bool exists = (bool)existsParameter.Value;

                    return exists;
                }
            }
        }
        catch (Exception ex)
        {
            // Handle the exception here, such as logging or throwing a custom exception
            Console.WriteLine($"An error occurred while checking if product name '{productName}' exists: {ex.Message}");
            // Optionally, you can throw the exception again to propagate it to the caller
            throw;
        }
    }


    public static List<String> getProductsCategories()
    {
        // this stored procedure will display a sets of one single table column that contains productsCategoriesNames
        short ColumnPosition_Of_CategoryProductsName = 0;
        List<string> productCategories = new List<string>();

        try
        {


            SqlCommand command = new SqlCommand("GetProductCategories", connection);
            command.CommandType = CommandType.StoredProcedure;
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    productCategories.Add(reader.GetString(ColumnPosition_Of_CategoryProductsName));
                }
            }

        }
        catch (Exception ex)
        {
            //throw;
            // we return the empty list if something goes wrong instead of creashing the app 
            return new List<string>() { };
        }

        finally
        {

            connection.Close();
        }


        return productCategories;
    }

    private static bool ContainsInvalidSqlCharacters(string input)
    {
        // Regular expression pattern to check for invalid SQL characters
        string pattern = @"['""]|[%&/\\;!?]";

        // Check if input matches the pattern
        return Regex.IsMatch(input, pattern);
    }
    // sql reader function are going to be teset by ui
    public static SqlDataReader GetProductsInfoReader()
    {
        SqlCommand command = new SqlCommand("SELECT * FROM ProductsInfoTable()", connection);

        if (connection.State != ConnectionState.Open)
            connection.Open();

        SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

        return reader;
    }

    public static SqlDataReader GetProductsInfoReaderByCategory(string categoryName)
    {

        SqlCommand command;
        SqlCommand commandOfQueryThatDosentReturnAnything = new SqlCommand($"SELECT * FROM dbo.ProductsInfoTableByCategory('')", connection);

        if (categoryName==null || ContainsInvalidSqlCharacters(categoryName))  command = commandOfQueryThatDosentReturnAnything;

        else  command = new SqlCommand($"SELECT * FROM dbo.ProductsInfoTableByCategory('{categoryName}')", connection);

        if (connection.State != ConnectionState.Open)
            connection.Open();

        SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

        return reader;
    }

    public static SqlDataReader GetProductsInfoListReaderBy_CategoryName_And_SearchProductName(string selectedCategory, string searchTerm)
    {
        SqlCommand command;
        SqlCommand commandOfQueryThatDosentReturnAnything = new SqlCommand($"SELECT * FROM dbo.SearchProductsByName('','')", connection);

        if (selectedCategory == null || searchTerm == null|| ContainsInvalidSqlCharacters(selectedCategory) || ContainsInvalidSqlCharacters(searchTerm)) command = commandOfQueryThatDosentReturnAnything;

        else command = new SqlCommand($"SELECT * FROM dbo.SearchProductsByName('{selectedCategory}', '{searchTerm}')", connection);

        if (connection.State != ConnectionState.Open)
            connection.Open();

        SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

        return reader;
    }

    public static SqlDataReader GetSoldItems(DateTime startDate, DateTime endDate)
    {
        SqlConnection connection = new SqlConnection(connectionString);
        SqlCommand command = new SqlCommand("SELECT * FROM dbo.GetSoldItems(@startDate, @endDate)", connection);

        // Add parameters to the command
        command.Parameters.AddWithValue("@startDate", startDate);
        command.Parameters.AddWithValue("@endDate", endDate);

        connection.Open();
        SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
        return reader;
    }


    // we use this function to convert Date to this form YYYYMMDD this is the form
    private static string ConvertDateTimeToUniversalFormat(DateTime dateTime)
    {
        // Convert DateTime to ISO 8601 format: 'yyyy-MM-ddTHH:mm:ss'
        string universalDateTime = dateTime.ToString("yyyy-MM-ddTHH:mm:ss");
        return universalDateTime;
    }

  

    public static SqlDataReader GetReturnedProducts(DateTime startDate, DateTime endDate)
    {
        SqlConnection connection = new SqlConnection(connectionString);
        SqlCommand command = new SqlCommand("SELECT * FROM dbo.GetReturnedProducts(@StartDate, @EndDate)", connection);
        command.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.Date) { Value = startDate });
        command.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.Date) { Value = endDate });

        connection.Open();
        SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
        return reader;
    }
    public static bool IsProductIdExistingInSalesItemTable(long productId)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand("Check_IfProductIdIsExisting_InSaleItems", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ProductID", SqlDbType.BigInt).Value = productId;
                SqlParameter outputParam = new SqlParameter("@Exists", SqlDbType.Bit)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputParam);

                cmd.ExecuteNonQuery();

                bool exists = (bool)cmd.Parameters["@Exists"].Value;
                return exists;
            }
        }
    }


    public static long GetNewProductID()
    {
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("spGetNewProductID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Execute the stored procedure
                    var newProductID = (long)command.ExecuteScalar();

                    return newProductID;
                }
            }
        }
        catch (Exception ex)
        {
            // Handle any exceptions (e.g., database connection error)
            Console.WriteLine($"Error: {ex.Message}");
            return -1; // Return a default value or handle the error as needed
        }
    }


    public static bool InsertNewCategory(string categoryName)
    {
        if (categoryName == null || categoryName == string.Empty || IsStringLengthHigherThan(categoryName, 50)) return false;
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("InsertNewCategory", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CategoryName", categoryName);

                    // Output parameter for success
                    var successParam = new SqlParameter("@Success", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(successParam);

                    command.ExecuteNonQuery();

                    // Get the result from the output parameter
                    return (bool)successParam.Value;
                }
            }
        }
        catch (SqlException sqlEx)
        {
            // Log or handle SQL exceptions
            Console.WriteLine($"SQL Error inserting category: {sqlEx.Message}");
            return false;
        }
        catch (Exception ex)
        {
            // Log or handle other exceptions
            Console.WriteLine($"Error inserting category: {ex.Message}");
            return false;
        }
    }


    public static bool UpdateProductCategory(string previewsCategoryName, string newCategoryName)
    {
        if (previewsCategoryName == null || previewsCategoryName == string.Empty) return false;

       if (newCategoryName == null || newCategoryName == string.Empty || IsStringLengthHigherThan(newCategoryName, 50)) return false;

        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("UpdateCategory", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add input parameters
                    command.Parameters.AddWithValue("@PreviewsCategoryName", previewsCategoryName);
                    command.Parameters.AddWithValue("@NewCategoryName", newCategoryName);

                    // Add output parameter for success
                    var successParam = new SqlParameter("@Success", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(successParam);

                    // Execute the stored procedure
                    command.ExecuteNonQuery();

                    // Get the result from the output parameter
                    return (bool)successParam.Value;
                }
            }
        }
        catch (SqlException sqlEx)
        {
            // Log or handle SQL exceptions
            Console.WriteLine($"SQL Error updating category: {sqlEx.Message}");
            return false;
        }
        catch (Exception ex)
        {
            // Log or handle other exceptions
            Console.WriteLine($"Error updating category: {ex.Message}");
            return false;
        }
    }

    public static bool DeleteCategory(string categoryName)
    {
        
        if (categoryName == null || categoryName == string.Empty || IsStringLengthHigherThan(categoryName, 50)) return false;
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("DeleteCategory", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CategoryName", categoryName);

                    // Output parameter for success
                    var successParam = new SqlParameter("@Success", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(successParam);

                    command.ExecuteNonQuery();

                    // Get the result from the output parameter
                    return (bool)successParam.Value;
                }
            }
        }
        catch (SqlException sqlEx)
        {
            // Log or handle SQL exceptions
            Console.WriteLine($"SQL Error deleting category: {sqlEx.Message}");
            return false;
        }
        catch (Exception ex)
        {
            // Log or handle other exceptions
            Console.WriteLine($"Error deleting category: {ex.Message}");
            return false;
        }
    }


    public static SqlDataReader GetProductById(long productId)
    {
        SqlConnection connection = new SqlConnection(connectionString);
        SqlCommand command = new SqlCommand("GetProductById", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.BigInt)).Value = productId;

        connection.Open();
        SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

        return reader;
    }

    public static bool SaveNewSaleOperationToDatabase(DateTime saleDateTime, float totalPrice, DataTable productDataTable)
    {
        if (saleDateTime == default(DateTime) || saleDateTime == null)
        {
            Console.WriteLine("Invalid saleDateTime: DateTime is not provided. or it null");
            return false;
        }

        // Validate totalPrice
        if (totalPrice <= 0 || IsThisFloatOutOfRange(totalPrice))
        {
            Console.WriteLine($"Invalid totalPrice: {totalPrice}. Total price must be greater than zero. or it s out of range");
            return false;
        }

        // Validate productDataTable
        if (productDataTable == null || productDataTable.Rows.Count == 0)
        {
            Console.WriteLine("Invalid productDataTable: DataTable is null or empty.");
            return false;
        }
   
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand("MakeSaleTransaction", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@SaleDateTime", SqlDbType.DateTime) { Value = saleDateTime });
                cmd.Parameters.Add(new SqlParameter("@TotalPrice", SqlDbType.Decimal) { Value = totalPrice });
                cmd.Parameters.Add(new SqlParameter("@ProductDataTable", SqlDbType.Structured)
                {
                    TypeName = "dbo.ProductBought",
                    Value = productDataTable
                });

                var returnValue = new SqlParameter("@ReturnValue", SqlDbType.Int)
                {
                    Direction = ParameterDirection.ReturnValue
                };
                cmd.Parameters.Add(returnValue);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    int result = (int)returnValue.Value;
                    return result == 1;
                }
                catch (SqlException ex)
                {
                    // Handle exception (log it, rethrow it, or handle it as needed)
                    Console.WriteLine("SQL Error: " + ex.Message);
                    return false;
                }
            }
        }
    }

    public static byte[] GetImageOfProductById(long id)
    {
        SqlConnection conn = null; // Declare SqlConnection outside using block

        if (id <= 0 || IsThisNumberOutOfRange(id)) return null;

        try
        {
            conn = new SqlConnection(connectionString);
            conn.Open();

            using (SqlCommand cmd = new SqlCommand("SELECT dbo.getImageOfProductByid(@id)", conn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@id", id));

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader[0] as byte[];
                    }
                }
            }
        }
        catch (SqlException sqlEx)
        {
            // Handle SQL exceptions
            Console.WriteLine($"SQL Error: {sqlEx.Message}");
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            // Ensure connection is closed in all cases
            if (conn != null && conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }

        return null;
    }

    public static bool InsertIntoReturnedProducts(long productId, string productName, int quantity, float sellingPrice, float profit)
    {
        if (productId < 0 || IsThisNumberOutOfRange(productId) || !IsProductIDExists(productId)) return false;

        if (productName == null || productName == string.Empty || IsStringLengthHigherThan(productName,250) ) return false;

        if (quantity < 0  || IsThisFloatOutOfRange(quantity)    )  return false;

        if (sellingPrice < 0 || IsThisFloatOutOfRange(sellingPrice) ) return false;

        if (IsThisFloatOutOfRange(profit) ) return false;

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("sp_InsertIntoReturnedProducts", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@productid", productId));
                    cmd.Parameters.Add(new SqlParameter("@productname", productName));
                    cmd.Parameters.Add(new SqlParameter("@quantity", quantity));
                    cmd.Parameters.Add(new SqlParameter("@sellingPrice", sellingPrice));
                    cmd.Parameters.Add(new SqlParameter("@profit", profit));

                    // Execute the stored procedure and get the number of affected rows
                    int affectedRows = cmd.ExecuteNonQuery();

                    // If any rows were affected, return true, otherwise return false
                    return affectedRows > 0;
                }
            }
        }
        catch (SqlException sqlEx)
        {
            // Handle SQL exceptions (e.g., log the error)
            Console.WriteLine($"SQL Error: {sqlEx.Message}");
            return false;
        }
        catch (Exception ex)
        {
            // Handle other exceptions (e.g., log the error)
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }

    // test this function in real life application and check if it returns the correct value
    // i've tested by inserting into the database 2000 records or solditems and 1000 record of returned itmes
    // also i test with different times

    // it remain now to check if the data is correct when we insert it through the app
    public static decimal GetTotalProfit(DateTime startTime, DateTime endTime)
    {
        if (startTime == default(DateTime) || startTime == null) return 0;
        if (endTime == default(DateTime) || endTime == null) return 0;
        if (startTime > endTime) return 0;

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetTotalProfit", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@StartTime", SqlDbType.Date).Value = startTime;
                    cmd.Parameters.Add("@EndTime", SqlDbType.Date).Value = endTime;

                    var profitParam = new SqlParameter("@NetProfit", SqlDbType.Decimal);
                    profitParam.Precision = 18; // Set the precision
                    profitParam.Scale = 2; // Set the scale
                    profitParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(profitParam);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    decimal profit = (decimal)cmd.Parameters["@NetProfit"].Value;

                    Debug.WriteLine(profit);

                    return profit;
                }
            }
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"Exception: {ex.Message}");
            // Optionally rethrow or handle the exception
            return 0;
        }
    }


    public static decimal GetTotalRevenue(DateTime startTime, DateTime endTime)
    {
        if (startTime == default(DateTime) || startTime == null) return 0;
        if (endTime == default(DateTime) || endTime == null) return 0;
        if (startTime > endTime) return 0;

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetTotalRevenue", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@StartTime", SqlDbType.Date).Value = startTime;
                    cmd.Parameters.Add("@EndTime", SqlDbType.Date).Value = endTime;

                    var revenueParam = new SqlParameter("@NetRevenue", SqlDbType.Decimal);
                    revenueParam.Precision = 18; // Set the precision
                    revenueParam.Scale = 2; // Set the scale
                    revenueParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(revenueParam);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    decimal revenue = (decimal)cmd.Parameters["@NetRevenue"].Value;

                    Debug.WriteLine(revenue);

                    return revenue;
                }
            }
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"Exception: {ex.Message}");
            // Optionally rethrow or handle the exception
            return 0;
        }
    }



    public static bool CheckIfLogNamesIncludeAdminAndUser()
    {
        bool result = false;
         // Remplacez par votre chaîne de connexion réelle

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand("CheckIfLogNamesIncludeAdminAndUser", connection);
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter resultParam = new SqlParameter("@Result", SqlDbType.Bit);
            resultParam.Direction = ParameterDirection.Output;
            command.Parameters.Add(resultParam);

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                result = (bool)resultParam.Value;
            }
            catch (SqlException e)
            {
                // Gérer l'exception
                Console.WriteLine(e.Message);
                
            }
        }

        return result;
    }

    public static bool InsertOrUpdateAdminAndUserLogins(
    string logNameAdmin, string passwordAdmin, string nationalIDOFAdmin,
    string logNameUser, string passwordUser, string nationalIDOFUser)
    {
        int outputResult = 0;
        // Replace with your actual connection string

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand("InsertOrUpdateLogins", connection);
            command.CommandType = CommandType.StoredProcedure;

            // Add parameters for the stored procedure here
            command.Parameters.AddWithValue("@LogNameAdmin", logNameAdmin);
            command.Parameters.AddWithValue("@PasswordAdmin", passwordAdmin);
            command.Parameters.AddWithValue("@NationalIDOFAdmin", nationalIDOFAdmin);
            command.Parameters.AddWithValue("@LogNameUser", logNameUser);
            command.Parameters.AddWithValue("@PasswordUser", passwordUser);
            command.Parameters.AddWithValue("@NationalIDOFUser", nationalIDOFUser);

            // Output parameter
            SqlParameter outputParam = new SqlParameter("@OutputResult", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(outputParam);

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                outputResult = (int)outputParam.Value;
            }
            catch (SqlException e)
            {
                // Handle exception
                Console.WriteLine(e.Message);
                return false;
            }
        }

        return outputResult == 1;
    }


    public static bool isThisNationalidBelongstoadmin(string nationalID)
    {
        bool result = false;

        if (nationalID == null || nationalID == string.Empty || IsStringLengthHigherThan(nationalID, 10)) return false;

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("isThisNationalIDBelongstoAdmin", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@NationalIDofRecived", nationalID);
                    SqlParameter resultParam = new SqlParameter("@Result", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(resultParam);

                    connection.Open();
                    command.ExecuteNonQuery();
                    result = (bool)resultParam.Value;
                }
            }
        }
        catch (Exception ex)
        {
            // Log the exception or handle as needed
            Console.WriteLine($"Exception: {ex.Message}");
            // Optionally rethrow or return a default value
            // return false; // Uncomment to return a default value
            throw; // Uncomment to rethrow the exception
        }

        return result;
    }



    public static bool CheckIfUserOrAdminPasswordIsCorrect(string userType, string password)
    {
        if (userType == "admin" || userType == "user") ;
        else return false;
        if(password == null || password == string.Empty) return false;

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("CheckPasswordByUserType", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserType", userType);
                    command.Parameters.AddWithValue("@InputPassword", password);

                    var returnParameter = command.Parameters.Add("@IsPasswordCorrect", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.Output;

                    connection.Open();
                    command.ExecuteNonQuery();

                    int result = (int)returnParameter.Value;
                    return result == 1;
                }
            }
        }
        catch (Exception ex)
        {
            // Log the exception or handle as needed
            Console.WriteLine($"Exception: {ex.Message}");
            // Optionally rethrow or return a default value
            // return false; // Uncomment to return a default value
            throw; // Uncomment to rethrow the exception
        }
    }


    public static bool CheckIfProductAlreadyExistingInSoldItemsList(long productId)
    {
        bool productExists = false;

        if (productId <= 0 || IsThisNumberOutOfRange(productId) || !IsProductIDExists(productId)) return false;

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("CheckIfProduct_AlreadyExisting_InSoldItemsList", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ProductID", SqlDbType.BigInt)).Value = productId;
                    var returnParameter = cmd.Parameters.Add("@Exists", SqlDbType.Bit);
                    returnParameter.Direction = ParameterDirection.Output;

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    productExists = Convert.ToBoolean(returnParameter.Value);
                }
            }
        }
        catch (Exception ex)
        {
            // Log the exception or handle as needed
            Console.WriteLine($"Exception: {ex.Message}");
            // Optionally rethrow or return a default value
            // return false; // Uncomment to return a default value
            throw; // Uncomment to rethrow the exception
        }

        return productExists;
    }


    public static bool IsLowStock()
    {
        bool isLowStock = false;  // Default value in case of exception

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("checkLowStock", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Define the output parameter
                    SqlParameter outputResultParam = new SqlParameter("@outputResult", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputResultParam);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                    // Check for DBNull.Value and convert to bool
                    isLowStock = (int)outputResultParam.Value == 1;
                }
            }
        }
        catch (Exception ex)
        {
            // Log the exception or handle as needed
            Console.WriteLine($"Exception: {ex.Message}");
            // Optionally rethrow or handle the exception
            // throw; // Uncomment to rethrow the exception
        }

        return isLowStock;
    }

    public static bool CheckIfLogoCompanyIsExisting()
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand("CheckIfLogoCompanyIsExisting", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter isLogoExistingParam = new SqlParameter("@IsLogoExisting", SqlDbType.Bit)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(isLogoExistingParam);

                connection.Open();
                command.ExecuteNonQuery();

                return (bool)isLogoExistingParam.Value;
            }
        }
    }

    public static DataTable GetCompanyDetails()
    {
        DataTable dataTable = new DataTable();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand("GetCompanyDetails", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dataTable);
                }
            }
        }

        return dataTable;
    }

    public static async Task<bool> UpdateCompanyDetailsAsync(byte[] companyLogo, string companyName, string companyLocation,string ICE, string TaxProfeesionalID, string TaxID)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            using (var command = new SqlCommand("UpdateCompanyDetails", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Add parameters
                command.Parameters.AddWithValue("@CompanyLogo", (object)companyLogo ?? DBNull.Value);
                command.Parameters.AddWithValue("@CompanyName", companyName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@CompanyLocation", companyLocation ?? (object)DBNull.Value);

                command.Parameters.AddWithValue("@ICE", ICE ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@TaxProfessionalID", TaxProfeesionalID ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@TaxID", TaxID ?? (object)DBNull.Value);

                try
                {
                    await connection.OpenAsync();
                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., log the error)
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    return false;
                }
            }
        }
    }

    public static int GetMinimalStockValue()
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetMinimalStockValue", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();

                    var result = command.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int minimalStockValue))
                    {
                        return minimalStockValue;
                    }
                    else
                    {
                        throw new Exception("Failed to retrieve MinimalStockValue.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Log the exception (implement logging as needed)
            throw new Exception("An error occurred while retrieving the MinimalStockValue.", ex);
        }
    }

    public static bool UpdateMinimalStockValue(int newMinimalStockValue)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("UpdateMinimalStockValue", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@NewMinimalStockValue", newMinimalStockValue);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            // Log the exception (implement logging as needed)
            return false;
        }
    
}

    public static bool AddNewClient(string clientName, string phoneNumber, string email)
    {
        // Variable to track success
        bool isInserted = false;

        // Create and open a connection to the database
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();

                // Create a SQL command to call the stored procedure
                using (SqlCommand command = new SqlCommand("AddNewClient", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters for the stored procedure
                    command.Parameters.AddWithValue("@ClientName", clientName);
                    command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                    command.Parameters.AddWithValue("@Email", email);

                    // Execute the stored procedure
                    int rowsAffected = command.ExecuteNonQuery();

                    // If at least one row was affected, the insert was successful
                    isInserted = rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                // Handle exception or log it
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // Return whether the insertion was successful
        return isInserted;
    }

    public static List<string> GetClientNames()
    {
        // The column index for the 'ClientName' field
       
        List<string> clientNames = new List<string>();

        try
        {
            // Initialize and open the SQL connection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Initialize the command to execute the stored procedure
                using (SqlCommand command = new SqlCommand("GetClientNames", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Execute the command and retrieve the data using SqlDataReader
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Loop through the result set
                        while (reader.Read())
                        {
                            string ClientName = reader.GetString(0);
                            string PhoneNumber = reader.GetString(1);
                            string ClientNameCombinedWidthPhoneNumber = ClientName + "-" + PhoneNumber;

                            clientNames.Add(ClientNameCombinedWidthPhoneNumber);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Log or handle the exception if necessary
            // Return an empty list to prevent application crashes
            return new List<string>();
        }

        // Return the list of client names
        return clientNames;
    }

    public static bool UpdateClientInfo(string clientName, string oldPhoneNumber, string newPhoneNumber, string email)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("UpdateClientInfo", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Adding parameters
                    command.Parameters.AddWithValue("@ClientName", clientName);
                    command.Parameters.AddWithValue("@OldPhoneNumber", oldPhoneNumber); // New parameter for old phone number
                    command.Parameters.AddWithValue("@NewPhoneNumber", newPhoneNumber); // New parameter for the new phone number
                    command.Parameters.AddWithValue("@Email", email);

                    // Execute the update command
                    int rowsAffected = command.ExecuteNonQuery();

                    // Return true if at least one row is affected
                    return rowsAffected > 0;
                }
            }
        }
        catch (Exception ex)
        {
            // Log or handle the exception
            return false; // Return false in case of any errors
        }
    }

    public static bool DeleteClientByPhoneNumber(string phoneNumber)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("DeleteClientByPhoneNumber", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add the phone number parameter
                    command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

                    // Execute the delete command
                    command.ExecuteNonQuery();

                    // Return true if at least one row is affected
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            // Log or handle the exception if necessary
            return false; // Return false in case of any errors
        }
    }
    public static bool IsPhoneNumberExists(string phoneNumber)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("CheckIfPhoneNumberExists", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

                    // Execute the command and get the result
                    int result = Convert.ToInt32(command.ExecuteScalar());

                    // If result is 1, the phone number exists, otherwise it doesn't
                    return result == 1;
                }
            }
        }
        catch (Exception ex)
        {
            // Log or handle the exception
            return false; // return false in case of any errors
        }
    }


    public static bool GetClientInfoByPhoneNumber(
        string phoneNumber,
        ref string clientName,
        ref string email)
    {
        bool isClientFound = false;

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("GetClientInfoByPhoneNumber", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            clientName = reader["ClientName"] as string;
                            email = reader["Email"] as string;
                            isClientFound = true;
                        }
                        else
                        {
                            // Set default values if client is not found
                            clientName = null;
                            email = null;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions (logging, rethrowing, etc.)
            Console.WriteLine($"Error: {ex.Message}");
        }

        return isClientFound;
    }

}











