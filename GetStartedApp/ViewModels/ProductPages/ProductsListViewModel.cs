using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GetStartedApp.Models;
using ReactiveUI;
using System.Windows.Input;
using System.Reactive;
using System.Reactive.Linq;
using System.Linq;
using GetStartedApp.Views;
using System.Threading.Tasks;
using GetStartedApp.ViewModels.DashboardPages;
using MsBox.Avalonia.Base;
using GetStartedApp.Models.Objects;
using GetStartedApp.Models.Enums;






namespace GetStartedApp.ViewModels.ProductPages
{
    public class ProductsListViewModel : ViewModelBase
    {

        private List<ProductInfo> ProductsList;

        private List<string> _Productcategories;
        public List<string> ProductCategories
        {
            get { return _Productcategories; }
            set { this.RaiseAndSetIfChanged(ref _Productcategories, value); }
        }

        private string _SelectedCategory;
        public string SelectedCategory
        {
            get { return _SelectedCategory; }
            set { this.RaiseAndSetIfChanged(ref _SelectedCategory, value); }
        }

        private string _SearchedProducts="0";
        public string SearchedProducts
        {
            get { return _SearchedProducts; }
            set { if(value.Length<=18)this.RaiseAndSetIfChanged(ref _SearchedProducts, value); }
        }

        public long ClickedProductInfoID;

       // public Interaction<AddProductViewModel, Unit> ShowAddProductDialog { get; }
        public Interaction<EditStockQuantitiyProductViewModel, Unit> ShowEditQuantityDialog { get; }
        public Interaction<EditPriceProductViewModel, Unit> ShowEditPriceDialog { get; }
        public Interaction<string, bool> ShowDeleteProductDialog { get; set; }
        public Interaction<AddProductViewModel, Unit> ShowEditAllInfoProductDialog { get; }
        public Interaction<ReturnProductViewModel, Unit> ShowReturnProductDialog { get; }

        public Interaction <long,Unit> ShowBarCodeViewGeneratorDialog { get; }
 
      //  public ICommand AddNewProductCommand { get; }

     
        private ObservableCollection<ProductInfo> _ProductsListObservable;
        public ObservableCollection<ProductInfo> ProductsListObservable
        {
            get { return _ProductsListObservable; }
            private set { this.RaiseAndSetIfChanged(ref _ProductsListObservable, value); }
        }

        public MainWindowViewModel MainWindowViewModel { get; }
   
        public ProductsListViewModel(MainWindowViewModel mainWindowViewModel)
        {
            this.MainWindowViewModel = mainWindowViewModel;

            LoadProductCategoryList();
            
            WhenUserChangesCategories_LoadProductList_Of_CategoryChosen();

            WhenUserSearchForProducts_LoadProductListFound();

            set_DefaultSelectedCategory();

            IObservable<bool> canExecute = Observable.Return(true);

            IObservable<bool> canDeleteProduct = Observable.Return(true);

           // ShowAddProductDialog = new Interaction<AddProductViewModel, Unit>();

            ShowEditQuantityDialog = new Interaction<EditStockQuantitiyProductViewModel, Unit>();

            ShowEditPriceDialog = new Interaction<EditPriceProductViewModel, Unit>();

            ShowDeleteProductDialog = new Interaction<string, bool>();

            ShowEditAllInfoProductDialog = new Interaction<AddProductViewModel, Unit>();

            ShowBarCodeViewGeneratorDialog = new Interaction<long, Unit>();

            ShowReturnProductDialog = new Interaction<ReturnProductViewModel, Unit>();

            //AddNewProductCommand = ReactiveCommand.Create(AddProductInfoOperation, canExecute);

        }


        protected void LoadProductCategoryList()
        {

            ProductCategories = getProductListCategoriesFromDb();
            
                     
        }

        // we initialize the value of selected categories and serachproduct term
        private void set_DefaultSelectedCategory() {

            //we added all category to the product list category so we can display all the products items
            string defaultCategory = "جميع الاصناف";
            ProductCategories.Add(defaultCategory);
            // we set it as default value tobe displayed when we lunch the product window
            SelectedCategory = defaultCategory;

            

        }

        private async void LoadAllProductListIntoScreen()
          {

             ProductsList = AccessToClassLibraryBackendProject.GetProductsInfoList();


            ProductsListObservable = new ObservableCollection<ProductInfo>(ProductsList);

            MainWindowViewModel.CheckIfSystemShouldRaiseBellNotificationIcon();

        }

        // check if this product has already been sold, because we can't delete sold product from products list 
        // this will cause inconsistent data and database will not allow that
        private async Task<bool> ThisProductHasAlreadyBeenSold(long productId)
        {
            bool thisProductHasAlreadyBeenBeenSold = AccessToClassLibraryBackendProject.CheckIfProductAlreadyExistingInSoldItemsList(productId);

            if (thisProductHasAlreadyBeenBeenSold) await ShowDeleteProductDialog.Handle("لا يمكنك حذف منتج مسجل في قائمة المبيعات");

            return thisProductHasAlreadyBeenBeenSold;
        }

