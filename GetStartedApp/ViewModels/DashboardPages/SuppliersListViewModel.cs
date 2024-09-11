using GetStartedApp.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using GetStartedApp.ViewModels.SupplierPages;
using System.Reactive.Linq;


namespace GetStartedApp.ViewModels.DashboardPages
{
    public class SuppliersListViewModel : ViewModelBase
    {
        // List for suppliers names
        private List<string> _suppliersList;
        public List<string> SuppliersList
        {
            get => _suppliersList;
            set => this.RaiseAndSetIfChanged(ref _suppliersList, value);
        }

        // Selected supplier
        private string _selectedSupplier;
        public string SelectedSupplier
        {
            get => _selectedSupplier;
            set => this.RaiseAndSetIfChanged(ref _selectedSupplier, value);
        }

        // Interaction for add new supplier dialog
        public Interaction<AddNewSupplierViewModel, Unit> ShowAddNewSupplierDialog { get; }

        // Command for adding new supplier
        public ReactiveCommand<Unit, Unit> AddNewSupplierCommand { get; }

        // Command for deleting selected supplier
       public ReactiveCommand<Unit, bool> DeleteSupplierCommand { get; }

        // Constructor
        public SuppliersListViewModel(MainWindowViewModel mainWindowViewModel)
        {
            SuppliersList = new List<string>();

            ShowAddNewSupplierDialog = new Interaction<AddNewSupplierViewModel, Unit>();

            AddNewSupplierCommand = ReactiveCommand.Create(AddNewSupplier);
        //    DeleteSupplierCommand = ReactiveCommand.Create(DeleteSupplier);

            LoadSuppliersList();
        }

        // Method to reload suppliers
        public void ReloadSuppliers()
        {
            LoadSuppliersList();
        }

        // Method to add a new supplier
        private async void AddNewSupplier()
        {
            var viewModel = new AddNewSupplierViewModel(this);
            await ShowAddNewSupplierDialog.Handle(viewModel);
        }

        // Method to delete the selected supplier
       public bool DeleteSupplier()
       {
           
               string phoneNumber = ExtractPhoneNumberFromSupplier(SelectedSupplier); // Placeholder for actual extraction
               if (AccessToClassLibraryBackendProject.DeleteSupplierByPhoneNumber(phoneNumber))
               {
                   ReloadSuppliers();
                   return true;
               }
           
           return false;
       }

        // Method to load the suppliers list
        private void LoadSuppliersList()
        {
            SuppliersList = AccessToClassLibraryBackendProject.GetSupplierNamePhoneNumberCombo();
        }

        // Placeholder method to extract phone number (replace with actual implementation)
        private string ExtractPhoneNumberFromSupplier(string supplier)
        {
            // Implement extraction logic here
            return PhoneNumberExtractor.ExtractPhoneNumber(SelectedSupplier); // Modify as needed
        }
    }
}
