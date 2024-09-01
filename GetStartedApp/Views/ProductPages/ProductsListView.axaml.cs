using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Threading.Tasks;
using ReactiveUI;
using GetStartedApp.ViewModels;
using Avalonia.VisualTree;
using GetStartedApp.ViewModels.ProductPages;
using System;
using Avalonia.Interactivity;
using System.Diagnostics;
using Avalonia.ReactiveUI;
using System.Reactive;
using DynamicData.Binding;
using System.Linq;
using System.Reflection;
using Avalonia.Threading;
using GetStartedApp.Models;
using Avalonia.Input;
using GetStartedApp.Helpers;


namespace GetStartedApp.Views.ProductPages
{
    public partial class ProductsListView : ReactiveUserControl<ProductsListViewModel>
    {

       

        public ProductsListView()
        {
            
            InitializeComponent();

            
            // we register the events responsible for showing dialog when a user want add or to edit products
            RegisterShowDialogProductEvents();


            SortItemsByLeastStockQuantity();



        }

         private void SortItemsByLeastStockQuantity()
        {
            int QuantitiyIndex = 3;
            Dispatcher.UIThread.InvokeAsync(() => this.ProductsListGrid.Columns[QuantitiyIndex].Sort());
        }


        private void RegisterShowDialogProductEvents()
        {

 
            this.WhenActivated(action =>
            {
                action(ViewModel!.ShowAddProductDialog.RegisterHandler(ShowDialogOfAddProductInfo));
               
                action(ViewModel!.ShowEditQuantityDialog.RegisterHandler(ShowDialogOfEditQuantityProduct));
               
                action(ViewModel!.ShowEditPriceDialog.RegisterHandler(ShowDialogOfEditPriceProduct));

                action(ViewModel!.ShowEditAllInfoProductDialog.RegisterHandler(ShowDialogOfEditAllInfoProducts));

                action(ViewModel!.ShowDeleteProductDialog.RegisterHandler(ShowDialogOfDeleteProduct));

                action(ViewModel!.ShowBarCodeViewGeneratorDialog.RegisterHandler(ShowDialogOfBarCodePrinter));

                action(ViewModel!.ShowReturnProductDialog.RegisterHandler(ShowDialogOfReturnProduct));




            });


        }

        private string GetTheTitleOfPageToShow_BasedOn_TheViewModelPageIsBoundTo(ViewModelBase viewModelPageIsBoundTo)
        {
            if (viewModelPageIsBoundTo.GetType() == typeof(AddProductViewModel))
            {
                return "إضافة منتوج جديد";
            }
            else if (viewModelPageIsBoundTo.GetType() == typeof(EditStockQuantitiyProductViewModel))
            {
                return "تعديل كمية المنتج";
            }
            else if (viewModelPageIsBoundTo.GetType() == typeof(EditPriceProductViewModel))
            {
                return "تعديل ثمن المنتج";
            }
            else if (viewModelPageIsBoundTo.GetType() == typeof(EditAllProductInfoViewModel))
            {
                return "تعديل جميع معلومات المنتج";
            }
            else if (viewModelPageIsBoundTo.GetType() == typeof(BarCodeGeneratorViewModel))
            {
                return "طباعة الباركود ";
            }
            else if (viewModelPageIsBoundTo.GetType() == typeof(ReturnProductViewModel))
            {
                return "ارجاع المنتج";
            }
            else
            {
                return "Unknown";
            }
        }


        // this function shows the view and set the viewmodel bind to it
        private async void ShowAddProductViewBoundToViewmodel_(ViewModelBase ViewmodelToBind)
        {
            var dialog = new DialogContainerView();
            dialog.Title = GetTheTitleOfPageToShow_BasedOn_TheViewModelPageIsBoundTo(ViewmodelToBind);
            // CurrentDialogOpened = dialog;
            // Create an instance of AddOrEditOrKnowProductView and set it as the Content of the dialog
            dialog.Content = new AddProductView()
            {
                DataContext = ViewmodelToBind
                
            };

            var window = this.GetVisualRoot() as Window;
            if (window == null)
            {
                throw new InvalidOperationException("Cannot show dialog because this control is not contained within a Window.");
            }

            
            var result = await dialog.ShowDialog<Unit>(window);
 
        }

