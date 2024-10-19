using GetStartedApp.Helpers;
using GetStartedApp.Models;
using GetStartedApp.Models.Objects;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using GetStartedApp.Helpers.CustomUIErrorAttributes;
using System.Reactive.Linq;


namespace GetStartedApp.ViewModels.DashboardPages
{
    public class BonLivraisonsViewModel : DevisViewModel
    {
        protected const long maxNumberOfProductsCanSystemHold = 1_000_000_000_000_000_000;

        private List<ClientOrCompanySaleInfo> _clientOrCompanySaleInfos;
        
        public List<ClientOrCompanySaleInfo> clientOrCompanySaleInfos{get =>  _clientOrCompanySaleInfos; set => this.RaiseAndSetIfChanged(ref _clientOrCompanySaleInfos, value); }

        private string _numberOfClientOrCompany;
        public string NumberOfClientOrCompany { get => _numberOfClientOrCompany; set => this.RaiseAndSetIfChanged(ref _numberOfClientOrCompany, value); }

        private string _nameOfClientOrCompany;

        public string NameOfClientOrCompany { get => _nameOfClientOrCompany; set => this.RaiseAndSetIfChanged(ref _nameOfClientOrCompany, value); }

        private string _saleID;
        [PositiveIntRange(1, 1_000_000, ErrorMessage = "ادخل رقم موجب وبدون فاصلة")]
        public string SaleID
        {
            get => _saleID;
            set => this.RaiseAndSetIfChanged(ref _saleID, value);
        }

        private string _barcodeNumber;
        [PositiveIntRange(1, maxNumberOfProductsCanSystemHold, ErrorMessage = "ادخل رقم موجب وبدون فاصلة ")]
        [ProductIdIsNotExistingInDb("هذا الرقم لا يوجد")]
        public string BarcodeNumber
        {
            get => _barcodeNumber;
            set => this.RaiseAndSetIfChanged(ref _barcodeNumber, value);
        }

    
        private string _minAmount;
        [PositiveFloatRange(1, 1_000_000, ErrorMessage = "ادخل رقم موجب")]
        public string MinAmount
        {
            get => _minAmount;
            set => this.RaiseAndSetIfChanged(ref _minAmount, value);
        }

        private string _maxAmount;
       
        [PositiveFloatRange(1, 1_000_000, ErrorMessage = "ادخل رقم موجب  ")]
        public string MaxAmount
        {
            get => _maxAmount;
            set => this.RaiseAndSetIfChanged(ref _maxAmount, value);
        }

        

        public ReactiveCommand<Unit, Unit> GetSaleInfoFromDbCommand { get; set; }
        public ReactiveCommand<object, Unit> DisplaySellInfoAsPdfCommand { get; set; }
        public ReactiveCommand<object, Unit> DisplayInvoiceInfoAsPdfCommand { get; set; }
        public ReactiveCommand<object, Unit> OpenProductBoughtBySaleID_Command { get; set; }
        
        public Interaction  <int,Unit> OpenProductsListToReturn { get; }

        public BonLivraisonsViewModel(MainWindowViewModel mainWindowViewModel):base(mainWindowViewModel)
        {
            ThisDayBtnCommand = ReactiveCommand.Create(setStartAndDateOfToday_WhenTodayBtnIsClicked);
            ThisWeekBtnCommand = ReactiveCommand.Create(setStartAndDateThisWeek_WhenWeekBtnIsClicked);
            ThisMonthBtnCommand = ReactiveCommand.Create(setStartAndDateOfThisMonth_WhenThisMonthBtnIsClicked);
           
            OpenProductsListToReturn = new Interaction<int, Unit>();

            GetSaleInfoFromDbCommand = ReactiveCommand.Create(GetSalesInfoFromDb_For_Clients_Or_Companies, CheckIfUserDidintMakeSearchMistake());
            DisplaySellInfoAsPdfCommand = ReactiveCommand.Create<object>(DisplaySalePdf_Based_on_SaleID);
            DisplayInvoiceInfoAsPdfCommand = ReactiveCommand.Create<object>(DisplayInvoicePdf_Based_on_SaleID);
            OpenProductBoughtBySaleID_Command = ReactiveCommand.Create<object>(DisplayProductListToReturn);

            setTheSearchMode_For_Client_Or_Company(SelectedClientType);
            when_UserSearchProductByName_DeleteBarcode_SearchText_And_ViceSera();


        }

