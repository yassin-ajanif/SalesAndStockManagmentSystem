using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using GetStartedApp.ViewModels.ProductPages;
using GetStartedApp.Views.ProductPages;

namespace GetStartedApp;

public partial class ReturnProductView : AddProductView
{
    public TextBlock NumberOfProductsReturnedLabel { get; private set; }
    public TextBox NumberOfProductsReturnedTextBox { get; private set; }
    public TextBlock PriceSoldOfProductsReturnedLabel { get; private set; }
    public TextBox PriceSoldOfProductsReturnedTextBox { get; private set; }

    public ReturnProductView(ReturnProductViewModel ViewmodelToBoundToThisView)
    {
        InitializeComponent();

        RemoveTheQuanityInfoAndCategory_From_Screen();
       
        CreateTheNewTagInputs_NumberOfProductUnits_And_Price_Of_SoldProductReturned();
       
        ReplaceTheRemoved_QuantityInfoAndCategory_From_Screen();
       
        BindTheNewInputsCreatedDynamically_To_Viewmodel(ViewmodelToBoundToThisView);

    }

    // we removed those info to have space so we can put the two textbox inputs quanitytoReturnOfProduct and priceProductWasBought with
    private void RemoveTheQuanityInfoAndCategory_From_Screen()
    {
        QuantityInStockXamlTagInput.Children.Clear();
        ProductCategoriesXamlTagInput.Children.Clear();
       
    }
    private void CreateTheNewTagInputs_NumberOfProductUnits_And_Price_Of_SoldProductReturned()
    {
        // Create a Label and TextBox for NumberOfProductsReturned
        NumberOfProductsReturnedLabel = new TextBlock
        {
            Text = "عدد المنتجات المسترجعة:",
            TextWrapping = TextWrapping.Wrap
        };
        NumberOfProductsReturnedTextBox = new TextBox { Name = "NumberOfProductsReturned" };

        // Create a Label and TextBox for PriceSoldOfProductsReturned
        PriceSoldOfProductsReturnedLabel = new TextBlock
        {
            Text = "السعر الذي تم بيع المنتج به:",
            TextWrapping = TextWrapping.Wrap
        };
        PriceSoldOfProductsReturnedTextBox = new TextBox { Name = "PriceSoldOfProductsReturned" };

    }
    private void ReplaceTheRemoved_QuantityInfoAndCategory_From_Screen()
    {
        QuantityInStockXamlTagInput.Children.Add(NumberOfProductsReturnedLabel);
        QuantityInStockXamlTagInput.Children.Add(NumberOfProductsReturnedTextBox);

        ProductCategoriesXamlTagInput.Children.Add(PriceSoldOfProductsReturnedLabel);
        ProductCategoriesXamlTagInput.Children.Add(PriceSoldOfProductsReturnedTextBox);
    }
    private void BindTheNewInputsCreatedDynamically_To_Viewmodel(ReturnProductViewModel ViewModelBoundTothisUi)
    {
       
        string numberOfProductsReturned = nameof(ViewModelBoundTothisUi.NumberOfProductsReturned);
        string priceSoldOfProductsReturned = nameof(ViewModelBoundTothisUi.PriceSoldOfProductsReturned);

        BindControl(ViewModelBoundTothisUi,NumberOfProductsReturnedTextBox, numberOfProductsReturned);
        BindControl(ViewModelBoundTothisUi,PriceSoldOfProductsReturnedTextBox, priceSoldOfProductsReturned);

    }
    private void BindControl(ReturnProductViewModel viewmodelBoundToThisUi, TextBox textBox, string propertyName)
    {
        // Bind the TextBox to the specified property
        textBox.DataContext = viewmodelBoundToThisUi;
        textBox.Bind(TextBox.TextProperty, new Binding(propertyName));
    }


}