        // this function shows the view and set the viewmodel bind to it

        private async void ShowReturnProductViewBoundToViewmodel_(ReturnProductViewModel ViewmodelToBind )
        {
            var dialog = new DialogContainerView();
            dialog.Title = GetTheTitleOfPageToShow_BasedOn_TheViewModelPageIsBoundTo(ViewmodelToBind);
            // CurrentDialogOpened = dialog;
            // Create an instance of AddOrEditOrKnowProductView and set it as the Content of the dialog
            dialog.Content = new ReturnProductView(ViewmodelToBind)
            {
                DataContext = ViewmodelToBind
            };

            var window = this.GetVisualRoot() as Window;
            if (window == null)
            {
                throw new InvalidOperationException("Cannot show dialog because this control is not contained within a Window.");
            }

            var result = await dialog.ShowDialog<Unit>(window);

        }

        private async Task ShowDialogOfAddProductInfo(InteractionContext<AddProductViewModel, Unit> interaction)
        {
            ViewModelBase VeiwmodelToBindWithAddProductView = interaction.Input;

            ShowAddProductViewBoundToViewmodel_(VeiwmodelToBindWithAddProductView);

            interaction.SetOutput(Unit.Default);
        }

        private async Task ShowDialogOfEditQuantityProduct(InteractionContext<EditStockQuantitiyProductViewModel, Unit> interaction)
        {
            ViewModelBase VeiwmodelToBindWithAddProductView = interaction.Input;

            ShowAddProductViewBoundToViewmodel_(VeiwmodelToBindWithAddProductView);
                
            interaction.SetOutput(Unit.Default);
        }

        private async Task ShowDialogOfEditPriceProduct(InteractionContext<EditPriceProductViewModel, Unit> interaction)
        {
            ViewModelBase VeiwmodelToBindWithAddProductView = interaction.Input;

            ShowAddProductViewBoundToViewmodel_(VeiwmodelToBindWithAddProductView);

            interaction.SetOutput(Unit.Default);
        }

        private async Task ShowDialogOfDeleteProduct(InteractionContext<string, bool> interaction)
        {
            // to show a message box you need to have a refrence of hte parent window

            var GetParentOfProductListView = this.GetVisualRoot() as Window;
            string messageToShow = interaction.Input;
           
            if (GetParentOfProductListView == null)
            {
                throw new InvalidOperationException("Cannot show dialog because this control is not contained within a Window.");
            }

            bool MessageBoxBtnsAreVisibleIf = (messageToShow == "هل تريد حقا حدف المنتج");

            var DeleteMessageBox = new ShowMessageBoxContainer(messageToShow, MessageBoxBtnsAreVisibleIf);
       
                var DoYouWantToDeleteProduct = 
                await DeleteMessageBox.ShowDialog<bool>(GetParentOfProductListView);

            interaction.SetOutput(DoYouWantToDeleteProduct);

        }
        
        private async Task ShowDialogOfEditAllInfoProducts(InteractionContext<AddProductViewModel, Unit> interaction)
        {
            ShowAddProductViewBoundToViewmodel_(interaction.Input);

            interaction.SetOutput(Unit.Default);
        }

        private async Task ShowDialogOfBarCodePrinter(InteractionContext<long, Unit> interaction)
        {

            var dialog = new DialogContainerView();
            dialog.Title = "طباعة الباركود ";
            // this is a default value i choosed it dosnt have any reason
            int barCodeNumbers = 1;

            string barCodeSerieNumber = interaction.Input.ToString();

            // we don't allow a user in this case to increment the number of bar
            bool userCanEditNumberOfBarCodeToPrint = true;

            // CurrentDialogOpened = dialog;
            // Create an instance of AddOrEditOrKnowProductView and set it as the Content of the dialog
            dialog.Content = new BarCodeGeneratorView()
            {
                DataContext = new BarCodeGeneratorViewModel(barCodeSerieNumber, barCodeNumbers.ToString(), userCanEditNumberOfBarCodeToPrint)
            };

            var window = this.GetVisualRoot() as Window;
            if (window == null)
            {
                throw new InvalidOperationException("Cannot show dialog because this control is not contained within a Window.");
            }

            await dialog.ShowDialog<Unit>(window);

            interaction.SetOutput(Unit.Default);

          
        }

