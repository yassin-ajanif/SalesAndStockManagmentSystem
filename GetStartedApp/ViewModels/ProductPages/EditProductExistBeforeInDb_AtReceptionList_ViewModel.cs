using GetStartedApp.ViewModels.DashboardPages;
using ReactiveUI;
using System.Reactive;
using System;
using GetStartedApp.Helpers;
using System.Reactive.Linq;
using GetStartedApp.Helpers.CustomUIErrorAttributes;
using GetStartedApp.Models.Objects;



namespace GetStartedApp.ViewModels.ProductPages
{
    public class EditProductExistBeforeInDb_AtReceptionList_ViewModel : AddProductViewModel
    {

        protected long _ProductID;

        protected string _EntredProductID;
        public virtual string EntredProductID
        {
            get { return _EntredProductID; }

            set
            {

                this.RaiseAndSetIfChanged(ref _EntredProductID, value);


                DataEntryPropertyLoader.ConvertStringToIntAndLoadPrivateProperty(value, ref _ProductID);

            }
        }
        private BonReceptionViewModel BonReceptionViewModel;
        private string _currentProductID;
        private string _currentProductName;

        private string _ProductName;
        public string EntredProductName
        {
            get { return _ProductName; }
            set { this.RaiseAndSetIfChanged(ref _ProductName, value); }
        }

        ProductScannedInfo_ToRecieve _currentProductScannedInfo_ToRecieve;

        public EditProductExistBeforeInDb_AtReceptionList_ViewModel(ProductScannedInfo_ToRecieve productScannedInfo_ToRecieve, BonReceptionViewModel bonReceptionViewModel)
            : base(bonReceptionViewModel, productScannedInfo_ToRecieve)
        {

            _currentProductScannedInfo_ToRecieve = productScannedInfo_ToRecieve;
            BonReceptionViewModel = bonReceptionViewModel;

            // Call the shared initialization method
            Initialize();

            LoadExistingDbProductInfo();

        }


        private void LoadExistingDbProductInfo()
        {
            // load all info of product added to edit at the form
            EntredProductID = _currentProductScannedInfo_ToRecieve.ProductInfo.id.ToString();
            SelectedImageToDisplay = _currentProductScannedInfo_ToRecieve.ProductInfo.SelectedProductImage;
            EntredProductName = _currentProductScannedInfo_ToRecieve.ProductInfo.name;
            EnteredProductDescription = _currentProductScannedInfo_ToRecieve.ProductInfo.description;
            EntredCost = _currentProductScannedInfo_ToRecieve.ProductInfo.cost.ToString();
            EnteredPrice = _currentProductScannedInfo_ToRecieve.ProductInfo.price.ToString();
            SelectedCategory = _currentProductScannedInfo_ToRecieve.ProductInfo.selectedCategory;
           
            // here we load the value of the real stock values from databse not like the value of ui stock values because it is an old product not the newestone
            // so it won't be any synchronisation between the 3 products stock values and the atual stock values dispalyed in the add productview 
            EntredStockQuantity = _currentProductScannedInfo_ToRecieve.ProductInfo.StockQuantity.ToString();
            EntredStockQuantity2 = _currentProductScannedInfo_ToRecieve.ProductInfo.StockQuantity2.ToString();
            EntredStockQuantity3 = _currentProductScannedInfo_ToRecieve.ProductInfo.StockQuantity3.ToString();
            ProductBtnOperation = "تعديل منتج";

            _currentProductID = EntredProductID;
            _currentProductName = EntredProductName;
        }
        private void Initialize()
        {


            EnableAllInputsExcept_ID_ProductName_Stocks();


            DisplayNoImage();


            // Set the list of products categories a user will choose among
            ProductCategories = GetProductsCategoryFromDatabase();

            PickImageCommand = ReactiveCommand.CreateFromTask(PickImageProduct);
            DeleteImageCommand = ReactiveCommand.CreateFromTask(DisplayNoImage);

          AddOrEditOrDeleteProductCommand = ReactiveCommand.Create(EditedProductAddedToBonReceptionList, CheckIfFormIsFilledCorreclty_WhenWeEdit_TheExistingProductInDb_Added());

            // This is an initialization of command that is going to open a message box 
            // when adding productOperation is submitted
            ShowMessageBoxDialog = new Interaction<string, Unit>();
            ShowMessageBoxDialog_For_BarCodePrintingPersmission = new Interaction<string, bool>();
            ShowBarCodePrinterPage = new Interaction<Unit, Unit>();


        }

