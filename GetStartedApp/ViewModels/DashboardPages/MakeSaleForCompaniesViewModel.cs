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
using System.ComponentModel.Design;

namespace GetStartedApp.ViewModels.DashboardPages
{
    public class MakeSaleForCompaniesViewModel : MakeSaleViewModel
    {

        MainWindowViewModel mainWindowViewModel;
        private string _selectedCompanyName;

        public string SelectedCompanyName
        {
            get => _selectedCompanyName;
            set => this.RaiseAndSetIfChanged(ref _selectedCompanyName, value);
        }
        private Dictionary<string, int> CompanyNames_And_TheirIds => GetAllCompanyNames_And_TheirIds();
        public List<string> CompanyNames => CompanyNames_And_TheirIds.Keys.ToList();
        public MakeSaleForCompaniesViewModel(MainWindowViewModel mainWindowViewModel) : base(mainWindowViewModel)
        {

            this.mainWindowViewModel = mainWindowViewModel;
        }

        private int getCompanyID_From_Its_Name()
        {
            return CompanyNames_And_TheirIds[SelectedCompanyName];
        }

        public Dictionary<string, int> GetAllCompanyNames_And_TheirIds()
        {
            return AccessToClassLibraryBackendProject.GetAllCompanyNames_And_IDs();
        }

     

     


        public override async void SubmitOperationSalesDataToDatabase
        (DateTime timeOfSellingOpperationIsNow, float TotalPriceOfSellingOperation, DataTable ProductsBoughtInThisOperation, string slectedPaymentMethodInEnglish, ChequeInfo userChequeInfo)
        {

            int selectedCompanyID_From_selectedCompanyName = getCompanyID_From_Its_Name();


            var result =
                AccessToClassLibraryBackendProject.
                AddNewSaleToDatabase_ForCompanies
                (timeOfSellingOpperationIsNow, TotalPriceOfSellingOperation, ProductsBoughtInThisOperation, selectedCompanyID_From_selectedCompanyName, slectedPaymentMethodInEnglish, userChequeInfo);

            int lastSaleID = result.bonLivraisonNumber;

            if (result.isSuccess)
            {

                await ShowAddSaleDialogInteraction.Handle("لقد تمت العملية بنجاح");

                // Use result.SalesId if needed for further processing
                if (await ShowDeleteSaleDialogInteraction.Handle(" هل تريد طباعة وصل الاستلام "))
                {

                    CreateBonLivraison_For_Company(selectedCompanyID_From_selectedCompanyName, ProductsBoughtInThisOperation, slectedPaymentMethodInEnglish, lastSaleID,timeOfSellingOpperationIsNow);
                }

                if (await ShowDeleteSaleDialogInteraction.Handle(" هل تريد طباعة الفاتورة ايضا "))
                {
                    int invoiceNumber = AccessToClassLibraryBackendProject.AddInvoiceIfNotExists(lastSaleID);
                    CreateInvoice_For_Company(lastSaleID, invoiceNumber,selectedCompanyID_From_selectedCompanyName, ProductsBoughtInThisOperation, SelectedPaymentMethod);                

                    ResetAllSellingInfoOperation();
                    mainWindowViewModel.CheckIfSystemShouldRaiseBellNotificationIcon();
                }
}

            else { await ShowAddSaleDialogInteraction.Handle(" لقد حصل خطأ ما تاكد من ان المنتجات اللتي تريد ان تضيف موجودة في المخزن  "); }
            
        }
    }
}
