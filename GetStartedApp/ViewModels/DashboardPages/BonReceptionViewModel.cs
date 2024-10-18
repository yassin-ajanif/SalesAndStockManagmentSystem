using GetStartedApp.Models;
using ReactiveUI;
using System.Collections.Generic;
using System.Reactive;
using GetStartedApp.ViewModels.ProductPages;
using System.Windows.Input;
using GetStartedApp.Helpers;
using System.Reactive.Linq;
using GetStartedApp.Models.Objects;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data;
using System.Diagnostics;
using GetStartedApp.Views;
using System;
using GetStartedApp.Helpers.CustomUIErrorAttributes;


namespace GetStartedApp.ViewModels.DashboardPages
{

    public class BonReceptionViewModel : MakeSaleViewModel
    {
        
        private string _selectedSupplierName_PhoneNumber;

        public string SelectedSupplierName_PhoneNumber
        {
            get => _selectedSupplierName_PhoneNumber;
            set => this.RaiseAndSetIfChanged(ref _selectedSupplierName_PhoneNumber, value);
        }


        private string _bonReceptionNumber;
        [CheckForInvalidCharacters]
        [MaxStringLengthAttribute_IS(50, "هذه الرقم طويل جدا")]
        public string BonReceptionNumber
        {
            get => _bonReceptionNumber;
            set
            {
                // Check if value is null or whitespace, and set to null
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = null;
                }
                this.RaiseAndSetIfChanged(ref _bonReceptionNumber, value);
            }
        }

