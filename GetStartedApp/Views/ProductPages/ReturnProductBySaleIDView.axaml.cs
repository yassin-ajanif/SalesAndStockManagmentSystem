using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using Avalonia.Media;
using System.Xml.Schema;
using System;
using GetStartedApp.Models.Objects;

namespace GetStartedApp.Views.ProductPages
{
    public partial class ReturnProductBySaleIDView : UserControl
    {
      
        public ReturnProductBySaleIDView()
        {
            InitializeComponent();
            // Subscribe to the event when the control is initialized
            ProductsListGrid.LoadingRow += EnableOnlyReturnableProducts;
        }

        // Method to handle the LoadingRow event
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