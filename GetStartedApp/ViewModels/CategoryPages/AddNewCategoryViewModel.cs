using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using GetStartedApp.Models;
using System.Reactive.Linq;
using GetStartedApp.ViewModels.DashboardPages;
using GetStartedApp.Helpers.CustomUIErrorAttributes;
using GetStartedApp.Helpers;



namespace GetStartedApp.ViewModels.CategoryPages
{
    public class AddNewCategoryViewModel : ViewModelBase
    {


        private string _categoryName;
        [StringMustHaveAtLeast_3_Letters(ErrorMessage ="التصنيف يجب ان يحتوي على الاقل 3 حروف")]
        [CheckForInvalidCharacters]
        [CheckIfCategoryExists(  ErrorMessage = "هذا الصنف يوجد من قبل")]
        [MaxStringLengthAttribute_IS(25, "هذه الجملة طويلة جدا")]
        public string CategoryName
        {
            get { return _categoryName; }
            set { this.RaiseAndSetIfChanged(ref _categoryName, value); }
        }

        private string _categoryAction;
        public string CategoryAction
        {
            get { return _categoryAction; }
            set { this.RaiseAndSetIfChanged(ref _categoryAction, value); }
        }

        private string _categoryNameLabel;
        public string CategoryNameLabel
        {
            get { return _categoryNameLabel; }
            set { this.RaiseAndSetIfChanged(ref _categoryNameLabel, value); }
        }

        public ReactiveCommand<Unit, Unit> AddOrEditOrDeleteCategoryCommand { get; set; }

        public Interaction <string,Unit> ShowDialogOfAddNewCategoryResponseMessage { get; set; }

        public IObservable<bool> CheckIfUserHasEnteredSomething =>
           this.WhenAnyValue(x => x.CategoryName,
          categoryName => !string.IsNullOrWhiteSpace(categoryName) && categoryName.Length > 3 && UiAttributeChecker.AreThesesAttributesPropertiesValid(this,nameof(CategoryName)));

        public CategoryProductsViewModel categoryProductsViewModel { get; }

        public AddNewCategoryViewModel(CategoryProductsViewModel categoryProductsViewModel)
        {
            this.categoryProductsViewModel = categoryProductsViewModel;
            //CategoryName = "add new category";
            CategoryAction = "إضافة فئة جديدة.";
            CategoryNameLabel = "اسم الفئة الجديدة";


            AddOrEditOrDeleteCategoryCommand = ReactiveCommand.Create(AddNewCategoryToDatabase, CheckIfUserHasEnteredSomething);

            ShowDialogOfAddNewCategoryResponseMessage = new Interaction<string, Unit>();

        }

     

        private async void AddNewCategoryToDatabase()
        {
            if (AccessToClassLibraryBackendProject.InsertNewCategoryOfProduct(CategoryName))
            {
               await ShowDialogOfAddNewCategoryResponseMessage.Handle("لقد تمت إضافة الفئة الجديدة بنجاح");

                categoryProductsViewModel.ReloadProductsCategories();
            }

            else
            {
                await ShowDialogOfAddNewCategoryResponseMessage.Handle("لقد حصل خطأ ما ");
            }
        }

        // this section is for testing 

        async public Task<bool> AddNewCategoryToDatabaseEndToEndTest(string categoryname)
        {    
            CategoryName = categoryname;
            
            if (!await CheckIfUserHasEnteredSomething.FirstAsync()) return false;

            if (!AccessToClassLibraryBackendProject.InsertNewCategoryOfProduct(CategoryName)) return false;

            return true;
        }

        async public Task<bool> UpdateCategoryToDatabaseEndToEndTest(string PreviousCategoryname, string NewCategoryName)
        {
            CategoryName = NewCategoryName;
            if (!await CheckIfUserHasEnteredSomething.FirstAsync()) return false;

            if (!AccessToClassLibraryBackendProject.EditCategoryOfProduct(PreviousCategoryname,NewCategoryName)) return false;

            return true;
        }

    }
}
