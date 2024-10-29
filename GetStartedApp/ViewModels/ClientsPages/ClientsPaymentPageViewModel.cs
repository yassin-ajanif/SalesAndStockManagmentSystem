using GetStartedApp.Models.Enums;
using GetStartedApp.Models.Objects;
using GetStartedApp.ViewModels.ProductPages;
using ReactiveUI;
using System;
using System.Reactive;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetStartedApp.Models;

namespace GetStartedApp.ViewModels.ClientsPages
{
    public class ClientsPaymentPageViewModel: ReturnProductBySaleIDViewModel
    {
        private bool _isCashPaymentMethodVisible;
        private bool _isCheckPaymentMethodVisible;
        private bool _isCreditPaymentMethodVisible;
        private bool _isTpePaymentMethodVisible;
        private string _previousPaymentMethod;
        private string _previousPaymentMethodDetails;
        private string _newChequeAmount;


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

        public bool NewChequeAmount
        {
            get => _isTpePaymentMethodVisible;
            set => this.RaiseAndSetIfChanged(ref _isTpePaymentMethodVisible, value);
        }


        public ePaymentMode PaymentModeToConvert;
     
        private ClientOrCompanySaleInfo clientOrCompanySalesInfo;
        public ReactiveCommand<Unit, Unit> ConvertPaymentMethodCommand { get; }

       
        private ChequeInfo _newChequeInfoLoadedByUser = null;
        public ClientsPaymentPageViewModel(ClientOrCompanySaleInfo clientOrCompanySalesInfo,ePaymentMode PaymentModeToConvert) :base(clientOrCompanySalesInfo.SaleID)
        {
            this.clientOrCompanySalesInfo = clientOrCompanySalesInfo;
            this.PaymentModeToConvert = PaymentModeToConvert;
            ConvertPaymentMethodCommand = ReactiveCommand.Create(ConvertThePaymentMethod_To_SelectedPaymentMethod);

            SetThePreviousMethodPaymentUi();
            SetTheAcutalMethodPaymentUi();
        }

        // this function is converting the payment method from and to these payments methods cash , credit , tpe , check , deposit
     
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
            PreviousPaymentMethodDetails = string.Empty;
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

            string amountPaidMessageInArabic = "و مبلغ الدفع هو ";
            PreviousPaymentMethodDetails = amountPaidMessageInArabic + getDepositAmountBySaleID.ToString()+"Dh";
        }
      
        private void SetInforamtionOfCheckPaymentMethod_AsPreviousPaymentMethod()
        {
            PreviousPaymentMethod = "شيك";
            long getCheckNumberFromSaleID = SalesProductsManagmentSystemBusinessLayer.clsChecks.GetCustomerChequeIDBySaleID(clientOrCompanySalesInfo.SaleID);

            if (getCheckNumberFromSaleID == -1) throw new Exception("No customer cheque found for the provided Sale ID. Please check the Sale ID or ensure a cheque was issued.");
            
            string CheckNumberInArabic = "ورقم الشيك هو ";
            PreviousPaymentMethodDetails = CheckNumberInArabic + getCheckNumberFromSaleID.ToString();
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

        private void ConvertThePaymentMethodTo_Cash() {

            bool isOperationSucceded = AccessToClassLibraryBackendProject.ExecuteProcessPayment
                (depositAmount: null, saleId :clientOrCompanySalesInfo.SaleID,selectedPaymentMethod: "cash", checkAmount: null, checkNumber: null, checkDate:null);



        }
        private void ConvertThePaymentMethodTo_Check() {

            bool isOperationSucceded =  AccessToClassLibraryBackendProject.ExecuteProcessPayment
        (depositAmount: null, saleId: clientOrCompanySalesInfo.SaleID, selectedPaymentMethod: "check", 
         checkAmount: _newChequeInfoLoadedByUser.Amount, checkNumber: _newChequeInfoLoadedByUser.ChequeNumber, checkDate: _newChequeInfoLoadedByUser.ChequeDate);

        }
        private void ConvertThePaymentMethodTo_Credit()
        {
            // Execute the process payment for Credit
            bool isOperationSucceded = AccessToClassLibraryBackendProject.ExecuteProcessPayment(
                depositAmount: null,
                saleId: clientOrCompanySalesInfo.SaleID,
                selectedPaymentMethod: "Credit",
                checkAmount: null,
                checkNumber: null,
                checkDate: null);
        }

        private void ConvertThePaymentMethodTo_Tpe()
        {
            // Execute the process payment for TPE
            bool isOperationSucceded = AccessToClassLibraryBackendProject.ExecuteProcessPayment(
                depositAmount: null,
                saleId: clientOrCompanySalesInfo.SaleID,
                selectedPaymentMethod: "TPe",
                checkAmount: null,
                checkNumber: null,
                checkDate: null);
        }

        private void ConvertThePaymentMethodTo_Deposit()
        {
            // Assuming you have a deposit amount to process for Deposit payment method
            decimal depositAmount = decimal.Parse(_newChequeAmount); // Assuming you have a way to load this information

            // Execute the process payment for Deposit
            bool isOperationSucceded = AccessToClassLibraryBackendProject.ExecuteProcessPayment(
                depositAmount: depositAmount,
                saleId: clientOrCompanySalesInfo.SaleID,
                selectedPaymentMethod: "Deposit",
                checkAmount: null,
                checkNumber: null,
                checkDate: null);
        }


        private void ConvertThePaymentMethod_To_SelectedPaymentMethod()
        {
            switch (PaymentModeToConvert)
            {
                case ePaymentMode.ToCash:
                    ConvertThePaymentMethodTo_Cash();
                    break;

                case ePaymentMode.ToCheck:
                    ConvertThePaymentMethodTo_Check();
                    break;

                case ePaymentMode.ToTpe:
                    ConvertThePaymentMethodTo_Tpe();
                    break;

                case ePaymentMode.ToCredit:
                    ConvertThePaymentMethodTo_Credit();
                    break;

                case ePaymentMode.ToDeposit:
                    ConvertThePaymentMethodTo_Deposit();
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported PaymentID: {clientOrCompanySalesInfo.PaymentID}");
            }
        }


    }
}
