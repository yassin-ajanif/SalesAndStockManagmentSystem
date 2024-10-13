using System;
using ReactiveUI;
using System.Reactive;
using GetStartedApp.Models.Objects;
using GetStartedApp.ViewModels.ProductPages;
using System.Reactive.Linq;
using System.Linq;


namespace GetStartedApp.ViewModels.DashboardPages
{
    public class EditNewProductAdded_AtReceptionList_ViewModel : AddProductViewModel
    {

        private BonReceptionViewModel _bonReceptionViewModel;
       private ProductScannedInfo_ToRecieve _currentProductScannedToRecieve;

        public EditNewProductAdded_AtReceptionList_ViewModel(ProductScannedInfo_ToRecieve productScannedInfo_ToRecieve, BonReceptionViewModel bonReceptionViewModel)
            : base(bonReceptionViewModel, productScannedInfo_ToRecieve)
        {
            this._bonReceptionViewModel = bonReceptionViewModel;
            _currentProductScannedToRecieve = productScannedInfo_ToRecieve;
            // Load product info from the scanned product data
            LoadProductInfo(productScannedInfo_ToRecieve);

            // Call the shared initialization method
            Initialize();
        }

        private void LoadProductInfo(ProductScannedInfo_ToRecieve productScannedInfo_ToRecieve)
        {
            // Assign values from productScannedInfo_ToRecieve to the ViewModel properties
            EntredProductID = productScannedInfo_ToRecieve.ProductInfo.id.ToString();
            SelectedImageToDisplay = productScannedInfo_ToRecieve.ProductInfo.SelectedProductImage;
            EntredProductName = productScannedInfo_ToRecieve.ProductInfo.name;
            EnteredProductDescription = productScannedInfo_ToRecieve.ProductInfo.description;
            EntredCost = productScannedInfo_ToRecieve.ProductInfo.cost.ToString();
            EnteredPrice = productScannedInfo_ToRecieve.ProductInfo.price.ToString();
            SelectedCategory = productScannedInfo_ToRecieve.ProductInfo.selectedCategory;
            EntredStockQuantity = productScannedInfo_ToRecieve.ProductsUnitsToReduce_From_Stock1.ToString();
            EntredStockQuantity2 = productScannedInfo_ToRecieve.ProductsUnitsToReduce_From_Stock2.ToString();
            EntredStockQuantity3 = productScannedInfo_ToRecieve.ProductsUnitsToReduce_From_Stock3.ToString();
            ProductBtnOperation = "تعديل منتج";

            _currentProductID = EntredProductID;
            _currentProductName = EntredProductName;
        }

        private void Initialize()
        {
            EnableAllInputsExceptID();

            // Set the list of product categories for user selection
            ProductCategories = GetProductsCategoryFromDatabase();

            PickImageCommand = ReactiveCommand.CreateFromTask(PickImageProduct);
            DeleteImageCommand = ReactiveCommand.CreateFromTask(DisplayNoImage);

            AddOrEditOrDeleteProductCommand = ReactiveCommand.Create(EditedProductAddedToBonReceptionList, CheckIfFormIsFilledCorreclty_WhenWeEdit_TheNewProductAdded());

            // Initialize interactions for message boxes and dialogs
            ShowMessageBoxDialog = new Interaction<string, Unit>();
            ShowMessageBoxDialog_For_BarCodePrintingPersmission = new Interaction<string, bool>();
            ShowBarCodePrinterPage = new Interaction<Unit, Unit>();
        }


        private void EditExistingNewProductAddedToReceptionList()
        {
            var existingProduct = _currentProductScannedToRecieve;

            if (existingProduct != null)
            {
                existingProduct.ProductInfo.name = EntredProductName;
                existingProduct.ProductInfo.description = EnteredProductDescription;
                existingProduct.ProductInfo.StockQuantity =   _StockQuantity;
                existingProduct.ProductInfo.StockQuantity2 = _StockQuantity2;
                existingProduct.ProductInfo.StockQuantity3 = _StockQuantity3;
                existingProduct.ProductInfo.price = _Price;
                existingProduct.ProductInfo.cost = _Cost;
                existingProduct.ProductInfo.profit = _Benefit;
                existingProduct.ProductInfo.selectedCategory = _SelectedCategory;
                existingProduct.ProductInfo.SelectedProductImage = _SelectedImageToDisplay;

                existingProduct.ProductsUnits = (_StockQuantity + _StockQuantity2 + _StockQuantity3).ToString();
                existingProduct.ProductsUnitsToReduce_From_Stock1 = EntredStockQuantity;
                existingProduct.ProductsUnitsToReduce_From_Stock2 = EntredStockQuantity2;
                existingProduct.ProductsUnitsToReduce_From_Stock3 = EntredStockQuantity3;

            }
        }


        public async void EditedProductAddedToBonReceptionList()
        {

            // Product is being edited, so remove the existing product
            EditExistingNewProductAddedToReceptionList();

              // Show success message
            await ShowMessageBoxDialog.Handle("تم تعديل المنتج بنجاح");
        }


        private IObservable<bool> CheckIfFormIsFilledCorreclty_WhenWeEdit_TheNewProductAdded()
        {
            var canAddProduct1 = this.WhenAnyValue(
                      x => x.EntredProductName,
                      x => x.EnteredProductDescription,
                      x => x.EnteredPrice,
                      x => x.EntredCost,
                      x => x.CalculatedBenefit,
                      x => x.EntredStockQuantity,
                      x => x.SelectedCategory,
                      (EntredProductName, EnteredProductDescription, EnteredPrice, EntredCost, CalculatedBenefit, EntredStockQuantity, SelectedCategory) =>
                          !string.IsNullOrEmpty(EntredProductName) && !string.IsNullOrWhiteSpace(EntredProductName) &&
                          !Is_UiError_Raised_If_TheProduct_Name_ToAdd_Is_AlreadyExistInDb() &&
                          !Is_UiError_Raised_If_TheProduct_Name_Is_AlreadyExistInBonReceptionList_WhenEditing() &&
                          !string.IsNullOrEmpty(EnteredPrice) && !string.IsNullOrWhiteSpace(EnteredPrice) &&
                          !string.IsNullOrEmpty(CalculatedBenefit) && !string.IsNullOrWhiteSpace(CalculatedBenefit) &&
                          !string.IsNullOrEmpty(EntredStockQuantity) && !string.IsNullOrWhiteSpace(EntredStockQuantity) &&
                          EnteredProductDescription != null &&
                          !string.IsNullOrEmpty(EntredCost) &&
                          !string.IsNullOrWhiteSpace(EntredCost) &&
                          !string.IsNullOrEmpty(SelectedCategory) && !string.IsNullOrWhiteSpace(SelectedCategory) &&
                          AreAllPropertiesAttributeValid()
                      );

            var canAddProduct2 = this.WhenAnyValue(
                x => x.EntredStockQuantity2,
                x => x.EntredStockQuantity3,
                (EntredStockQuantity2, EntredStockQuantity3) =>
                    !string.IsNullOrEmpty(EntredStockQuantity2) && !string.IsNullOrWhiteSpace(EntredStockQuantity2) &&
                    !string.IsNullOrEmpty(EntredStockQuantity3) && !string.IsNullOrWhiteSpace(EntredStockQuantity3) &&
                    AreAllPropertiesAttributeValid()

            );

            // Combine the two observables using CombineLatest
            return canAddProduct1.CombineLatest(canAddProduct2, (isValid1, isValid2) => isValid1 && isValid2);
        }
    }
}
