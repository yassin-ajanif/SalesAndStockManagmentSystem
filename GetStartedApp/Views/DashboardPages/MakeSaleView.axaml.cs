
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
using System.Linq;
using GetStartedApp.ViewModels;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Data;



namespace GetStartedApp.Views.DashboardPages;

public partial class MakeSaleView : ReactiveUserControl<MakeSaleViewModel>
{
  
    
public MakeSaleView()
    {
        InitializeComponent();

        RegisterShowDialogSalesEvents();

        LoadClientListAt_ClientSearchBar_WhenViewIsCompleted();
    }

    private void LoadClientListAt_ClientSearchBar()
    {
        // Retrieve and sort the clients list
        var clientsList = ViewModel.GetClientsListFromDb().OrderBy(x => x).ToList();
        clients.ItemsSource = clientsList;

        // Define the search term you are looking for
        string searchTerm = "Normal"; // Replace with your actual search term

        // Find the client that matches the search term
        var DefaultClientIsUnkonwClient = clientsList.FirstOrDefault(client => client.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0);

        clients.SelectedItem = DefaultClientIsUnkonwClient;
    }

   

    private void LoadClientListAt_ClientSearchBar_WhenViewIsCompleted()
    {
        this.WhenActivated(_=>LoadClientListAt_ClientSearchBar());
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