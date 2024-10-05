using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GetStartedApp.Helpers;
using GetStartedApp.Models;
using GetStartedApp.Helpers.CustomUIErrorAttributes;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive;
using DynamicData;
using static System.Net.Mime.MediaTypeNames;
using System.Reactive.Subjects;
using GetStartedApp.ViewModels.ProductPages;
using System.Data.SqlTypes;
using System.Data;
using System.Globalization;
using Avalonia.Threading;
using System.Collections.Generic;
using GetStartedApp.Models.Objects;


namespace GetStartedApp.ViewModels.DashboardPages
{
    public class MakeSaleViewModel : ViewModelBase
    {
        private const long maxNumberOfProductsCanSystemHold = 1_000_000_000_000_000_000;

        private string _barcode ;

        [PositiveIntRange(1, maxNumberOfProductsCanSystemHold, ErrorMessage = "ادخل رقم موجب وبدون فاصلة ")]
        [ProductIdIsNotExistingInDb ("هذا الرقم لا يوجد") ]
        public string Barcode
        {
            get => _barcode;
            set  {

                
                this.RaiseAndSetIfChanged(ref _barcode, value); } 
        }

        private string _manualBarcodeValue;

        [PositiveIntRange(1, maxNumberOfProductsCanSystemHold, ErrorMessage = "ادخل رقم موجب وبدون فاصلة ")]
        [ProductIdIsNotExistingInDb("هذا الرقم لا يوجد")]
        public string ManualBarcodeValue
        {
            get => _manualBarcodeValue;
            set {
             
                this.RaiseAndSetIfChanged(ref _manualBarcodeValue, value);
            }
        }

        private string _total="0";
        public string Total
        {
            get => _total;
            set => this.RaiseAndSetIfChanged(ref _total, value);
        }

       
        private string _amountPaid;
        [PositiveFloatRange(1, maxNumberOfProductsCanSystemHold, ErrorMessage = "ادخل رقم موجب  ")]
        [StringNotEmpty(ErrorMessage ="ادخل رقم")]
        public string AmountPaid
        {
            get => _amountPaid;
            set => this.RaiseAndSetIfChanged(ref _amountPaid, value);
        }

        private string _exchange="0";
        public string Exchange
        {
            get => _exchange;
            set => this.RaiseAndSetIfChanged(ref _exchange, value);
        }

        private string _TotalNumberOfProducts="0";
        public string TotalNumberOfProducts
        {
            get => _TotalNumberOfProducts;
            set => this.RaiseAndSetIfChanged(ref _TotalNumberOfProducts, value);
        }

        private string _selectedClientName_PhoneNumber;
        public string SelectedClientName_PhoneNumber
        {
            get => _selectedClientName_PhoneNumber;
            set => this.RaiseAndSetIfChanged(ref _selectedClientName_PhoneNumber, value);
        }

        private string _productNameTermToSerach;

        [NotWhitespaceAttribute(ErrorMessage= "اختر منتج او دع الخانة فارغة")]
        
        public string ProductNameTermToSerach
        {
            get => _productNameTermToSerach;
            set => this.RaiseAndSetIfChanged(ref _productNameTermToSerach, value);
        }

        private string _selectedProductNameTermSerach;

        public string SelectedProductNameTermSerach
        {
            get => _selectedProductNameTermSerach;
            set => this.RaiseAndSetIfChanged(ref _selectedProductNameTermSerach, value);
        }

        private List<string> _productSuggestionsAfterManualSearch;
        public List<string> ProductSuggestionsAfterManualSearch
        {
            get => _productSuggestionsAfterManualSearch;
            set => this.RaiseAndSetIfChanged(ref _productSuggestionsAfterManualSearch, value);
        }


        private List<string> _paymentsMethods;
        public List<string> PaymentsMethods 
        {
            get => _paymentsMethods;
            set => this.RaiseAndSetIfChanged(ref _paymentsMethods, value);
        }

        private Dictionary<string , long > _productNamesAndTheirIDs;


