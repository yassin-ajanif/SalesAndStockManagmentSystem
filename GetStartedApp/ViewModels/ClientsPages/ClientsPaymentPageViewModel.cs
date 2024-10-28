using GetStartedApp.Models.Enums;
using GetStartedApp.Models.Objects;
using GetStartedApp.ViewModels.ProductPages;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStartedApp.ViewModels.ClientsPages
{
    public class ClientsPaymentPageViewModel: ReturnProductBySaleIDViewModel
    {
        private bool _isCashPaymentMethodVisible;
        private bool _isCheckPaymentMethodVisible;
        private bool _isCreditPaymentMethodVisible;
        private bool _isTpePaymentMethodVisible;
        private string _previousPaymentMethod="دفع كلي";
        private string _previousPaymentMethodDetails= "دفع مسبقا 150dh";


        public string PreviousPaymentMethod
        {
            get => _previousPaymentMethod;
            set => this.RaiseAndSetIfChanged(ref _previousPaymentMethod, value);
        }

        public string PreviousPaymentMethodDetails
        {
            get => _previousPaymentMethodDetails;
            set => this.RaiseAndSetIfChanged(ref _previousPaymentMethodDetails, value);
        }
        public bool IsCashPaymentMethodVisible
        {
            get => _isCashPaymentMethodVisible;
            set => this.RaiseAndSetIfChanged(ref _isCashPaymentMethodVisible, value);
        }

        public bool IsCheckPaymentMethodVisible
        {
            get => _isCheckPaymentMethodVisible;
            set => this.RaiseAndSetIfChanged(ref _isCheckPaymentMethodVisible, value);
        }

        public bool IsCreditPaymentMethodVisible
        {
            get => _isCreditPaymentMethodVisible;
            set => this.RaiseAndSetIfChanged(ref _isCreditPaymentMethodVisible, value);
        }

        public bool IsTpePaymentMethodVisible
        {
            get => _isTpePaymentMethodVisible;
            set => this.RaiseAndSetIfChanged(ref _isTpePaymentMethodVisible, value);
        }

        public ePaymentMode PaymentModeToConvert;
     
        private ClientOrCompanySaleInfo clientOrCompanySalesInfo;

        public ClientsPaymentPageViewModel(ClientOrCompanySaleInfo clientOrCompanySalesInfo,ePaymentMode PaymentModeToConvert) :base(clientOrCompanySalesInfo.SaleID)
        {
            this.clientOrCompanySalesInfo = clientOrCompanySalesInfo;
            this.PaymentModeToConvert = PaymentModeToConvert;

            SetThePreviousMethodPaymentUi();
            SetTheAcutalMethodPaymentUi();
        }
        public void DisableAllPaymentMethods()
        {
            IsCashPaymentMethodVisible = false;
            IsCheckPaymentMethodVisible = false;
            IsCreditPaymentMethodVisible = false;
            IsTpePaymentMethodVisible = false;
        }
        private void EnableCashPaymentMethod()
        {
            DisableAllPaymentMethods();
            IsCashPaymentMethodVisible = true;

        }

        private void EnableCheckPaymentMethod()
        {
            DisableAllPaymentMethods();
            IsCheckPaymentMethodVisible = true;
        }

        private void EnableCreditPaymentMethod()
        {
            DisableAllPaymentMethods();
            IsCreditPaymentMethodVisible = true;
        }

        private void EnableDepositPaymentMethod()
        {
            EnableCreditPaymentMethod();
        }
        private void EnableTpePaymentMethod()
        {
            DisableAllPaymentMethods();
            IsTpePaymentMethodVisible = true;
        }

        private void SetInforamtionOfCashPaymentMethod_AsPreviousPaymentMethod()
        {
            PreviousPaymentMethod = "دفع كلي";
            PreviousPaymentMethodDetails = "فارغ";
        }

        private void SetInforamtionOfCreditPaymentMethod_AsPreviousPaymentMethod()
        {
            PreviousPaymentMethod = "دين";
            PreviousPaymentMethodDetails = string.Empty;
        }

        private void SetInforamtionOfDepositPaymentMethod_AsPreviousPaymentMethod()
        {
            PreviousPaymentMethod = "دفع مسبق";
            decimal getDepositAmountBySaleID = SalesProductsManagmentSystemBusinessLayer.clsDeposits.RetrieveDepositAmountBySaleID(clientOrCompanySalesInfo.SaleID);
            if(getDepositAmountBySaleID==-1) throw new Exception("No customer Deposit found for the provided Sale ID. Please check the Sale ID or ensure a cheque was issued.");
            PreviousPaymentMethodDetails = getDepositAmountBySaleID.ToString();
        }
        private void SetInforamtionOfCheckPaymentMethod_AsPreviousPaymentMethod()
        {
            PreviousPaymentMethod = "شيك";
            long getCheckNumberFromSaleID = SalesProductsManagmentSystemBusinessLayer.clsChecks.GetCustomerChequeIDBySaleID(clientOrCompanySalesInfo.SaleID);

            if (getCheckNumberFromSaleID == -1) throw new Exception("No customer cheque found for the provided Sale ID. Please check the Sale ID or ensure a cheque was issued.");
            
            PreviousPaymentMethodDetails = getCheckNumberFromSaleID.ToString();
        }

        private void SetInforamtionOfTpePaymentMethod_AsPreviousPaymentMethod()
        {
            PreviousPaymentMethod = "Tpe";
            PreviousPaymentMethodDetails = string.Empty;
        }

        private void SetThePreviousMethodPaymentUi()
        {
            switch (clientOrCompanySalesInfo.PaymentID)
            {
                case (int)ePaymentMode.ToCash:   SetInforamtionOfCashPaymentMethod_AsPreviousPaymentMethod(); break;

                case (int)ePaymentMode.ToCheck:  SetInforamtionOfCheckPaymentMethod_AsPreviousPaymentMethod(); break;

                case (int)ePaymentMode.ToTpe: SetInforamtionOfTpePaymentMethod_AsPreviousPaymentMethod(); break;

                case (int)ePaymentMode.ToCredit: SetInforamtionOfCreditPaymentMethod_AsPreviousPaymentMethod(); break;

                case (int)ePaymentMode.ToDeposit: SetInforamtionOfDepositPaymentMethod_AsPreviousPaymentMethod(); break;

                default:
                    throw new InvalidOperationException($"Unsupported PaymentID: {clientOrCompanySalesInfo.PaymentID}");
            }
        }

        private void SetTheAcutalMethodPaymentUi()
        {
            switch (PaymentModeToConvert)
            {
                case ePaymentMode.ToCash: EnableCashPaymentMethod(); break;

                case ePaymentMode.ToCheck: EnableCheckPaymentMethod(); break;

                case ePaymentMode.ToTpe: EnableTpePaymentMethod(); break;

                case ePaymentMode.ToCredit: EnableCreditPaymentMethod(); break;

                case ePaymentMode.ToDeposit: EnableDepositPaymentMethod(); break;

                default:
                    throw new InvalidOperationException($"Unsupported PaymentID: {clientOrCompanySalesInfo.PaymentID}");
            }
        }


    }
}
