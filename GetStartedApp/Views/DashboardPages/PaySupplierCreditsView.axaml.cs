using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using GetStartedApp.ViewModels.DashboardPages;
using Avalonia.VisualTree;
using GetStartedApp.ViewModels.DashboardPages;
using GetStartedApp.ViewModels.ProductPages;
using GetStartedApp.Views.ProductPages;
using ReactiveUI;
using GetStartedApp.Models.Enums;
using GetStartedApp.Models.Objects;
using GetStartedApp.ViewModels.ClientsPages;
using GetStartedApp.Views.ClientsPages;
using System.Threading.Tasks;
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using GetStartedApp.ViewModels.DashboardPages;
using GetStartedApp.ViewModels.ProductPages;
using GetStartedApp.Views.ProductPages;
using ReactiveUI;
using System.Threading.Tasks;
using System;
using Avalonia.ReactiveUI;
using System.Reactive;
using GetStartedApp.ViewModels.ClientsPages;
using GetStartedApp.Views.ClientsPages;
using GetStartedApp.Models.Objects;
using GetStartedApp.Models.Enums;
using GetStartedApp.ViewModels.SupplierPages;



namespace GetStartedApp.Views.DashboardPages;

public partial class PaySupplierCreditsView : ReactiveUserControl<PaySupplierCreditsViewModel>
{
    public PaySupplierCreditsView()
    {
        InitializeComponent();
        RegisterShowDialogProductEvents();
    }

    private void RegisterShowDialogProductEvents()
    {
        this.WhenActivated(action =>
        {
            action(ViewModel!.OpenPaySupplierCreditPage.RegisterHandler(ShowProductListToReturnByID));
          //  action(ViewModel!.OpenPayClientCreditAsCheckPage.RegisterHandler(ShowCheckPaymentDialog));
          //  action(ViewModel!.OpenPayClientCreditAsTpePage.RegisterHandler(ShowTpePaymentDialog));
         //   action(ViewModel!.OpenConvertClientCreditToCreditPage.RegisterHandler(ShowConvertToCreditDialog));
         //   action(ViewModel!.OpenConvertClientCreditToDepositPage.RegisterHandler(ShowConvertToDepositDialog));


        });
    }

    private async Task ShowProductListToReturnByID(InteractionContext<BonReception, Unit> interaction)
    {
        var dialog = new DialogContainerView
        {
            Title = "تسديد المبيعة نقدا",
            Content = new ClientsPaymentPageView
            {
                DataContext = new SupplierPaymentPageViewModel(interaction.Input)
            }
        };

        var window = this.GetVisualRoot() as Window;
        if (window == null)
        {
            throw new InvalidOperationException("Cannot show dialog because this control is not contained within a Window.");
        }

        await dialog.ShowDialog<Unit>(window);
        interaction.SetOutput(Unit.Default);
    }




}