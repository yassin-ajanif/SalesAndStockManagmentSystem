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
            public ObservableCollection<string> _ClientsList;
            public ObservableCollection<string> ClientsList
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

                ClientsList = new ObservableCollection<string>(ClientNames);

                ShowAddNewClientDialog = new Interaction<AddNewClientViewModel, Unit>();

                AddNewClientCommand = ReactiveCommand.Create(AddNewClient);
            }

            // Method to reload clients
            public void ReloadClients()
            {
                LoadClientList();
                ClientsList = new ObservableCollection<string>(ClientNames);
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
         //      if (AccessToClassLibraryBackendProject.DeleteClient(SelectedClient))
         //      {
         //          ReloadClients();
         //          return true;
         //      }
              return false;
            }

            // Method to load the client list
            private void LoadClientList()
            {
                ClientNames = new List<string> { "Client 1", "Client 2", "Client 3" };
            }

            // Temporary property to simulate loading from a backend
            public List<string> ClientNames { get; private set; }
        }
    }

