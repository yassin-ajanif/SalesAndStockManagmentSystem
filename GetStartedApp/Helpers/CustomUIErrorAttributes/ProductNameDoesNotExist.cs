using System;
using System.ComponentModel.DataAnnotations;
using GetStartedApp.Models;
 // Adjust the namespace based on your actual setup

namespace GetStartedApp.Helpers.CustomUIErrorAttributes
{
    public class ProductNameDoesNotExist : ValidationAttribute
    {
        private readonly string       _errorMessage;
        private readonly long          _productid;
        private readonly eProductMode _productMode;

        public ProductNameDoesNotExist(string errorMessage,eProductMode productMode)
        {
            _errorMessage = errorMessage;
            
            _productMode = productMode;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string productName = value as string;

            if (!string.IsNullOrEmpty(productName))
            { 
                long getProductIdFromProductName = AccessToClassLibraryBackendProject.GetProductIDFromProductName(productName);
                // Initialize the boolean variable to check existence
                bool exists = AccessToClassLibraryBackendProject.DoesProductNameAlreadyExist(productName,(int)_productMode, getProductIdFromProductName);

                if (exists)
                {
                    // Return the custom error message if the product name exists
                    return new ValidationResult(_errorMessage);
                }
            }

            // Return success if the product name does not exist or is null/empty
            return ValidationResult.Success;
        }
    }
}