        private void LoadProductsListIntoScreenByCategory()
        {
       
            ProductsList = AccessToClassLibraryBackendProject.GetProductsInfoListByCategories(SelectedCategory);
       
            ProductsListObservable = new ObservableCollection<ProductInfo>(ProductsList);
      
           
       }
      
        private void LoadProductsListIntoScreenByCategory_WhenUserSwitchCategories()
        {
            // this is the first time when the user opens up a widnow selected category selectedcategory is set to All by default
            
            if ( SelectedCategory == "جميع الاصناف")  LoadAllProductListIntoScreen();
           
            else LoadProductsListIntoScreenByCategory();

        }
   
        private void EraseSearchBar()
        {
            SearchedProducts = "";
        }
     
        private void SearchProductByBarcodeNumber(string SelectedCategoryToSendToDatabase,Decimal barcodeNumber)
        {
            ProductsList = AccessToClassLibraryBackendProject.GetProductsInfoListBy_CategoryName_And_SearchProductID(SelectedCategoryToSendToDatabase, barcodeNumber);
            ProductsListObservable = new ObservableCollection<ProductInfo>(ProductsList);
        }

        private void SearchProudctByDigitsThatANameContain(string SelectedCategoryToSendToDatabase, String searchedProduct)
        {
            ProductsList = AccessToClassLibraryBackendProject.GetProductsInfoListBy_CategoryName_And_SearchProductName(SelectedCategoryToSendToDatabase, SearchedProducts);

            ProductsListObservable = new ObservableCollection<ProductInfo>(ProductsList);
        }
        private void Detect_How_To_SearchProduct_ByItsName_Or_BarCodeNumber(string SelectedCategoryToSendToDatabase, string SearchedProducts)
        {
            Decimal searchNumberStringConvertedSuccesffullyToNumber;
            bool SearchedProduct_Has_Only_Digits = decimal.TryParse(SearchedProducts, out searchNumberStringConvertedSuccesffullyToNumber);

            // if searched product has only digits it mean that you are searching product by barcode number not name so we have to call a specific function that 
            // search products by barcode number wich is product id     
            if(SearchedProduct_Has_Only_Digits) { SearchProductByBarcodeNumber(SelectedCategoryToSendToDatabase, searchNumberStringConvertedSuccesffullyToNumber); }

            else
            {
                SearchProudctByDigitsThatANameContain(SelectedCategoryToSendToDatabase, SearchedProducts);
            }
        }
        private void LoadProductListIntoScreen_By_SelectedCategory_And_SearchProductName_WhenUserSearchProduct()
        {
            // the stored procedure regoginze All not in arabic so for this raison i made this state change
            string SelectedCategoryToSendToDatabase;
            if (SelectedCategory == "جميع الاصناف")  SelectedCategoryToSendToDatabase = "All";
            else SelectedCategoryToSendToDatabase = SelectedCategory;


            Detect_How_To_SearchProduct_ByItsName_Or_BarCodeNumber(SelectedCategoryToSendToDatabase, SearchedProducts);


        }
      
        private void WhenUserSearchForProducts_LoadProductListFound()
        {
            this.WhenAnyValue(x => x.SearchedProducts).Subscribe(x =>
             {
                 LoadProductListIntoScreen_By_SelectedCategory_And_SearchProductName_WhenUserSearchProduct(); 

                 }
            );
        }

        private void WhenUserChangesCategories_LoadProductList_Of_CategoryChosen()
        {

            this.WhenAnyValue(x => x.SelectedCategory).Subscribe(x => {
               
                EraseSearchBar();
                LoadProductsListIntoScreenByCategory_WhenUserSwitchCategories();

                });

        }

        public void ReloadProductListIntoSceen() {

            LoadAllProductListIntoScreen();

        }

        // this mehtods are invoked by event trigger set up into xaml code onEditClickedBtn
       
        private async Task<bool> IsLoggerIsNotAnAdmin()
        {
            bool UserDosentHavePermission = AppLoginMode == eLoginMode.User;

            // showdelete rpoduct dialog is a message box container that show message , normally i should give it a general name but later i will do that
            if (UserDosentHavePermission) { await ShowDeleteProductDialog.Handle("ليست لديك الصلاحية كمستخدم"); }

            return UserDosentHavePermission;
        }

