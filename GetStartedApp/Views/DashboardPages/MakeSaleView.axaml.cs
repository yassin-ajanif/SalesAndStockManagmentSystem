
using Avalonia.Controls;
using Avalonia.Interactivity;

using GetStartedApp.ViewModels.DashboardPages;

using System;

using Avalonia.ReactiveUI;
using Avalonia.Input;
using GetStartedApp.Models;
using Avalonia.VisualTree;
using ReactiveUI;
using System.Threading.Tasks;
using System.Reactive;



namespace GetStartedApp.Views.DashboardPages;

public partial class MakeSaleView : ReactiveUserControl<MakeSaleViewModel>
{
    public MakeSaleViewModel MakeSaleViewModelBoundTothisView { get; set; }
    public MakeSaleView()
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


        });


    }


    private void OnDeleteProductClicked(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;

        // Get the data context of the button (should be a ProductInfo object)
        var clickedItem = button?.DataContext as ProductsScannedInfo;

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


}