        protected virtual void when_UserSearchProductByName_DeleteBarcode_SearchText_And_ViceSera()
        {
            // Listen to changes in ProductNameTermToSerach
            this.WhenAnyValue(x => x.ProductNameTermToSerach)
                .Subscribe(productName =>
                {
                    // If the user types in ProductNameTermToSerach, clear BarcodeNumber
                    if (!string.IsNullOrEmpty(productName))
                    {
                        BarcodeNumber = string.Empty;
                    }
                });

            // Listen to changes in BarcodeNumber
            this.WhenAnyValue(x => x.BarcodeNumber)
                .Subscribe(barcode =>
                {
                    // If the user types in BarcodeNumber, clear ProductNameTermToSerach and SelectedProductNameTermSerach
                    if (!string.IsNullOrEmpty(barcode))
                    {
                        ProductNameTermToSerach = string.Empty;
                        SelectedProductNameTermSerach = string.Empty;
                    }
                });
        }

        private string setTheSearchMode_For_Client_Or_Company(string SelectedClientType)
        {
            string SelectedClientOrCompanyInEnglish = SelectedClientType;
            if (SelectedClientType == "زبون عادي") { SelectedClientOrCompanyInEnglish = "Client"; NumberOfClientOrCompany = "رقم الزبون"; NameOfClientOrCompany = "اسم الزبون"; }
            else if (SelectedClientType == "شركة") { SelectedClientOrCompanyInEnglish = "Company"; NumberOfClientOrCompany = " رقم الشركة"; NameOfClientOrCompany = "اسم الشركة"; }

            return SelectedClientOrCompanyInEnglish;
        }

 
        protected virtual IObservable<bool> CheckIfUserDidintMakeSearchMistake()
        {
            var canAddProduct1 = this.WhenAnyValue(
                x => x.SaleID,
                x => x.BarcodeNumber,
                x => x.ProductNameTermToSerach,
                x => x.SelectedClientType,
                x => x.SelectedClientOrCompany,
                x => x.MinAmount,
                x => x.MaxAmount,
                x => x.ClientOrCompanyNameEnteredByUser,
                (SaleID, BarcodeNumber, ProductNameTermToSerach, SelectedClientType, SelectedClientOrCompany, MinAmount, MaxAmount, ClientOrCompanyNameEnteredByUser) =>
                   
                UiAttributeChecker.AreThesesAttributesPropertiesValid(
                        this,
                        nameof(SaleID),
                        nameof(BarcodeNumber),
                        nameof(ProductNameTermToSerach),
                        nameof(SelectedClientType),
                        nameof(SelectedClientOrCompany),
                        nameof(MinAmount),
                        nameof(MaxAmount),
                        // this property is inherieted from the base class and and use in this calss
                        nameof(ClientOrCompanyNameEnteredByUser)) 
                    && 
                    TheSystemIsNotShowingError()
                      
                    

            );

            return canAddProduct1;
        }


        public void GetSalesInfoFromDb_For_Clients_Or_Companies()
        {

            string SelectedClientOrCompanyInEnglish = setTheSearchMode_For_Client_Or_Company(SelectedClientType);

            int paymentIdFromSelectedPaymentName = 
                AccessToClassLibraryBackendProject.GetPaymentTypeID(WordTranslation.TranslatePaymentIntoTargetedLanguage(SelectedPaymentMethod, "en"));

            // it means that the payment id returned from db is not existing
            if (paymentIdFromSelectedPaymentName == -1) return;

            clientOrCompanySaleInfos =
                                        AccessToClassLibraryBackendProject.LoadSalesForClientOrCompany(
                                            SelectedClientOrCompanyInEnglish,
                                            StartDate.Date,
                                            EndDate.Date,
                                            paymentIdFromSelectedPaymentName,
                                            decimal.TryParse(MinAmount, out decimal parsedMinAmount) ? parsedMinAmount : (decimal?)null,
                                            decimal.TryParse(MaxAmount, out decimal parsedMaxAmount) ? parsedMaxAmount : (decimal?)null,
                                            long.TryParse(BarcodeNumber, out long parsedBarcodeNumber) ? parsedBarcodeNumber : (long?)null, // Polished BarcodeNumber handling
                                            ProductNameTermToSerach = string.IsNullOrEmpty(ProductNameTermToSerach) ? null : ProductNameTermToSerach,
                                            int.TryParse(SaleID, out int parsedSaleID) ? parsedSaleID : (int?)null,
                                            ClientID > 0 ? ClientID : (int?)null,
                                            CompanyID > 0 ? CompanyID : (int?)null
     );


        }