      //  public async void AddProductInfoOperation()
      //  {
      //      // 1 is the default all category string we add which make the count equal 1 once we add a new category a value becomes more than 1
      //      // which indicate that there is a category or more added to the system
      //      bool ThereIsNoCategoriesAddedYetToSystem = _Productcategories.Count == 1;
      //
      //      if (ThereIsNoCategoriesAddedYetToSystem) { await ShowDeleteProductDialog.Handle(" لا توجد تصنيفات اضف تصنيف جديد "); return; }
      //
      //      var userControlToShowInsideDialog = new AddProductViewModel(this);
      //
      //      await ShowAddProductDialog.Handle(userControlToShowInsideDialog);
      //  }
        public async void EditProductQuantityOperation()
        {

            if (await IsLoggerIsNotAnAdmin()) return;

            ProductInfo productInfoSelected = getProductInfoFromProductListByID(ClickedProductInfoID);

            var userControlToShowInsideDialog =

                new EditStockQuantitiyProductViewModel
                (productInfoSelected.SelectedProductImage, productInfoSelected.id, productInfoSelected.name, productInfoSelected.description, productInfoSelected.cost,
                productInfoSelected.price, productInfoSelected.profit, productInfoSelected.StockQuantity,productInfoSelected.StockQuantity2,productInfoSelected.StockQuantity3,
                productInfoSelected.selectedCategory,this);

            
            await ShowEditQuantityDialog.Handle(userControlToShowInsideDialog);
        }

        public async void EditProductPriceOperation(){

            if (await IsLoggerIsNotAnAdmin()) return;

            ProductInfo productInfoSelected = getProductInfoFromProductListByID(ClickedProductInfoID);

            var userControlToShowInsideDialog =

                new EditPriceProductViewModel
                (productInfoSelected.SelectedProductImage, productInfoSelected.id, productInfoSelected.name, productInfoSelected.description, productInfoSelected.cost,
                productInfoSelected.price, productInfoSelected.profit,
                productInfoSelected.StockQuantity, productInfoSelected.StockQuantity2, productInfoSelected.StockQuantity3, productInfoSelected.selectedCategory,this);


            await ShowEditPriceDialog.Handle(userControlToShowInsideDialog);
        }


        public async void EditAllProductsInfoOperation()
        {
            if (await IsLoggerIsNotAnAdmin()) return;

            ProductInfo productInfoSelected = getProductInfoFromProductListByID(ClickedProductInfoID);

            var userControlToShowInsideDialog = new EditAllProductInfoViewModel
                (productInfoSelected.SelectedProductImage, productInfoSelected.id, productInfoSelected.name, productInfoSelected.description, productInfoSelected.cost,
                productInfoSelected.price, productInfoSelected.profit, productInfoSelected.StockQuantity, productInfoSelected.StockQuantity2, productInfoSelected.StockQuantity3
                , productInfoSelected.selectedCategory, this);

            await ShowEditAllInfoProductDialog.Handle(userControlToShowInsideDialog);
        }
       

        public async void DeleteProductOperation() {

            if (await IsLoggerIsNotAnAdmin()) return;

            long SelectedProductId = getProductInfoFromProductListByID(ClickedProductInfoID).id;

            if (await ThisProductHasAlreadyBeenSold(SelectedProductId)) return;

            bool UserHasClickedYesToDeleteProductBtn = await ShowDeleteProductDialog.Handle("هل تريد حقا حدف المنتج");

            if (UserHasClickedYesToDeleteProductBtn)
            {
               if(AccessToClassLibraryBackendProject.DeleteProduct(SelectedProductId)) {

                    await ShowDeleteProductDialog.Handle("تم حدف المنتج بنجاح");
                    ReloadProductListIntoSceen();
                }              

               else await ShowDeleteProductDialog.Handle("هناك مشلكة في حذف هذا المنتج");
            }

        }

        public async void PrintBarCodesOfThisProduct()
        {
            long productInfoIdSelected = getProductInfoFromProductListByID(ClickedProductInfoID).id;

             // we send teh productid which is going to be the barcode number to be printed at barcodeviewGenrator
            await ShowBarCodeViewGeneratorDialog.Handle(productInfoIdSelected);
        }

        private List<string> getProductListCategoriesFromDb()
        {
            return AccessToClassLibraryBackendProject.GetProductsCategoryFromDatabase();
        }
        
        private ProductInfo getProductInfoFromProductListByID(long ID)
        {
            return ProductsList.FirstOrDefault(p => p.id == ID);
        }

        public async void ReturnProductOperation() {

           
            ProductInfo productInfoSelected = getProductInfoFromProductListByID(ClickedProductInfoID);
            bool thisProductHasntBeenSold = !AccessToClassLibraryBackendProject.CheckIfProductAlreadyExistingInSoldItemsList(ClickedProductInfoID);

            if (thisProductHasntBeenSold) {  await ShowDeleteProductDialog.Handle("لا يمكنك استرجاع منتج لم يبع قط"); return; }
            
            var userControlToShowInsideDialog =

               new ReturnProductViewModel
               (productInfoSelected.SelectedProductImage, productInfoSelected.id, productInfoSelected.name, productInfoSelected.description, productInfoSelected.cost,
               productInfoSelected.price, productInfoSelected.profit, productInfoSelected.StockQuantity, productInfoSelected.StockQuantity2, productInfoSelected.StockQuantity3,
               productInfoSelected.selectedCategory, this);

            await ShowReturnProductDialog.Handle(userControlToShowInsideDialog);
         
        }


    }
}