        private string _selectedPaymentMethod;
        public string SelectedPaymentMethod
        {
            get => _selectedPaymentMethod;
            set => this.RaiseAndSetIfChanged(ref _selectedPaymentMethod, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        private bool _isErrorLabelVisible;
        public bool isErrorLabelVisible
        {
            get => _isErrorLabelVisible;
            set => this.RaiseAndSetIfChanged(ref _isErrorLabelVisible, value);
        }

        private string _blackOrRedColor="Black";
        public string BlackOrRedColor
        {
            get => _blackOrRedColor;
            set => this.RaiseAndSetIfChanged(ref _blackOrRedColor, value);
        }

        private List<string> _clientList;

        private ChequeInfo _loadedCheckInfoByUser_When_Choosing_CheckPaymentType;

        public ReactiveCommand<Unit, Unit> SubmitBarCodeManually { get; }

        private ObservableCollection<ProductsScannedInfo> _productsListScanned;

        public ObservableCollection<ProductsScannedInfo> ProductsListScanned
        {
            get => _productsListScanned;
            private set => this.RaiseAndSetIfChanged(ref _productsListScanned, value);
        }

        

        private IDisposable ProductListObservationSubsription { get; set; }

        public ReactiveCommand<Unit, Unit> SaveSellingOperationCommand { get; }

        public ReactiveCommand<Unit, Unit> MakeNewSellingOperationCommand { get; }

        public Interaction<string,bool> ShowAddSaleDialogInteraction { get; set; }

        public Interaction<string,bool> ShowDeleteSaleDialogInteraction { get; set; }

        public Interaction<AddNewChequeInfoViewModel, bool> ShowAddChequeInfoDialogInteraction { get; set; }

        MainWindowViewModel mainWindowViewModel { get; set; }
        public MakeSaleViewModel(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;

            ProductsListScanned = new ObservableCollection<ProductsScannedInfo>();

            LoadPaymentMethods_InArabic_And_SetDefault_PaymentMode();

            SubmitBarCodeManually = ReactiveCommand.Create(AddProductScannedToScreenOperation, CreateCheckIfUsercanAddBarCodeManually());

            // as you can see i've tested all thes function manually and they are done

            WhenUserScanNewProduct_BarCodeSearchBarIsChanged();

            WhenUserEnterBarCodeManually_EraseAutomaicBarcode_SerchBar();

            WhenUserSetInvalidProduct_Price_Or_Quantity_Block_TheSystem_From_Adding_NewProducts_AndShowError_Plus_MakeAdditional_Checkings();

            whenUserEnterMoneyHandedByClient_CalculateTheExchange();

            whenTheExchangeIsLessThan_0_MakeItInRedColor();

            CountTheNumberOfProductsInTheListScanned_InEvery_500ms();

            WhenUserAddOrRemoveScannedProduct_ExecuteTheseCheckings();

            WhenUserUserChooseThe_CheckPyament_Or_DebtMode_CheckIfHePickedTheClient_NotUnkownClient();

            WhenUserStartLookingFroProductManually_GetProductsList_ThatStart_With_SearchTerm();

            whenUserClickToProductSearchedManually_GetItProductID_And_PutIntoBarCodeSearchBar();
         
            SaveSellingOperationCommand =  
                ReactiveCommand.Create(SaveSellingOperationToDatabse, CheckIfSystemIsNotRaisingError_And_ExchangeIsPositiveNumber_And_ProductListIsNotEmpty_Every_500ms());

            MakeNewSellingOperationCommand = ReactiveCommand.Create(MakeNewSellingOperation, CheckIfUserHasScannedAtLeastOneProduct());

            ShowAddSaleDialogInteraction = new Interaction<string, bool>();

            ShowDeleteSaleDialogInteraction = new Interaction<string, bool>();

            ShowAddChequeInfoDialogInteraction = new Interaction<AddNewChequeInfoViewModel, bool>();

          

    }

    ~MakeSaleViewModel()
        {
            stopTheCounterOfNumberScannedProductList();
        }

        public List<string> TranslatePaymentMethodsListToArabic(List<string> paymentMethods, string originalLanguageCode)
        {
            var translatedPaymentMethods = new List<string>();

            foreach (var paymentMethod in paymentMethods)
            {
                try
                {
                    // Get the translated payment method in Arabic, specifying the original language code
                    string translatedPaymentMethod = WordTranslation.TranslatePaymentIntoTargetedLanguage(paymentMethod,"ar");
                    translatedPaymentMethods.Add(translatedPaymentMethod);
                }
                catch (Exception ex)
                {
                    // Handle the case where a translation is not found
                    // For simplicity, we'll add the original payment method in case of an error
                    translatedPaymentMethods.Add(paymentMethod); // or handle error differently
                    Console.WriteLine($"Error translating '{paymentMethod}': {ex.Message}");
                }
            }

            return translatedPaymentMethods;
        }


        private void LoadPaymentMethods_InArabic_And_SetDefault_PaymentMode()
        {
            PaymentsMethods = TranslatePaymentMethodsListToArabic(AccessToClassLibraryBackendProject.GetPaymentTypes(),"ar");

            SelectedPaymentMethod = PaymentsMethods .First(p => p.Equals("نقدا", StringComparison.OrdinalIgnoreCase));
        }

        private string TranslateSelectedPaymentMethod_To_English_OriginalDb()
        {
            return WordTranslation.TranslatePaymentIntoTargetedLanguage(SelectedPaymentMethod,"en");
        }


        private bool isThisNumberOutRange(int IntegerNumber)
        {
            return IntegerNumber <= int.MinValue || IntegerNumber >= int.MaxValue;
        }

        private bool isThisNumberOutRange(long IntegerNumber)
        {
            return IntegerNumber <= long.MinValue || IntegerNumber >= long.MaxValue;
        }

        private bool isThisNumberOutRange(float FloatNumber)
        {
            return FloatNumber <= float.MinValue || FloatNumber >= float.MaxValue;
        }

        private bool isThisNumberOutRange(decimal DecimalNumber)
        {
            return DecimalNumber <= decimal.MinValue || DecimalNumber >= decimal.MaxValue;
        }
        // this function is tested using reflection
        protected DataTable LoadProductBoughtFromScannedListIntoADataTable()
        {
            DataTable ProductsBoughtTable = new DataTable();
            ProductsBoughtTable.Columns.Add("ProductId", typeof(long));
            ProductsBoughtTable.Columns.Add("ProductName", typeof(string));
            ProductsBoughtTable.Columns.Add("QuantitySold", typeof(int));
            ProductsBoughtTable.Columns.Add("QuantitySold2", typeof(int));
            ProductsBoughtTable.Columns.Add("QuantitySold3", typeof(int));
            ProductsBoughtTable.Columns.Add("UnitPrice", typeof(decimal));
            ProductsBoughtTable.Columns.Add("UnitSoldPrice", typeof(decimal));
            ProductsBoughtTable.Columns.Add("IsReturned", typeof(bool));
            ProductsBoughtTable.Columns.Add("Profit", typeof(decimal));

            foreach (var product in ProductsListScanned)
            {
                // normally this variable should be as propertyof an object but i forgot to add and adding it this time will be time costly
                // it require to set an event or obsrvable that watch the price entred by user then update it
                // so i did this quck and simple hack
       if (!float.TryParse(product.PriceOfProductSold, out float priceOfProductSoldConvertedToFloat) || priceOfProductSoldConvertedToFloat < 0 || isThisNumberOutRange(priceOfProductSoldConvertedToFloat)) break;
       if (product.ProductInfo.cost <= 0 || isThisNumberOutRange(product.ProductInfo.cost)) break;
       if (product.ProductInfo.id <= 0 || isThisNumberOutRange(product.ProductInfo.id)) break;
      
                if (
                    
                    !int.TryParse(product.ProductsUnitsToReduce_From_Stock1, out int ProductsUnitsToReduce_From_Stock1) ||
                    !int.TryParse(product.ProductsUnitsToReduce_From_Stock2, out int ProductsUnitsToReduce_From_Stock2) ||
                    !int.TryParse(product.ProductsUnitsToReduce_From_Stock3, out int ProductsUnitsToReduce_From_Stock3) ||
                    ProductsUnitsToReduce_From_Stock1 < 0 || 
                    ProductsUnitsToReduce_From_Stock2 < 0 || 
                    ProductsUnitsToReduce_From_Stock3 < 0 ||
                    isThisNumberOutRange(ProductsUnitsToReduce_From_Stock1)||
                    isThisNumberOutRange(ProductsUnitsToReduce_From_Stock2)||
                    isThisNumberOutRange(ProductsUnitsToReduce_From_Stock3) 
                    ) break;

                var profitFromSoldProduct = priceOfProductSoldConvertedToFloat - product.ProductInfo.cost;

                ProductsBoughtTable.Rows.Add
                (product.ProductInfo.id, product.ProductInfo.name,ProductsUnitsToReduce_From_Stock1, ProductsUnitsToReduce_From_Stock2, 
                ProductsUnitsToReduce_From_Stock3, product.ProductInfo.price,product.PriceOfProductSold, false, profitFromSoldProduct);
            }

            return ProductsBoughtTable;
        }

       private bool SellerIsLosingMoneyInOneProductOrMore()
        {
            float profitFromSoldProduct =0;

            foreach (var product in ProductsListScanned)
            {
                profitFromSoldProduct = float.Parse(product.PriceOfProductSold) - product.ProductInfo.cost;
               
                if(profitFromSoldProduct<0) return true;
            }

            return false;
        }
        
        public List<string> GetClientsListFromDb()
        {
            string UnkonwClientString = "Normal";
            List<string> ClientList = AccessToClassLibraryBackendProject.GetClientNames_And_Their_Phones_As_String();
            // we add default unkown client to the list of client so a user can choose it if he deals with unregistred client
            ClientList.Add(UnkonwClientString);
           
            // load the clientlist gloabl viarble so we can use it somewhere else
            _clientList = ClientList;

            return ClientList;

        } 

        public void GetProductsNamesFrom_Loaded_ProductsNameAndTheirIDs_Dictionary()
        {

            if (string.IsNullOrEmpty(ProductNameTermToSerach)) { ProductSuggestionsAfterManualSearch = new List<string>(); return; }

            // ths is a dictionary string , int productname and it id list 
            _productNamesAndTheirIDs = AccessToClassLibraryBackendProject.GetProductsByStartingLetter(ProductNameTermToSerach);
            var productNamesListExtractedFromDictionary = _productNamesAndTheirIDs.Keys.ToList();
            ProductSuggestionsAfterManualSearch = productNamesListExtractedFromDictionary;
           
        }

     
        // this function is already test and passed 
        private bool CheckIfProductBoughtInformationsAreValid(DataTable ProductsBoughtInThisOperation)
        {
           try { 
                foreach (DataRow row in ProductsBoughtInThisOperation.Rows)
                {
                // Validate ProductId
                if (row.IsNull("ProductId") || !(row["ProductId"] is long productId) || productId <= 0 || isThisNumberOutRange(productId))
                {
                    return false; // Invalid ProductId
                }

                // Validate QuantitySold
                if (row.IsNull("QuantitySold") || !(row["QuantitySold"] is int quantitySold) || quantitySold <= 0 || isThisNumberOutRange(quantitySold))
                {
                    return false; // Invalid QuantitySold
                }

                // Validate UnitPrice
                if (row.IsNull("UnitPrice") || !(row["UnitPrice"] is decimal unitPrice) || unitPrice <= 0 || isThisNumberOutRange(unitPrice))
                {
                    return false; // Invalid UnitPrice
                }

                // Validate Profit
                if (row.IsNull("Profit") || !(row["Profit"] is decimal profit) || isThisNumberOutRange(profit))
                {
                    return false; // Invalid Profit
                }

                if (row.IsNull("IsReturned"))
                    {
                        return false; // Invalid IsReturned (should be true or false)
                    }
                }

                // All rows are valid
                return true;
            }

            catch (Exception ex)
            {
                throw;
                return false;
            }
        }
       
        private bool CheckIfTimeIsValid(DateTime date)
            {
                // Example validation rules (adjust as per your requirements)
                return date.Year >= 2000 && date <= DateTime.Now;
            }
       
        private bool CheckIfTotalPriceOfSellingOperationIsValid(float FloatNumber) {

            return FloatNumber > 0 && !isThisNumberOutRange(FloatNumber) ;
        }

        private bool Are_All_TheseSalesInfoValid(DataTable ProductsBoughtsInThisOperation, DateTime sellingTimeIsValid, float TotalPriceOfSalesIsValid)
        {
            return CheckIfProductBoughtInformationsAreValid(ProductsBoughtsInThisOperation) 
                && CheckIfTimeIsValid(sellingTimeIsValid) 
                && CheckIfTotalPriceOfSellingOperationIsValid(TotalPriceOfSalesIsValid);
        }

        private (int, string) GetLastSaleClientID_And_Name()
        {
            int clientID = 0;
            string clientName = "";

            // Call the function to load the values by reference
            AccessToClassLibraryBackendProject.GetLastSaleClientId_And_Name(ref clientID, ref clientName);

            // we wont display unkwnclient we will convert it to empty string
            if (clientName == "unknownClient") clientName = string.Empty;
            // Return the tuple (int, string)
            return (clientID, clientName);
        }


        

        private bool UserHasSelected_Check_PaymentMode()
        {
            string slectedPaymentMethodInEnglish = TranslateSelectedPaymentMethod_To_English_OriginalDb();
            return string.Equals(slectedPaymentMethodInEnglish,"check", StringComparison.OrdinalIgnoreCase);
        }

        private bool UserHasSelected_Debt_PaymentMode()
        {
            string slectedPaymentMethodInEnglish = TranslateSelectedPaymentMethod_To_English_OriginalDb();
            return string.Equals(slectedPaymentMethodInEnglish, "credit", StringComparison.OrdinalIgnoreCase);
        }

        private async Task<(bool UserHasFilledCorrectlyTheInfo_And_DidntLeaveThePage, ChequeInfo ChequeInfo)> OpenChequePage_And_ReturnInfo_FilledByUser_Plus_Check_IfUserHasntLeaveThePage()
        {
            var chequeInfoToFillByUser = new ChequeInfo();
            var userControlToShowInsideDialog = new AddNewChequeInfoViewModel(ref chequeInfoToFillByUser);

            // Awaiting the dialog interaction and getting the result from the user
            bool UserHasFilledCorrectlyTheInfo_And_DidntLeaveThePage = await ShowAddChequeInfoDialogInteraction.Handle(userControlToShowInsideDialog);

            // Returning the tuple with the user interaction result and the filled cheque info
            return (UserHasFilledCorrectlyTheInfo_And_DidntLeaveThePage, chequeInfoToFillByUser);
        }

        

        private long GetClientID_FromPhoneNumber()
        {
            return long.Parse(PhoneNumberExtractor.ExtractPhoneNumber(SelectedClientName_PhoneNumber));
        }
        private async void SaveSellingOperationToDatabse()
        {
            try {
                // we reused an already created shodelteproductDialog in the base class productlistViewModel so we prevent loosing time and duplicate cod
                 bool UserHasClickedNoToAddSaleOperationBtn = !await 
                         (ShowAddSaleDialogInteraction.Handle($"هل تريد حقا تسجيل هذه المبيعة"));
               
                bool UserHasntAcceptedToLoseMoneyInOneProductOrMore = false; 

            if (UserHasClickedNoToAddSaleOperationBtn) return;

            if (SellerIsLosingMoneyInOneProductOrMore()) UserHasntAcceptedToLoseMoneyInOneProductOrMore = !await ShowAddSaleDialogInteraction.Handle("هل تريد ان تخسر في منتوج او اكتر");

                if (UserHasntAcceptedToLoseMoneyInOneProductOrMore) return;

                var timeOfSellingOpperationIsNow = DateTime.Now;
                float TotalPriceOfSellingOperation = float.Parse(Total);
                DataTable ProductsBoughtInThisOperation = LoadProductBoughtFromScannedListIntoADataTable();

            bool SomeSalesInfoAreWrong = !Are_All_TheseSalesInfoValid(ProductsBoughtInThisOperation, timeOfSellingOpperationIsNow, TotalPriceOfSellingOperation);

                if (SomeSalesInfoAreWrong) { await ShowAddSaleDialogInteraction.Handle("هناك معلومات خاطئة حول المنتجات"); return; }

                string slectedPaymentMethodInEnglish = TranslateSelectedPaymentMethod_To_English_OriginalDb();

                if (UserHasSelected_Check_PaymentMode())
                {
                    // Await the method to get the tuple result
                    var (UserHasFilledCorrectlyTheInfo_And_DidntLeaveThePage, chequeInfoUserHasFilled) = await OpenChequePage_And_ReturnInfo_FilledByUser_Plus_Check_IfUserHasntLeaveThePage();
                    bool userClosedThePage = !UserHasFilledCorrectlyTheInfo_And_DidntLeaveThePage;
                    _loadedCheckInfoByUser_When_Choosing_CheckPaymentType = chequeInfoUserHasFilled;
                    
                    if (userClosedThePage) return;
                    
                }

                SubmitOperationSalesDataToDatabase(timeOfSellingOpperationIsNow,TotalPriceOfSellingOperation, ProductsBoughtInThisOperation, 
                                                       slectedPaymentMethodInEnglish,_loadedCheckInfoByUser_When_Choosing_CheckPaymentType);



            }

            catch(Exception ex) { await ShowAddSaleDialogInteraction.Handle(" لقد حصل خطأ ماتاكد من ان المنتجات اللتي تريد ان تضيف موجودة في المخزن "); }
        }

        private void CreateBonLivraison_For_Client(int saleID, int ClientID,DataTable ProductsBoughtInThisOperation,string slectedPaymentMethodInEnglish,ChequeInfo userChequeInfo)
        {
            string selectedPaymentMethodInFrench = WordTranslation.TranslatePaymentIntoTargetedLanguage(slectedPaymentMethodInEnglish, "fr");
            decimal TVA = 20;
            AccessToClassLibraryBackendProject.GenerateBonLivraison_ForClient(ProductsBoughtInThisOperation, ClientID, selectedPaymentMethodInFrench, TVA, saleID);
        }

        private void CreateInvoice_For_Client(int saleID, int InvoiceID,int ClientID, DataTable ProductsBoughtInThisOperation, string slectedPaymentMethodInEnglish, ChequeInfo userChequeInfo)
        {
            string selectedPaymentMethodInFrench = WordTranslation.TranslatePaymentIntoTargetedLanguage(slectedPaymentMethodInEnglish, "fr");
            decimal TVA = 20;
            AccessToClassLibraryBackendProject.GenerateInvoice_ForClient(ProductsBoughtInThisOperation, ClientID, selectedPaymentMethodInFrench, TVA, saleID, InvoiceID);
        }


        public virtual async void SubmitOperationSalesDataToDatabase
        (DateTime timeOfSellingOpperationIsNow, float TotalPriceOfSellingOperation, DataTable ProductsBoughtInThisOperation, string slectedPaymentMethodInEnglish,ChequeInfo userChequeInfo)
        {
            var result = AccessToClassLibraryBackendProject.AddNewSaleToDatabase(
                                                                                 timeOfSellingOpperationIsNow,
                                                                                 TotalPriceOfSellingOperation,
                                                                                 ProductsBoughtInThisOperation,
                                                                                 SelectedClientName_PhoneNumber,
                                                                                 slectedPaymentMethodInEnglish,
                                                                                 userChequeInfo);

            if (result.Success)
            {
                await ShowAddSaleDialogInteraction.Handle("لقد تمت العملية بنجاح");

                int lastSaleID = result.SalesId;
                int ClientID = GetLastSaleClientID_And_Name().Item1;
                // Use result.SalesId if needed for further processing
                if (await ShowDeleteSaleDialogInteraction.Handle("هل تريد طباعة وصل الاستلام "))
                {
                        
                    CreateBonLivraison_For_Client(lastSaleID,ClientID,ProductsBoughtInThisOperation,SelectedPaymentMethod,userChequeInfo);
                }

                if (await ShowDeleteSaleDialogInteraction.Handle(" هل تريد طباعة الفاتورة ايضا "))
                {
                   int invoiceNumber = AccessToClassLibraryBackendProject.AddInvoiceIfNotExists(lastSaleID);
                    CreateInvoice_For_Client(lastSaleID, invoiceNumber, ClientID, ProductsBoughtInThisOperation, SelectedPaymentMethod, userChequeInfo);
                }

                ResetAllSellingInfoOperation();
                mainWindowViewModel.CheckIfSystemShouldRaiseBellNotificationIcon();
            }


            else { await ShowAddSaleDialogInteraction.Handle(" لقد حصل خطأ ما تاكد من ان المنتجات اللتي تريد ان تضيف موجودة في المخزن  "); }
        }
        private async void MakeNewSellingOperation()
        {  
            bool UserHasClickedYesToDeleteSaleOperationBtn = await ShowDeleteSaleDialogInteraction.Handle("هل تريد حقا حدف هذه المبيعة");

            if (UserHasClickedYesToDeleteSaleOperationBtn) ResetAllSellingInfoOperation();
        }

        protected void ResetAllSellingInfoOperation()
        {
            deleteAllScannedItems();
            AmountPaid = string.Empty;
            ManualBarcodeValue = string.Empty;
            Total = Exchange="0";
            deleteDisplayedError();

        }

        void deleteAllScannedItems()
        {
            if (ProductsListScanned.Count == 0) return;
            ProductsListScanned.Clear();
        }
        // we make a sum of each productunits of each item to get the total number of duplicated and non ones

        void CountTheNumberOfProductsInTheListScanned()
        {
            int TotalNumberOfItems = 0;

            foreach (var product in ProductsListScanned)
            {

                int units;

                // when the user enter empty space or any weird caracter 
                if (!int.TryParse(product.ProductsUnits, out units) || units <= 0)
                {
                    // If parsing fails or units are zero or negative, set units to 1
                    units = 1;
                }
                TotalNumberOfItems += units;
            }

            TotalNumberOfProducts = TotalNumberOfItems.ToString();

        }

        // i used this function to calculate the number of productscanned list every 100 ms 
        // because it is difficult to watch the objects inside the list and i tighted with a time
       IDisposable CountTheNumberOfProductsInTheListScanned_InEvery_500ms()
        {
             var ProductListObservation = Observable.Interval(TimeSpan.FromMilliseconds(500));

            // Subscribe to the observable
            ProductListObservationSubsription = ProductListObservation.Subscribe( _ => CountTheNumberOfProductsInTheListScanned());


            return ProductListObservationSubsription;
        }


        void stopTheCounterOfNumberScannedProductList()
        {
            ProductListObservationSubsription.Dispose();
        }

        // this function is tested using reflection and it done
        private IObservable<bool> CreateCheckIfUsercanAddBarCodeManually()
        {
            return this.WhenAnyValue(x => x.ManualBarcodeValue).Select(_ =>  CheckIfManualBarCodeScannedIsValidNumber());
            
        }


        // i'm not sure if this observable i get garbage collected or not 
        // this function might cause a memory leak but i let it now for later
        // i don't know if it is garabage collected automatically or not
        
     protected virtual IObservable<bool> CheckIfSystemIsNotRaisingError_And_ExchangeIsPositiveNumber_And_ProductListIsNotEmpty_Every_500ms()
      {
            // Initial condition to keep the observable running indefinitely

            var observable = Observable.Interval(TimeSpan.FromMilliseconds(500))
                                       .Select(_ =>
                                           TheSystemIsNotShowingError() &&
                                           CheckIfExchangeIsBiggerOrEqual_0() &&
                                           ProductsListScanned.Count > 0
                                       )
                                       .DistinctUntilChanged()
                                      // .Do(_ => Debug.WriteLine("didi")) // Invoke Debug.WriteLine for each emission                                
                                       .ObserveOn(RxApp.MainThreadScheduler);
            // Ensure this observable runs on the UI thread

          return observable;
       }


        protected IObservable<bool> CheckIfUserHasScannedAtLeastOneProduct()
        {
            return this.WhenAnyValue(x => x.ProductsListScanned.Count)
                       .Select(count => count > 0);
        }


        // the barcode scanner must be valid only if
        // it is not an empty string
        //  a positive number
        // a number that exist in the database which is productid
        private bool CheckIfBarCodeScannedIsValidNumber()
        {

           return UiAttributeChecker.AreThesesAttributesPropertiesValid( this,nameof(Barcode)) && !string.IsNullOrEmpty(Barcode) && !string.IsNullOrWhiteSpace(Barcode);
        }

        // this function check if the uer has missed with a quanity of product by entring weird carachter not a number or empty string
        // also
        protected virtual bool CheckIf_ProductsUnits_And_SoldPrices_Of_ScannedProducts_Are_Valid()
        {
            foreach (var ProductScanned in ProductsListScanned) {

                if (string.IsNullOrEmpty(ProductScanned.ProductsUnits) || string.IsNullOrWhiteSpace(ProductScanned.ProductsUnits) ) return false;
                if (string.IsNullOrEmpty(ProductScanned.PriceOfProductSold) || string.IsNullOrWhiteSpace(ProductScanned.PriceOfProductSold) ) return false;
                if (ProductScanned.ProductStockHasErrors) return false;

                if (!UiAttributeChecker.AreThesesAttributesPropertiesValid
                    (ProductScanned, nameof(ProductScanned.ProductsUnits), nameof(ProductScanned.PriceOfProductSold)))
                { 
                        
                        return false;  
                }

            }

            return true;
        }

       
        private bool CheckIfManualBarCodeScannedIsValidNumber()
        {

            return UiAttributeChecker.AreThesesAttributesPropertiesValid(this, nameof(ManualBarcodeValue)) && !string.IsNullOrEmpty(ManualBarcodeValue) && !string.IsNullOrWhiteSpace(ManualBarcodeValue);
        }

        private bool CheckIfUserIsExistingInDatabaseById(string barcodeScanned)
        {
            return AccessToClassLibraryBackendProject.IsThisProductIdAlreadyExist(long.Parse(barcodeScanned));
        }
      
        private ProductInfo RetrieveProductFromDatabaseByBarCodeId(string BarcodeNumberScanned)
        {
            return AccessToClassLibraryBackendProject.GetProductInfoByBarCode(long.Parse(BarcodeNumberScanned));
        }

        private ProductsScannedInfo Add_UnitsOfSoldProduct_And_SoldProductPrice(string BarcodeNumberScanned)
        {
            ProductInfo ProductFound = RetrieveProductFromDatabaseByBarCodeId(BarcodeNumberScanned);

            // productscannedinfo is a lcass that contans a product retrived from daabase in addtion to the price and units info a user or buyter will submit
            ProductsScannedInfo ProductFound_Plus_PriceAndUnitsOfSoldProduct = new ProductsScannedInfo(ProductFound);

            return ProductFound_Plus_PriceAndUnitsOfSoldProduct;
        }
        private void AddProductScannedToProductListScanned(string barcodeRecieved )
        {
            ProductsScannedInfo ProductFound = Add_UnitsOfSoldProduct_And_SoldProductPrice(barcodeRecieved);

            if (ProductFound == null) return;
           
            ProductsListScanned.Add(ProductFound);
        }
       
        private bool TheProductIsAlreadyAddedToAlist(string BarcodeNumberScanned)
        {
            long productId = long.Parse(BarcodeNumberScanned);

            return ProductsListScanned.Any(product => product.ProductInfo.id == productId);
        }

        // we detect which barcode textbox a user is filling manual or auto
        // by the way a user can't fill them both 
        // if he start filling one the other get erased
        private string WhichBarCodeUserIsFillingManualOrAuto()
        {
            if (!string.IsNullOrEmpty(Barcode)) return Barcode;
            else return ManualBarcodeValue;
        }

        private string IncreaseByOneTheStringIntNumber(string stringIntNumber)
        { 
            return (int.Parse(stringIntNumber)+1).ToString();
        }
     
        private ProductsScannedInfo GetTheProductFromScannedListById(long id)
        {
            return ProductsListScanned.FirstOrDefault(product => product.ProductInfo.id == id);
        }
       
        private void Increase_TheNumberByOne_Of_ProductScannedTheSecondTimeInArow(string duplicatedBarCodeEntred)
        {

            long DuplicatedProductId = long.Parse(duplicatedBarCodeEntred);

            ProductsScannedInfo ProductDuplicatedFound = GetTheProductFromScannedListById(DuplicatedProductId);
            //
            // we increment by 1 a string value the increase funciton is converting the value to int to increament it then going back to string to be bound to the ui
            ProductDuplicatedFound.ProductsUnits = IncreaseByOneTheStringIntNumber(ProductDuplicatedFound.ProductsUnits);
            //
        }

        private bool IsTheScreenShowingError()
        {
            return isErrorLabelVisible;
        }
        private void AddProductScannedToScreenOperation()
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

            else  AddProductScannedToProductListScanned(BarCodeUserHasEntred);

        }


        private void WhenUserScanNewProduct_BarCodeSearchBarIsChanged()
        {
            this.WhenAnyValue(x => x.Barcode)
                .Throttle(TimeSpan.FromMilliseconds(500)) // this gives the half second between each reading operation to let time for barcodescanner to read longest barcodes 
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(async BarcodeNumberScanned =>
                {
                    if (CheckIfBarCodeScannedIsValidNumber())
                    {
                        await Dispatcher.UIThread.InvokeAsync(() => AddProductScannedToScreenOperation());
                    }
                    else
                    {
                      //  await Task.Delay(1500);
                    }

                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        EraseAutomaticBarCodeSearchBar();
                        EraseManualBarCodeSearchBar();
                    });
                });
        }
        private void WhenUserEnterBarCodeManually_EraseAutomaicBarcode_SerchBar()
        {
            this.WhenAnyValue(x => x.ManualBarcodeValue)
              .Subscribe( async BarcodeNumberScanned =>
             {
                 EraseAutomaticBarCodeSearchBar();
                 
             }
              );
        }

    
       private void WhenUserStartLookingFroProductManually_GetProductsList_ThatStart_With_SearchTerm()
       {
           this.WhenAnyValue(x => x.ProductNameTermToSerach)
          .Throttle(TimeSpan.FromMilliseconds(200))
          .ObserveOn(RxApp.MainThreadScheduler)// Optional: Add a debounce time to limit rapid requests
          .Subscribe(async searchTerm =>{ GetProductsNamesFrom_Loaded_ProductsNameAndTheirIDs_Dictionary();});
       }

