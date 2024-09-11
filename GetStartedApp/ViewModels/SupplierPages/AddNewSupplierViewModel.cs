using GetStartedApp.Models;
using GetStartedApp.ViewModels.DashboardPages;
using ReactiveUI;
using System.ComponentModel.DataAnnotations;
using System.Reactive;
using System.Reactive.Linq;


namespace GetStartedApp.ViewModels.SupplierPages
{
    public class AddNewSupplierViewModel : ViewModelBase
    {
        private string _supplierName;
        private string _fiscalIdentifier;
        private string _patented;
        private string _rc;
        private string _cnss;
        private string _ice;
        private string _phoneNumber;
        private string _address;
        private string _bankAccount;

        // Property for SupplierName
        [Required]
        public string SupplierName
        {
            get => _supplierName;
            set => this.RaiseAndSetIfChanged(ref _supplierName, value);
        }

        // Property for FiscalIdentifier
        public string FiscalIdentifier
        {
            get => _fiscalIdentifier;
            set => this.RaiseAndSetIfChanged(ref _fiscalIdentifier, value);
        }

        // Property for Patented
        public string Patented
        {
            get => _patented;
            set => this.RaiseAndSetIfChanged(ref _patented, value);
        }

        // Property for RC
        public string RC
        {
            get => _rc;
            set => this.RaiseAndSetIfChanged(ref _rc, value);
        }

        // Property for CNSS
        public string CNSS
        {
            get => _cnss;
            set => this.RaiseAndSetIfChanged(ref _cnss, value);
        }

        // Property for ICE
        public string ICE
        {
            get => _ice;
            set => this.RaiseAndSetIfChanged(ref _ice, value);
        }

        // Property for PhoneNumber
        [Required]
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => this.RaiseAndSetIfChanged(ref _phoneNumber, value);
        }

        // Property for Address
        public string Address
        {
            get => _address;
            set => this.RaiseAndSetIfChanged(ref _address, value);
        }

        private string _supplierActionBtnName;
        public string SupplierActionBtnName
        {
            get => _supplierActionBtnName;
            set => this.RaiseAndSetIfChanged(ref _supplierActionBtnName, value);
        }

        public string BankAccount
        {
            get => _bankAccount;
            set => this.RaiseAndSetIfChanged(ref _bankAccount, value);
        }

        // Interaction to show success or failure messages
        public Interaction<string, Unit> ShowDialogOfAddNewSupplierResponseMessage { get; }

        // Command to handle adding the supplier
        public ReactiveCommand<Unit, Unit> AddOrEditOrDeleteSupplier { get; set; }

        // Reference to SuppliersListViewModel to refresh the list after adding a supplier
        public SuppliersListViewModel SuppliersListViewModel { get; }

        // Method to add the supplier to the database
        private async void AddNewSupplierToDatabase()
        {
            if (AccessToClassLibraryBackendProject.AddNewSupplierToDb(SupplierName, FiscalIdentifier, Patented, RC, CNSS, ICE, BankAccount,PhoneNumber, Address))
            {
                await ShowDialogOfAddNewSupplierResponseMessage.Handle("تم إضافة المورد بنجاح");
                SuppliersListViewModel.ReloadSuppliers();
            }
            else
            {
                await ShowDialogOfAddNewSupplierResponseMessage.Handle("حصل خطأ ما");
            }
        }

        // Constructor
        public AddNewSupplierViewModel(SuppliersListViewModel suppliersListViewModel)
        {
            SupplierActionBtnName = "إضافة مزود";
           AddOrEditOrDeleteSupplier = ReactiveCommand.Create(AddNewSupplierToDatabase);
            ShowDialogOfAddNewSupplierResponseMessage = new Interaction<string, Unit>();
            SuppliersListViewModel = suppliersListViewModel;
        }
    }
}

