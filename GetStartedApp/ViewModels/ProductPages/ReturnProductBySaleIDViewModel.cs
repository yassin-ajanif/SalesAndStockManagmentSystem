using GetStartedApp.Models;
using GetStartedApp.Models.Objects;
using ReactiveUI;
using System;
using System.Reactive;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData.Binding;
using DynamicData;
using System.Reactive.Subjects;

namespace GetStartedApp.ViewModels.ProductPages
{
    public class ReturnProductBySaleIDViewModel : ViewModelBase
    {

      ProductSoldInfos productsSoldInfo;
      DataTable ProductsSoldTable;

      private ObservableCollection<ReturnedProduct> _productsToReturnList;
      public ObservableCollection<ReturnedProduct> ProductsToReturnList { 
            get => _productsToReturnList; 
            set => this.RaiseAndSetIfChanged(ref _productsToReturnList, value);
        }

     
      private bool _isAllChecked;
      public bool IsAllChecked
      {
          get => _isAllChecked;
          set
          {
              this.RaiseAndSetIfChanged(ref _isAllChecked, value);
              // When header checkbox is toggled, update all items
              foreach (var product in ProductsToReturnList)
              {
                    //product.IsCheckedForReturn = value;
                    if (product.IsProductReturnable)
                    {
                        product.IsCheckedForReturn = value;

                        // when we check all the products must be returned all and each prduct must have the maximum quanity to return this is the job of the first if statement
                        if (isThisProductCheckedForReturn(value)) SetTheMaximumUnitsToReturnPerThisProduct(product);
                        // else if we do the reverse we uncheck the all product all product are going back to their original values 
                        else if (isThisProductUnCheckedForReturn(value)) SetThePreviousUnitsReturnedToPerThisProduct(product);
                    }              
              }
          }
      }

        public ReactiveCommand<Unit, Unit> ReturnCommand { get; }

        private BehaviorSubject<bool> _isOneOfProductToReturnIsEdited = new BehaviorSubject<bool>(false);
        public IObservable<bool> IsOneOfProductToReturnIsEdited { get => _isOneOfProductToReturnIsEdited; set => _isOneOfProductToReturnIsEdited.AsObservable(); }
        public ReturnProductBySaleIDViewModel(int saleID) {

             productsSoldInfo = AccessToClassLibraryBackendProject.LoadProductSoldInfoFromReader(saleID);
             ProductsToReturnList = ConvertDataTableToProductSoldList(productsSoldInfo.ProductsBoughtInThisOperation);
             ReturnCommand = ReactiveCommand.Create(saveReturnedProductsToDatabase, IsOneOfProductToReturnIsEdited);

             CheckIfUserHasEditedProductsToReturnEvery500ms();
        }

        public ObservableCollection<ReturnedProduct> ConvertDataTableToProductSoldList(DataTable productsDataTable)
        {
            ObservableCollection<ReturnedProduct> returnedProductsList = new ObservableCollection<ReturnedProduct>();

            foreach (DataRow row in productsDataTable.Rows)
            {
                // Create a new ProductSold object using values from the DataRow
                var productSold = new ReturnedProduct(
                    productId: row.Field<long>("ProductID"),
                    productName: row.Field<string>("ProductName"),
                    originalPrice: Convert.ToSingle(row.Field<decimal>("UnitPrice")),  // Convert from decimal to float
                    soldPrice: Convert.ToSingle(row.Field<decimal>("UnitSoldPrice")),  // Convert from decimal to float
                    quantitiy: row.Field<int>("QuantitySold"),
                    Image: null,
                    soldItemID : row.Field<int>("SoldItemID")
                );


                // Add the ProductSold object to the list
                returnedProductsList.Add(productSold);
            }

            return returnedProductsList;
        }
   
        private bool isThisProductCheckedForReturn(bool isChecked) => isChecked;
        private bool isThisProductUnCheckedForReturn(bool isChecked) => !isChecked;
        private void SetTheMaximumUnitsToReturnPerThisProduct(ReturnedProduct returnedProduct)
        {
           returnedProduct.QuanityToReturn= returnedProduct.maximumProductsUserCanReturn.ToString();
        }
        private void SetThePreviousUnitsReturnedToPerThisProduct(ReturnedProduct returnedProduct)
        {
            returnedProduct.QuanityToReturn = returnedProduct.PreviousReturnedQuantity;
        }
        private bool userHasEditedOneOrMoreProductsToReturn()
        {
          
           foreach(ReturnedProduct productToReturn in ProductsToReturnList)
            {
                if (productToReturn.IsCheckedForReturn)
                {
                    return true; 

                }
            }

            return false;
        }
        private void CheckIfUserHasEditedProductsToReturnEvery500ms()
        {
            Observable.Interval(TimeSpan.FromMilliseconds(500))
                .Select(_ => userHasEditedOneOrMoreProductsToReturn())
                .DistinctUntilChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(isEdited =>
                {
                    // Emit the new value into the BehaviorSubject
                    _isOneOfProductToReturnIsEdited.OnNext(isEdited);
                });
        }

        // check products to return are proudct being detected to return 
        public DataTable GetCheckedProductsToReturnAsDataTable()
        {
            // Create a new DataTable instance
            DataTable productsTable = new DataTable();

            // Define the columns of the table
            productsTable.Columns.Add("ProductID", typeof(long));
            productsTable.Columns.Add("ProductName", typeof(string));
            productsTable.Columns.Add("OriginalPrice", typeof(float));
            productsTable.Columns.Add("SoldPrice", typeof(float));
            productsTable.Columns.Add("Quantity", typeof(int));
            productsTable.Columns.Add("QuantityToReturn", typeof(string));
            productsTable.Columns.Add("PreviousReturnedQuantity", typeof(string));
            productsTable.Columns.Add("SoldItemID", typeof(int));


            // Iterate over the ProductsToReturnList observable collection
            foreach (var product in ProductsToReturnList)
            {
                // Only add the products that are checked for return
                if (product.IsCheckedForReturn)
                {
                    // Create a new DataRow
                    DataRow row = productsTable.NewRow();

                    // Assign values to the DataRow from the product object
                    row["ProductID"] = product.ProductId;
                    row["ProductName"] = product.ProductName;
                    row["OriginalPrice"] = product.OriginalPrice;
                    row["SoldPrice"] = product.SoldPrice;
                    row["Quantity"] = product.Quantity;
                    row["QuantityToReturn"] = product.QuanityToReturn;
                    row["PreviousReturnedQuantity"] = product.PreviousReturnedQuantity;
                    row["SoldItemID"] = product.SoldItemID;

                    // Add the row to the DataTable
                    productsTable.Rows.Add(row);
                }
            }

            // Return the DataTable with the filtered products
            return productsTable;
        }

        private void saveReturnedProductsToDatabase()
        {
            var productsTableToReturn = GetCheckedProductsToReturnAsDataTable();
        }
    }
}
