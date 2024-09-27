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

namespace GetStartedApp.ViewModels.DashboardPages
{
    public class DevisViewModel : MakeSaleViewModel
    {
       
      
        private int    _clientOrCompanyID;
        private string _ClientOrCompanyName;

        DataTable TableOfProductsInfoScanned;
        int         CompanyID;
        int         ClientID;
        MainWindowViewModel mainWindowViewModel;  
        
        public List<string> TypeOfClients => new List<string> { "زبون عادي", "شركة" };

        private List<string> _clientsListOrCompanyList;

        // Property for ClientsList_Or_CompanyList
        public List<string> ClientsList_Or_CompanyList
        {
            get => _clientsListOrCompanyList;
            set => this.RaiseAndSetIfChanged(ref _clientsListOrCompanyList, value);
        }


        private Dictionary<string,int> _companyNamesAndTheirIds = AccessToClassLibraryBackendProject.GetAllCompanyNames_And_IDs();

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

        private string _taxValue;

        [PositiveIntRange(1, 100, ErrorMessage = "الرقم يجب ان يكون بين 1 و 100")]
        public string TaxValue
        {
            get => _taxValue;
            set => this.RaiseAndSetIfChanged(ref _taxValue, value);
        }

        public ReactiveCommand <Unit, Unit> MakeDevisCommand { get; }
        public ReactiveCommand <Unit, Unit> DeleteAllProductsCommand { get; }

        public bool IsTVA_Enabled => false;

        public DevisViewModel
            (MainWindowViewModel mainWindowViewModel):base(mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            SelectedClientType = "زبون عادي";
            LoadTypeOfClientList_Clients_Or_Companies_WheUserChoose_ClientType();
            WhenUserUserSetClienType_Or_PickTheClient_LoadInfo_Of_Client_Or_Company_Choosed();
          
            MakeDevisCommand =ReactiveCommand.Create(MakeDevisOperation, CheckIfSystemIsNotRaisingError_And_ExchangeIsPositiveNumber_And_ProductListIsNotEmpty_Every_500ms());
            DeleteAllProductsCommand = ReactiveCommand.Create(DeleteAllProductsScanned, CheckIfUserHasScannedAtLeastOneProduct());
        }


        private void LoadTypeOfClientList_Clients_Or_Companies_WheUserChoose_ClientType()
        {
            // Subscribe to changes in SelectedClientType
            this.WhenAnyValue(x => x.SelectedClientType)
                .Subscribe(selectedClientType =>
                {
                    SelectedClientOrCompany=string.Empty;
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

       protected override bool User_HasPicked_Known_Client()
       {
            if (string.IsNullOrEmpty(SelectedClientOrCompany)) return false;
            
            bool existsInClientListAsCompanyOrClient = _clientsListOrCompanyList.Contains(SelectedClientOrCompany, StringComparer.OrdinalIgnoreCase);
     
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
            }

            return true; // All checks passed
        }


        protected override void WhenUserUserChooseThe_CheckPyament_Or_DebtMode_CheckIfHePickedTheClient_NotUnkownClient()
        {
            this.WhenAnyValue(x => x.SelectedPaymentMethod,x=>x.SelectedClientOrCompany).

                 Subscribe(_ => {

                     deleteDisplayedError();

                     if (!User_HasPicked_Known_Client())
                     {
                         displayErrorMessage("ادخل زبون اوشركة موجودة في القائمة");
                     }

                 });
        }

        protected void WhenUserUserSetClienType_Or_PickTheClient_LoadInfo_Of_Client_Or_Company_Choosed()
        {
            this.WhenAnyValue( x => x.SelectedClientOrCompany).

                 Subscribe(_ => {

                     if (string.IsNullOrEmpty(SelectedClientOrCompany)) return;
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
                              else displayErrorMessage("هناك خطأ في كمية المنتج او السعر");

                              WhenUserUserChooseThe_CheckPyament_Or_DebtMode_CheckIfHePickedTheClient_NotUnkownClient();
                              CalculateTheTotalPriceOfOperation();
                          }
                          );
                  }
              }
                );
        }

        protected async void DeleteAllProductsScanned()
        {
            bool UserHasClickedYesToDeleteSaleOperationBtn = await ShowDeleteSaleDialogInteraction.Handle("هل تريد حذف جميع المنتجات");

            if (UserHasClickedYesToDeleteSaleOperationBtn) ResetAllSellingInfoOperation();
        }
        private async void MakeDevisOperation()
        {
            TableOfProductsInfoScanned=LoadProductBoughtFromScannedListIntoADataTable();

            if (SelectedClientType == "زبون عادي") AccessToClassLibraryBackendProject.GenerateDevis_ForClient(TableOfProductsInfoScanned,ClientID);

            else if (SelectedClientType == "شركة") AccessToClassLibraryBackendProject.GenerateDevis_ForCompany(CompanyID,TableOfProductsInfoScanned );

        }

        private void LoadChosenClientID()
        {
                int IDofChoosedClient = 0;
                string ChoosedClientPhoneNumber = PhoneNumberExtractor.ExtractPhoneNumber(SelectedClientOrCompany);
                string ClientName = "";
                string Email = "";
          
                AccessToClassLibraryBackendProject.GetClientInfo(ref IDofChoosedClient, ChoosedClientPhoneNumber, ref ClientName, ref Email);

            ClientID = IDofChoosedClient; 
        }

        private void LoadChosenCompanyID()
        {
            CompanyID  = CompanyNamesAndTheirIds[SelectedClientOrCompany];
        }

    }
}

