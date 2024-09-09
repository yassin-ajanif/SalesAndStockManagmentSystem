using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using GetStartedApp.Models;
using GetStartedApp.ViewModels.DashboardPages;

namespace GetStartedApp.ViewModels.ClientsPages
{
    public class AddNewClientViewModel : ViewModelBase
    {
        private string _clientName;
        private string _phoneNumber;
        private string _email;
        private string _city;

        // Property for ClientName
        public string ClientName
        {
            get => _clientName;
            set => this.RaiseAndSetIfChanged(ref _clientName, value);
        }

        // Property for PhoneNumber
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => this.RaiseAndSetIfChanged(ref _phoneNumber, value);
        }

        // Property for Email
        public string Email
        {
            get => _email;
            set => this.RaiseAndSetIfChanged(ref _email, value);
        }

        // Property for City
        public string City
        {
            get => _city;
            set => this.RaiseAndSetIfChanged(ref _city, value);
        }

        // Command to handle any actions, e.g., save or submit
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        private async void AddNewClient()
        {
            if(AccessToClassLibraryBackendProject.AddClient(ClientName, PhoneNumber, Email))
            {

            }

            else
            {

            }
        }

   //     private async void AddNewCategoryToDatabase()
   //     {
   //         if (AccessToClassLibraryBackendProject.InsertNewCategoryOfProduct(CategoryName))
   //         {
   //             await ShowDialogOfAddNewCategoryResponseMessage.Handle("لقد تمت إضافة الفئة الجديدة بنجاح");
   //
   //             categoryProductsViewModel.ReloadProductsCategories();
   //         }
   //
   //         else
   //         {
   //             await ShowDialogOfAddNewCategoryResponseMessage.Handle("لقد حصل خطأ ما ");
   //         }
   //     }
        public AddNewClientViewModel(ViewModelBase mainViewModel)
        {
            // Initialize the command (e.g., to save or submit the client data)
            SaveCommand = ReactiveCommand.Create(AddNewClient);
          
        }


    }
}

