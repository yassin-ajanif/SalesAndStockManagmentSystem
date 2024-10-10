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
using ZXing.QrCode.Internal;


namespace GetStartedApp.Models.Objects
{


    public class ProductsScannedInfo_ToSale : ProductScannedInfo
    {

       
        public ProductsScannedInfo_ToSale(ProductInfo productInfo):base(productInfo)
        {
            ProductInfo = productInfo;
            // we set product unit 1 as user will expect to buy one item but it can be changed if he want more
           // ProductsUnits = "1";
            PriceOfProductSold = productInfo.price.ToString();
     
            whenTheStockValueIsChanged_CheckIfProductUnitsAreDistributedCorrectlyAcrossStocks();

        }

        // the number of prudct untis must be equal to the sum of number porducts in shop in addtion to stock 1 and stock 2
        private bool Is_TheNumberOfProductsUnits_Equal_To_SumOfAllStocks()
        {
            // Check if all fields are parsable
            if (!int.TryParse(ProductsUnits, out int productsUnits) ||
                !int.TryParse(ProductsUnitsToReduce_From_Stock1, out int stock1Units) ||
                !int.TryParse(ProductsUnitsToReduce_From_Stock2, out int stock2Units) ||
                !int.TryParse(ProductsUnitsToReduce_From_Stock3, out int stock3Units))
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
            if (!int.TryParse(ProductsUnitsToReduce_From_Stock1, out int stock1Units) ||
                !int.TryParse(ProductsUnitsToReduce_From_Stock2, out int stock2Units) ||
                !int.TryParse(ProductsUnitsToReduce_From_Stock3, out int stock3Units))
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
            bool isExceeding = ProductInfo.StockQuantity < int.Parse(ProductsUnitsToReduce_From_Stock1);
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
            bool isExceeding = ProductInfo.StockQuantity2 < int.Parse(ProductsUnitsToReduce_From_Stock2);
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
            bool isExceeding = ProductInfo.StockQuantity3 < int.Parse(ProductsUnitsToReduce_From_Stock3);
            string errorMessage = $"هناك {ProductInfo.StockQuantity3} قطع متوفرة في المستودع 2";

            if (isExceeding)
            {
                ShowUiError(nameof(ProductsUnitsToReduce_From_Stock3), errorMessage);
                return false;
            }

            DeleteUiError(nameof(ProductsUnitsToReduce_From_Stock3), errorMessage);
            return true;
        }

      
        protected override void whenTheStockValueIsChanged_CheckIfProductUnitsAreDistributedCorrectlyAcrossStocks()
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
                        ProductStockHasErrors = true;
                        DeleteAllUi_Erros_WhenUser_DosentSetValidNumber_ForAllThreeTypesOfStocks();
                        return;
                    }

                    
                    // Call the existing validation functions
                    bool Stock_1_IsValid = Are_ProductsUnitsExceedTheOneInStock_1();
                    bool Stock_2_IsValid = Are_ProductsUnitsExceedTheOneInStock_2();
                    bool Stock_3_IsValid = Are_ProductsUnitsExceedTheOneInStock_3();



                    NumberOfProductsUnits_NotEqual_TheSumOf_SumOfThreeStock = !AreProductsUnitsSpreadAcrossAllStock_Correctly();

                    // if the product is not for sale like in bon de reception or in bon de command we won't check the stock status
                    if ( Stock_1_IsValid && Stock_2_IsValid && Stock_3_IsValid && !NumberOfProductsUnits_NotEqual_TheSumOf_SumOfThreeStock) ProductStockHasErrors = false;

                    else ProductStockHasErrors = true;

                });
        }


        // Helper method to try parsing a product unit
        private bool TryParseProductUnit(string productUnit)
        {
            return !string.IsNullOrEmpty(productUnit) && int.TryParse(productUnit, out _);
        }


    }
}
