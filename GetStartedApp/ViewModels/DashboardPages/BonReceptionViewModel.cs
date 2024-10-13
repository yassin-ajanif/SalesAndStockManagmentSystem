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

        public ReactiveCommand<Unit, Unit> AddNewProductCommand { get; set; }

        public ReactiveCommand<Unit, Unit> SaveRecieveOperationCommand { get; }
        public Interaction<AddProductViewModel, Unit> ShowAddProductDialog { get; }

        public BonReceptionViewModel(MainWindowViewModel mainWindowViewModel) : base(mainWindowViewModel)
        {
            // addProductViewModel = new ProductsListViewModel(mainWindowViewModel);
            AddNewProductCommand = ReactiveCommand.Create(AddProductInfoOperation);
            ShowAddProductDialog = new Interaction<AddProductViewModel, Unit>();
            SaveRecieveOperationCommand =  ReactiveCommand.Create(AddListOfProductRecivedToDatabase);
            ProductsListScanned_To_Recive = new ObservableCollection<ProductScannedInfo_ToRecieve>();
            SuppliersList = getSuppliers();
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
            productTable.Columns.Add("Price", typeof(float));
            productTable.Columns.Add("Cost", typeof(float));
            productTable.Columns.Add("Profit", typeof(float));
            productTable.Columns.Add("SelectedCategory", typeof(string));
            productTable.Columns.Add("SelectedProductImage", typeof(byte[]));
            productTable.Columns.Add("ProductsUnits", typeof(string));
            productTable.Columns.Add("ProductsUnitsToAddToStock1", typeof(string));
            productTable.Columns.Add("ProductsUnitsToAddToStock2", typeof(string));
            productTable.Columns.Add("ProductsUnitsToAddToStock3", typeof(string));
            productTable.Columns.Add("ThisProductIsExistingInDB", typeof(bool));

            return productTable;
        }

        void LoadListOfProductsRecived_To_Table()
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

            DisplayDataTable(productTable);

            // Optional: You can return the DataTable or do something with it
        }

        private void DisplayDataTable(DataTable table)
        {
            // Ensure the table is not null
            if (table == null)
            {
                Debug.WriteLine("DataTable is null.");
                return;
            }

            // Display table name
            Debug.WriteLine($"Table: {table.TableName}");

            // Display each row with formatted output
            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn column in table.Columns)
                {
                    Debug.Write($"{column.ColumnName} => {row[column]}\t");
                }
                Debug.WriteLine(""); // New line after each row
            }
        }

        private void AddListOfProductRecivedToDatabase()
        {
            LoadListOfProductsRecived_To_Table();
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
