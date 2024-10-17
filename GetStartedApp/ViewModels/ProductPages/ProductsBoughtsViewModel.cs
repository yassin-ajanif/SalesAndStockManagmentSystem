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
using GetStartedApp.ViewModels.DashboardPages;

namespace GetStartedApp.ViewModels.ProductPages
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
        [CheckForInvalidCharacters]
        [MaxStringLengthAttribute_IS(50, "هذه الجملة طويلة جدا")]
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
        private List<BonReception> _bonReceptions;
        public List<BonReception> BonReceptions
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
                    nameof(BarcodeNumber),
                    nameof(ProductNameTermToSerach),
                    nameof(SelectedSupplier),
                    nameof(SupplierNameEnteredByUser),
                    nameof(MinAmount),
                    nameof(MaxAmount))
                &&
                TheSystemIsNotShowingError()
            ) ;

            return canAddProduct;
        }



        public ProductsBoughtsViewModel(MainWindowViewModel mainWindowViewModel) : base(mainWindowViewModel)
        {

            GetBoughtProductListFromDbCommand = ReactiveCommand.Create(GetBoughtProductListFromDatabase, CheckIfUserDidintMakeSearchMistake());
            SuppliersList = AccessToClassLibraryBackendProject.GetSupplierNamePhoneNumberCombo();
        }

    //    protected override void when_UserSearchProductByName_DeleteBarcode_SearchText_And_ViceSera()
    //    {
    //        // Listen to changes in ProductNameTermToSerach
    //        this.WhenAnyValue(x => x.ProductNameTermToSerach)
    //            .Subscribe(productName =>
    //            {
    //                // If the user types in ProductNameTermToSerach, clear BarcodeNumber
    //                if (!string.IsNullOrWhiteSpace(productName))
    //                {
    //                    BarcodeNumber = null;
    //                }
    //            });
    //
    //        // Listen to changes in BarcodeNumber
    //        this.WhenAnyValue(x => x.BarcodeNumber)
    //            .Subscribe(barcode =>
    //            {
    //                // If the user types in BarcodeNumber, clear ProductNameTermToSerach and SelectedProductNameTermSerach
    //                if (!string.IsNullOrWhiteSpace(barcode))
    //                {
    //                    ProductNameTermToSerach = null;
    //                    SelectedProductNameTermSerach = null;
    //                }
    //            });
    //    }

        private void GetBoughtProductListFromDatabase()
        {
            // Extract supplier name from the selected combo box input
            string supplierName = StringHelper.ExtractNameFrom_Combo_NamePhoneNumber(SelectedSupplier);
            // Convert the barcode number, min amount, and max amount from strings to their respective types
            long? barcodeNumber = long.TryParse(BarcodeNumber, out long result) ? result : (long?)null;
            decimal? minAmount = decimal.TryParse(MinAmount, out decimal minResult) ? minResult : (decimal?)null;
            decimal? maxAmount = decimal.TryParse(MaxAmount, out decimal maxResult) ? maxResult : (decimal?)null;
            // productNameTermtoserach when you set it to null it dosent it get back to string to empty this is due the algroithm of custom tag i made it a bug so
            // for this reason i defined this variable to ensure the nullability
            string ProductName = string.IsNullOrEmpty(ProductNameTermToSerach) ? null : ProductNameTermToSerach;
            string SupplierNumber = string.IsNullOrEmpty(BonReceptionID) ? null : BonReceptionID;
            string selectedPaymentMethodInEnglish = WordTranslation.TranslatePaymentIntoTargetedLanguage(SelectedPaymentMethod, "en");

            // Retrieve the list of BonReceptions from the backend
            BonReceptions = AccessToClassLibraryBackendProject.RetrieveBonReceptions(
                StartDate.DateTime,
                EndDate.DateTime,
                SupplierNumber,
                supplierName,
                barcodeNumber,
                ProductName,
                operationTypeName: null,
                costProduct: null,
                minAmount,
                maxAmount,
                selectedPaymentMethodInEnglish
            );
        }





    }
}
