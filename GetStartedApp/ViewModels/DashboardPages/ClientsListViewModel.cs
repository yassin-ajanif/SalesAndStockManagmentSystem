using GetStartedApp.Models;
using GetStartedApp.ViewModels.ProductPages;
using ReactiveUI;
using System.Reactive;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using GetStartedApp.ViewModels.ClientsPages;
using GetStartedApp.ViewModels.DashboardPages;

namespace GetStartedApp.ViewModels.DashboardPages
{
    public class ClientsListViewModel: ViewModelBase
    {
      
          // Observable collection for clients list
            public List<string> _ClientsList;
            public List<string> ClientsList
            {
                get { return _ClientsList; }
                set { this.RaiseAndSetIfChanged(ref _ClientsList, value); }
            }

            // Selected client
            private string _selectedClient;
            public string SelectedClient
            {
                get => _selectedClient;
                set => this.RaiseAndSetIfChanged(ref _selectedClient, value);
            }

            // Interaction for add new client dialog
            public Interaction<AddNewClientViewModel, Unit> ShowAddNewClientDialog { get; }

            // Command for adding new client
            public ReactiveCommand<Unit, Unit> AddNewClientCommand { get; }

            // Constructor
            public ClientsListViewModel(MainWindowViewModel mainWindowViewModel)
            {
                LoadClientList();

                ShowAddNewClientDialog = new Interaction<AddNewClientViewModel, Unit>();

                AddNewClientCommand = ReactiveCommand.Create(AddNewClient);
            }

            // Method to reload clients
            public void ReloadClients()
            {
                LoadClientList();
            }

            // Method to add a new client
            private async void AddNewClient()
            {
                var userControlToShowInsideDialog = new AddNewClientViewModel(this);
                await ShowAddNewClientDialog.Handle(userControlToShowInsideDialog);
            }

            // Method to delete selected client
            public bool DeleteClient()
            {
              string PhoneClientNumber = PhoneNumberExtractor.ExtractPhoneNumber(SelectedClient);

              if (AccessToClassLibraryBackendProject.DeleteClient(PhoneClientNumber))
              {
                  ReloadClients();
                  return true;
              }
              return false;
            }


            // Method to load the client list
            private void LoadClientList()
            {
                ClientsList = AccessToClassLibraryBackendProject.GetClientNames_And_Their_Phones_As_String();
            }

        }
    }

