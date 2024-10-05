using GetStartedApp.Helpers;
using GetStartedApp.Models;
using GetStartedApp.Models.Objects;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using GetStartedApp.Helpers.CustomUIErrorAttributes;

namespace GetStartedApp.ViewModels.DashboardPages
{
    public class BonLivraisonsViewModel : DevisViewModel
    {

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
        public string BarcodeNumber
        {
            get => _barcodeNumber;
            set => this.RaiseAndSetIfChanged(ref _barcodeNumber, value);
        }

        private string _nameOrCompanyName;
        public string NameOrCompanyName
        {
            get => _nameOrCompanyName;
            set => this.RaiseAndSetIfChanged(ref _nameOrCompanyName, value);
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
        public BonLivraisonsViewModel(MainWindowViewModel mainWindowViewModel):base(mainWindowViewModel)
        {
            ThisDayBtnCommand = ReactiveCommand.Create(setStartAndDateOfToday_WhenTodayBtnIsClicked);
            ThisWeekBtnCommand = ReactiveCommand.Create(setStartAndDateThisWeek_WhenWeekBtnIsClicked);
            ThisMonthBtnCommand = ReactiveCommand.Create(setStartAndDateOfThisMonth_WhenThisMonthBtnIsClicked);

            GetSaleInfoFromDbCommand = ReactiveCommand.Create(GetSalesInfoFromDb_For_Clients_Or_Companies);
            
        }

        public void GetSalesInfoFromDb_For_Clients_Or_Companies()
        {
            string SelectedClientOrCompanyInEnglish = SelectedClientType;
            if (SelectedClientType == "زبون عادي") { SelectedClientOrCompanyInEnglish = "Client";  NumberOfClientOrCompany = "رقم الزبون";  NameOfClientOrCompany = "اسم الزبون"; }
            else if (SelectedClientType == "شركة") { SelectedClientOrCompanyInEnglish = "Company"; NumberOfClientOrCompany = " رقم الشركة"; NameOfClientOrCompany = "اسم الشركة"; }

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



    }
}
