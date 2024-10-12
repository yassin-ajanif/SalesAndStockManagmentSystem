using GetStartedApp.Models;
using ReactiveUI;
using System.Collections.Generic;
using System.Reactive;
using GetStartedApp.ViewModels.ProductPages;
using System.Windows.Input;

using System.Reactive.Linq;
using GetStartedApp.Models.Objects;
using System.Collections.ObjectModel;

using System.Linq;

namespace GetStartedApp.ViewModels.DashboardPages
{

    public class BonReceptionViewModel : MakeSaleViewModel
       {

        private List<string> _suppliersList;
        public  List<string> SuppliersList { get => _suppliersList; set => this.RaiseAndSetIfChanged(ref _suppliersList , value); }

        private ObservableCollection<ProductScannedInfo_ToRecieve> _productsListScanned_To_Recive;

        public ObservableCollection<ProductScannedInfo_ToRecieve> ProductsListScanned_To_Recive
        {
            get => _productsListScanned_To_Recive;
            private set => this.RaiseAndSetIfChanged(ref _productsListScanned_To_Recive, value);
        }

        public ICommand AddNewProductCommand { get; set; }

        public Interaction<AddProductViewModel, Unit> ShowAddProductDialog { get; }

        public BonReceptionViewModel(MainWindowViewModel mainWindowViewModel) : base(mainWindowViewModel)
        {
            // addProductViewModel = new ProductsListViewModel(mainWindowViewModel);
            AddNewProductCommand = ReactiveCommand.Create(AddProductInfoOperation);
            ShowAddProductDialog = new Interaction<AddProductViewModel, Unit>();
            ProductsListScanned_To_Recive = new ObservableCollection<ProductScannedInfo_ToRecieve>();
            SuppliersList = getSuppliers();
        }

        private List<string> getSuppliers()
        {
           return AccessToClassLibraryBackendProject.GetSupplierNamePhoneNumberCombo();
        }

        private List<string> getProductListCategoriesFromDb()
        {
            return AccessToClassLibraryBackendProject.GetProductsCategoryFromDatabase();
        }

        public async void AddProductInfoOperation()
        {
            
           bool ThereIsNoCategoriesAddedYetToSystem = getProductListCategoriesFromDb().Count == 0;
      // 
            if (ThereIsNoCategoriesAddedYetToSystem) { await ShowDeleteSaleDialogInteraction.Handle(" لا توجد تصنيفات اضف تصنيف جديد "); return; }
       
            var userControlToShowInsideDialog = new AddProductViewModel(this);
       
            await ShowAddProductDialog.Handle(userControlToShowInsideDialog);
        }

        protected override bool TheProductIsAlreadyAddedToAlist(string BarcodeNumberScanned)
        {
            long productId = long.Parse(BarcodeNumberScanned);

            return ProductsListScanned_To_Recive.Any(product => product.ProductInfo.id == productId);
        }
        protected virtual ProductScannedInfo_ToRecieve Add_UnitsOfSoldProduct_And_SoldProductPrice(string BarcodeNumberScanned)
        {
            ProductInfo ProductFound = RetrieveProductFromDatabaseByBarCodeId(BarcodeNumberScanned);
            // productscannedinfo is a lcass that contans a product retrived from daabase in addtion to the price and units info a user or buyter will submit
            ProductScannedInfo_ToRecieve ProductFound_Plus_PriceAndUnitsOfSoldProduct = new ProductScannedInfo_ToRecieve(ProductFound,this);

            return ProductFound_Plus_PriceAndUnitsOfSoldProduct;
        }

        protected virtual void AddProductScannedToProductListScanned(string barcodeRecieved)
        {
            ProductScannedInfo_ToRecieve ProductFound = Add_UnitsOfSoldProduct_And_SoldProductPrice(barcodeRecieved);

            if (ProductFound == null) return;

            ProductsListScanned_To_Recive.Add(ProductFound);
        }

        protected virtual ProductScannedInfo_ToRecieve GetTheProductFromScannedListById(long id)
        {
            return ProductsListScanned_To_Recive.FirstOrDefault(product => product.ProductInfo.id == id);
        }

        protected override void Increase_TheNumberByOne_Of_ProductScannedTheSecondTimeInArow(string duplicatedBarCodeEntred)
        {

            long DuplicatedProductId = long.Parse(duplicatedBarCodeEntred);

            ProductScannedInfo_ToRecieve ProductDuplicatedFound = GetTheProductFromScannedListById(DuplicatedProductId);
            //
            // we increment by 1 a string value the increase funciton is converting the value to int to increament it then going back to string to be bound to the ui
            ProductDuplicatedFound.ProductsUnits = IncreaseByOneTheStringIntNumber(ProductDuplicatedFound.ProductsUnits);
            //
        }

        protected override void AddProductScannedToScreenOperation()
        {

            string BarCodeUserHasEntred = WhichBarCodeUserIsFillingManualOrAuto();

            bool UserDidntScanOrEnteredManualBarcode = string.IsNullOrEmpty(BarCodeUserHasEntred);

            if (UserDidntScanOrEnteredManualBarcode) return;

            // bool ThereIsAnErrorShownAtScreen = IsTheScreenShowingError();

            // if (ThereIsAnErrorShownAtScreen) return;

            if (TheProductIsAlreadyAddedToAlist(BarCodeUserHasEntred))
            {
                Increase_TheNumberByOne_Of_ProductScannedTheSecondTimeInArow(BarCodeUserHasEntred);
            }

            else AddProductScannedToProductListScanned(BarCodeUserHasEntred);

        }


    }

}