        private async Task ShowDialogOfReturnProduct(InteractionContext<ReturnProductViewModel, Unit> interaction)
        {
            ShowReturnProductViewBoundToViewmodel_(interaction.Input);

            interaction.SetOutput(Unit.Default);

        }



        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the DataGrid
            var dataGrid = (DataGrid)sender;

            // Get the index of the selected row
            var selectedItemProduct = dataGrid.SelectedItem as ProductInfo;

            if (selectedItemProduct == null) return;

            long selectedItemProductId = selectedItemProduct.id;

           // int IdOfSelectedItemProduct = selectedItemProduct[0];
            // Now you can send the index to your ViewModel
            if (DataContext is ProductsListViewModel myMainWindowViewModel)
            {
                //  set send the value of the click item index to viewmodel so we can use it to retrive the productinfo from
                //  a list productinfo we have into viewmodel 
                   myMainWindowViewModel.ClickedProductInfoID = selectedItemProductId;

                Debug.WriteLine("triggerd selection");

                // we set default focus to serach bar
                SearchBar.Focus();

            }
        }

        private void OnUserHoverTheProductRow(object sender, PointerEventArgs e)
        {
            if (DataContext is not ProductsListViewModel myMainWindowViewModel) return;
           
            var dataGrid = (DataGrid)sender;

            // Get the hovered row based on the mouse position
            var rowUnderMouse = dataGrid.GetVisualDescendants()
                                         .OfType<DataGridRow>()
                                         .FirstOrDefault(row => row.IsPointerOver);

            if (rowUnderMouse == null) return;

            // Get the DataContext of the hovered DataGridRow
            var hoveredProduct = rowUnderMouse.DataContext as ProductInfo;
            if (hoveredProduct == null) return;

            long hoveredProductId = hoveredProduct.id;

        
            bool IsImageLoadedBefore = hoveredProduct.SelectedProductImage != null;

            if (IsImageLoadedBefore) return;
            // Update the last hovered product ID
          

            // we load the image by retiving it from database and stored to the productinfo hovered object
            hoveredProduct!.SelectedProductImage = AccessToClassLibraryBackendProject.getImageOfProductFromDatabaseById(hoveredProductId);
            // Load the image for the hovered product
            //LoadTheImageHoveredById(hoveredProductId);

            Debug.WriteLine($"Hovered over product with ID: {hoveredProductId}");
        }



        private void OnEditQuantityBtnClicked(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProductsListViewModel myMainWindowViewModel)
             {
                myMainWindowViewModel.EditProductQuantityOperation();
             }
        }

        private void OnEditPriceBtnClicked(object sender, RoutedEventArgs e)
        {

            if (DataContext is ProductsListViewModel myMainWindowViewModel)
            {
                myMainWindowViewModel.EditProductPriceOperation();
            }
        }

        private void OnDeletePriceBtnClicked(Object sender, RoutedEventArgs e)
        {
            if (DataContext is ProductsListViewModel myMainWindowViewModel)
            {
                myMainWindowViewModel.DeleteProductOperation();
            }
        }
  
        private void OnEditAllInfoProducts(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProductsListViewModel myMainWindowViewModel)
            {
                myMainWindowViewModel.EditAllProductsInfoOperation();
            }
        }

        private void OnPrintBarCodeForThisProduct(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProductsListViewModel myMainWindowViewModel)
            {
                myMainWindowViewModel.PrintBarCodesOfThisProduct();
            }
        }

        private void OnReturnProductBtnClicked(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProductsListViewModel myMainWindowViewModel)
            {
                myMainWindowViewModel.ReturnProductOperation();
            }
        }
    }
}
