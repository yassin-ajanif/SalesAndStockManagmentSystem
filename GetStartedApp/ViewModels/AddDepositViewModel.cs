using ReactiveUI;
using System.Reactive;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using GetStartedApp.Helpers.CustomUIErrorAttributes;
using GetStartedApp.Helpers;
using System.Reactive.Linq;


namespace GetStartedApp.ViewModels
{
    public class AddDepositViewModel : ViewModelBase
    {
        private string _totalSalesAmount;

        private string _depositAmount;
        [PositiveFloatRange(1, 100_000, ErrorMessage = "ادخل رقم موجب")]
        public string DepositAmount
        {
            get => _depositAmount;
            set => this.RaiseAndSetIfChanged(ref _depositAmount, value);
        }

        // Reactive command to check the deposit amount
        public ReactiveCommand<Unit, Unit> CheckDepositCommand { get; }
        public Interaction<Unit,Unit> AddDepositInteraction { get; }

        // Constructor to initialize the DepositAmount
        public AddDepositViewModel(string TotalSalesAmount)
        {
            _totalSalesAmount = TotalSalesAmount;

            AddDepositInteraction = new Interaction<Unit, Unit>();
            CheckDepositCommand = ReactiveCommand.Create(submitDeposit, IsDepositAmountValid());
        }

        private bool IsDepositAmountLessThanSalesAmount()
        {
            bool SaleAmountIsBiggerThanDeposit = decimal.Parse(_totalSalesAmount) > decimal.Parse(DepositAmount);

            // Format the total sales amount with currency
            string formattedSalesAmount = $"{_totalSalesAmount} DH";

            // Define the base error message
            string BaseErrorMessage = "التسبيق يجب أن يكون أصغر من ثمن المبيعة";
            string ErrorMessage = $"({formattedSalesAmount}) {BaseErrorMessage} ";

            if (!SaleAmountIsBiggerThanDeposit) ShowUiError(nameof(DepositAmount), ErrorMessage);
            else DeleteUiError(nameof(DepositAmount), ErrorMessage);

            return SaleAmountIsBiggerThanDeposit;
        }




        private bool DepositAmountIsValidNumber => UiAttributeChecker.AreThesesAttributesPropertiesValid(this, nameof(DepositAmount));

        public IObservable<bool> IsDepositAmountValid()
        {
           return this.WhenAnyValue( x => x.DepositAmount, (DepositAmount) => !string.IsNullOrWhiteSpace(DepositAmount) && DepositAmountIsValidNumber && IsDepositAmountLessThanSalesAmount() );

        }

        public async void submitDeposit()
        {
            await AddDepositInteraction.Handle(Unit.Default);
        }

       

    }
}

