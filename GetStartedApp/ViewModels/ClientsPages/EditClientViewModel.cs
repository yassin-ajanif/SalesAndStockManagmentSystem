using GetStartedApp.Models;
using GetStartedApp.ViewModels.DashboardPages;
using ReactiveUI;
using System.Reactive.Linq;

namespace GetStartedApp.ViewModels.ClientsPages
{
    public class EditClientViewModel : AddNewClientViewModel
    {

        // this value will stored the categoryname chosen by user to be edited
        private string ClientName_Phone_ToChange { get; }
        private ClientsListViewModel clientsListViewModel;
        private string oldPhoneNumber;
        public EditClientViewModel(ClientsListViewModel clientsListViewModel) : base(clientsListViewModel)
        {
            this.clientsListViewModel = clientsListViewModel;
            ClientActionBtnName = "تعديل معلومات الزبون ";

            // CategoryName = categoryProductsViewModel.SelectedProductCategory;

            ClientName_Phone_ToChange = clientsListViewModel.SelectedClient;

            string ClientPhoneNumber = PhoneNumberExtractor.ExtractPhoneNumber(clientsListViewModel.SelectedClient);

            LoadClientInfoFromDb_At_UI_ToUpdate(ClientPhoneNumber);

            AddOrEditOrDeleteClient = ReactiveCommand.Create(EditClientInfoDatabase);
        }

        private void LoadClientInfoFromDb_At_UI_ToUpdate(string ClientPhoneNumber)
        {
            string ClientName = string.Empty;
            string PhoneNumber = ClientPhoneNumber;
            string Email=string.Empty;
            int ClientID=0;
            // we load this variable to use it to update our client info in case we update phone number we need to have the old one which is the first one loaded to ui before it edited
            oldPhoneNumber = ClientPhoneNumber;

            AccessToClassLibraryBackendProject.GetClientInfo(ref ClientID,PhoneNumber, ref ClientName, ref Email);
                
            base.ClientName = ClientName;
            base.PhoneNumber = PhoneNumber;
            base.Email = Email;

        }

        private async void EditClientInfoDatabase()
        {
           

          if (AccessToClassLibraryBackendProject.UpdateClient(base.ClientName,oldPhoneNumber, base.PhoneNumber,base.Email))
          {
              await ShowDialogOfAddNewClientResponseMessage.Handle("لقد تمت العملية بنجاح");

                clientsListViewModel.ReloadClients();
          }
        
          else
          {
              await ShowDialogOfAddNewClientResponseMessage.Handle("لقد حصل خطأ ما ");
          }
        }
    }
}
