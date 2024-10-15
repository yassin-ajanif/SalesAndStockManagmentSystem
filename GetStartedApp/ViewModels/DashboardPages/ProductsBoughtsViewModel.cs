using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Windows.Input;
using GetStartedApp.Models.Objects;
using GetStartedApp.Helpers.CustomUIErrorAttributes;
using System.Collections.Generic;
using GetStartedApp.Helpers;
using GetStartedApp.Models;


namespace GetStartedApp.ViewModels.DashboardPages
{
    public class ProductsBoughtsViewModel : BonLivraisonsViewModel
    {
        private const long maxNumberOfProductsCanSystemHold = 1_000_000_000_000_000_000;
        //    // Property for the selected supplier
        private string _selectedSupplier;
        public string SelectedSupplier
        {
            get => _selectedSupplier;
            set => this.RaiseAndSetIfChanged(ref _selectedSupplier, value);
        }
        //
        private string _bonReceptionID;
        [PositiveIntRange(1, 1_000_000, ErrorMessage = "ادخل رقم موجب وبدون فاصلة")]
        public string BonReceptionID { get => _bonReceptionID; set => this.RaiseAndSetIfChanged(ref _bonReceptionID, value); }

        private string _timeOfOperation;
        public string TimeOfOperation
        {
            get => _timeOfOperation;
            set => this.RaiseAndSetIfChanged(ref _timeOfOperation, value);
        }

        //    // Property for user-entered supplier name
        private string _supplierNameEnteredByUser;
        public string SupplierNameEnteredByUser
        {
            get => _supplierNameEnteredByUser;
            set => this.RaiseAndSetIfChanged(ref _supplierNameEnteredByUser, value);
        }

        //    // ObservableCollection for the list of suppliers
        private List<string> _suppliersList;
        public List<string> SuppliersList
        {
            get => _suppliersList;
            set => this.RaiseAndSetIfChanged(ref _suppliersList, value);
        }

        // ObservableCollection for the list of BonReception objects
        private ObservableCollection<BonReception> _bonReceptions;
        public ObservableCollection<BonReception> BonReceptions
        {
            get => _bonReceptions;
            set => this.RaiseAndSetIfChanged(ref _bonReceptions, value);
        }


        private string _supplierNameEntredByUser;
        public string SupplierNameEntredByUser
        {
            get => _supplierNameEntredByUser;
            set => this.RaiseAndSetIfChanged(ref _supplierNameEntredByUser, value);
        }


        public ReactiveCommand<Unit, Unit> GetBoughtProductListFromDbCommand { get; set; }

        protected override IObservable<bool> CheckIfUserDidintMakeSearchMistake()
        {
            var canAddProduct = this.WhenAnyValue(
                x => x.BonReceptionID,
                x => x.BarcodeNumber,
                x => x.ProductNameTermToSerach,
                x => x.SelectedSupplier,
                x => x.SupplierNameEnteredByUser,
                x => x.MinAmount,
                x => x.MaxAmount,
                x => x.isErrorLabelVisible,
                (BonReceptionID, BarCodeNumber, ProductNameTermToSerach, SelectedSupplier, SupplierNameEnteredByUser, MinAmount, MaxAmount, isErrorLabelVisible) =>

                UiAttributeChecker.AreThesesAttributesPropertiesValid(
                    this,
                    nameof(BonReceptionID),
                    nameof(BarCodeNumber),
                    nameof(ProductNameTermToSerach),
                    nameof(SelectedSupplier),
                    nameof(SupplierNameEnteredByUser),
                    nameof(MinAmount),
                    nameof(MaxAmount))
                &&
                TheSystemIsNotShowingError()
            );

            return canAddProduct;
        }



        public ProductsBoughtsViewModel(MainWindowViewModel mainWindowViewModel) : base(mainWindowViewModel) {

            GetBoughtProductListFromDbCommand = ReactiveCommand.Create(GetBoughtProductListFromDatabse);
            SuppliersList = AccessToClassLibraryBackendProject.GetSupplierNamePhoneNumberCombo();
        }


        private void GetBoughtProductListFromDatabse()
        {
            BonReceptions = new ObservableCollection<BonReception>();
          
        }


     

    }
}
