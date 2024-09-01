using GetStartedApp.ViewModels.DashboardPages;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using System.Diagnostics;
using GetStartedApp.Models;
using System.Reactive.Linq;


namespace GetStartedApp.ViewModels.CategoryPages
{
    public class EditCategoryViewModel : AddNewCategoryViewModel
    {


        // this value will stored the categoryname chosenby user to be edited
        private string CategoryNameToChange { get; }
      
        public EditCategoryViewModel(CategoryProductsViewModel categoryProductsViewModel) : base(categoryProductsViewModel)
        {

            CategoryAction = "تعديل الفئة";
            CategoryNameLabel = "اسم الفئة ";

            CategoryName = categoryProductsViewModel.SelectedProductCategory;

            CategoryNameToChange = CategoryName;
            

            AddOrEditOrDeleteCategoryCommand = ReactiveCommand.Create(EditCategoryToDatabase, CheckIfUserHasEnteredSomething);
        }


       private async void EditCategoryToDatabase()
        {
            // cateogryName is teh value of actual edit categoryName while CategoryNameTochange is the value of orinial one 
            // when we opned up the editCategoryUI

            if (AccessToClassLibraryBackendProject.EditCategoryOfProduct(CategoryNameToChange, CategoryName)) 
            {
                await ShowDialogOfAddNewCategoryResponseMessage.Handle("لقد تمت تعديل الفئة بنجاح");

                categoryProductsViewModel.ReloadProductsCategories();
            }

            else
            {
                await ShowDialogOfAddNewCategoryResponseMessage.Handle("لقد حصل خطأ ما ");
            }
        }
    }
}
