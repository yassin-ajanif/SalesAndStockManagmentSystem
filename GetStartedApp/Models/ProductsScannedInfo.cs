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
                // we used this process validation by implementing intofiyInterfacedataerro in the viewmodel base
                // we couldn't use attributes because they can't deal with dynamic variable they require const variable which is not in our case

                bool Are_ProductsUnits_Exceed_Stock = ProductsUnitsNumberExceedTheOneInStock();
                bool hasError = Are_ProductsUnits_Exceed_Stock; // Example validation condition       
               if(hasError) ShowUiError(nameof(ProductsUnits), "لاتوجد هذه الكمية في المخزن");
               else DeleteUiError(nameof(ProductsUnits), "لاتوجد هذه الكمية في المخزن");
            }
        }

       

        private string _soldProductPriceUnitColor = "Black";
        public string SoldProductPriceUnitColor
        {
            get => _soldProductPriceUnitColor;
            set => this.RaiseAndSetIfChanged(ref _soldProductPriceUnitColor, value);
        }

        public float ProfitFromSoldProduct {get;}

        // this class will beholidng the product info retrived from database in addtion to the sold price and product units
        // these sold price and product units are not existing in the database so for this reason we decided
        // to make a class that has all info so the info will be encapsulated 
        public ProductsScannedInfo(ProductInfo ProductRetrivedFromDatabase)
        {
            this.ProductInfo = ProductRetrivedFromDatabase;
            // we set product unit 1 as user will expect to buy one item but it can be changed if he want more
            this.ProductsUnits = "1";
            this.PriceOfProductSold = ProductRetrivedFromDatabase.price.ToString();
           
        }

        public bool ProductsUnitsNumberExceedTheOneInStock()
        {
            if (string.IsNullOrEmpty(ProductsUnits) || string.IsNullOrWhiteSpace(ProductsUnits))
            {
                return false;
            }

            // Try to parse the ProductsUnits value to an integer
            if (int.TryParse(ProductsUnits, out int units))
            {
                return units > ProductInfo.StockQuantity;
            }

            // Return false if parsing fails
            return false;
        }
    }
}
