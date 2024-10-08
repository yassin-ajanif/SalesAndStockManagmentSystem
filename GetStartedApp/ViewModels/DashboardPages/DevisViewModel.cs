﻿using GetStartedApp.Models;

using System;
using System.Reactive;
using System.Collections.Generic;
using Avalonia.Media.Imaging;
using System.Data;
using ReactiveUI;
using System.Linq;
using GetStartedApp.Helpers.CustomUIErrorAttributes;
using GetStartedApp.Models.Objects;
using System.Reactive.Linq;
using GetStartedApp.Helpers;
using ZXing;

namespace GetStartedApp.ViewModels.DashboardPages
{
    public class DevisViewModel : MakeSaleViewModel
    {


        private int _clientOrCompanyID;
        private string _ClientOrCompanyName;

        DataTable TableOfProductsInfoScanned;
        protected int CompanyID { get; set; }
        protected int ClientID { get; set; }
        MainWindowViewModel mainWindowViewModel;

        public List<string> TypeOfClients => new List<string> { "زبون عادي", "شركة" };

        protected List<string> _clientsListOrCompanyList;

        // Property for ClientsList_Or_CompanyList
        public List<string> ClientsList_Or_CompanyList
        {
            get => _clientsListOrCompanyList;
            set => this.RaiseAndSetIfChanged(ref _clientsListOrCompanyList, value);
        }

        private string _clientOrCompanyNameEnteredByUser;

        [NotWhitespaceAttribute(ErrorMessage = "إحذف الفراغ")]
        public string ClientOrCompanyNameEnteredByUser
        {
            get => _clientOrCompanyNameEnteredByUser;
            set => this.RaiseAndSetIfChanged(ref _clientOrCompanyNameEnteredByUser, value);
        }

        private Dictionary<string, int> _companyNamesAndTheirIds = AccessToClassLibraryBackendProject.GetAllCompanyNames_And_IDs();

        // Property for ClientsList_Or_CompanyList
        public Dictionary<string, int> CompanyNamesAndTheirIds
        {
            get => _companyNamesAndTheirIds;
            set => this.RaiseAndSetIfChanged(ref _companyNamesAndTheirIds, value);
        }


        private string _selectedClientType;
        public string SelectedClientType
        {
            get => _selectedClientType;  // Return "زبون عادي" if _selectedClientType is null
            set
            {

                this.RaiseAndSetIfChanged(ref _selectedClientType, value);
                // Call method after setting the value
            }
        }

        private string _selectedClientOrCompany;
        public string SelectedClientOrCompany
        {
            get => _selectedClientOrCompany;  // Return "زبون عادي" if _selectedClientType is null
            set
            {

                this.RaiseAndSetIfChanged(ref _selectedClientOrCompany, value);
                // Call method after setting the value
            }
        }

        private string _clientTypeLabel;
        public string ClientTypeLabel
        {
            get => _clientTypeLabel;
            private set => this.RaiseAndSetIfChanged(ref _clientTypeLabel, value);
        }

        

        public ReactiveCommand<Unit, Unit> MakeDevisCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteAllProductsCommand { get; }

        public bool IsTVA_Enabled => true;

        public DevisViewModel
            (MainWindowViewModel mainWindowViewModel) : base(mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            SelectedClientType = "زبون عادي";
            LoadTypeOfClientList_Clients_Or_Companies_WheUserChoose_ClientType();
            WhenUserUserSetClienType_Or_PickTheClient_LoadInfo_Of_Client_Or_Company_Choosed();

            MakeDevisCommand = ReactiveCommand.Create(MakeDevisOperation, CheckIfSystemIsNotRaisingError_And_TVA_Is_Valid_Number_And_ProductListIsNotEmpty_Every_500ms());
            DeleteAllProductsCommand = ReactiveCommand.Create(DeleteAllProductsScanned, CheckIfUserHasScannedAtLeastOneProduct());
        }


        private void LoadTypeOfClientList_Clients_Or_Companies_WheUserChoose_ClientType()
        {
            // Subscribe to changes in SelectedClientType
            this.WhenAnyValue(x => x.SelectedClientType)
                .Subscribe(selectedClientType =>
                {
                    SelectedClientOrCompany = string.Empty;
                    // Handle the SelectedClientType value
                    if (selectedClientType == "زبون عادي")
                    {
                        ClientTypeLabel = "اسم الزبون:";
                        ClientsList_Or_CompanyList = AccessToClassLibraryBackendProject.GetClientNames_And_Their_Phones_As_String();
                    }
                    else if (selectedClientType == "شركة")
                    {
                        ClientTypeLabel = "اسم الشركة:";
                        ClientsList_Or_CompanyList = CompanyNamesAndTheirIds.Keys.ToList();
                    }

                });
        }

