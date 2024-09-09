using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using ReactiveUI;
using System.Threading.Tasks;
using System;
using System.Reactive;
using Avalonia.ReactiveUI;
using GetStartedApp.ViewModels.DashboardPages;

namespace GetStartedApp.Views.ClientsPages;

public partial class AddNewClientView : UserControl
{
    public AddNewClientView()
    {
        InitializeComponent();
    }

    private void RegisterShowDialogCategoriesEvents()
    {


    //    this.WhenActivated(action =>
    //    {
    //        action(ViewModel!.Show.RegisterHandler(showDialogWhenUserAddNewClient));
    //
    //    });


    }

  //  private async Task showDialogWhenUserAddNewClient(InteractionContext<string, Unit> interaction)
  //  {
  //      var GetParentOfProductListView = this.GetVisualRoot() as Window;
  //      GetParentOfProductListView.Title = "اضافة تصنيف جديد";
  //      string messageToShow = interaction.Input;
  //
  //      if (GetParentOfProductListView == null)
  //      {
  //          throw new InvalidOperationException("Cannot show dialog because this control is not contained within a Window.");
  //      }
  //
  //      bool MessageBoxBtnsAreVisibleIf = (messageToShow == "هل تريد حقا حدف المنتج");
  //
  //      var DeleteMessageBox = new ShowMessageBoxContainer(messageToShow, MessageBoxBtnsAreVisibleIf);
  //
  //
  //      await DeleteMessageBox.ShowDialog<Unit>(GetParentOfProductListView);
  //
  //      interaction.SetOutput(Unit.Default);
  //
  //      // close the windows that contain the AddCategoryProductView
  //      GetParentOfProductListView.Close();
  //  }

}