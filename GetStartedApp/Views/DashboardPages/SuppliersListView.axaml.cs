using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using GetStartedApp.ViewModels.DashboardPages;
using GetStartedApp.ViewModels.SupplierPages;
using GetStartedApp.ViewModels;
using ReactiveUI;
using System.Threading.Tasks;
using System;
using System.Reactive;
using Avalonia.VisualTree;


namespace GetStartedApp.Views.DashboardPages;

public partial class SuppliersListView : ReactiveUserControl<SuppliersListViewModel>
{
    public SuppliersListView()
    {
        RegisterShowDialogSupplierEvents();
        InitializeComponent();
    }

    private void RegisterShowDialogSupplierEvents()
    {
        this.WhenActivated(action =>
        {
            action(ViewModel!.ShowAddNewSupplierDialog.RegisterHandler(ShowDialogOfAddNewSupplier));
        });
    }

    private async Task ShowDialogOfAddNewSupplier(InteractionContext<AddNewSupplierViewModel, Unit> interaction)
    {
        var dialog = new DialogContainerView();

        // Bind the AddNewSupplierView with its ViewModel
        dialog.Content = new SupplierPages.AddNewSupplierView() { DataContext = interaction.Input };
        dialog.Title = "إضافة مورد جديد"; // "Add New Supplier" in Arabic
        var window = this.GetVisualRoot() as Window;
        if (window == null)
        {
            throw new InvalidOperationException("Cannot show dialog because this control is not contained within a Window.");
        }

        var result = await dialog.ShowDialog<Unit>(window);
        interaction.SetOutput(Unit.Default);
    }

    private async void OnEditSupplierInfoClicked(object sender, RoutedEventArgs e)
    {
        var dialog = new DialogContainerView();

        // Bind the EditSupplierViewModel to AddNewSupplierView
        dialog.Content = new SupplierPages.AddNewSupplierView()
        {

            DataContext = new EditSupplierViewModel(ViewModel!)
        };
        dialog.Title = "تعديل معلومات المورد"; // "Edit Supplier Info" in Arabic

        var window = this.GetVisualRoot() as Window;
        if (window == null)
        {
            throw new InvalidOperationException("Cannot show dialog because this control is not contained within a Window.");
        }

        var result = await dialog.ShowDialog<Unit>(window);
    }
    //
    private async void OnDeleteSuppliersClicked(object sender, RoutedEventArgs e)
    {
        var GetParentOfSupplierListView = this.GetVisualRoot() as Window;
        string messageToShow = "هل تريد حقا حذف هذا المورد؟"; // "Do you really want to delete this supplier?" in Arabic

        if (GetParentOfSupplierListView == null)
        {
            throw new InvalidOperationException("Cannot show dialog because this control is not contained within a Window.");
        }

        bool MessageBoxBtnsAreVisible = true;

        var DeleteMessageBox = new ShowMessageBoxContainer(messageToShow, MessageBoxBtnsAreVisible);

        var DoYouWantToDeleteSupplier = await DeleteMessageBox.ShowDialog<bool>(GetParentOfSupplierListView);

        // We use this variable to check if the deletion operation is successful
        bool deletionOperationSucceeded = false;

        if (DoYouWantToDeleteSupplier)
        {
            deletionOperationSucceeded = ViewModel!.DeleteSupplier();
        }
        else
        {
            return;
        }

        if (deletionOperationSucceeded)
        {
            MessageBoxBtnsAreVisible = false;
            DeleteMessageBox = new ShowMessageBoxContainer("تمت العملية بنجاح", MessageBoxBtnsAreVisible); // "Operation succeeded" in Arabic
            await DeleteMessageBox.ShowDialog<bool>(GetParentOfSupplierListView);
        }
        else
        {
            MessageBoxBtnsAreVisible = false;
            DeleteMessageBox = new ShowMessageBoxContainer("لا يمكنك حذف مورد مسجل في قائمة المبيعات", MessageBoxBtnsAreVisible); // "Cannot delete supplier registered in sales list" in Arabic
            await DeleteMessageBox.ShowDialog<bool>(GetParentOfSupplierListView);
        }
    }
}