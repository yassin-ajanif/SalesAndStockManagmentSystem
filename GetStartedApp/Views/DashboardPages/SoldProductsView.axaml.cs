
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using GetStartedApp.Models;
using GetStartedApp.Models.Objects;
using GetStartedApp.ViewModels.DashboardPages;
using System.Diagnostics;
using System.Linq;


namespace GetStartedApp.Views.DashboardPages;

public partial class SoldProductsView : ReactiveUserControl<SoldProductsViewModel>
{
    private int lastHoveredProductId=-1;
    public SoldProductsView()
    {
        InitializeComponent();
    }

    private void OnUserHoverTheProductRow(object sender, PointerEventArgs e)
    {
        if (DataContext is not SoldProductsViewModel myMainWindowViewModel) return;

        var dataGrid = (DataGrid)sender;

        // Get the hovered row based on the mouse position
        var rowUnderMouse = dataGrid.GetVisualDescendants()
                                     .OfType<DataGridRow>()
                                     .FirstOrDefault(row => row.IsPointerOver);

        if (rowUnderMouse == null) return;

        // Get the DataContext of the hovered DataGridRow
        var hoveredProduct = rowUnderMouse.DataContext as ProductSold;
        if (hoveredProduct == null) return;

        long hoveredProductId = hoveredProduct.ProductId;

 
        bool IsImageLoadedBefore = hoveredProduct.Image != null;

        if (IsImageLoadedBefore) return;
        // Update the last hovered product ID
      

        // we load the image by retiving it from database and stored to the productinfo hovered object
        hoveredProduct!.Image = AccessToClassLibraryBackendProject.getImageOfProductFromDatabaseById(hoveredProductId);
        // Load the image for the hovered product
        //LoadTheImageHoveredById(hoveredProductId);

        Debug.WriteLine($"Hovered over product with ID: {hoveredProductId}");
    }
}