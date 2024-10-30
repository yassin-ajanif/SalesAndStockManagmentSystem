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

namespace GetStartedApp.Views.DashboardPages;

public partial class PayClientCreditsView : ReactiveUserControl<PayClientCreditsViewModel>
{
    public PayClientCreditsView()
    {
        InitializeComponent();
        RegisterShowDialogProductEvents();
    }

    private void RegisterShowDialogProductEvents()
    {
        this.WhenActivated(action =>
        {
            action(ViewModel!.OpenPayClientCreditPage.RegisterHandler(ShowProductListToReturnByID));
            action(ViewModel!.OpenPayClientCreditAsCheckPage.RegisterHandler(ShowCheckPaymentDialog));
            action(ViewModel!.OpenPayClientCreditAsTpePage.RegisterHandler(ShowTpePaymentDialog));
            action(ViewModel!.OpenConvertClientCreditToCreditPage.RegisterHandler(ShowConvertToCreditDialog)); 
            action(ViewModel!.OpenConvertClientCreditToDepositPage.RegisterHandler(ShowConvertToDepositDialog)); 
    

        });
    }

    private async Task ShowProductListToReturnByID(InteractionContext<ClientOrCompanySaleInfo, Unit> interaction)
    {
        var dialog = new DialogContainerView
        {
            Title = "تسديد المبيعة نقدا",
            Content = new ClientsPaymentPageView
            {
                DataContext = new ClientsPaymentPageViewModel(interaction.Input, ePaymentMode.ToCash)
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

    private async Task ShowCheckPaymentDialog(InteractionContext<ClientOrCompanySaleInfo, Unit> interaction)
    {
        var dialog = new DialogContainerView
        {
            Title = "تسديد المبيعة بواسطة شيك",
            Content = new ClientsPaymentPageView
            {
                DataContext = new ClientsPaymentPageViewModel(interaction.Input, ePaymentMode.ToCheck)
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

    private async Task ShowTpePaymentDialog(InteractionContext<ClientOrCompanySaleInfo, Unit> interaction)
    {
        var dialog = new DialogContainerView
        {
            Title = "تسديد المبيعة بواسطة TPE",
            Content = new ClientsPaymentPageView
            {
                DataContext = new ClientsPaymentPageViewModel(interaction.Input, ePaymentMode.ToTpe)
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

    private async Task ShowConvertToCreditDialog(InteractionContext<ClientOrCompanySaleInfo, Unit> interaction)
    {
        var dialog = new DialogContainerView
        {
            Title = "تحويل إلى دين",
            Content = new ClientsPaymentPageView
            {
                DataContext = new ClientsPaymentPageViewModel(interaction.Input, ePaymentMode.ToCredit)
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

    private async Task ShowConvertToDepositDialog(InteractionContext<ClientOrCompanySaleInfo, Unit> interaction)
    {
        var dialog = new DialogContainerView
        {
            Title = "تحويل الى تسبيق",
            Content = new ClientsPaymentPageView
            {
                DataContext = new ClientsPaymentPageViewModel(interaction.Input, ePaymentMode.ToDeposit)
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