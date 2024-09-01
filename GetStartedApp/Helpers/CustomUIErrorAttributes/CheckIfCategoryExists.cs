using GetStartedApp.ViewModels.DashboardPages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GetStartedApp.Helpers.CustomUIErrorAttributes
{
    public class CheckIfCategoryExists : ValidationAttribute
    {
        private readonly List<string> AllProductCategories;

        public CheckIfCategoryExists()
        {
            var mainWindowViewModel = new MainWindowViewModel();
            var categoryProductsViewModel = new CategoryProductsViewModel(mainWindowViewModel);
            AllProductCategories = categoryProductsViewModel.ProductCategories
                                  .Select(category => category.ToLower())
                                  .ToList();
        }

        public override bool IsValid(object value)
        {
            string categoryNameToCheck = value as string;

            if (string.IsNullOrEmpty(categoryNameToCheck))
            {
                return true; // or false, depending on your requirement
            }

            return !AllProductCategories.Contains(categoryNameToCheck.ToLower());
        }
    }
}