        // the both valid cases are when user enter a client or company that exist in the list or let the textbox empty 
        protected bool User_HasPicked_Known_Client_Or_Let_The_Textbox_Empty()
        {
            if (string.IsNullOrEmpty(ClientOrCompanyNameEnteredByUser)) return true;

            bool existsInClientListAsCompanyOrClient = _clientsListOrCompanyList.Contains(ClientOrCompanyNameEnteredByUser, StringComparer.OrdinalIgnoreCase);

            return existsInClientListAsCompanyOrClient;
        }

        protected override bool CheckIf_ProductsUnits_And_SoldPrices_Of_ScannedProducts_Are_Valid()
        {
            foreach (var productScanned in ProductsListScanned)
            {
                // Check if ProductsUnits is null, empty, whitespace, or not a valid positive integer
                if (string.IsNullOrEmpty(productScanned.ProductsUnits) ||
                    string.IsNullOrWhiteSpace(productScanned.ProductsUnits) ||
                    !int.TryParse(productScanned.ProductsUnits, out int units) ||
                    units <= 0)
                {
                    return false;
                }

                // Check if PriceOfProductSold is null, empty, whitespace, or not a valid positive decimal
                if (string.IsNullOrEmpty(productScanned.PriceOfProductSold) ||
                    string.IsNullOrWhiteSpace(productScanned.PriceOfProductSold) ||
                    !decimal.TryParse(productScanned.PriceOfProductSold, out decimal price) ||
                    price <= 0)
                {
                    return false;
                }
                //
                // if (!UiAttributeChecker.AreThesesAttributesPropertiesValid
                //    (productScanned, nameof(productScanned.ProductsUnits), nameof(productScanned.PriceOfProductSold)))
                // {
                //
                //     return false;
                // }
            }

            return true; // All checks passed
        }

        protected IObservable<bool> CheckIfSystemIsNotRaisingError_And_TVA_Is_Valid_Number_And_ProductListIsNotEmpty_Every_500ms()
        {
            // Initial condition to keep the observable running indefinitely

            var observable = Observable.Interval(TimeSpan.FromMilliseconds(500))
                                       .Select(_ =>
                                           TheSystemIsNotShowingError() &&
                                           CheckIf_TVA_Is_Valid_Number() &&
                                           ProductsListScanned.Count > 0
                                       )
                                       .DistinctUntilChanged()
                                       // .Do(_ => Debug.WriteLine("didi")) // Invoke Debug.WriteLine for each emission                                
                                       .ObserveOn(RxApp.MainThreadScheduler);
            // Ensure this observable runs on the UI thread

            return observable;
        }
        private void RaiseAnErrorIfUser_Didnt_Introduced_Client_Name_Or_Company_For_CheckAndCredit_PaymentMethods()
        {
            deleteDisplayedError();

            if (!User_HasPicked_Known_Client_Or_Let_The_Textbox_Empty())
            {
                displayErrorMessage("ادخل زبون اوشركة موجودة في القائمة او دع الخانة فارغة ");
            }
        }
        protected override void WhenUserUserChooseThe_CheckPyament_Or_DebtMode_CheckIfHePickedTheClient_NotUnkownClient()
        {
            this.WhenAnyValue(x => x.SelectedPaymentMethod, x => x.ClientOrCompanyNameEnteredByUser, x => x.ProductsListScanned.Count).

                 Subscribe(_ => {

                     RaiseAnErrorIfUser_Didnt_Introduced_Client_Name_Or_Company_For_CheckAndCredit_PaymentMethods();

                 });
        }

        private bool CheckIf_TVA_Is_Valid_Number()
        {
            return !string.IsNullOrEmpty(TaxValue) && UiAttributeChecker.AreThesesAttributesPropertiesValid(this, nameof(TaxValue));
        }

        protected void WhenUserUserSetClienType_Or_PickTheClient_LoadInfo_Of_Client_Or_Company_Choosed()
        {
            this.WhenAnyValue(x => x.ClientOrCompanyNameEnteredByUser).

                 Subscribe(_ => {

                    // if (string.IsNullOrEmpty(ClientOrCompanyNameEnteredByUser)) return;
                     if (SelectedClientType == "زبون عادي") LoadChosenClientID();
                     else if (SelectedClientType == "شركة") LoadChosenCompanyID();
                 });
        }

