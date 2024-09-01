using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetStartedApp.Models;
using System;
using System.ComponentModel.DataAnnotations;


namespace GetStartedApp.Helpers.CustomUIErrorAttributes
{

    public class ProductIdAlreadyExists : ValidationAttribute
    {
        private readonly string _errorMessage;

        public ProductIdAlreadyExists(string errorMessage)
        {
            _errorMessage = errorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            long productId;
            if (value !=null && long.TryParse(value.ToString(), out productId))
            {
                // Call your custom function here. Replace "AccessToClassLibraryBackendProject" with the class that contains your function.
                bool exists = AccessToClassLibraryBackendProject.IsThisProductIdAlreadyExist(productId);

                if (exists)
                {
                    // If the product ID exists, return a failure result with your custom error message.
                    return new ValidationResult(_errorMessage);
                }
            }

            // If the product ID does not exist or is not a valid integer, return a success result.
            return ValidationResult.Success;
        }
    }
}
