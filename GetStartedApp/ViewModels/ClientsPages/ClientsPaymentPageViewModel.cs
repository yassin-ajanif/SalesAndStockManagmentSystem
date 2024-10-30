using GetStartedApp.Models.Enums;
using GetStartedApp.Models.Objects;
using GetStartedApp.ViewModels.ProductPages;
using ReactiveUI;
using System;
using System.Reactive;
using GetStartedApp.Models;
using GetStartedApp.Helpers.CustomUIErrorAttributes;
using GetStartedApp.Helpers;
using System.Reactive.Linq;



namespace GetStartedApp.ViewModels.ClientsPages
{
    public class ClientsPaymentPageViewModel: ReturnProductBySaleIDViewModel
    {
        private bool   _isCashPaymentMethodVisible;
        private bool   _isCheckPaymentMethodVisible;
        private bool   _isCreditPaymentMethodVisible;
        private bool   _isTpePaymentMethodVisible;
        private bool   _isDepositPaymentMethodVisible;
        private string _previousPaymentMethod;
        private string _previousPaymentMethodDetails;
        private string _newDepositAmount;
        private bool _isFullPaymentMethodChecked;
        private bool _isCreditPaymentMethodChecked;
        private bool _isTpePaymentMethodChecked;
        private string _newEntredCheckNumber;


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

        public bool IsDepositPaymentMethodVisible
        {
            get => _isDepositPaymentMethodVisible;
            set => this.RaiseAndSetIfChanged(ref _isDepositPaymentMethodVisible, value);
        }

        [PositiveFloatRange(1,100_000,ErrorMessage = "ادخل رقم موجب")]
        public string NewDepositAmount
        {
            get => _newDepositAmount;
            set => this.RaiseAndSetIfChanged(ref _newDepositAmount, value);
        }
        [PositiveIntRange(1,1_0_000_000_000_000,ErrorMessage = "ادخل رقم موجب")]
        public string NewEntredCheckNumber
        {
            get => _newEntredCheckNumber;
            set => this.RaiseAndSetIfChanged(ref _newEntredCheckNumber, value);
        }
        // Public properties
        public bool IsFullPaymentMethodChecked
        {
            get => _isFullPaymentMethodChecked;
            set => this.RaiseAndSetIfChanged(ref _isFullPaymentMethodChecked, value);
        }

        public bool IsCreditPaymentMethodChecked
        {
            get => _isCreditPaymentMethodChecked;
            set => this.RaiseAndSetIfChanged(ref _isCreditPaymentMethodChecked, value);
        }

        public bool IsTpePaymentMethodChecked
        {
            get => _isTpePaymentMethodChecked;
            set => this.RaiseAndSetIfChanged(ref _isTpePaymentMethodChecked, value);
        }

        public ePaymentMode PaymentModeToConvert;
     
        private ClientOrCompanySaleInfo clientOrCompanySalesInfo;
        public ReactiveCommand<Unit, Unit> ConvertPaymentMethodCommand { get; }

        public Interaction<string,bool> ConfirmPaymentMethodToConvert { get; }
        public Interaction<string,Unit> ShowIfOperationSuccedeOrFaildDialog { get; }

