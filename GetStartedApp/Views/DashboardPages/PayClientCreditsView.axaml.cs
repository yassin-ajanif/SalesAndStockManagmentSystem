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

        });

    }
    private async Task ShowProductListToReturnByID(InteractionContext<int, Unit> interaction)
    {

        var dialog = new DialogContainerView();
        dialog.Title = "تسديد المبيعة نقدا";


        dialog.Content = new ClientsPaymentPageView()
        {
            DataContext = new ClientsPaymentPageViewModel(interaction.Input,dialog.Title)
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