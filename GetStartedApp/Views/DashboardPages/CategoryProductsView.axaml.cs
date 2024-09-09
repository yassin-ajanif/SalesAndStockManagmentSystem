using Avalonia.Controls;
using Avalonia.VisualTree;
using ReactiveUI;
using System.Threading.Tasks;
using System;
using System.Reactive;
using Avalonia.ReactiveUI;
using System.Reactive;
using GetStartedApp.ViewModels.DashboardPages;
using GetStartedApp.Views.CategoryPages;
using GetStartedApp.ViewModels.CategoryPages;
using GetStartedApp.ViewModels.ProductPages;
using System.Diagnostics;
using Microsoft.VisualBasic;
using Avalonia.Interactivity;
using GetStartedApp.Models;



namespace GetStartedApp.Views.DashboardPages
{
    public partial class CategoryProductsView : ReactiveUserControl<CategoryProductsViewModel>
    {
        public CategoryProductsView()
        {
            InitializeComponent();

            RegisterShowDialogProductEvents();
        }

        private void RegisterShowDialogProductEvents()
        {


            this.WhenActivated(action =>
            {
                action(ViewModel!.ShowAddCategoryDialog.RegisterHandler(ShowDialogOfAddNewCategory));
                //
                //action(ViewModel!.ShowEditQuantityDialog.RegisterHandler(ShowDialogOfEditQuantityProduct));

            });


        }


        private async Task ShowDialogOfAddNewCategory(InteractionContext<AddNewCategoryViewModel, Unit> interaction)
        {
            var dialog = new DialogContainerView();

            // the interaction is an instance of the viewmodel we want to bind to the view we want to display 
            dialog.Content = new AddNewCategoryView() { DataContext = interaction.Input };
            dialog.Title = "اضافة تصنيف جديد";
            var window = this.GetVisualRoot() as Window;
            if (window == null)
            {
                throw new InvalidOperationException("Cannot show dialog because this control is not contained within a Window.");
            }

            var result = await dialog.ShowDialog<Unit>(window);


            interaction.SetOutput(Unit.Default);
        }



        private async void OnEditCategoryClicked(object sender, RoutedEventArgs e) {


            var dialog = new DialogContainerView();

           // var selectedCategory = ListBox.SelectedItemProperty;

            // the interaction is an instance of the viewmodel we want to bind to the view we want to display 
            dialog.Content = new AddNewCategoryView() { DataContext = new EditCategoryViewModel(ViewModel!) };
            dialog.Title = "تعديل فئة المنتجات";

            var window = this.GetVisualRoot() as Window;
            if (window == null)
            {
                throw new InvalidOperationException("Cannot show dialog because this control is not contained within a Window.");
            }

            var result = await dialog.ShowDialog<Unit>(window);
        }

   
        private async void OnDeleteCategoryClicked(object sender, RoutedEventArgs e)
        {
            
                // to show a message box you need to have a refrence of hte parent window

                var GetParentOfProductListView = this.GetVisualRoot() as Window;
            string messageToShow = " هل تريد حقا حدف هذه الفئة ";


                if (GetParentOfProductListView == null)
                {
                    throw new InvalidOperationException("Cannot show dialog because this control is not contained within a Window.");
                }

              bool MessageBoxBtnsAreVisible = true;

              var DeleteMessageBox = new ShowMessageBoxContainer(messageToShow, MessageBoxBtnsAreVisible);
 
              var DoYouWantToDeleteProductCategory =  await DeleteMessageBox.ShowDialog<bool>(GetParentOfProductListView);

            // we use this variable to check if the deleteion operation is succed 
            bool deletionOperationIsScceded = false;

            if (DoYouWantToDeleteProductCategory) { deletionOperationIsScceded = ViewModel!.DeleteProductCategory(); }

            else return;

            if (deletionOperationIsScceded) {

                 MessageBoxBtnsAreVisible = false;

                 DeleteMessageBox = new ShowMessageBoxContainer(" لقد تمت العملية بنجاح ", MessageBoxBtnsAreVisible);

                 DoYouWantToDeleteProductCategory = await DeleteMessageBox.ShowDialog<bool>(GetParentOfProductListView);
            }

            else {

                MessageBoxBtnsAreVisible = false;

                DeleteMessageBox = new ShowMessageBoxContainer(" لا يمكنك حذف صنف مسجل في قائمة المبيعات ", MessageBoxBtnsAreVisible);

                DoYouWantToDeleteProductCategory = await DeleteMessageBox.ShowDialog<bool>(GetParentOfProductListView);
            }
        }

    }
}
