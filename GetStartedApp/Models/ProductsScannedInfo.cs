using GetStartedApp.Helpers;
using GetStartedApp.Helpers.CustomUIErrorAttributes;
using GetStartedApp.ViewModels;
using ReactiveUI;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System;
using System.Collections;
using Svg;


namespace GetStartedApp.Models
{


    public class ProductsScannedInfo : ViewModelBase
    {


        const int maxNumberOfProductsCanSystemHold = 1000;
        const int maxPriceCanProductWorth = 100_000;
        public ProductInfo ProductInfo { get; set; }

        private string _priceOfProductSold;
        [PositiveFloatRange(1, maxPriceCanProductWorth, ErrorMessage = "ادخل رقم موجب")]
        [StringNotEmpty(ErrorMessage = "ادخل رقم")]
        public string PriceOfProductSold
        {
            get => _priceOfProductSold;
            set => this.RaiseAndSetIfChanged(ref _priceOfProductSold, value);
        }

        private string _productsUnits;
        [PositiveIntRange(1, maxNumberOfProductsCanSystemHold, ErrorMessage = "ادخل رقم موجب  صغير وبدون فاصلة")]
        [StringNotEmpty(ErrorMessage = "ادخل رقم")] 
        public string ProductsUnits
        {
            get => _productsUnits;
            set  

            {
                
                this.RaiseAndSetIfChanged(ref _productsUnits, value);
                // we link products units to stock1 to facilate the flow of work for poeple who don't have others stocks or didn't choose to use other stock
                ProductsUnitsToReduce_From_Stock1 = value;

                // we used this process validation by implementing intofiyInterfacedataerro in the viewmodel base
                // we couldn't use attributes because they can't deal with dynamic variable they require const variable which is not in our case

                // bool Are_ProductsUnits_Exceed_Stock = ProductsUnitsNumberExceedTheOneInStock();
                // bool hasError = Are_ProductsUnits_Exceed_Stock; // Example validation condition       
                // if(hasError) ShowUiError(nameof(ProductsUnits), "لاتوجد هذه الكمية في المخزن");
                // else DeleteUiError(nameof(ProductsUnits), "لاتوجد هذه الكمية في المخزن");
            }
        }

        private string _productsUnitsToReduce_From_Stock1;
        [PositiveIntRange(0, maxNumberOfProductsCanSystemHold, ErrorMessage = "ادخل رقم موجب  صغير وبدون فاصلة")]
        [StringNotEmpty(ErrorMessage = "ادخل رقم")]
        public string ProductsUnitsToReduce_From_Stock1
        {
            get => _productsUnitsToReduce_From_Stock1;
            set

            {
                this.RaiseAndSetIfChanged(ref _productsUnitsToReduce_From_Stock1, value);
                // we used this process validation by implementing intofiyInterfacedataerro in the viewmodel base
                // we couldn't use attributes because they can't deal with dynamic variable they require const variable which is not in our case

                // bool Are_ProductsUnits_Exceed_Stock = ProductsUnitsNumberExceedTheOneInStock();
                // bool hasError = Are_ProductsUnits_Exceed_Stock; // Example validation condition       
                // if(hasError) ShowUiError(nameof(ProductsUnits), "لاتوجد هذه الكمية في المخزن");
                // else DeleteUiError(nameof(ProductsUnits), "لاتوجد هذه الكمية في المخزن");
            }
        }

        // Stock 2
        private string _productsUnitsToReduce_From_Stock2="0";
        [PositiveIntRange(0, maxNumberOfProductsCanSystemHold, ErrorMessage = "ادخل رقم موجب  صغير وبدون فاصلة")]
        [StringNotEmpty(ErrorMessage = "ادخل رقم")]
        public string ProductsUnitsToReduce_From_Stock2
        {
            get => _productsUnitsToReduce_From_Stock2;
            set
            {
                this.RaiseAndSetIfChanged(ref _productsUnitsToReduce_From_Stock2, value);
                // Similar validation logic for Stock 2
                // bool Are_ProductsUnits_Exceed_Stock = ProductsUnitsNumberExceedTheOneInStock();
                // bool hasError = Are_ProductsUnits_Exceed_Stock;
                // if(hasError) ShowUiError(nameof(ProductsUnitsToReduce_From_Stock2), "لاتوجد هذه الكمية في المخزن");
                // else DeleteUiError(nameof(ProductsUnitsToReduce_From_Stock2), "لاتوجد هذه الكمية في المخزن");
            }
        }

