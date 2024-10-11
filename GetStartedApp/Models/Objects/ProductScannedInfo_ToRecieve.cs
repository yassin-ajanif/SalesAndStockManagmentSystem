using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using GetStartedApp.ViewModels.ProductPages;
using GetStartedApp.ViewModels.DashboardPages;
using System.Reactive.Linq;


namespace GetStartedApp.Models.Objects
{
    public class ProductScannedInfo_ToRecieve : ProductScannedInfo
    {

        private bool _thisProductIsExistingInDB;

        public bool ThisProductIsExistingInDB
        {
            get => _thisProductIsExistingInDB;
            set => this.RaiseAndSetIfChanged(ref _thisProductIsExistingInDB, value);
        }

        public ReactiveCommand<Unit, Unit> EditNewProductToReceiveInfoCommand { get; }

        public BonReceptionViewModel  bonReceptionViewModel{ get; }


        public ProductScannedInfo_ToRecieve(ProductInfo productInfo, BonReceptionViewModel bonReceptionViewModel, bool ThisProductIsExistingInDb=true) : base(productInfo)
            {
                ProductInfo = productInfo;

                EditNewProductToReceiveInfoCommand = ReactiveCommand.Create(OpenTheNewProductAdded_In_AddOrEditProductView_To_EditItsInfo);

                PriceOfProductSold = productInfo.price.ToString();
                this.ThisProductIsExistingInDB = ThisProductIsExistingInDb;
                this.bonReceptionViewModel = bonReceptionViewModel;


            }


        private List<string> getProductListCategoriesFromDb()
        {
            return AccessToClassLibraryBackendProject.GetProductsCategoryFromDatabase();
        }
        private async void OpenTheNewProductAdded_In_AddOrEditProductView_To_EditItsInfo()
        {

            bool ThereIsNoCategoriesAddedYetToSystem = getProductListCategoriesFromDb().Count == 0;

            var viewModelToBindWidthAddProductView = new AddProductViewModel(this, bonReceptionViewModel);

            await bonReceptionViewModel.ShowAddProductDialog.Handle(viewModelToBindWidthAddProductView);

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


            // Helper method to try parsing a product unit
            private bool TryParseProductUnit(string productUnit)
            {
                return !string.IsNullOrEmpty(productUnit) && int.TryParse(productUnit, out _);
            }


        }
    }

