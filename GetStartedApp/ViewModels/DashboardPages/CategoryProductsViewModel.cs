using GetStartedApp.Models;
using GetStartedApp.ViewModels.CategoryPages;
using GetStartedApp.ViewModels.ProductPages;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;



namespace GetStartedApp.ViewModels.DashboardPages
{
    public class CategoryProductsViewModel : ProductsListViewModel
    {


        public ObservableCollection<string> _ProductsCategoryList;
        public ObservableCollection<string> ProductsCategoryList {
           
            get {  return _ProductsCategoryList; }

            set { this.RaiseAndSetIfChanged(ref _ProductsCategoryList, value); }
        }

  
        private string _selectedProductCategory;
        public string SelectedProductCategory
        {
            get => _selectedProductCategory;
            set => this.RaiseAndSetIfChanged(ref _selectedProductCategory, value);
        }

        public Interaction<AddNewCategoryViewModel, Unit> ShowAddCategoryDialog { get; }

        public ReactiveCommand<Unit, Unit> AddNewCategoryCommand { get; }

        public CategoryProductsViewModel(MainWindowViewModel mainWindowViewModel) : base(mainWindowViewModel) 
        {
        
                LoadProductCategoryList();

                ProductsCategoryList = new ObservableCollection<string>(ProductCategories);

                ShowAddCategoryDialog = new Interaction<AddNewCategoryViewModel, Unit>();
                
                AddNewCategoryCommand = ReactiveCommand.Create(AddNewCategory);
        }

        public void ReloadProductsCategories()
        {
            LoadProductCategoryList();
            ProductsCategoryList = new ObservableCollection<string>(ProductCategories);
        }

        private async void AddNewCategory()
        {
           
         var userControlToShowInsideDialog = new AddNewCategoryViewModel(this);

            await ShowAddCategoryDialog.Handle(userControlToShowInsideDialog);
        }

        public bool DeleteProductCategory()
        {
            if(AccessToClassLibraryBackendProject.DeleteCategory(SelectedProductCategory)) {

                ReloadProductsCategories();
                return true;
            }
            return false;
        }
    
    }
}
