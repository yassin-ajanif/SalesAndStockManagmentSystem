using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using GetStartedApp.Models;
using GetStartedApp.ViewModels.DashboardPages;
using System.Reactive.Linq;

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

        private string _clientActionBtnName;
        public string ClientActionBtnName
        {
            get { return _clientActionBtnName; }
            set { this.RaiseAndSetIfChanged(ref _clientActionBtnName, value); }
        }

        public Interaction<string, Unit> ShowDialogOfAddNewClientResponseMessage { get; set; }
        // Command to handle any actions, e.g., save or submit
        public ReactiveCommand<Unit, Unit> AddOrEditOrDeleteClient { get; set; }

        public ClientsListViewModel ClientsListViewModel { get; set; }

        private async void AddNewClientToDatabase()
        {
            if(AccessToClassLibraryBackendProject.AddClient(ClientName, PhoneNumber, Email))
            {
                await ShowDialogOfAddNewClientResponseMessage.Handle("لقد تمت اضافة زبون جديد بنجاح ");

                ClientsListViewModel.ReloadClients();
            }

            else
            {
                await ShowDialogOfAddNewClientResponseMessage.Handle("لقد حصل خطأ ما ");
            }
        }


        public AddNewClientViewModel(ClientsListViewModel clientsListViewModel)
        {
            ClientActionBtnName = "إضافة زبون";
            // Initialize the command (e.g., to save or submit the client data)
            AddOrEditOrDeleteClient = ReactiveCommand.Create(AddNewClientToDatabase);

            ShowDialogOfAddNewClientResponseMessage = new Interaction<string, Unit>();

            ClientsListViewModel = clientsListViewModel;

        }


    }
}

