using GetStartedApp.Models;
using System;
using System.Windows.Input;
using System.Collections.Generic;
using ReactiveUI;
using GetStartedApp.ViewModels.ProductPages;
using System.Threading.Tasks;
using GetStartedApp.Helpers;
using Avalonia.Media.Imaging;
using GetStartedApp.ViewModels.CategoryPages;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Collections.ObjectModel;

namespace GetStartedApp.ViewModels.DashboardPages;

public class MainWindowViewModel : ViewModelBase
{

    private ViewModelBase _CurrentPage;

    private Stack<ViewModelBase> navigationHistory = new Stack<ViewModelBase>();
    
    public ViewModelBase CurrentPage
    {

        get { return _CurrentPage; }
         set { this.RaiseAndSetIfChanged(ref _CurrentPage, value); }
    }

    private bool _BtnIsVisible;
    
    public bool IsBackPageBtnVisible
   {
       get { return _BtnIsVisible; }
       private set { this.RaiseAndSetIfChanged(ref _BtnIsVisible, value); }
   }

    private bool _IsappHeaderVisible=true;

    public bool IsAppHeaderVisible
    {
        get => _IsappHeaderVisible;
        set => this.RaiseAndSetIfChanged(ref _IsappHeaderVisible, value);
    }

    // this variable will hold two paths of bell images one image bell without red circle indicationg no product with least count in stock 
    // and other with red circle that indicate a stock has products with minor quantity

    private Bitmap? _notificationBellImage;

    public Bitmap NotificationBellImage
    {
        get => _notificationBellImage;
        set => this.RaiseAndSetIfChanged(ref _notificationBellImage, value);
    }


    public ICommand goBackToPreviousPageCommand { get; set; }




    public MainWindowViewModel()
    {

        initializePagesToNavigate();

        // add page to navigation history to go back to it
       // navigationHistory.Push(_CurrentPage);

         IsBackPageBtnVisible = false;
         IsAppHeaderVisible = true;

        // the backup is done every one we open the program
        DoBackup();

        CheckIfSystemShouldRaiseBellNotificationIcon();

        CultureHelper.SetLanguageSystem("ar-Ma");
       // GoToLiscencePage();
        
        GoToDashboardPage();
        // ObservableCollection<ProductsScannedInfo> productsScannedInfos = new ObservableCollection<ProductsScannedInfo>();

        // CurrentPage = new BLViewModel(this,new MakeSaleViewModel(this), productsScannedInfos);


        // CurrentPage = new ClientsListViewModel(this);

       // CurrentPage = new CompanyInfosViewModel();

    }

    private void DoBackup()
    {
        AccessToClassLibraryBackendProject.DoDailyBackup();
    }

    public void CheckIfSystemShouldRaiseBellNotificationIcon()
    {
        if(AccessToClassLibraryBackendProject.IsLowStock()) EnableTheBellNotification_Of_ProductsWith_LowerQuantityInTheStock();

        else DisableTheBellNotification_Of_ProductsWith_LowerQuantityInTheStock() ;
    }
    private bool CheckIfUserIsHasEntredLicense()
    {
        return AccessToClassLibraryBackendProject.CheckIfAdminHasSignedBefore();
    }
    // initialzing the pages that will be displaying these pages are the ones that 
    // will be binded with xaml code like for example { Biniding _CurrentPage}
    // each page icon of dashboard will open a new dialog of given usercontrol
    private void initializePagesToNavigate()
    {
     

        GoToDashboardPage();

        HideheaderAndShowBtnBack();

        // this is the command that will run from ui to go back of the page
        goBackToPreviousPageCommand = ReactiveCommand.Create(GoBackToPreviousPage);
     
    }

 
    // this method trigger an event that invoke each method existing in the view control
    // which is responsible to show a view as dialog

