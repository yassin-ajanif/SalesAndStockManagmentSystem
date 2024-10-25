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

        public Interaction<int, Unit> OpenPayClientCreditPage { get; }
        public Interaction<int, Unit> OpenPayClientCreditAsCheckPage { get; }
        public Interaction<int, Unit> OpenPayClientCreditAsTpePage { get; }
        public Interaction<int, Unit> OpenConvertClientCreditToCreditPage { get; }

        public PayClientCreditsViewModel(MainWindowViewModel mainWindowViewModel) : base(mainWindowViewModel)
        {
            OpenPayClientCreditPage = new Interaction<int, Unit>();
            OpenPayClientCreditAsCheckPage = new Interaction<int, Unit>();
            OpenPayClientCreditAsTpePage = new Interaction<int, Unit>();
            OpenConvertClientCreditToCreditPage = new Interaction<int, Unit>();
            // Initialize commands by directly referencing the methods
            PayClientCreditAsCashCommand = ReactiveCommand.Create<object>(PayClientCreditAsCash);
            PayClientCreditAsCheckCommand = ReactiveCommand.Create<object>(PayClientCreditAsCheck);
            PayClientCreditAsTpeCommand = ReactiveCommand.Create<object>(PayClientCreditAsTpe);
            ConvertClientCreditToCreditCommand = ReactiveCommand.Create<object>(ConvertClientCreditToCredit);


        }

        // Methods for each command, updated to return void
        private async void PayClientCreditAsCash(object selectedItem)
        {
            // Attempt to cast selectedItem to ClientOrCompanySaleInfo
            if (selectedItem is ClientOrCompanySaleInfo saleInfo)
            {
                // Access the properties of the saleInfo object
                int saleID = saleInfo.SaleID; // Ensure you access SaleID correctly

                await OpenPayClientCreditPage.Handle(saleID);
            }
        
    }

        private async void PayClientCreditAsCheck(object selectedItem)
        {
            // Attempt to cast selectedItem to ClientOrCompanySaleInfo
            if (selectedItem is ClientOrCompanySaleInfo saleInfo)
            {
                // Access the properties of the saleInfo object
                int saleID = saleInfo.SaleID; // Ensure you access SaleID correctly

                await OpenPayClientCreditAsCheckPage.Handle(saleID);
            }
        }

        private async void PayClientCreditAsTpe(object selectedItem)
        {
            // Attempt to cast selectedItem to ClientOrCompanySaleInfo
            if (selectedItem is ClientOrCompanySaleInfo saleInfo)
            {
                // Access the properties of the saleInfo object
                int saleID = saleInfo.SaleID; // Ensure you access SaleID correctly

                await OpenPayClientCreditAsTpePage.Handle(saleID);
            }
        }

        private async void ConvertClientCreditToCredit(object selectedItem)
        {
            // Attempt to cast selectedItem to ClientOrCompanySaleInfo
            if (selectedItem is ClientOrCompanySaleInfo saleInfo)
            {
                // Access the properties of the saleInfo object
                int saleID = saleInfo.SaleID; // Ensure you access SaleID correctly

                await OpenConvertClientCreditToCreditPage.Handle(saleID);
            }
        }


    }
}
