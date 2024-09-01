using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using GetStartedApp.ViewModels.ProductPages;
using System;
using Avalonia.Interactivity;
using System.Diagnostics;
using Avalonia.ReactiveUI;
using System.Reactive;
using GetStartedApp.ViewModels.DashboardPages;
using ReactiveUI;
using GetStartedApp.ViewModels;
using GetStartedApp.Views.ProductPages;
using GetStartedApp.Views;
using System.Threading.Tasks;
using GetStartedApp.ViewModels.CategoryPages;
using Microsoft.VisualBasic;




namespace GetStartedApp.Views.CategoryPages;

public partial class AddNewCategoryView : ReactiveUserControl<AddNewCategoryViewModel>
{
    public AddNewCategoryView()
    {
        InitializeComponent();

        RegisterShowDialogCategoriesEvents();
    }


    private void RegisterShowDialogCategoriesEvents()
    {


        this.WhenActivated(action =>
        {
            action(ViewModel!.ShowDialogOfAddNewCategoryResponseMessage.RegisterHandler(showDialogWhenUserAddNewCategoryProduct));

        });


    }

    private async Task showDialogWhenUserAddNewCategoryProduct(InteractionContext<string,Unit> interaction)
    {
        var GetParentOfProductListView = this.GetVisualRoot() as Window;
        GetParentOfProductListView.Title = "اضافة تصنيف جديد";
        string messageToShow = interaction.Input;

        if (GetParentOfProductListView == null)
        {
            throw new InvalidOperationException("Cannot show dialog because this control is not contained within a Window.");
        }

        bool MessageBoxBtnsAreVisibleIf = (messageToShow == "هل تريد حقا حدف المنتج");

        var DeleteMessageBox = new ShowMessageBoxContainer(messageToShow, MessageBoxBtnsAreVisibleIf);

        
        await DeleteMessageBox.ShowDialog<Unit>(GetParentOfProductListView);

        interaction.SetOutput(Unit.Default);

        // close the windows that contain the AddCategoryProductView
        GetParentOfProductListView.Close();
    }




}