        protected bool ProductIsProfitable()
        {
            if (TheSystemIsShowingError()) return false;

            foreach (var product in ProductsListScanned)
            {
                // if the price user entered is less than a cost it mean that a buyer should be aware teh color of products units becomes red
                // to notify him
                if (float.Parse(product.PriceOfProductSold) - product.ProductInfo.cost < 0) { product.SoldProductPriceUnitColor = "#ff000d"; return false; }

                // we go back to the non error color which is a black
                else product.SoldProductPriceUnitColor = "Black";
            }

            return true;
        }
        protected override void WhenUserSetInvalidProduct_Price_Or_Quantity_Block_TheSystem_From_Adding_NewProducts_AndShowError_Plus_MakeAdditional_Checkings()
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
                          p => p.PriceOfProductSold
                          )
                          .Subscribe(_ =>
                          {

                              deleteDisplayedError();
                              if (CheckIf_ProductsUnits_And_SoldPrices_Of_ScannedProducts_Are_Valid() && ProductIsProfitable()) ;
                              else { displayErrorMessage("هناك خطأ في كمية المنتج او السعر"); return; };

                              RaiseAnErrorIfUser_Didnt_Introduced_Client_Name_Or_Company_For_CheckAndCredit_PaymentMethods();
                              CalculateTheTotalPriceOfOperation();
                          }
                          );
                  }
              }
                );
        }

        protected virtual void CalculateTheTotalPriceOfOperation()
        {
            decimal totalPriceOfProductsCalculated = 0;

            foreach (var product in ProductsListScanned)
            {
                // Try to parse PriceOfProductSold and ProductsUnits
                if (decimal.TryParse(product.PriceOfProductSold, out decimal productPrice) &&
                    int.TryParse(product.ProductsUnits, out int productUnits))
                {
                    // If both values are parsed successfully, calculate the total
                    totalPriceOfProductsCalculated += productPrice * productUnits;
                }

            }

            Total = totalPriceOfProductsCalculated.ToString();
        }


        protected async void DeleteAllProductsScanned()
        {
            bool UserHasClickedYesToDeleteSaleOperationBtn = await ShowDeleteSaleDialogInteraction.Handle("هل تريد حذف جميع المنتجات");

            if (UserHasClickedYesToDeleteSaleOperationBtn) ResetAllSellingInfoOperation();
        }

        protected string TranslateTheSelectedPaymentMethodInFrench()
        {
            return WordTranslation.TranslatePaymentIntoTargetedLanguage(SelectedPaymentMethod, "fr");
        }
        

        private bool userHasDecidedToMakeDevisForUnkown_Client_Or_Company()
        {
            return string.IsNullOrEmpty(ClientOrCompanyNameEnteredByUser);
        }
        
        private async void MakeDevisOperation()
        {
            TableOfProductsInfoScanned = LoadProductBoughtFromScannedListIntoADataTable();
            string SelectedPaymentMethodInFrench = TranslateTheSelectedPaymentMethodInFrench();
            decimal TVA = decimal.Parse(TaxValue);

            // we do that to prevent happening a bug in case a user has send firstly to a know client then later he decide to send devis for unknow client
            // the previous info of selected client or company are going to be sent to the unkown or of the name of unkwon client or company
            if(userHasDecidedToMakeDevisForUnkown_Client_Or_Company()) { ClientID = 0; CompanyID = 0; }

            if (SelectedClientType == "زبون عادي") AccessToClassLibraryBackendProject.GenerateDevis_ForClient(TableOfProductsInfoScanned, ClientID, SelectedPaymentMethodInFrench, TVA);

            else if (SelectedClientType == "شركة") AccessToClassLibraryBackendProject.GenerateDevis_ForCompany(CompanyID, TableOfProductsInfoScanned, SelectedPaymentMethodInFrench, TVA);

        }

        private bool UserHasDeletedClientNameOrCompany =>  string.IsNullOrEmpty(ClientOrCompanyNameEnteredByUser); 
        private bool userhasEntredInvalidCompanyNameOrClientName => PhoneNumberExtractor.ExtractPhoneNumber(ClientOrCompanyNameEnteredByUser) == "";
        private void LoadChosenClientID()
        {
            int IDofChoosedClient = 0;
         
            // we set clientid or companyid to zero if user delte the searchbox or client or company names or has entred invalid or inexistant client or company name
            if (UserHasDeletedClientNameOrCompany || userhasEntredInvalidCompanyNameOrClientName) { ClientID = IDofChoosedClient; return; }

            string ChoosedClientPhoneNumber = PhoneNumberExtractor.ExtractPhoneNumber(ClientOrCompanyNameEnteredByUser);
            string ClientName = "";
            string Email = "";

            AccessToClassLibraryBackendProject.GetClientInfo(ref IDofChoosedClient, ChoosedClientPhoneNumber, ref ClientName, ref Email);

            ClientID = IDofChoosedClient;
        }

        private void LoadChosenCompanyID()
        {
            // we wont search for an empty or null client or company because they re not existing
            if (UserHasDeletedClientNameOrCompany || userhasEntredInvalidCompanyNameOrClientName) return;
            CompanyID = CompanyNamesAndTheirIds[SelectedClientOrCompany];
        }

    }
}