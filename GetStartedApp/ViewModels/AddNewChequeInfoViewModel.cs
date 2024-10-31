using GetStartedApp.Models.Objects;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;



namespace GetStartedApp.ViewModels
{
    public class AddNewChequeInfoViewModel : ViewModelBase
    {
        // Property for the cheque number
        private string _chequeNumber;
        public string ChequeNumber
        {
            get => _chequeNumber;
            set => this.RaiseAndSetIfChanged(ref _chequeNumber, value);
        }


        // Property for the cheque amount
        private string _amount;
        public string Amount
        {
            get => _amount;
            set => this.RaiseAndSetIfChanged(ref _amount, value);
        }

        // Property for the cheque date
        private DateTimeOffset _chequeDate = DateTimeOffset.Now;
        public DateTimeOffset ChequeDate
        {
            get => _chequeDate;
            set => this.RaiseAndSetIfChanged(ref _chequeDate, value);
        }

        public ReactiveCommand<Unit, Unit> AddCheckInfo { get; }

        public Interaction<Unit, Unit> addNewChequeInfoInteraction{get;set;}

        private ChequeInfo _chequeInfoToFillByUser ;
        public ChequeInfo ChequeInfoToFillByUser
        {
            get => _chequeInfoToFillByUser;
            set => this.RaiseAndSetIfChanged(ref _chequeInfoToFillByUser, value);
        }

        // this constructor is made to load the ui or viewmodel values where the chequeinfo values are null 
        public AddNewChequeInfoViewModel(ref ChequeInfo chequeInfoToFillByUser)
        {
            _chequeInfoToFillByUser = chequeInfoToFillByUser ;
            addNewChequeInfoInteraction = new Interaction<Unit, Unit>();
            AddCheckInfo = ReactiveCommand.Create(AddChequeInfo);
        }
   
     
        public void LoadChequeInfoEntredByUser()
        {   
            ChequeInfoToFillByUser.ChequeNumber = ChequeNumber;
            ChequeInfoToFillByUser.Amount = Decimal.Parse(Amount);
            ChequeInfoToFillByUser.ChequeDate = ChequeDate.DateTime;
        }
        private async void AddChequeInfo()
        {
            LoadChequeInfoEntredByUser();
           await addNewChequeInfoInteraction.Handle(Unit.Default);
        }

    }

}