    private void GoToLiscencePage()
    {
        // if the user is paid or still have time on his trial mode the page returned will be login which is going to allow him to go and use the app
        // if not it will be stuck at license key pase untill he enteer the right password or license key 
        ViewModelBase PageToGoAfterCheckingLicenseKey = new LisenceKeyVerificationViewModel(this).CheckIfAdminIsRegistredBeforeOrNot_And_returnThePageToGoOn();
        
        CurrentPage = PageToGoAfterCheckingLicenseKey;
    }

    public void GoToProductsBoughtsPage()
    {
      
        CurrentPage = new ProductsBoughtsViewModel(this);
        navigationHistory.Push(CurrentPage);
        HideheaderAndShowBtnBack();
    }

    

    public void GoToSuppliersPage()
    {
        CurrentPage = new SuppliersListViewModel(this);
        navigationHistory.Push(CurrentPage);
        HideheaderAndShowBtnBack();
    }

    public void GoToBonReceptionPage() { 

        CurrentPage = new BonReceptionViewModel(this);
        navigationHistory.Push(CurrentPage);
        HideheaderAndShowBtnBack();
    }

    public void GoToBonLivraisonsPage()
    {
        CurrentPage = new BonLivraisonsViewModel(this);
        navigationHistory.Push(CurrentPage);
        HideheaderAndShowBtnBack();
    }

    public void GoToDevisPage()
    {
        CurrentPage = new DevisViewModel(this);
        navigationHistory.Push(CurrentPage);
        HideheaderAndShowBtnBack();
    }

    public void GoToMakeSaleForCompaniesPage()
    {
        CurrentPage = new MakeSaleForCompaniesViewModel(this);
        navigationHistory.Push(CurrentPage);
        HideheaderAndShowBtnBack();
    }

    public void GoToClientsPage()
    {
        CurrentPage = new ClientsListViewModel(this);

        navigationHistory.Push(CurrentPage);

        HideheaderAndShowBtnBack();
    }


    public void GoToDashboardPage()
    {
        CurrentPage = new DashboardViewModel(this);

        navigationHistory.Push(CurrentPage);

        // we set uis like buttn or any elmnts that dynmically appears and dissaper to default state for the welcome page
       // HideheaderAndShowBtnBack();

    }

    public void GoToProductPage()
    {
    
        CurrentPage = new ProductsListViewModel(this);

        navigationHistory.Push(CurrentPage);

        // we set uis like buttn or any elements that dynmically appears and dissaper to default state for the welcome page
        HideheaderAndShowBtnBack();

    }

    public void EnableTheBellNotification_Of_ProductsWith_LowerQuantityInTheStock()
    {  
        NotificationBellImage= ImageHelper.LoadFromResource(new Uri("avares://GetStartedApp/Assets/Icons/NotificationYes.png"));
    }

    public void DisableTheBellNotification_Of_ProductsWith_LowerQuantityInTheStock()
    {

        NotificationBellImage = ImageHelper.LoadFromResource(new Uri("avares://GetStartedApp/Assets/Icons/NotificationNo.png"));
    }

    public void GoToCategoriesProductPage()
    {
         //navigationHistory.Push(CategoryProductsPage);

        CurrentPage = new CategoryProductsViewModel(this);

        navigationHistory.Push(CurrentPage);

        HideheaderAndShowBtnBack();
    }

    public void GoToBarCodeProductPage()
    {
        // we set theses default numbers to opne barcode in the manual mode
        // in the auto mode these values are set depedning on the products quantity added 
        string defaultBarCodeNumber = "1";
        string defaultNumberOfBarCodes = "1";
        bool canIChangeBarcodeNumbers = true;

        CurrentPage = new BarCodeGeneratorViewModel(defaultBarCodeNumber,defaultNumberOfBarCodes,canIChangeBarcodeNumbers);

        navigationHistory.Push(CurrentPage);

        HideheaderAndShowBtnBack();
    }

    public void GoToFinancesPage()
    {
        CurrentPage = new FinancesViewModel();

        navigationHistory.Push(CurrentPage);

        HideheaderAndShowBtnBack();
    }

