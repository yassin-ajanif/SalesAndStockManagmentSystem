using GetStartedApp.Helpers.CustomUIErrorAttributes;
using GetStartedApp.Models.Enums;
using GetStartedApp.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStartedApp.Models.Objects
{
    public class ProductScannedInfo :ViewModelBase
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

            }
        }

        // Stock 2
        private string _productsUnitsToReduce_From_Stock2 = "0";
        [PositiveIntRange(0, maxNumberOfProductsCanSystemHold, ErrorMessage = "ادخل رقم موجب  صغير وبدون فاصلة")]
        [StringNotEmpty(ErrorMessage = "ادخل رقم")]
        public string ProductsUnitsToReduce_From_Stock2
        {
            get => _productsUnitsToReduce_From_Stock2;
            set
            {
                this.RaiseAndSetIfChanged(ref _productsUnitsToReduce_From_Stock2, value);

            }
        }

        // Stock 3
        private string _productsUnitsToReduce_From_Stock3 = "0";
        [PositiveIntRange(0, maxNumberOfProductsCanSystemHold, ErrorMessage = "ادخل رقم موجب  صغير وبدون فاصلة")]
        [StringNotEmpty(ErrorMessage = "ادخل رقم")]
        public string ProductsUnitsToReduce_From_Stock3
        {
            get => _productsUnitsToReduce_From_Stock3;
            set
            {
                this.RaiseAndSetIfChanged(ref _productsUnitsToReduce_From_Stock3, value);

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
        public ProductScannedInfo(ProductInfo productInfo)
        {
            ProductInfo = productInfo;
            // we set product unit 1 as user will expect to buy one item but it can be changed if he want more
            ProductsUnits = "1";
            ProductsUnitsToReduce_From_Stock1 = ProductsUnits;
            ProductsUnitsToReduce_From_Stock2 = "0";
            ProductsUnitsToReduce_From_Stock3 = "0";
            PriceOfProductSold = productInfo.price.ToString();

            whenTheStockValueIsChanged_CheckIfProductUnitsAreDistributedCorrectlyAcrossStocks();
            whenUserUnitPriceChanges_SetIts_Value_tobe_Equal_To_StockQuantity1();

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
            return productsUnits == stock1Units + stock2Units + stock3Units;
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

    
        // we use this function to eliminate the erros that can confuse the user
        // when a stock value 1 or 2 or shop stock proeprties arent having valid numbers like digits or emptystring 
        // that will trigger a lot of errors especially in this fucntion below 
        // so we let the user to fix the error shown by attribute that syas enter a valid number
        // then when he fixes that erros others erros are going to be shown
        public void DeleteAllUi_Erros_WhenUser_DosentSetValidNumber_ForAllThreeTypesOfStocks()
        {
            DeleteAllUiErrorsProperty(nameof(ProductsUnitsToReduce_From_Stock1));
            DeleteAllUiErrorsProperty(nameof(ProductsUnitsToReduce_From_Stock2));
            DeleteAllUiErrorsProperty(nameof(ProductsUnitsToReduce_From_Stock3));
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

        protected virtual void whenTheStockValueIsChanged_CheckIfProductUnitsAreDistributedCorrectlyAcrossStocks()
        {
            // Observe changes in stock units and product units
            this.WhenAnyValue(
                x => x.ProductsUnitsToReduce_From_Stock1,
                x => x.ProductsUnitsToReduce_From_Stock2,
                x => x.ProductsUnitsToReduce_From_Stock3,
                x => x.ProductsUnits
                )
                .Subscribe(_ =>
                {
                    // Check if any of the properties are null, empty, or non-parsable
                    if (!TryParseProductUnit(ProductsUnitsToReduce_From_Stock1) ||
                        !TryParseProductUnit(ProductsUnitsToReduce_From_Stock2) ||
                        !TryParseProductUnit(ProductsUnitsToReduce_From_Stock3) ||
                        !TryParseProductUnit(ProductsUnits))
                    {
                        ProductStockHasErrors = true;
                        DeleteAllUi_Erros_WhenUser_DosentSetValidNumber_ForAllThreeTypesOfStocks();
                        return;
                    }


                    NumberOfProductsUnits_NotEqual_TheSumOf_SumOfThreeStock = !AreProductsUnitsSpreadAcrossAllStock_Correctly();

                    // if the product is not for sale like in bon de reception or in bon de command we won't check the stock status
                    if (NumberOfProductsUnits_NotEqual_TheSumOf_SumOfThreeStock) ProductStockHasErrors = false;

                    else ProductStockHasErrors = true;

                });
        }

        private void whenUserUnitPriceChanges_SetIts_Value_tobe_Equal_To_StockQuantity1() {
           
            this.WhenAnyValue(
                   x => x.ProductsUnits) .Subscribe(_ => ProductsUnitsToReduce_From_Stock1=ProductsUnits);
                }

            // Helper method to try parsing a product unit
            private bool TryParseProductUnit(string productUnit)
        {
            return !string.IsNullOrEmpty(productUnit) && int.TryParse(productUnit, out _);
        }

    }
}