        private void whenUserClickToProductSearchedManually_GetItProductID_And_PutIntoBarCodeSearchBar()
        {
            this.WhenAnyValue(x => x.SelectedProductNameTermSerach)
         .ObserveOn(RxApp.MainThreadScheduler)// Optional: Add a debounce time to limit rapid requests
         .Subscribe(async searchTerm => {

             var getTheProductIdOfSelectedProductNameIfItFound = GetProductID_Of_SelectedProductName_If_Exist();

             if(getTheProductIdOfSelectedProductNameIfItFound.isFound)
             {
                 long proudctIdFound = getTheProductIdOfSelectedProductNameIfItFound.productID;

                 Barcode = proudctIdFound.ToString();
             }


         });
        }

      


        private (bool isFound, long productID) GetProductID_Of_SelectedProductName_If_Exist()
        {
            // Try to get the productID from the dictionary
            if (_productNamesAndTheirIDs == null) return (false, -1);

            // we used ProductNameTermtoserch insted selectedProductName becuase once we click or selecteditem the selecteditem is set to null this issue from my custom autocomplete
            // this is not perfect but this was the solution or quick solution
            if (_productNamesAndTheirIDs.TryGetValue(ProductNameTermToSerach, out long productID))
            {
                return (true, productID); // Return true and the productID if found
            }

            return (false, -1); // Return false and a default value (-1) if not found
        }


