using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using GetStartedApp.ViewModels.CategoryPages;
using GetStartedApp.Views.CategoryPages;
using ReactiveUI;
using System.Threading.Tasks;
using System;
using System.Reactive;
using Avalonia.ReactiveUI;
using GetStartedApp.ViewModels.DashboardPages;
using GetStartedApp.ViewModels.ClientsPages;
using Avalonia.Interactivity;


namespace GetStartedApp.Views.DashboardPages;

public partial class ClientsListView : ReactiveUserControl<ClientsListViewModel>
{
    public ClientsListView()
    {
         RegisterShowDialogProductEvents();
          InitializeComponent();
    }

  private void RegisterShowDialogProductEvents()
  {
 
 
      this.WhenActivated(action =>
      {
          action(ViewModel!.ShowAddNewClientDialog.RegisterHandler(ShowDialogOfAddNewClient));
          
      });
 
 
  }
 
 
  private async Task ShowDialogOfAddNewClient(InteractionContext<AddNewClientViewModel, Unit> interaction)
  {
      var dialog = new DialogContainerView();
 
      // the interaction is an instance of the viewmodel we want to bind to the view we want to display 
      dialog.Content = new ClientsPages.AddNewClientView() { DataContext = interaction.Input };
      dialog.Title = "اضافة زبون جديد";
      var window = this.GetVisualRoot() as Window;
      if (window == null)
      {
          throw new InvalidOperationException("Cannot show dialog because this control is not contained within a Window.");
      }
 
      var result = await dialog.ShowDialog<Unit>(window);
 
 
      interaction.SetOutput(Unit.Default);
  }

    private async void OnEditClientInfoClicked(object sender, RoutedEventArgs e)
    {


        var dialog = new DialogContainerView();

        // var selectedCategory = ListBox.SelectedItemProperty;

        // the interaction is an instance of the viewmodel we want to bind to the view we want to display 
        dialog.Content = new ClientsPages.AddNewClientView() { DataContext = new EditClientViewModel(ViewModel!) };
        dialog.Title = "تعديل معلومات الزبون";

        var window = this.GetVisualRoot() as Window;
        if (window == null)
        {
            throw new InvalidOperationException("Cannot show dialog because this control is not contained within a Window.");
        }

        var result = await dialog.ShowDialog<Unit>(window);
    }


   private async void OnDeleteClientsClicked(object sender, RoutedEventArgs e)
   {
   
       // to show a message box you need to have a refrence of hte parent window
   
       var GetParentOfProductListView = this.GetVisualRoot() as Window;
       string messageToShow = " هل تريد حقا حذف هذا الزبون ";
   
   
       if (GetParentOfProductListView == null)
       {
           throw new InvalidOperationException("Cannot show dialog because this control is not contained within a Window.");
       }
   
       bool MessageBoxBtnsAreVisible = true;
   
       var DeleteMessageBox = new ShowMessageBoxContainer(messageToShow, MessageBoxBtnsAreVisible);
   
       var DoYouWantToDeleteClient = await DeleteMessageBox.ShowDialog<bool>(GetParentOfProductListView);
   
       // we use this variable to check if the deleteion operation is succed 
       bool deletionOperationIsScceded = false;
   
       if (DoYouWantToDeleteClient) { deletionOperationIsScceded = ViewModel!.DeleteClient(); }
   
       else return;
   
       if (deletionOperationIsScceded)
       {
   
           MessageBoxBtnsAreVisible = false;
   
           DeleteMessageBox = new ShowMessageBoxContainer(" لقد تمت العملية بنجاح ", MessageBoxBtnsAreVisible);

            DoYouWantToDeleteClient = await DeleteMessageBox.ShowDialog<bool>(GetParentOfProductListView);
       }
   
       else
       {
   
           MessageBoxBtnsAreVisible = false;
   
           DeleteMessageBox = new ShowMessageBoxContainer(" لا يمكنك حذف زبون مسجل في قائمة المبيعات ", MessageBoxBtnsAreVisible);

            DoYouWantToDeleteClient = await DeleteMessageBox.ShowDialog<bool>(GetParentOfProductListView);
       }
   }

}