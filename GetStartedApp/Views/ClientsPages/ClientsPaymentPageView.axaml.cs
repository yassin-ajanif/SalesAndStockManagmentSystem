using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using GetStartedApp.Models.Objects;
using GetStartedApp.ViewModels;
using GetStartedApp.ViewModels.ClientsPages;
using GetStartedApp.ViewModels.ProductPages;
using GetStartedApp.Views.DashboardPages;
using GetStartedApp.Views.ProductPages;
using ReactiveUI;
using System;
using System.Reactive;
using System.Threading.Tasks;

namespace GetStartedApp.Views.ClientsPages;

public partial class ClientsPaymentPageView : ReactiveUserControl<ClientsPaymentPageViewModel>
{
    public ClientsPaymentPageView()
    {
        InitializeComponent();
        ProductsListGrid.LoadingRow += EnableOnlyReturnableProducts;
        RegisterShowDialogProductEvents();
    }

    public void EnableOnlyReturnableProducts(object sender, DataGridRowEventArgs e)
    {
        // Access the DataContext of the current row
        var rowData = e.Row.DataContext as ReturnedProduct; // Assuming ReturnedProduct is your model class

        if (rowData != null)
        {
            e.Row.IsEnabled = rowData.IsProductReturnable;
        }
    }

    private void RegisterShowDialogProductEvents()
    {
        this.WhenActivated(action =>
        {
            action(ViewModel!.ConfirmPaymentMethodToConvert.RegisterHandler(ShowMessageBoxForConversion_ToPaymentMethod_Confirmation)); 
            action(ViewModel!.ShowIfOperationSuccedeOrFaildDialog.RegisterHandler(ShowMessageBoxIfOperationHasSuccededOrFailed)); 
            action(ViewModel!.ShowAddChequeInfoDialogInteraction.RegisterHandler(ShowDialogOfAddNewChequeInfo)); 

        });
    }

    private async Task ShowMessageBoxForConversion_ToPaymentMethod_Confirmation(InteractionContext<string, bool> context)
    {
   
        var window = this.GetVisualRoot() as Window;
        bool validatonBtnsAreVisible = true;
        var dialog = new ShowMessageBoxContainer(context.Input, validatonBtnsAreVisible);

        bool userHasConfirmedByYes = await dialog.ShowDialog<bool>(window);

        context.SetOutput(userHasConfirmedByYes);

    }

    private async Task ShowMessageBoxIfOperationHasSuccededOrFailed(InteractionContext<string, Unit> context)
    {

        var window = this.GetVisualRoot() as Window;
        bool validatonBtnsAreVisible = false;
        var dialog = new ShowMessageBoxContainer(context.Input, validatonBtnsAreVisible);

        await dialog.ShowDialog<bool>(window);

        context.SetOutput(Unit.Default);

        window.Close();
    }


    private async Task ShowDialogOfAddNewChequeInfo(InteractionContext<AddNewChequeInfoViewModel, bool> interaction)
    {
        var dialog = new DialogContainerView();

        // the interaction is an instance of the viewmodel we want to bind to the view we want to display 
        dialog.Content = new AddNewChequeInfoView() { DataContext = interaction.Input };
        dialog.Title = "اضافة معلومات الشيك";
        var window = this.GetVisualRoot() as Window;
        if (window == null)
        {
            throw new InvalidOperationException("Cannot show dialog because this control is not contained within a Window.");
        }

        var userHasClosedTheAddNewWindow_Or_SubmitedTheChequeInfo = await dialog.ShowDialog<bool>(window);


        interaction.SetOutput(userHasClosedTheAddNewWindow_Or_SubmitedTheChequeInfo);
    }


}