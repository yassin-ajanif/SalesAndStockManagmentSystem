using GetStartedApp.Models;
using ReactiveUI;
using System.Collections.Generic;
using System.Reactive;
using GetStartedApp.ViewModels.ProductPages;
using System.Windows.Input;

using System.Reactive.Linq;
using GetStartedApp.Models.Objects;
using System.Collections.ObjectModel;


namespace GetStartedApp.ViewModels.DashboardPages
{

    public class BonReceptionViewModel : MakeSaleViewModel
       {

        private List<string> _suppliersList;
        public  List<string> SuppliersList { get => _suppliersList; set => this.RaiseAndSetIfChanged(ref _suppliersList , value); }


        public ICommand AddNewProductCommand { get; set; }

        public Interaction<AddProductViewModel, Unit> ShowAddProductDialog { get; }

        public BonReceptionViewModel(MainWindowViewModel mainWindowViewModel) : base(mainWindowViewModel)
        {
            // addProductViewModel = new ProductsListViewModel(mainWindowViewModel);
            AddNewProductCommand = ReactiveCommand.Create(AddProductInfoOperation);
            ShowAddProductDialog = new Interaction<AddProductViewModel, Unit>();
            SuppliersList = getSuppliers();
        }

        private List<string> getSuppliers()
        {
           return AccessToClassLibraryBackendProject.GetSupplierNamePhoneNumberCombo();
        }

        private List<string> getProductListCategoriesFromDb()
        {
            return AccessToClassLibraryBackendProject.GetProductsCategoryFromDatabase();
        }

        public async void AddProductInfoOperation()
        {
            // 1 is the default all category string we add which make the count equal 1 once we add a new category a value becomes more than 1
            // which indicate that there is a category or more added to the system
           bool ThereIsNoCategoriesAddedYetToSystem = getProductListCategoriesFromDb().Count == 1;
      // 
            if (ThereIsNoCategoriesAddedYetToSystem) { await ShowDeleteSaleDialogInteraction.Handle(" لا توجد تصنيفات اضف تصنيف جديد "); return; }
       
            var userControlToShowInsideDialog = new AddProductViewModel(this);
       
            await ShowAddProductDialog.Handle(userControlToShowInsideDialog);
        }
    }

}