        private void DisplaySalePdf_Based_on_SaleID(object selectedItem)
        {
            // Attempt to cast selectedItem to ClientOrCompanySaleInfo
            if (selectedItem is ClientOrCompanySaleInfo saleInfo)
            {
                // Access the properties of the saleInfo object
                int saleID = saleInfo.SaleID; // Ensure you access SaleID correctly
           
               ProductSoldInfos productsSoldInfo =AccessToClassLibraryBackendProject.LoadProductSoldInfoFromReader(saleID);

                bool TheseProductsSoldInfo_Are_Made_For_Client = productsSoldInfo.ClientID != null;
                bool TheseProductsSoldInfo_Are_Made_For_Company = productsSoldInfo.CompanyID != null;
                // Assign the first row's "TVA" column value to a decimal variable becuase in
                // this current version all products that belong to a sale id have the same TVA the same thing for insertion time

                decimal TVA = (decimal)productsSoldInfo.ProductsBoughtInThisOperation.Rows[0]["TVA"];
                DateTime SalesTime  = (DateTime)productsSoldInfo.ProductsBoughtInThisOperation.Rows[0]["InsertionTime"];

                int invoiceNumber = AccessToClassLibraryBackendProject.AddInvoiceIfNotExists(saleID);
                bool DoesUserHasPrintedInvoiceBefore = false;
                bool invoiceNumberIsAlreadyExisting = invoiceNumber == -1;
                // if the user has printed before the invoice is going tobe found at the invices table in databse so the addinvoiceifnot exist will return -1 instead of the new invoice created

                DoesUserHasPrintedInvoiceBefore = invoiceNumberIsAlreadyExisting;
                if (invoiceNumberIsAlreadyExisting) invoiceNumber = AccessToClassLibraryBackendProject.GetInvoiceIDBySaleID(saleID);


                if (TheseProductsSoldInfo_Are_Made_For_Client)
                {


                    CreateBonLivraison_For_Client
                        (productsSoldInfo.SaleID, productsSoldInfo.ClientID.Value, productsSoldInfo.ProductsBoughtInThisOperation,
                        SelectedPaymentMethod,null,SalesTime);
                }

                else if (TheseProductsSoldInfo_Are_Made_For_Company)
                {

                    CreateBonLivraison_For_Company(productsSoldInfo.CompanyID.Value, productsSoldInfo.ProductsBoughtInThisOperation, SelectedPaymentMethod, saleID, SalesTime);
                }
                
            }
        }


        private void DisplayInvoicePdf_Based_on_SaleID(object selectedItem)
        {
            // Attempt to cast selectedItem to ClientOrCompanySaleInfo
            if (selectedItem is ClientOrCompanySaleInfo saleInfo)
            {
                // Access the properties of the saleInfo object
                int saleID = saleInfo.SaleID; // Ensure you access SaleID correctly

                ProductSoldInfos productsSoldInfo = AccessToClassLibraryBackendProject.LoadProductSoldInfoFromReader(saleID);

                bool TheseProductsSoldInfo_Are_Made_For_Client = productsSoldInfo.ClientID != null;
                bool TheseProductsSoldInfo_Are_Made_For_Company = productsSoldInfo.CompanyID != null;
                // Assign the first row's "TVA" column value to a decimal variable becuase in
                // this current version all products that belong to a sale id have the same TVA the same thing for insertion time

                decimal TVA = (decimal)productsSoldInfo.ProductsBoughtInThisOperation.Rows[0]["TVA"];
                DateTime SalesTime = (DateTime)productsSoldInfo.ProductsBoughtInThisOperation.Rows[0]["InsertionTime"];

                int invoiceNumber = AccessToClassLibraryBackendProject.AddInvoiceIfNotExists(saleID);
                bool DoesUserHasPrintedInvoiceBefore = false;
                bool invoiceNumberIsAlreadyExisting = invoiceNumber == -1;
                // if the user has printed before the invoice is going tobe found at the invices table in databse so the addinvoiceifnot exist will return -1 instead of the new invoice created

                DoesUserHasPrintedInvoiceBefore = invoiceNumberIsAlreadyExisting;
                if (invoiceNumberIsAlreadyExisting) invoiceNumber = AccessToClassLibraryBackendProject.GetInvoiceIDBySaleID(saleID);

                if (TheseProductsSoldInfo_Are_Made_For_Client)
                {
                    
                    CreateInvoice_For_Client
                        (productsSoldInfo.SaleID, invoiceNumber, productsSoldInfo.ClientID.Value,
                        productsSoldInfo.ProductsBoughtInThisOperation, SelectedPaymentMethod,null, DoesUserHasPrintedInvoiceBefore,SalesTime );
                 
                }

                else if (TheseProductsSoldInfo_Are_Made_For_Company)
                {
                    CreateInvoice_For_Company(productsSoldInfo.SaleID, invoiceNumber, productsSoldInfo.CompanyID.Value,
                        productsSoldInfo.ProductsBoughtInThisOperation, SelectedPaymentMethod, DoesUserHasPrintedInvoiceBefore, SalesTime);
                }
            }
        }

   
        private async void DisplayProductListToReturn(object SelectedItem)
        {
            // Attempt to cast selectedItem to ClientOrCompanySaleInfo
            if (SelectedItem is ClientOrCompanySaleInfo saleInfo)
            {
                // Access the properties of the saleInfo object
                int saleID = saleInfo.SaleID; // Ensure you access SaleID correctly

                await OpenProductsListToReturn.Handle(saleID);
            }
        }
    
    }
}
