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
        public string ChangingPaymentOperation { get; }
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

        private void SetInforamtionOfCheckPaymentMethod_AsPreviousPaymentMethod()
        {
            PreviousPaymentMethod = "شيك";
            PreviousPaymentMethodDetails = "رقم الشيك 123456";
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

                default:
                    throw new InvalidOperationException($"Unsupported PaymentID: {clientOrCompanySalesInfo.PaymentID}");
            }
        }


    }
}
