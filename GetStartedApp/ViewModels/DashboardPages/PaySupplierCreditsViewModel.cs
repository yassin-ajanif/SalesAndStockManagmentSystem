using ReactiveUI;
using System.Reactive;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using GetStartedApp.Models.Objects;
using System.Reactive.Linq;
using GetStartedApp.ViewModels.ProductPages;

namespace GetStartedApp.ViewModels.DashboardPages
{
    public class PaySupplierCreditsViewModel : ProductsBoughtsViewModel
    {
         public ReactiveCommand<object, Unit> PaySupplierCreditAsCashCommand { get; set; }
        // public ReactiveCommand<object, Unit> PaySupplierCreditAsCheckCommand { get; set; }
        // public ReactiveCommand<object, Unit> PaySupplierCreditAsTpeCommand { get; set; }
        // public ReactiveCommand<object, Unit> ConvertSupplierCreditToCreditCommand { get; set; }
        // public ReactiveCommand<object, Unit> ConvertSupplierCreditToDepositCommand { get; set; }
        //
         public Interaction<BonReception, Unit> OpenPaySupplierCreditPage { get; }
        // public Interaction<SupplierSaleInfo, Unit> OpenPaySupplierCreditAsCheckPage { get; }
        // public Interaction<SupplierSaleInfo, Unit> OpenPaySupplierCreditAsTpePage { get; }
        // public Interaction<SupplierSaleInfo, Unit> OpenConvertSupplierCreditToCreditPage { get; }
        // public Interaction<SupplierSaleInfo, Unit> OpenConvertSupplierCreditToDepositPage { get; }
        //
         public PaySupplierCreditsViewModel(MainWindowViewModel mainWindowViewModel) : base(mainWindowViewModel)
        {
               OpenPaySupplierCreditPage = new Interaction<BonReception, Unit>();
        //     OpenPaySupplierCreditAsCheckPage = new Interaction<SupplierSaleInfo, Unit>();
        //     OpenPaySupplierCreditAsTpePage = new Interaction<SupplierSaleInfo, Unit>();
        //     OpenConvertSupplierCreditToCreditPage = new Interaction<SupplierSaleInfo, Unit>();
        //     OpenConvertSupplierCreditToDepositPage = new Interaction<SupplierSaleInfo, Unit>();
        //
        //     // Initialize commands by directly referencing the methods
               PaySupplierCreditAsCashCommand = ReactiveCommand.Create<object>(PaySupplierCreditAsCash);
        //     PaySupplierCreditAsCheckCommand = ReactiveCommand.Create<object>(PaySupplierCreditAsCheck);
        //     PaySupplierCreditAsTpeCommand = ReactiveCommand.Create<object>(PaySupplierCreditAsTpe);
        //     ConvertSupplierCreditToCreditCommand = ReactiveCommand.Create<object>(ConvertSupplierCreditToCredit);
        //     ConvertSupplierCreditToDepositCommand = ReactiveCommand.Create<object>(ConvertSupplierCreditToDeposit);
        }

        //
        // // Methods for each command, updated to return void
        private async void PaySupplierCreditAsCash(object selectedItem)
        {
            // Attempt to cast selectedItem to SupplierSaleInfo
            if (selectedItem is BonReception saleInfo)
            {
                await OpenPaySupplierCreditPage.Handle(saleInfo);
            }
        }
        //
        // private async void PaySupplierCreditAsCheck(object selectedItem)
        // {
        //     // Attempt to cast selectedItem to SupplierSaleInfo
        //     if (selectedItem is SupplierSaleInfo saleInfo)
        //     {
        //         await OpenPaySupplierCreditAsCheckPage.Handle(saleInfo);
        //     }
        // }
        //
        // private async void PaySupplierCreditAsTpe(object selectedItem)
        // {
        //     // Attempt to cast selectedItem to SupplierSaleInfo
        //     if (selectedItem is SupplierSaleInfo saleInfo)
        //     {
        //         await OpenPaySupplierCreditAsTpePage.Handle(saleInfo);
        //     }
        // }
        //
        // private async void ConvertSupplierCreditToCredit(object selectedItem)
        // {
        //     // Attempt to cast selectedItem to SupplierSaleInfo
        //     if (selectedItem is SupplierSaleInfo saleInfo)
        //     {
        //         await OpenConvertSupplierCreditToCreditPage.Handle(saleInfo);
        //     }
        // }
        //
        // private async void ConvertSupplierCreditToDeposit(object selectedItem)
        // {
        //     // Attempt to cast selectedItem to SupplierSaleInfo
        //     if (selectedItem is SupplierSaleInfo saleInfo)
        //     {
        //         await OpenConvertSupplierCreditToDepositPage.Handle(saleInfo);
        //     }
        // }
    }
}