        // Stock 3
        private string _productsUnitsToReduce_From_Stock3="0";
        [PositiveIntRange(0, maxNumberOfProductsCanSystemHold, ErrorMessage = "ادخل رقم موجب  صغير وبدون فاصلة")]
        [StringNotEmpty(ErrorMessage = "ادخل رقم")]
        public string ProductsUnitsToReduce_From_Stock3
        {
            get => _productsUnitsToReduce_From_Stock3;
            set
            {
                this.RaiseAndSetIfChanged(ref _productsUnitsToReduce_From_Stock3, value);
                // Similar validation logic for Stock 3
                // bool Are_ProductsUnits_Exceed_Stock = ProductsUnitsNumberExceedTheOneInStock();
                // bool hasError = Are_ProductsUnits_Exceed_Stock;
                // if(hasError) ShowUiError(nameof(ProductsUnitsToReduce_From_Stock3), "لاتوجد هذه الكمية في المخزن");
                // else DeleteUiError(nameof(ProductsUnitsToReduce_From_Stock3), "لاتوجد هذه الكمية في المخزن");
            }
        }




        private string _soldProductPriceUnitColor = "Black";
        public string SoldProductPriceUnitColor
        {
            get => _soldProductPriceUnitColor;
            set => this.RaiseAndSetIfChanged(ref _soldProductPriceUnitColor, value);
        }

        private bool _numberOfProductsUnits_NotEqual_TheSumOf_SumOfThreeStock = false;
        public bool NumberOfProductsUnits_NotEqual_TheSumOf_SumOfThreeStock
        {
            get => _numberOfProductsUnits_NotEqual_TheSumOf_SumOfThreeStock;
            set => this.RaiseAndSetIfChanged(ref _numberOfProductsUnits_NotEqual_TheSumOf_SumOfThreeStock, value);
        }

        public bool ProductStockHasErrors = false;

        // this class will beholidng the product info retrived from database in addtion to the sold price and product units
        // these sold price and product units are not existing in the database so for this reason we decided
        // to make a class that has all info so the info will be encapsulated 
        public ProductsScannedInfo(ProductInfo ProductRetrivedFromDatabase)
        {
            ProductInfo = ProductRetrivedFromDatabase;
            // we set product unit 1 as user will expect to buy one item but it can be changed if he want more
            ProductsUnits = "1";
            PriceOfProductSold = ProductRetrivedFromDatabase.price.ToString();

            whenTheStockValueIsChanged_CheckIfProductUnitsAreDistributedCorrectlyAcrossStocks();


        }

        // the number of prudct untis must be equal to the sum of number porducts in shop in addtion to stock 1 and stock 2
        private bool Is_TheNumberOfProductsUnits_Equal_To_SumOfAllStocks()
        {
            // Check if all fields are parsable
            if (!int.TryParse(_productsUnits, out int productsUnits) ||
                !int.TryParse(_productsUnitsToReduce_From_Stock1, out int stock1Units) ||
                !int.TryParse(_productsUnitsToReduce_From_Stock2, out int stock2Units) ||
                !int.TryParse(_productsUnitsToReduce_From_Stock3, out int stock3Units))
            {
                // If any of the fields are not parsable, return false
                return false;
            }



            // If all are parsable, check the condition
            return productsUnits == (stock1Units + stock2Units + stock3Units);
        }

        private bool Is_SumOfStockUnits_NotZero()
        {
            // Check if all fields are parsable
            if (!int.TryParse(_productsUnitsToReduce_From_Stock1, out int stock1Units) ||
                !int.TryParse(_productsUnitsToReduce_From_Stock2, out int stock2Units) ||
                !int.TryParse(_productsUnitsToReduce_From_Stock3, out int stock3Units))
            {
                // If any of the fields are not parsable, return false
                return false;
            }

            // Check if the sum of the stock units is greater than zero
            int totalStockUnits = stock1Units + stock2Units + stock3Units;
            return totalStockUnits > 0;
        }

        private bool Are_ProductsUnitsExceedTheOneInStock_1()
        {
            bool isExceeding = ProductInfo.StockQuantity < int.Parse(_productsUnitsToReduce_From_Stock1);
            string errorMessage = $"هناك {ProductInfo.StockQuantity} قطع متوفرة في المحل";

            if (isExceeding)
            {
                ShowUiError(nameof(ProductsUnitsToReduce_From_Stock1), errorMessage);
                return false;
            }

            DeleteUiError(nameof(ProductsUnitsToReduce_From_Stock1), errorMessage);
            return true;
        }

        private bool Are_ProductsUnitsExceedTheOneInStock_2()
        {
            bool isExceeding = ProductInfo.StockQuantity2 < int.Parse(_productsUnitsToReduce_From_Stock2);
            string errorMessage = $"هناك {ProductInfo.StockQuantity2} قطع متوفرة في المستودع 1";

            if (isExceeding)
            {
                ShowUiError(nameof(ProductsUnitsToReduce_From_Stock2), errorMessage);
                return false;
            }

            DeleteUiError(nameof(ProductsUnitsToReduce_From_Stock2), errorMessage);
            return true;
        }

