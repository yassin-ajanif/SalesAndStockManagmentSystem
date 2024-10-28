using ReactiveUI;
using System.Reactive;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using GetStartedApp.Models.Objects;
using System.Reactive.Linq;

namespace GetStartedApp.ViewModels.DashboardPages
{
    public class PayClientCreditsViewModel : BonLivraisonsViewModel
    {
        public ReactiveCommand<object, Unit> PayClientCreditAsCashCommand { get; set; }
        public ReactiveCommand<object, Unit> PayClientCreditAsCheckCommand { get; set; }
        public ReactiveCommand<object, Unit> PayClientCreditAsTpeCommand { get; set; }
        public ReactiveCommand<object, Unit> ConvertClientCreditToCreditCommand { get; set; }
       // public ReactiveCommand<object, Unit> ConvertClientCreditToDepositCommand { get; set; }

        public Interaction<ClientOrCompanySaleInfo, Unit> OpenPayClientCreditPage { get; }
        public Interaction<ClientOrCompanySaleInfo, Unit> OpenPayClientCreditAsCheckPage { get; }
        public Interaction<ClientOrCompanySaleInfo, Unit> OpenPayClientCreditAsTpePage { get; }
        public Interaction<ClientOrCompanySaleInfo, Unit> OpenConvertClientCreditToCreditPage { get; }
        public Interaction<ClientOrCompanySaleInfo, Unit> OpenConvertClientCreditToDepositPage { get; }

        public PayClientCreditsViewModel(MainWindowViewModel mainWindowViewModel) : base(mainWindowViewModel)
        {
            OpenPayClientCreditPage = new Interaction<ClientOrCompanySaleInfo, Unit>();
            OpenPayClientCreditAsCheckPage = new Interaction<ClientOrCompanySaleInfo, Unit>();
            OpenPayClientCreditAsTpePage = new Interaction<ClientOrCompanySaleInfo, Unit>();
            OpenConvertClientCreditToCreditPage = new Interaction<ClientOrCompanySaleInfo, Unit>();
      //      OpenConvertClientCreditToDepositPage = new Interaction<ClientOrCompanySaleInfo, Unit>();
            
            // Initialize commands by directly referencing the methods
            PayClientCreditAsCashCommand = ReactiveCommand.Create<object>(PayClientCreditAsCash);
            PayClientCreditAsCheckCommand = ReactiveCommand.Create<object>(PayClientCreditAsCheck);
            PayClientCreditAsTpeCommand = ReactiveCommand.Create<object>(PayClientCreditAsTpe);
            ConvertClientCreditToCreditCommand = ReactiveCommand.Create<object>(ConvertClientCreditToCredit);
        //    ConvertClientCreditToDepositCommand = ReactiveCommand.Create<object>(ConvertClientCreditToCredit);


        }

        // Methods for each command, updated to return void
        private async void PayClientCreditAsCash(object selectedItem)
        {
            // Attempt to cast selectedItem to ClientOrCompanySaleInfo
            if (selectedItem is ClientOrCompanySaleInfo saleInfo)
            {
            
                await OpenPayClientCreditPage.Handle(saleInfo);
            }
        
    }

        private async void PayClientCreditAsCheck(object selectedItem)
        {
            // Attempt to cast selectedItem to ClientOrCompanySaleInfo
            if (selectedItem is ClientOrCompanySaleInfo saleInfo)
            {

                await OpenPayClientCreditAsCheckPage.Handle(saleInfo);
            }
        }

        private async void PayClientCreditAsTpe(object selectedItem)
        {
            // Attempt to cast selectedItem to ClientOrCompanySaleInfo
            if (selectedItem is ClientOrCompanySaleInfo saleInfo)
            {
                await OpenPayClientCreditAsTpePage.Handle(saleInfo);
            }
        }

        private async void ConvertClientCreditToCredit(object selectedItem)
        {
            // Attempt to cast selectedItem to ClientOrCompanySaleInfo
            if (selectedItem is ClientOrCompanySaleInfo saleInfo)
            {

                await OpenConvertClientCreditToCreditPage.Handle(saleInfo);
            }
        }

      //  private async void ConvertClientCreditToDeposit(object selectedItem)
     //  {
     //      // Attempt to cast selectedItem to ClientOrCompanySaleInfo
     //      if (selectedItem is ClientOrCompanySaleInfo saleInfo)
     //      {
     //
     //          await OpenConvertClientCreditToDepositPage.Handle(saleInfo);
     //      }
     //  }


    }
}
