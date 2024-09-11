using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using ReactiveUI;
using System.Threading.Tasks;
using System;
using Avalonia.ReactiveUI;
using GetStartedApp.ViewModels.SupplierPages;
using System.Reactive;

namespace GetStartedApp.Views.SupplierPages;

public partial class AddNewSupplierView : ReactiveUserControl<AddNewSupplierViewModel>
{
    public AddNewSupplierView()
    {
        InitializeComponent();
        RegisterShowDialogSuppliersEvents();
    }

    private void RegisterShowDialogSuppliersEvents()
    {
        this.WhenActivated(action =>
        {
            action(ViewModel!.ShowDialogOfAddNewSupplierResponseMessage.RegisterHandler(showDialogWhenUserAddNewSupplier));
        });
    }

    private async Task showDialogWhenUserAddNewSupplier(InteractionContext<string, Unit> interaction)
    {
        var GetParentOfSupplierListView = this.GetVisualRoot() as Window;
        GetParentOfSupplierListView.Title = "إضافة مورد جديد"; // Title in Arabic for "Add New Supplier"
        string messageToShow = interaction.Input;

        if (GetParentOfSupplierListView == null)
        {
            throw new InvalidOperationException("Cannot show dialog because this control is not contained within a Window.");
        }

        bool MessageBoxBtnsAreVisibleIf = (messageToShow == "هل تريد حقا حدف المورد؟"); // Arabic for "Do you really want to delete the supplier?"

        var DeleteMessageBox = new ShowMessageBoxContainer(messageToShow, MessageBoxBtnsAreVisibleIf);

        await DeleteMessageBox.ShowDialog<Unit>(GetParentOfSupplierListView);

        interaction.SetOutput(Unit.Default);

        // Close the window that contains the AddNewSupplierView
        GetParentOfSupplierListView.Close();
    }
}