using GetStartedApp.Models;
using GetStartedApp.ViewModels.DashboardPages;
using ReactiveUI;
using System.Reactive.Linq;

namespace GetStartedApp.ViewModels.SupplierPages
{
    public class EditSupplierViewModel : AddNewSupplierViewModel
    {
        // This value will store the supplier name and phone number chosen by the user to be edited
        private string SupplierName_Phone_ToChange { get; }
        private SuppliersListViewModel suppliersListViewModel;
        private string oldPhoneNumber;

        public EditSupplierViewModel(SuppliersListViewModel suppliersListViewModel) : base(suppliersListViewModel)
        {
            this.suppliersListViewModel = suppliersListViewModel;
            SupplierActionBtnName = "تعديل معلومات المورد";

            // Initialize SupplierName_Phone_ToChange with the selected supplier's info
            SupplierName_Phone_ToChange = suppliersListViewModel.SelectedSupplier;

            string supplierPhoneNumber = PhoneNumberExtractor.ExtractPhoneNumber(suppliersListViewModel.SelectedSupplier);

            // Load supplier info from the database and update the UI
            LoadSupplierInfoFromDb_At_UI_ToUpdate(supplierPhoneNumber);

            // Initialize command for updating supplier info
            AddOrEditOrDeleteSupplier = ReactiveCommand.Create(EditSupplierInfoDatabase);
        }

        private void LoadSupplierInfoFromDb_At_UI_ToUpdate(string supplierPhoneNumber)
        {
            string supplierName = string.Empty;
            string phoneNumber = supplierPhoneNumber;
            string email = string.Empty;
            string bankAccount = string.Empty;
            string fiscalIdentifier = string.Empty;
            string rc = string.Empty;
            string ice = string.Empty;
            string patented = string.Empty;
            string cnss = string.Empty;
            string address = string.Empty;

            // Load the old phone number for updating
            oldPhoneNumber = supplierPhoneNumber;

            // Retrieve supplier info from the database
            AccessToClassLibraryBackendProject.getSupplierInfo(phoneNumber, ref supplierName, ref bankAccount,
                ref fiscalIdentifier, ref rc, ref ice, ref patented, ref cnss,ref address);

            // Set the properties for the UI
            base.SupplierName = supplierName;
            base.PhoneNumber = phoneNumber;
            base.BankAccount = bankAccount;
            base.FiscalIdentifier = fiscalIdentifier;
            base.RC = rc;
            base.ICE = ice;
            base.Patented = patented;
            base.CNSS = cnss;
            base.Address = address;
        }

      private async void EditSupplierInfoDatabase()
      {
          // Attempt to update supplier info in the database
          if (AccessToClassLibraryBackendProject.UpdateSupplierByPhoneNumber(oldPhoneNumber,base.PhoneNumber,base.SupplierName,
               base.FiscalIdentifier, base.Patented,base.RC, base.CNSS,base.ICE,base.BankAccount,base.Address))
          {
              // Show success message if the update was successful
              await ShowDialogOfAddNewSupplierResponseMessage.Handle("لقد تمت العملية بنجاح");
     
              // Reload the supplier list
              suppliersListViewModel.ReloadSuppliers();
          }
          else
          {
              // Show error message if the update failed
              await ShowDialogOfAddNewSupplierResponseMessage.Handle("لقد حصل خطأ ما");
          }
      }
    }
}