        protected void deleteDisplayedError()
        {
            isErrorLabelVisible = false;
        }
        protected void displayErrorMessage(string ErrorMessageToShowUser)
        {
            ErrorMessage = ErrorMessageToShowUser;
            isErrorLabelVisible = true;
        }

        private bool CheckIfExchangeIsBiggerOrEqual_0(){ 
        
            return float.Parse(Exchange)>=0;
        }
            
       private void WhenUserAddOrRemoveScannedProduct_ExecuteTheseCheckings()
       {
           this.WhenAnyValue(x => x.ProductsListScanned.Count).
               
                Subscribe( _ => { if (ProductsListScanned.Count == 0) ResetAllSellingInfoOperation(); });
       }


        protected virtual bool User_HasPicked_Known_Client()
        {
            // Check if SelectedClientName_PhoneNumber is not "Normal" and exists in _clientList
            bool isKnownClient = !string.Equals(SelectedClientName_PhoneNumber, "Normal", StringComparison.OrdinalIgnoreCase);
            bool existsInClientList = _clientList.Contains(SelectedClientName_PhoneNumber, StringComparer.OrdinalIgnoreCase);

            return isKnownClient && existsInClientList;
        }



        protected virtual void WhenUserUserChooseThe_CheckPyament_Or_DebtMode_CheckIfHePickedTheClient_NotUnkownClient()
      {
          this.WhenAnyValue(x => x.SelectedPaymentMethod, x=>x.SelectedClientName_PhoneNumber).

               Subscribe(_ => {
                   
                      deleteDisplayedError();
                   if (UserHasSelected_Check_PaymentMode()|| UserHasSelected_Debt_PaymentMode()) 
                   {
                       if (User_HasPicked_Known_Client());
                       else displayErrorMessage("يجب ان تختار زبون لديه رقم هاتف");
                   }

                        
                               
               });
      }

      
        protected virtual void WhenUserSetInvalidProduct_Price_Or_Quantity_Block_TheSystem_From_Adding_NewProducts_AndShowError_Plus_MakeAdditional_Checkings()
          {
        
            this.WhenAnyValue(x => x.ProductsListScanned.Count)
              .Subscribe(async BarcodeNumberScanned =>
              {
        
                    foreach (var product in ProductsListScanned)
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
                                if (CheckIf_ProductsUnits_And_SoldPrices_Of_ScannedProducts_Are_Valid()) deleteDisplayedError();
                                else displayErrorMessage("هناك خطأ في كمية المنتج او السعر");

                                CheckIfProductIsProfitableOrNot();
                                CalculateTheTotalPriceOfOperation();
                                CalculateTheRemainingPriceOfOperation();
                            }
                            );
                    }
                }
                );
          }

        private bool CheckIfTheAmountPaidUserEntredIsValid()
        {
            return UiAttributeChecker.AreThesesAttributesPropertiesValid(this, nameof(AmountPaid));
        }
        private void whenUserEnterMoneyHandedByClient_CalculateTheExchange()
        {
            this.WhenAnyValue(x=>x.AmountPaid).Subscribe(_=> CalculateTheRemainingPriceOfOperation());
        }

        private void ChangeTheColorOfExchangeLable()
        {
            // when the exchange is less than 0 it means we need more money 
            if (float.Parse(Exchange) < 0) BlackOrRedColor = "red";
            else BlackOrRedColor = "Black";
        }
        private void whenTheExchangeIsLessThan_0_MakeItInRedColor()
        {
            this.WhenAnyValue(x => x.Exchange).Subscribe(_ => ChangeTheColorOfExchangeLable()); 
        }


        // these Erase function are inside whenanyvalue whic run on different thread 
        // so avalonia forces any property bound to ui like barcode or manualbarcodevalue to be called from ui thread
        // for this reason we used async task and set the uithred tonot be called from the ui that whenanyvalue runs from

        private async Task EraseAutomaticBarCodeSearchBar()
        {
            
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => Barcode = "");
        }

        private async Task EraseManualBarCodeSearchBar()
        {
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => ManualBarcodeValue = "");
        }

        protected bool TheSystemIsShowingError()
        {
            return isErrorLabelVisible;
        }

        protected bool TheSystemIsNotShowingError()
        {
            return !TheSystemIsShowingError();
        }

        protected void CalculateTheTotalPriceOfOperation()
        {
            decimal TotoalPriceOfProductsCalculated = 0;

            if (TheSystemIsShowingError()) return;
           
            foreach(var product in ProductsListScanned)
            {
              
                TotoalPriceOfProductsCalculated += decimal.Parse(product.PriceOfProductSold)*int.Parse(product.ProductsUnits);
            }

            Total = TotoalPriceOfProductsCalculated.ToString();
        }

        
        private void CalculateTheRemainingPriceOfOperation()
        {
            decimal remaingMoney = 0;

            if (TheSystemIsShowingError()) return;

            // when a user enter something non valid like caracter or non parsable int we consider him enterd 0
            if (!CheckIfTheAmountPaidUserEntredIsValid()) remaingMoney= -decimal.Parse(Total);
            else  remaingMoney = decimal.Parse(AmountPaid) - decimal.Parse(Total);

            Exchange = remaingMoney.ToString();
        }

        private void CheckIfProductIsProfitableOrNot()
        {
            if (TheSystemIsShowingError()) return;

            foreach (var product in ProductsListScanned)
            {
                // if the price user entered is less than a cost it mean that a buyer should be aware teh color of products units becomes red
                // to notify him
                if (float.Parse(product.PriceOfProductSold) - product.ProductInfo.cost < 0) product.SoldProductPriceUnitColor = "#ff000d";

                // we go back to the non error color which is a black
                else product.SoldProductPriceUnitColor = "Black";
            }
        }

        // this section is for a test 

   //    async void AreTheAllProductScannedSuccessffully_100_Times()
   //    {
   //
   //      for (int j = 1; j <= 1; j++) { 
   //
   //          for (int i = 1; i <= 100; i++)
   //            {
   //                Barcode = i.ToString();
   //                await Task.Delay(500);
   //                Debug.WriteLine($" i is : {i} and totalNumber is {TotalNumberOfProducts}");
   //                
   //            }
   //            
   //            if (int.Parse(TotalNumberOfProducts) == 100) Debug.WriteLine("operation succeded");
   //            else { Debug.WriteLine("something went wrong"); return; }
   //          //  ProductsListScanned.Clear();
   //        }
   //
   //        Debug.WriteLine("Test Is Passed");
   //    }
   //
   //    async void AreTheAllManualProductScannedSuccessffully_100_Times()
   //    {
   //        for (int j = 1; j <= 5; j++)
   //        {
   //
   //            for (int i = 1; i <= 100; i++)
   //            {
   //                ManualBarcodeValue = i.ToString();
   //                AddProductScannedToScreenOperation();
   //                await Task.Delay(500);
   //                Debug.WriteLine($" i is : {i} and totalNumber is {TotalNumberOfProducts}");
   //
   //            }
   //
   //            if (int.Parse(TotalNumberOfProducts) == 100) Debug.WriteLine("operation succeded");
   //            else { Debug.WriteLine("something went wrong"); return; }
   //            ProductsListScanned.Clear();
   //        }
   //
   //        Debug.WriteLine("Test Is Passed");
   //    }
   //
   //    async void IsTotalPriceCorrectForAllTheItemsScannedAutomatically_500_Items()
   //    {  
   //            for (int i = 1; i <= 500; i++)
   //            {
   //                Barcode = i.ToString();
   //                await Task.Delay(500);
   //                Debug.WriteLine($" i is : {i} and totalNumber is {TotalNumberOfProducts}");
   //                
   //            }
   //            if ( decimal.Parse(Total) != 25495.7m) { Debug.WriteLine("total price is not exact"); return; }
   //            if (int.Parse(TotalNumberOfProducts) == 500) Debug.WriteLine("operation succeded");
   //            else { Debug.WriteLine("something went wrong"); return; }
   //            ProductsListScanned.Clear();
   //
   //        Debug.WriteLine("Test Is Passed");
   //    }
   //
   //    async void IsTotalPriceCorrectForAllTheItemsScannedManually_500_Items()
   //    {
   //        for (int i = 1; i <= 500; i++)
   //        {
   //            ManualBarcodeValue = i.ToString();
   //            AddProductScannedToScreenOperation();
   //            await Task.Delay(500);
   //            Debug.WriteLine($" i is : {i} and totalNumber is {TotalNumberOfProducts}");
   //
   //        }
   //        if (decimal.Parse(Total) != 637856.41m) { Debug.WriteLine("total price is not exact"); return; }
   //        if (int.Parse(TotalNumberOfProducts) == 100) Debug.WriteLine("operation succeded");
   //        else { Debug.WriteLine("something went wrong"); return; }
   //        ProductsListScanned.Clear();
   //
   //        Debug.WriteLine("Test Is Passed");
   //    }
   //
   //
   //     void insertEachDayOf_2024_ProductId_1_And_See_If_EachDay_Of_2024_Has_This_ProductId_Inserted()
   //    {
   //        float TotalPriceOfSellingOperation = 1000;
   //        int productId = 1;
   //        int quantitySold = 1;
   //        decimal unitPrice = 10; // You can set this to any appropriate value
   //        bool isReturned = false; // Assuming product is not returned
   //        decimal profit = 5; // You can set this to any appropriate value
   //
   //        DataTable ProductsBoughtTable = new DataTable();
   //        ProductsBoughtTable.Columns.Add("ProductId", typeof(int));
   //        ProductsBoughtTable.Columns.Add("QuantitySold", typeof(int));
   //        ProductsBoughtTable.Columns.Add("UnitPrice", typeof(decimal));
   //        ProductsBoughtTable.Columns.Add("IsReturned", typeof(bool));
   //        ProductsBoughtTable.Columns.Add("Profit", typeof(decimal));
   //
   //        // Loop through each day of the year 2024
   //        for (int month = 1; month <= 12; month++)
   //        {
   //            for (int day = 1; day <= DateTime.DaysInMonth(2024, month); day++)
   //            {
   //                DateTime timeOfSellingOpperation = new DateTime(2024, month, day);
   //
   //                if (day == 6)
   //                {
   //                    Console.WriteLine("");
   //                }
   //                // Clear previous rows
   //                ProductsBoughtTable.Rows.Clear();
   //
   //                // Add a new row for the product
   //                ProductsBoughtTable.Rows.Add(productId, quantitySold, unitPrice, isReturned, profit);
   //
   //                // Call the method to add new sale to the database
   //                bool productIsInsertedAtDatabase = AccessToClassLibraryBackendProject.AddNewSaleToDatabase(timeOfSellingOpperation, TotalPriceOfSellingOperation, ProductsBoughtTable);
   //                if (!productIsInsertedAtDatabase) 
   //                {
   //                    Debug.WriteLine($"something wrong happened at day :{day} month : {month} ");
   //                    return;
   //                }
   //
   //                // Optionally add a delay to avoid overwhelming the database
   //                // await Task.Delay(100); // Uncomment if needed
   //                Debug.WriteLine($"final successful day :{day} month : {month} ");
   //            }
   //        }
   //    }
   //
   //
   //     void insertEachDayOf_2023_ProductId_1_And_See_If_EachDay_Of_2023_Has_This_ProductId_Inserted()
   //    {
   //        float TotalPriceOfSellingOperation = 1000;
   //        int productId = 1;
   //        int quantitySold = 1;
   //        decimal unitPrice = 10; // You can set this to any appropriate value
   //        bool isReturned = false; // Assuming product is not returned
   //        decimal profit = 5; // You can set this to any appropriate value
   //
   //        DataTable ProductsBoughtTable = new DataTable();
   //        ProductsBoughtTable.Columns.Add("ProductId", typeof(int));
   //        ProductsBoughtTable.Columns.Add("QuantitySold", typeof(int));
   //        ProductsBoughtTable.Columns.Add("UnitPrice", typeof(decimal));
   //        ProductsBoughtTable.Columns.Add("IsReturned", typeof(bool));
   //        ProductsBoughtTable.Columns.Add("Profit", typeof(decimal));
   //
   //        // Loop through each day of the year 2024
   //        for (int month = 1; month <= 12; month++)
   //        {
   //            for (int day = 1; day <= DateTime.DaysInMonth(2023, month); day++)
   //            {
   //                DateTime timeOfSellingOpperation = new DateTime(2023, month, day);
   //
   //                // Clear previous rows
   //                ProductsBoughtTable.Rows.Clear();
   //
   //                // Add a new row for the product
   //                ProductsBoughtTable.Rows.Add(productId, quantitySold, unitPrice, isReturned, profit);
   //
   //                // Call the method to add new sale to the database
   //                bool productIsInsertedAtDatabase = AccessToClassLibraryBackendProject.AddNewSaleToDatabase(timeOfSellingOpperation, TotalPriceOfSellingOperation, ProductsBoughtTable);
   //                if (!productIsInsertedAtDatabase)
   //                {
   //                    Debug.WriteLine($"something wrong happened at day :{day} month : {month} ");
   //                    return;
   //                }
   //
   //                // Optionally add a delay to avoid overwhelming the database
   //                // await Task.Delay(100); // Uncomment if needed
   //
   //                Debug.WriteLine($"final successful day :{day} month : {month} ");
   //            }
   //
   //        }
   //    }
   //
   //
   //    void insertEachDayOf_2024_ProductId_1_And_2_And_See_If_EachDay_Of_2024_Has_This_ProductId_Inserted()
   //    {
   //        float TotalPriceOfSellingOperation = 1000;
   //        int productId = 1;
   //        int quantitySold = 1;
   //        decimal unitPrice = 10; // You can set this to any appropriate value
   //        bool isReturned = false; // Assuming product is not returned
   //        decimal profit = 5; // You can set this to any appropriate value
   //
   //        DataTable ProductsBoughtTable = new DataTable();
   //        ProductsBoughtTable.Columns.Add("ProductId", typeof(int));
   //        ProductsBoughtTable.Columns.Add("QuantitySold", typeof(int));
   //        ProductsBoughtTable.Columns.Add("UnitPrice", typeof(decimal));
   //        ProductsBoughtTable.Columns.Add("IsReturned", typeof(bool));
   //        ProductsBoughtTable.Columns.Add("Profit", typeof(decimal));
   //
   //        // Loop through each day of the year 2024
   //        for (int month = 1; month <= 12; month++)
   //        {
   //            for (int day = 1; day <= DateTime.DaysInMonth(2024, month); day++)
   //            {
   //                DateTime timeOfSellingOpperation = new DateTime(2024, month, day);
   //
   //                // Clear previous rows
   //                ProductsBoughtTable.Rows.Clear();
   //
   //                // Add a new row for the product
   //                ProductsBoughtTable.Rows.Add(productId, quantitySold, unitPrice, isReturned, profit);
   //                ProductsBoughtTable.Rows.Add(productId+1, quantitySold, unitPrice, isReturned, profit);
   //
   //                // Call the method to add new sale to the database
   //                bool productIsInsertedAtDatabase = AccessToClassLibraryBackendProject.AddNewSaleToDatabase(timeOfSellingOpperation, TotalPriceOfSellingOperation, ProductsBoughtTable);
   //                if (!productIsInsertedAtDatabase)
   //                {
   //                    Debug.WriteLine($"something wrong happened at day :{day} month : {month} ");
   //                    return;
   //                }
   //
   //                // Optionally add a delay to avoid overwhelming the database
   //                // await Task.Delay(100); // Uncomment if needed
   //                Debug.WriteLine($"final successful day :{day} month : {month} ");
   //            }
   //        }
   //    }
   
       



    }
}
