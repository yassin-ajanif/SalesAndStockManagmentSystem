using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using GetStartedApp.ViewModels;
using ReactiveUI;
using System.Threading.Tasks;
using System;
using Avalonia.ReactiveUI;
using GetStartedApp.ViewModels.DashboardPages;
using GetStartedApp.Models.Objects;

namespace GetStartedApp.Views.DashboardPages;

public partial class DevisView : ReactiveUserControl<DevisViewModel>
{
    public DevisView()
    {
        InitializeComponent();
        RegisterShowDialogSalesEvents();
    }

    private void RegisterShowDialogSalesEvents()
    {


        this.WhenActivated(action =>
        {
            action(ViewModel!.ShowAddSaleDialogInteraction.RegisterHandler(ShowDialogOfAddSell));

            action(ViewModel!.ShowDeleteSaleDialogInteraction.RegisterHandler(ShowDialogOfDeleteSell));

            action(ViewModel!.ShowAddChequeInfoDialogInteraction.RegisterHandler(ShowDialogOfAddNewChequeInfo));



        });


    }


    private void OnDeleteProductClicked(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;

        // Get the data context of the button (should be a ProductInfo object)
        var clickedItem = button?.DataContext as ProductsScannedInfo_ToSale;

        int index = ViewModel.ProductsListScanned.IndexOf(clickedItem);

        ViewModel.ProductsListScanned.RemoveAt(index);
    }

    private async Task ShowDialogOfDeleteSell(InteractionContext<string, bool> interaction)
    {
        // to show a message box you need to have a refrence of hte parent window

        var GetParentOfMakeSaleView = this.GetVisualRoot() as Window;
        string messageToShow = interaction.Input;

        if (GetParentOfMakeSaleView == null)
        {
            throw new InvalidOperationException("Cannot show dialog because this control is not contained within a Window.");
        }

        bool MessageBoxBtnsAreVisible = true;

        var DeleteMessageBox = new ShowMessageBoxContainer(messageToShow, MessageBoxBtnsAreVisible);

        var DoYouWantToDeleteSell =
        await DeleteMessageBox.ShowDialog<bool>(GetParentOfMakeSaleView);

        interaction.SetOutput(DoYouWantToDeleteSell);

    }

    private async Task ShowDialogOfAddSell(InteractionContext<string, bool> interaction)
    {
        // to show a message box you need to have a refrence of hte parent window

        var GetParentOfMakeSaleView = this.GetVisualRoot() as Window;
        string messageToShow = interaction.Input;

        if (GetParentOfMakeSaleView == null)
        {
            throw new InvalidOperationException("Cannot show dialog because this control is not contained within a Window.");
        }


        bool MessageBoxBtnsAreVisibleif = interaction.Input == "هل تريد حقا تسجيل هذه المبيعة" || interaction.Input == "هل تريد ان تخسر في منتوج او اكتر";

        var DeleteMessageBox = new ShowMessageBoxContainer(messageToShow, MessageBoxBtnsAreVisibleif);

        var DoYouWantToDeleteSell =
        await DeleteMessageBox.ShowDialog<bool>(GetParentOfMakeSaleView);

        interaction.SetOutput(DoYouWantToDeleteSell);

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