    public void GoToRetrunProductPage()
    {
        CurrentPage = new ReturnedProductsViewModel();

        navigationHistory.Push(CurrentPage);

        HideheaderAndShowBtnBack();
    }

    public void GoToMakeSalePage()
    {
      
        CurrentPage = new MakeSaleViewModel(this);

        navigationHistory.Push(CurrentPage);

        HideheaderAndShowBtnBack();
    }

    public void GoToSoldItemsPage()
    {
        CurrentPage = new SoldProductsViewModel();

        navigationHistory.Push(CurrentPage);

        HideheaderAndShowBtnBack();
    }

    public ViewModelBase GoToLoginPage()
    {
        CurrentPage = new LoginPageViewModel(this);

        navigationHistory.Clear();

        return CurrentPage;
    }

    // Normally the stack is equal to 1 including the pricipal page
    // when a user click to a button that leads to another page the stack count
    // increase by 1 which contain the page he entred , at the end a stack becomes greater than 1
    // this is indicate that UserIsNotInThePricipalPage and it could return back 
    private bool UserIsNotInThePrincipalPage()
    {
        return navigationHistory.Count > 1;
    }

    private bool UserIsInPrincipalPage() {

        return navigationHistory.Count ==1; 
    }

    public void setTheDefaultUiDynamicOptions()
    {
        IsBackPageBtnVisible = false;
        IsAppHeaderVisible = true;
    }

    public void HideheaderAndShowBtnBack()
    {
        IsBackPageBtnVisible = true;
        IsAppHeaderVisible = false;
    }
    public void GoBackToPreviousPage()
    {

        if (UserIsNotInThePrincipalPage())
        {
            navigationHistory.Pop();
            CurrentPage = navigationHistory.Peek();

            if (UserIsInPrincipalPage())
            {
                setTheDefaultUiDynamicOptions();
            }
        }
 
    }

   
    // this section is for test
//    async Task<bool> AddingNewCategoryProduct_Test_Operation(string categoryName)
//    {
        
//        return  await new AddNewCategoryViewModel(null).AddNewCategoryToDatabaseEndToEndTest(categoryName);
//    }

//    async Task<(bool, string)> EditingTheNewCategoryProductAdded_Test_Operation(string previousCategoryName)
//    {
//        string newCategoryName = previousCategoryName + " EditedCategory";
//        bool result = await new AddNewCategoryViewModel(null).UpdateCategoryToDatabaseEndToEndTest(previousCategoryName, newCategoryName);
//        return (result, newCategoryName);
//    }

//    async Task <bool> AddNewProduct_Test_Operation(string productName, string productDescritption , string stockQuantity , string price , string cost , string categoryName)
//    {
          
//        return await new AddProductViewModel(null).AddProductPartOfEndToEndTest(productName, productDescritption, stockQuantity, price, cost, categoryName);

//    }

  
//    async void EndToEndTest()
//    {
        
//       string productName = "product 1";
//       string productDescritption = "productDescription 1";
//       string stockQuantity = "1";
//       string price = "2";
//       string cost = "1";
//       string categoryName ="paints";


//       // return;

//        if (!await AddingNewCategoryProduct_Test_Operation(categoryName)) { Debug.WriteLine("Error In Adding New CategoryName"); return; }

//        var (ProductCategoryIsEditedSuccessfully, updatedCategoryName) = await EditingTheNewCategoryProductAdded_Test_Operation(categoryName);

//        if (!ProductCategoryIsEditedSuccessfully) { Debug.WriteLine("Error In Editing New CategoryName"); return; }

//        if (!await AddNewProduct_Test_Operation(productName,productDescritption,stockQuantity,price,cost, updatedCategoryName)) { Debug.WriteLine("Error In Editing New CategoryName"); return; }
//    }


//#pragma warning disable CA1822 // Mark members as static
//    public string Greeting1 => "ClsAccess";
//#pragma warning restore CA1822 // Mark members as static

//    public string CustomisedGreetings => "hellow app";


}


