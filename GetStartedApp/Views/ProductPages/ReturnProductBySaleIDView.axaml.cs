using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using Avalonia.Media;
using System.Xml.Schema;
using System;
using System.Reactive;
using GetStartedApp.Models.Objects;
using Avalonia.ReactiveUI;
using GetStartedApp.ViewModels.ProductPages;
using ReactiveUI;
using Avalonia.VisualTree;
using System.Threading.Tasks;

namespace GetStartedApp.Views.ProductPages
{
    public partial class ReturnProductBySaleIDView : ReactiveUserControl<ReturnProductBySaleIDViewModel>
    {
      
        public ReturnProductBySaleIDView()
        {
             InitializeComponent();
            // Subscribe to the event when the control is initialized
            ProductsListGrid.LoadingRow += EnableOnlyReturnableProducts;
            RegisterShowMessageBoxDialogProductEvents();
        }

        private void RegisterShowMessageBoxDialogProductEvents()
        {
            this.WhenActivated(action => {

                action(ViewModel!.ShowMessageBoxDialog.RegisterHandler(ShowMessageBox));
              
            }
            );
        }


        private async Task ShowMessageBox(InteractionContext<string, Unit> context)
        {


            var window = this.GetVisualRoot() as Window;
            var dialog = new ShowMessageBoxContainer(context.Input);

            await dialog.ShowDialog(window);

            //  window.Close();          
            context.SetOutput(Unit.Default);

            window.Close();

        }
        public void EnableOnlyReturnableProducts(object sender, DataGridRowEventArgs e)
        {
            // Access the DataContext of the current row
            var rowData = e.Row.DataContext as ReturnedProduct; // Assuming ReturnedProduct is your model class

            if (rowData != null)
            {
                e.Row.IsEnabled = rowData.IsProductReturnable;
            }
        }



        // Destructor (optional)
        ~ReturnProductBySaleIDView()
        {
            ProductsListGrid.LoadingRow -= EnableOnlyReturnableProducts;
        }
    }
}