        private ChequeInfo _newChequeInfoLoadedByUser = null;
        public ClientsPaymentPageViewModel(ClientOrCompanySaleInfo clientOrCompanySalesInfo,ePaymentMode PaymentModeToConvert) :base(clientOrCompanySalesInfo.SaleID)
        {
            this.clientOrCompanySalesInfo = clientOrCompanySalesInfo;
            this.PaymentModeToConvert = PaymentModeToConvert;
            ConvertPaymentMethodCommand = ReactiveCommand.Create(ConvertThePaymentMethod_To_SelectedPaymentMethod, ValidateSelectedPaymentMethod());
            
            ConfirmPaymentMethodToConvert = new Interaction<string, bool>();
            ShowIfOperationSuccedeOrFaildDialog = new Interaction<string, Unit>();


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
            IsDepositPaymentMethodVisible = false;
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
            DisableAllPaymentMethods();
            IsDepositPaymentMethodVisible = true;
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

        private async void ConvertThePaymentMethodTo_Cash() {

           bool  isUserHasConfirmedTheOperation = await ConfirmPaymentMethodToConvert.Handle("هل تريد حقا تسجيل العملية ");

           if (isUserHasConfirmedTheOperation) { 
               
              bool operationHasSucceded = AccessToClassLibraryBackendProject.ExecuteProcessPayment
                                         (depositAmount: null, saleId :clientOrCompanySalesInfo.SaleID,selectedPaymentMethod: "cash", checkAmount: null, checkNumber: null, checkDate:null);
           
               if (operationHasSucceded) await ShowIfOperationSuccedeOrFaildDialog.Handle("لقد تمت العملية بنجاح");
           
               else await ShowIfOperationSuccedeOrFaildDialog.Handle("لقد حصل خطأ ما");
           }
       
        }


        private async void ConvertThePaymentMethodTo_Check()
        {
            bool isUserHasConfirmedTheOperation = await ConfirmPaymentMethodToConvert.Handle("هل تريد حقا تسجيل العملية");

            if (isUserHasConfirmedTheOperation)
            {
                bool isOperationSucceeded = AccessToClassLibraryBackendProject.ExecuteProcessPayment(
                    depositAmount: null,
                    saleId: clientOrCompanySalesInfo.SaleID,
                    selectedPaymentMethod: "check",
                    checkAmount: _newChequeInfoLoadedByUser.Amount,
                    checkNumber: _newChequeInfoLoadedByUser.ChequeNumber,
                    checkDate: _newChequeInfoLoadedByUser.ChequeDate
                );

                if (isOperationSucceeded)
                    await ShowIfOperationSuccedeOrFaildDialog.Handle("لقد تمت العملية بنجاح");
                else
                    await ShowIfOperationSuccedeOrFaildDialog.Handle("لقد حصل خطأ ما");
            }
        }

        private async void ConvertThePaymentMethodTo_Credit()
        {
            bool isUserHasConfirmedTheOperation = await ConfirmPaymentMethodToConvert.Handle("هل تريد حقا تسجيل العملية");

            if (isUserHasConfirmedTheOperation)
            {
                bool isOperationSucceeded = AccessToClassLibraryBackendProject.ExecuteProcessPayment(
                    depositAmount: null,
                    saleId: clientOrCompanySalesInfo.SaleID,
                    selectedPaymentMethod: "Credit",
                    checkAmount: null,
                    checkNumber: null,
                    checkDate: null
                );

                if (isOperationSucceeded)
                    await ShowIfOperationSuccedeOrFaildDialog.Handle("لقد تمت العملية بنجاح");
                else
                    await ShowIfOperationSuccedeOrFaildDialog.Handle("لقد حصل خطأ ما");
            }
        }

        private async void ConvertThePaymentMethodTo_Tpe()
        {
            bool isUserHasConfirmedTheOperation = await ConfirmPaymentMethodToConvert.Handle("هل تريد حقا تسجيل العملية");

            if (isUserHasConfirmedTheOperation)
            {
                bool isOperationSucceeded = AccessToClassLibraryBackendProject.ExecuteProcessPayment(
                    depositAmount: null,
                    saleId: clientOrCompanySalesInfo.SaleID,
                    selectedPaymentMethod: "Tpe",
                    checkAmount: null,
                    checkNumber: null,
                    checkDate: null
                );

                if (isOperationSucceeded)
                    await ShowIfOperationSuccedeOrFaildDialog.Handle("لقد تمت العملية بنجاح");
                else
                    await ShowIfOperationSuccedeOrFaildDialog.Handle("لقد حصل خطأ ما");
            }
        }

        private async void ConvertThePaymentMethodTo_Deposit()
        {
            bool isUserHasConfirmedTheOperation = await ConfirmPaymentMethodToConvert.Handle("هل تريد حقا تسجيل العملية");

            if (isUserHasConfirmedTheOperation)
            {
                decimal depositAmount = decimal.Parse(NewDepositAmount); // Assuming you have a way to load this information

                bool isOperationSucceeded = AccessToClassLibraryBackendProject.ExecuteProcessPayment(
                    depositAmount: depositAmount,
                    saleId: clientOrCompanySalesInfo.SaleID,
                    selectedPaymentMethod: "Deposit",
                    checkAmount: null,
                    checkNumber: null,
                    checkDate: null
                );

                if (isOperationSucceeded)
                    await ShowIfOperationSuccedeOrFaildDialog.Handle("لقد تمت العملية بنجاح");
                else
                    await ShowIfOperationSuccedeOrFaildDialog.Handle("لقد حصل خطأ ما");
            }
        }

     bool AreFull_PaymentMethod_InfoToSubmitValid   =>  IsFullPaymentMethodChecked && _newChequeInfoLoadedByUser == null && NewDepositAmount == null;       
     bool AreDeposit_PaymentMethod_InfoToSubmitValid =>  NewDepositAmount != null && _newChequeInfoLoadedByUser == null  && UiAttributeChecker.AreThesesAttributesPropertiesValid(this,nameof(NewDepositAmount));
     bool AreCheck_PaymentMethod_InfoToSubmitValid   => NewEntredCheckNumber != null &&  NewDepositAmount == null &&
            _newChequeInfoLoadedByUser !=null && UiAttributeChecker.AreThesesAttributesPropertiesValid(this, nameof(NewEntredCheckNumber) );        
     bool AreCredit_PaymentMethod_InfoToSubmitValid  =>  IsCreditPaymentMethodChecked && _newChequeInfoLoadedByUser == null && NewDepositAmount == null;
     bool AreTpe_PaymentMethod_InfoToSubmitValid     =>  IsTpePaymentMethodChecked && _newChequeInfoLoadedByUser == null && NewDepositAmount == null;

        public IObservable<bool> ValidateFullPaymentMethod()
        {
            return this.WhenAnyValue( x => x.IsFullPaymentMethodChecked,(isFullPaymentChecked)  => isFullPaymentChecked && AreFull_PaymentMethod_InfoToSubmitValid);
        }

        public IObservable<bool> ValidateDepositPaymentMethod()
        {
            return this.WhenAnyValue(
                x => x.NewDepositAmount,
                (newDepositAmount) => !string.IsNullOrWhiteSpace(newDepositAmount) && AreDeposit_PaymentMethod_InfoToSubmitValid
            );
        }

        public IObservable<bool> ValidateCheckPaymentMethod()
        {
            return this.WhenAnyValue (x => x.NewEntredCheckNumber,(NewEntredCheckNumber) =>!string.IsNullOrWhiteSpace(NewEntredCheckNumber) && AreCheck_PaymentMethod_InfoToSubmitValid);
        }

        public IObservable<bool> ValidateCreditPaymentMethod()
        {
            return this.WhenAnyValue(x => x.IsCreditPaymentMethodChecked,(isCreditChecked) => isCreditChecked && AreCredit_PaymentMethod_InfoToSubmitValid);
        }

        public IObservable<bool> ValidateTpePaymentMethod()
        {
            return this.WhenAnyValue( x => x.IsTpePaymentMethodChecked, (isTpeChecked) => isTpeChecked && AreTpe_PaymentMethod_InfoToSubmitValid );
        }

        public IObservable<bool> ValidateSelectedPaymentMethod()
        {
            switch (PaymentModeToConvert)
            {
                case ePaymentMode.ToCash: return ValidateFullPaymentMethod();


                case ePaymentMode.ToCheck: return ValidateCheckPaymentMethod();


                case ePaymentMode.ToTpe: return ValidateTpePaymentMethod();


                case ePaymentMode.ToCredit: return ValidateCreditPaymentMethod();


                case ePaymentMode.ToDeposit: return ValidateDepositPaymentMethod();


                default:
                    throw new InvalidOperationException($"Unsupported PaymentID: {clientOrCompanySalesInfo.PaymentID}");
            }
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
