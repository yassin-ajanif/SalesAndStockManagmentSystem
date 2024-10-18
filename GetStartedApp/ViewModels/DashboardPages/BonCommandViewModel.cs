using GetStartedApp.Helpers;
using GetStartedApp.Models;
using GetStartedApp.Models.Objects;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStartedApp.ViewModels.DashboardPages
{
    public class BonCommandViewModel:BonReceptionViewModel
    {

        public  ReactiveCommand<Unit,Unit> GenerateBonCommand_Command { get; }
        public BonCommandViewModel(MainWindowViewModel mainWindowViewModel):base(mainWindowViewModel) {

            GenerateBonCommand_Command = ReactiveCommand.Create(GenerateBonCommandPdf, CheckIfSystemIsNotRaisingError_And_ExchangeIsPositiveNumber_And_ProductListIsNotEmpty_Every_500ms());
        }

        private DataTable LoadListOfProductsToCommand() {

            DataTable ProductsTableToCommand = new DataTable();
            ProductsTableToCommand.Columns.Add("ProductId", typeof(long));
            ProductsTableToCommand.Columns.Add("ProductName", typeof(string));
            ProductsTableToCommand.Columns.Add("Description", typeof(string));
            ProductsTableToCommand.Columns.Add("QuantityToCommand", typeof(int));
            ProductsTableToCommand.Columns.Add("Price", typeof(decimal));



            foreach (var product in ProductsListScanned_To_Recive)
            {
                // Create a new row for the DataTable
                DataRow row = ProductsTableToCommand.NewRow();

                // Assign values to the row
                row["ProductId"] = product.ProductInfo.id;
                row["ProductName"] = product.ProductInfo.name;
                row["Description"] = product.ProductInfo.description;
                row["Price"] = product.ProductInfo.price;
                // i use this productto reduce wich is wrong naming because it actually were working in the base class that sales but in this cases these products are to command
                // due the time pression i will go back and change the name of prorpties to be more expressive
                row["QuantityToCommand"] = Convert.ToInt16(product.ProductsUnitsToReduce_From_Stock1) + 
                    Convert.ToInt16(product.ProductsUnitsToReduce_From_Stock2) +
                    Convert.ToInt16(product.ProductsUnitsToReduce_From_Stock3);


                // Add the row to the DataTable
                ProductsTableToCommand.Rows.Add(row);
            }

            return ProductsTableToCommand;


        }

        protected override void WhenUserSetInvalidProduct_Price_Or_Quantity_Block_TheSystem_From_Adding_NewProducts_AndShowError_Plus_MakeAdditional_Checkings()
        {

            this.WhenAnyValue(x => x.ProductsListScanned_To_Recive.Count, x => x.BonReceptionNumber, x => x.EntredSupplierName_PhoneNumber)
              .Subscribe(async BarcodeNumberScanned =>
              {

                  deleteAllErrors_When_ProductList_Empty();

                  foreach (var product in ProductsListScanned_To_Recive)
                  {
                      // Observe each property of each product
                      product.
                    WhenAnyValue(
                          p => p.ProductsUnits,
                          p => p.PriceOfProductSold,
                          p => p.ProductsUnitsToReduce_From_Stock1,
                          p => p.ProductsUnitsToReduce_From_Stock2,
                          p => p.ProductsUnitsToReduce_From_Stock3


                          )
                          .Subscribe(_ =>
                          {
                              // this function iside has error to show
                              if (IsEntredSupplierName_PhoneNumber_IsNotExisting()) displayErrorMessage("يرجى إدخال مورد مسجل.");
                              else if (!CheckIf_ProductsUnits_And_SoldPrices_Of_ScannedProducts_Are_Valid()) displayErrorMessage("هناك خطأ في كمية المنتج او السعر");
                              else deleteDisplayedError();

                              CalculateTheTotalPriceOfOperation();

                          }
                          );
                  }
              }
                );
        }

        private void GenerateBonCommandPdf()
        {
            string SelectedPaymentMethodInFrench = WordTranslation.TranslatePaymentIntoTargetedLanguage(SelectedPaymentMethod, "fr");
            string SupplierPhoneNumber = PhoneNumberExtractor.ExtractPhoneNumber(EntredSupplierName_PhoneNumber);

            string supplierName = string.Empty;
            string phoneNumber = SupplierPhoneNumber;
            string email = string.Empty;
            string bankAccount = string.Empty;
            string fiscalIdentifier = string.Empty;
            string rc = string.Empty;
            string ice = string.Empty;
            string patented = string.Empty;
            string cnss = string.Empty;
            string address = string.Empty;

            // Retrieve supplier info from the database
            AccessToClassLibraryBackendProject.getSupplierInfo(phoneNumber, ref supplierName, ref bankAccount,
                ref fiscalIdentifier, ref rc, ref ice, ref patented, ref cnss, ref address);

            DataTable productsTableToCommand = LoadListOfProductsToCommand();


            AccessToClassLibraryBackendProject.
                GenerateBonCommand(0, productsTableToCommand, SelectedPaymentMethodInFrench, 0,0,0,DateTime.Now,supplierName,phoneNumber,email,bankAccount,fiscalIdentifier,rc,ice,patented,cnss,address);

        }
    }
}
