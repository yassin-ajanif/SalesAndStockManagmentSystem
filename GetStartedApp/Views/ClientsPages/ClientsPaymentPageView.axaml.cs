using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using GetStartedApp.Models.Objects;
using GetStartedApp.ViewModels.ProductPages;
using GetStartedApp.Views.DashboardPages;
using GetStartedApp.Views.ProductPages;

namespace GetStartedApp.Views.ClientsPages;

public partial class ClientsPaymentPageView : UserControl
{
    public ClientsPaymentPageView()
    {
        InitializeComponent();
        ProductsListGrid.LoadingRow += EnableOnlyReturnableProducts;
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

    private void PayFullyRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        // Disable the partial payment section when "Pay Fully" is selected
        PartialPaymentTemplate.IsEnabled = false;
    }

    private void PayPartiallyRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        // Enable the partial payment section when "Pay Partially" is selected
        PartialPaymentTemplate.IsEnabled = true;
    }



}