using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using GetStartedApp.ViewModels;
using ReactiveUI;
using System.Threading.Tasks;
using System;
using System.Reactive;
using Avalonia.ReactiveUI;
using GetStartedApp.ViewModels.DashboardPages;
using GetStartedApp.Views.ProductPages;
using GetStartedApp.ViewModels.ProductPages;


namespace GetStartedApp.Views.DashboardPages;

public partial class BonLivraisonsView : ReactiveUserControl<BonLivraisonsViewModel> 
{
    public BonLivraisonsView()
    {
        InitializeComponent();

        RegisterShowDialogProductEvents();
    }


    private void RegisterShowDialogProductEvents()
    {


        this.WhenActivated(action =>
        {

            action(ViewModel!.OpenProductsListToReturn.RegisterHandler(ShowProductListToReturnByID));

        });

    }

    private async Task ShowProductListToReturnByID(InteractionContext<int, Unit> interaction)
      {

            var dialog = new DialogContainerView();
            dialog.Title = "ارجاع المنتجات";


            dialog.Content = new ReturnProductBySaleIDView()
            {
                DataContext = new ReturnProductBySaleIDViewModel(interaction.Input)
            };
          
            var window = this.GetVisualRoot() as Window;
            if (window == null)
            {
                throw new InvalidOperationException("Cannot show dialog because this control is not contained within a Window.");
            }
          
            await dialog.ShowDialog<Unit>(window);
          
            interaction.SetOutput(Unit.Default);


    }
}