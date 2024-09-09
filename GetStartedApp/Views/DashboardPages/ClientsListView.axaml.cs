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


namespace GetStartedApp.Views.DashboardPages;

public partial class ClientsListView : ReactiveUserControl<ClientsListViewModel
    >
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
}