        private bool Are_ProductsUnitsExceedTheOneInStock_3()
        {
            bool isExceeding = ProductInfo.StockQuantity3 < int.Parse(_productsUnitsToReduce_From_Stock3);
            string errorMessage = $"هناك {ProductInfo.StockQuantity3} قطع متوفرة في المستودع 2";

            if (isExceeding)
            {
                ShowUiError(nameof(ProductsUnitsToReduce_From_Stock3), errorMessage);
                return false;
            }

            DeleteUiError(nameof(ProductsUnitsToReduce_From_Stock3), errorMessage);
            return true;
        }


        // we use this function to eliminate the erros that can confuse the user
        // when a stock value 1 or 2 or shop stock proeprties arent having valid numbers like digits or emptystring 
        // that will trigger a lot of errors especially in this fucntion below 
        // so we let the user to fix the error shown by attribute that syas enter a valid number
        // then when he fixes that erros others erros are going to be shown
        public void DeleteAllUi_Erros_WhenUser_DosentSetValidNumber_ForAllThreeTypesOfStocks()
        {
            DeleteUiError(nameof(ProductsUnitsToReduce_From_Stock1), string.Empty);
            DeleteUiError(nameof(ProductsUnitsToReduce_From_Stock2), string.Empty);
            DeleteUiError(nameof(ProductsUnitsToReduce_From_Stock3), string.Empty);
            NumberOfProductsUnits_NotEqual_TheSumOf_SumOfThreeStock = false;
        }
        public bool AreProductsUnitsSpreadAcrossAllStock_Correctly()
        {
            // Call both validation functions
            bool isSumNotZero = Is_SumOfStockUnits_NotZero();
            bool isProductUnitsEqual = Is_TheNumberOfProductsUnits_Equal_To_SumOfAllStocks();

            // Update the flag based on both conditions
            return isSumNotZero && isProductUnitsEqual;
        }

        private void whenTheStockValueIsChanged_CheckIfProductUnitsAreDistributedCorrectlyAcrossStocks()
        {
            // Observe changes in stock units and product units
            this.WhenAnyValue(
                x => x.ProductsUnitsToReduce_From_Stock1,
                x => x.ProductsUnitsToReduce_From_Stock2,
                x => x.ProductsUnitsToReduce_From_Stock3,
                x => x.ProductsUnits)
                .Subscribe(_ =>
                {
                    // Check if any of the properties are null, empty, or non-parsable
                    if (!TryParseProductUnit(ProductsUnitsToReduce_From_Stock1) ||
                        !TryParseProductUnit(ProductsUnitsToReduce_From_Stock2) ||
                        !TryParseProductUnit(ProductsUnitsToReduce_From_Stock3) ||
                        !TryParseProductUnit(ProductsUnits))
                    {
                        ProductStockHasErrors=true;
                        DeleteAllUi_Erros_WhenUser_DosentSetValidNumber_ForAllThreeTypesOfStocks();
                        return;
                    }

                    
                    // Call the existing validation functions
                   bool Stock_1_IsValid= Are_ProductsUnitsExceedTheOneInStock_1();
                   bool Stock_2_IsValid= Are_ProductsUnitsExceedTheOneInStock_2();
                   bool Stock_3_IsValid = Are_ProductsUnitsExceedTheOneInStock_3();

                    NumberOfProductsUnits_NotEqual_TheSumOf_SumOfThreeStock = !AreProductsUnitsSpreadAcrossAllStock_Correctly();

                    if (Stock_1_IsValid && Stock_2_IsValid && Stock_3_IsValid && !NumberOfProductsUnits_NotEqual_TheSumOf_SumOfThreeStock) { 
                        
                        ProductStockHasErrors = false;
                    }

                    else ProductStockHasErrors = true;

                });
        }

        // Helper method to try parsing a product unit
        private bool TryParseProductUnit(string productUnit)
        {
            return !string.IsNullOrEmpty(productUnit) && int.TryParse(productUnit, out _);
        }




        //  public bool ProductsUnitsNumberExceedTheOneInStock()
        //  {
        //      if (string.IsNullOrEmpty(ProductsUnits) || string.IsNullOrWhiteSpace(ProductsUnits))
        //      {
        //          return false;
        //      }
        //
        //      // Try to parse the ProductsUnits value to an integer
        //      if (int.TryParse(ProductsUnits, out int units))
        //      {
        //          return units > ProductInfo.StockQuantity;
        //      }
        //
        //      // Return false if parsing fails
        //      return false;
        //  }




    }
}