        private void EnableAllInputsExcept_ID_ProductName_Stocks()
        {
            EnableAllInputsExceptID();
            IsProductNameReadOnly = false;
            IsStockQuantityReadOnly = false;
            IsStockQuantityReadOnly2 = false;
            IsStockQuantityReadOnly3 = false;
        }


        private void EditExisting_Old_ProductAddedToReceptionList()
        {
            var existingProduct = _currentProductScannedInfo_ToRecieve;

            if (existingProduct != null)
            {
              
                existingProduct.ProductInfo.description = EnteredProductDescription;
                existingProduct.ProductInfo.price = _Price;
                existingProduct.ProductInfo.cost = _Cost;
                existingProduct.ProductInfo.profit = _Benefit;
                existingProduct.ProductInfo.selectedCategory = _SelectedCategory;
                existingProduct.ProductInfo.SelectedProductImage = _SelectedImageToDisplay;


            }
        }

        public async void EditedProductAddedToBonReceptionList()
        {

            // Product is being edited, so remove the existing product
            EditExisting_Old_ProductAddedToReceptionList();

            // Show success message
            await ShowMessageBoxDialog.Handle("تم تعديل المنتج بنجاح");
        }
        private IObservable<bool> CheckIfFormIsFilledCorreclty_WhenWeEdit_TheExistingProductInDb_Added()
        {
            var canAddProduct1 = this.WhenAnyValue(
                      x => x.EnteredProductDescription,
                      x => x.EnteredPrice,
                      x => x.EntredCost,
                      x => x.CalculatedBenefit,
                      x => x.EntredStockQuantity,
                      x => x.SelectedCategory,
                      (EnteredProductDescription, EnteredPrice, EntredCost, CalculatedBenefit, EntredStockQuantity, SelectedCategory) =>
                          !string.IsNullOrEmpty(EnteredPrice) && !string.IsNullOrWhiteSpace(EnteredPrice) &&
                          !string.IsNullOrEmpty(CalculatedBenefit) && !string.IsNullOrWhiteSpace(CalculatedBenefit) &&
                          !string.IsNullOrEmpty(EntredStockQuantity) && !string.IsNullOrWhiteSpace(EntredStockQuantity) &&
                          EnteredProductDescription != null &&
                          !string.IsNullOrEmpty(EntredCost) &&
                          !string.IsNullOrWhiteSpace(EntredCost) &&
                          !string.IsNullOrEmpty(SelectedCategory) && !string.IsNullOrWhiteSpace(SelectedCategory)
                          && AreAllPropertiesAttributeValid()
                      );

            var canAddProduct2 = this.WhenAnyValue(
                x => x.EntredStockQuantity2,
                x => x.EntredStockQuantity3,
                (EntredStockQuantity2, EntredStockQuantity3) =>
                    !string.IsNullOrEmpty(EntredStockQuantity2) && !string.IsNullOrWhiteSpace(EntredStockQuantity2) &&
                    !string.IsNullOrEmpty(EntredStockQuantity3) && !string.IsNullOrWhiteSpace(EntredStockQuantity3)
                    && AreAllPropertiesAttributeValid()

            );

            // Combine the two observables using CombineLatest
            return canAddProduct1.CombineLatest(canAddProduct2, (isValid1, isValid2) => isValid1 && isValid2);
        }

    }
}