        private string _entredSupplierName_PhoneNumber;
        public string EntredSupplierName_PhoneNumber
        {
            get => _entredSupplierName_PhoneNumber;
            set
            {
                // Check if value is null or whitespace, and set to null
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = null;
                }
                this.RaiseAndSetIfChanged(ref _entredSupplierName_PhoneNumber, value);
            }
        }


        private List<string> _suppliersList;
        private string _supplierName;
        public List<string> SuppliersList { get => _suppliersList; set => this.RaiseAndSetIfChanged(ref _suppliersList, value); }

        private ObservableCollection<ProductScannedInfo_ToRecieve> _productsListScanned_To_Recive;

        public ObservableCollection<ProductScannedInfo_ToRecieve> ProductsListScanned_To_Recive
        {
            get => _productsListScanned_To_Recive;
            private set => this.RaiseAndSetIfChanged(ref _productsListScanned_To_Recive, value);
        }

        public ReactiveCommand<Unit, Unit> AddNewProductCommand { get; set; }

        public ReactiveCommand<Unit, Unit> SaveRecieveOperationCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteAllProductScannedCommand { get; }
        public Interaction<AddProductViewModel, Unit> ShowAddProductDialog { get; }

        public BonReceptionViewModel(MainWindowViewModel mainWindowViewModel) : base(mainWindowViewModel)
        {
            // addProductViewModel = new ProductsListViewModel(mainWindowViewModel);
            AddNewProductCommand = ReactiveCommand.Create(AddProductInfoOperation);
            ShowAddProductDialog = new Interaction<AddProductViewModel, Unit>();
            SaveRecieveOperationCommand = ReactiveCommand.Create(AddListOfProductRecivedToDatabase, CheckIfSystemIsNotRaisingError_And_ExchangeIsPositiveNumber_And_ProductListIsNotEmpty_Every_500ms());
            DeleteAllProductScannedCommand = ReactiveCommand.Create(DeleteAllProductAddedToReceptionList);
            ProductsListScanned_To_Recive = new ObservableCollection<ProductScannedInfo_ToRecieve>();
            SuppliersList = getSuppliers();
           
        }



        protected bool IsEntredSupplierName_PhoneNumber_IsNotExisting()
        {
      
            return (!SuppliersList.Contains(EntredSupplierName_PhoneNumber));           
                   
        }

        protected override IObservable<bool> CheckIfSystemIsNotRaisingError_And_ExchangeIsPositiveNumber_And_ProductListIsNotEmpty_Every_500ms()
        {
            // Initial condition to keep the observable running indefinitely

            var observable = Observable.Interval(TimeSpan.FromMilliseconds(500))
                                       .Select(_ =>
                                           TheSystemIsNotShowingError() &&
                                           ProductsListScanned_To_Recive.Count > 0
                                           
                                       )
                                       .DistinctUntilChanged()
                                       // .Do(_ => Debug.WriteLine("didi")) // Invoke Debug.WriteLine for each emission                                
                                       .ObserveOn(RxApp.MainThreadScheduler);
            // Ensure this observable runs on the UI thread

            return observable;
        }

        bool IsBonReception_And_SelectedSupplierName_Empty => string.IsNullOrWhiteSpace(BonReceptionNumber) && string.IsNullOrWhiteSpace(EntredSupplierName_PhoneNumber);
        bool IsBonReception_And_SelectedSupplierName_Filled => !string.IsNullOrWhiteSpace(BonReceptionNumber) && !string.IsNullOrWhiteSpace(EntredSupplierName_PhoneNumber);
      
        private bool IsUiShowingError_Of_AlreadyExisted_BlSupplierName()
        {
            
            if (IsBonReception_And_SelectedSupplierName_Filled) {

                 string SupplierName = StringHelper.ExtractNameFrom_Combo_NamePhoneNumber(EntredSupplierName_PhoneNumber);
                bool ThisBlNumberAlreadyExistingForThisSupplierName =   AccessToClassLibraryBackendProject.IsSupplierBLNumberAlreadyExisitg_For_ThisSupplierName(SupplierName, BonReceptionNumber);

                if (ThisBlNumberAlreadyExistingForThisSupplierName) { ShowUiError(nameof(BonReceptionNumber), "هذا الرقم موجود من قبل"); return true; }

            }
            DeleteUiError(nameof(BonReceptionNumber), "هذا الرقم موجود من قبل");
         
            return false;
        }
        protected override bool CheckIf_ProductsUnits_And_SoldPrices_Of_ScannedProducts_Are_Valid()
        {
            foreach (var ProductScanned in ProductsListScanned_To_Recive)
            {
   
                if (string.IsNullOrEmpty(ProductScanned.ProductsUnits) || string.IsNullOrWhiteSpace(ProductScanned.ProductsUnits)) return false;
                if (string.IsNullOrEmpty(ProductScanned.PriceOfProductSold) || string.IsNullOrWhiteSpace(ProductScanned.PriceOfProductSold)) return false;
                if (ProductScanned.ProductStockHasErrors) return false;
   
                if (!UiAttributeChecker.AreThesesAttributesPropertiesValid
                    (ProductScanned, nameof(ProductScanned.ProductsUnits), nameof(ProductScanned.PriceOfProductSold)))
                {
   
                    return false;
                }
   
            }
   
            return true;
        }

        private bool Are_SuppplierName_And_BlSupplier_Valid()
        {

            // Check if either of the fields is null or contains only whitespace
            if (IsBonReception_And_SelectedSupplierName_Empty || IsBonReception_And_SelectedSupplierName_Filled)
            {

                if (IsBonReception_And_SelectedSupplierName_Filled && IsEntredSupplierName_PhoneNumber_IsNotExisting()) { displayErrorMessage("يرجى إدخال مورد مسجل."); return false; }
                else if (IsBonReception_And_SelectedSupplierName_Filled && IsUiShowingError_Of_AlreadyExisted_BlSupplierName()) { displayErrorMessage("هناك خطأ في معلومات المزود"); return false; }

                else { deleteDisplayedError(); return true; }
            }



            else { displayErrorMessage("ادخل الموزع ورقم الوصل او دعهما فارغين"); return false; }

                    
               
        }

        protected void deleteAllErrors_When_ProductList_Empty() { 
          
            deleteDisplayedError();
            DeleteAllUiErrorsProperty(nameof(BonReceptionNumber));
        }
        protected override void WhenUserSetInvalidProduct_Price_Or_Quantity_Block_TheSystem_From_Adding_NewProducts_AndShowError_Plus_MakeAdditional_Checkings()
        {

            this.WhenAnyValue(x => x.ProductsListScanned_To_Recive.Count, x => x.BonReceptionNumber, x => x.EntredSupplierName_PhoneNumber)
              .Subscribe(async BarcodeNumberScanned =>
              {
                 
                  deleteAllErrors_When_ProductList_Empty();

                  foreach (var product in ProductsListScanned_To_Recive)
                  {
                      // Observe each property of each product
                      product.
                    WhenAnyValue(
                          p => p.ProductsUnits,
                          p => p.PriceOfProductSold,
                          p => p.ProductsUnitsToReduce_From_Stock1,
                          p => p.ProductsUnitsToReduce_From_Stock2,
                          p => p.ProductsUnitsToReduce_From_Stock3
                         

                          )
                          .Subscribe(_ =>
                          {
                              // this function iside has error to show
                              if (!Are_SuppplierName_And_BlSupplier_Valid()) ;
                              else if (!CheckIf_ProductsUnits_And_SoldPrices_Of_ScannedProducts_Are_Valid()) displayErrorMessage("هناك خطأ في كمية المنتج او السعر");
                              else deleteDisplayedError();

                              CalculateTheTotalPriceOfOperation();

                          }
                          );
                  }
              }
                );
        }


        private DataTable CreateTableOfProductInfoRecived_To_Send_To_Database()
        {
            DataTable productTable = new DataTable("Products");

            // Define columns based on the object properties
            productTable.Columns.Add("Id", typeof(long));
            productTable.Columns.Add("Name", typeof(string));
            productTable.Columns.Add("Description", typeof(string));
            productTable.Columns.Add("StockQuantity", typeof(int));
            productTable.Columns.Add("StockQuantity2", typeof(int));
            productTable.Columns.Add("StockQuantity3", typeof(int));
            productTable.Columns.Add("Price", typeof(decimal));
            productTable.Columns.Add("Cost", typeof(decimal));
            productTable.Columns.Add("Profit", typeof(decimal));
            productTable.Columns.Add("SelectedCategory", typeof(string));
            productTable.Columns.Add("SelectedProductImage", typeof(byte[]));
            productTable.Columns.Add("ProductsUnitsToAddToStock1", typeof(int));
            productTable.Columns.Add("ProductsUnitsToAddToStock2", typeof(int));
            productTable.Columns.Add("ProductsUnitsToAddToStock3", typeof(int));
            productTable.Columns.Add("ThisProductIsExistingInDB", typeof(bool));

            return productTable;
        }

       protected DataTable LoadListOfProductsRecived_To_Table()
        {
            DataTable productTable = CreateTableOfProductInfoRecived_To_Send_To_Database();

            foreach (var product in ProductsListScanned_To_Recive)
            {
                // Create a new row for the DataTable
                DataRow row = productTable.NewRow();

                // Assign values to the row
                row["Id"] = product.ProductInfo.id;
                row["Name"] = product.ProductInfo.name;
                row["Description"] = product.ProductInfo.description;
                row["StockQuantity"] = product.ProductInfo.StockQuantity;
                row["StockQuantity2"] = product.ProductInfo.StockQuantity2;
                row["StockQuantity3"] = product.ProductInfo.StockQuantity3;
                row["Price"] = product.ProductInfo.price;
                row["Cost"] = product.ProductInfo.cost;
                row["Profit"] = product.ProductInfo.profit;
                row["SelectedCategory"] = product.ProductInfo.selectedCategory;
                row["SelectedProductImage"] = ImageConverter.BitmapToByteArray(product.ProductInfo.SelectedProductImage);
                // the naming are not matching becuase the productsunit to reduce from stock belong to anohter class that actually reduces the stock which is make sale
                // i didn't have the time to create another properties with corrected description so for this reason i decided 
                row["ProductsUnitsToAddToStock1"] = product.ProductsUnitsToReduce_From_Stock1;
                row["ProductsUnitsToAddToStock2"] = product.ProductsUnitsToReduce_From_Stock2;
                row["ProductsUnitsToAddToStock3"] = product.ProductsUnitsToReduce_From_Stock3;
                row["ThisProductIsExistingInDB"] = product.ThisProductIsExistingInDB;

                // Add the row to the DataTable
                productTable.Rows.Add(row);
            }

            return productTable;
        }

        protected void ClearAllReceptionListItems()
        {
            ProductsListScanned_To_Recive.Clear();
        }

        private async void DeleteAllProductAddedToReceptionList()
        {
            bool UserHasClickedYesToDeleteSaleOperationBtn = await ShowDeleteSaleDialogInteraction.Handle("هل تريد حقا حذف جميع المنتوجات");

            if (UserHasClickedYesToDeleteSaleOperationBtn) ClearAllReceptionListItems();
        }
        private async void AddListOfProductRecivedToDatabase()
        {
            DataTable productToAddOrUpdateTable = LoadListOfProductsRecived_To_Table();
            string SupplierName = StringHelper.ExtractNameFrom_Combo_NamePhoneNumber(EntredSupplierName_PhoneNumber);
            string SelectedPaymentMethodInArabic = WordTranslation.TranslatePaymentIntoTargetedLanguage(SelectedPaymentMethod, "en");

            if (AccessToClassLibraryBackendProject.AddOrUpdateProducts(productToAddOrUpdateTable, SupplierName, BonReceptionNumber, SelectedPaymentMethodInArabic))
            {
                await ShowAddSaleDialogInteraction.Handle("لقد تمت العملية بنجاح");

                ClearAllReceptionListItems();
            }
         
            else await ShowAddSaleDialogInteraction.Handle("لقد حصل خطأ ما");
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

            if (TheProductIsAlreadyAddedToAlist(BarCodeUserHasEntred))
            {
                Increase_TheNumberByOne_Of_ProductScannedTheSecondTimeInArow(BarCodeUserHasEntred);
            }

            else AddProductScannedToProductListScanned(BarCodeUserHasEntred);

        }


    }

}
