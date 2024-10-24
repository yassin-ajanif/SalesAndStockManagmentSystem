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
      private int _saleID;

      private ObservableCollection<ReturnedProduct> _productsToReturnList;
      public ObservableCollection<ReturnedProduct> ProductsToReturnList { 
            get => _productsToReturnList; 
            set => this.RaiseAndSetIfChanged(ref _productsToReturnList, value);
        }

      public Interaction<string,Unit> ShowMessageBoxDialog { get; }

      private bool _isAllChecked;
      public bool IsAllChecked
      {
          get => _isAllChecked;
          set
          {
              this.RaiseAndSetIfChanged(ref _isAllChecked, value);
            
          }
      }

        public ReactiveCommand<Unit, Unit> ReturnCommand { get; }

        private BehaviorSubject<bool> _isOneOfProductToReturnIsEdited = new BehaviorSubject<bool>(false);
        public IObservable<bool> IsOneOfProductToReturnIsEdited { get => _isOneOfProductToReturnIsEdited; set => _isOneOfProductToReturnIsEdited.AsObservable(); }
        public ReturnProductBySaleIDViewModel(int saleID) {

             productsSoldInfo = AccessToClassLibraryBackendProject.LoadProductSoldInfoFromReader(saleID);
             ProductsToReturnList = ConvertDataTableToProductSoldList(productsSoldInfo.ProductsBoughtInThisOperation);
             ReturnCommand = ReactiveCommand.Create(saveReturnedProductsToDatabase, IsOneOfProductToReturnIsEdited);
             _saleID = saleID;
             ShowMessageBoxDialog = new Interaction<string, Unit>();
             WhenUserCheckAllItemsReturnAllProducts();
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
                    PreviousReturnedUnits: row.Field<int>("ReturnedUnits"),
                    soldItemID : row.Field<int>("SoldItemID")
                );


                // Add the ProductSold object to the list
                returnedProductsList.Add(productSold);
            }

            return returnedProductsList;
        }
   
        private void SetTheMaximumUnitsToReturnPerThisProduct(ReturnedProduct returnedProduct)
        {
           returnedProduct.QuanityToReturn= returnedProduct.maximumProductsUserCanReturn.ToString();
        }
        private void SetTheUnitsReturnedPerThisProductToZero(ReturnedProduct returnedProduct)
        {
            returnedProduct.QuanityToReturn = "0";
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
                .ObserveOn(RxApp.MainThreadScheduler)  // Ensures the code runs on the main thread for UI updates
                .Subscribe(isEdited =>
                {
                    // Emit the new value into the BehaviorSubject
                    _isOneOfProductToReturnIsEdited.OnNext(isEdited);

                    // Call the SetReturnAllCheckbox function to check the remaining quantities
                    SetReturnAllCheckbox_When_User_HasReturnAllProducts_And_Their_Remaining_Quantities();
                });
        }

        public void WhenUserCheckAllItemsReturnAllProducts()
        {
            // Observe changes to the IsAllChecked property
            this.WhenAnyValue(x => x.IsAllChecked).Subscribe(value => { 
                
                if (IsAllChecked) ReturnAllProducts(); });

        }

        private void ReturnAllProducts()
        {
            foreach (var product in ProductsToReturnList)
            {
                //product.IsCheckedForReturn = value;
                if (product.IsProductReturnable) SetTheMaximumUnitsToReturnPerThisProduct(product);
            }

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

        private void SetReturnAllCheckbox_When_User_HasReturnAllProducts_And_Their_Remaining_Quantities()
        {
            foreach (ReturnedProduct product in ProductsToReturnList)
            {
                bool isQuanityToReturnValid = int.TryParse(product.QuanityToReturn, out int _);

                if (!isQuanityToReturnValid) { IsAllChecked = false; return; }
             
                // Calculate the difference between sold quantity, to be returned, and previously returned quantity
                int remainingQuantity = product.Quantity - (int.Parse(product.QuanityToReturn) + int.Parse(product.PreviousReturnedQuantity));
                bool userDidnt_ReturnAllItemsOfThisProduct = remainingQuantity > 0;
                bool userWantedToReturnQuantityMoreThanSold = remainingQuantity < 0;

                // If either condition is met, return early and don't check 'All Checked'
                if (userDidnt_ReturnAllItemsOfThisProduct || userWantedToReturnQuantityMoreThanSold) { IsAllChecked = false; return; }
               
            }

            // If all products are fully returned, check the 'All Checked' checkbox
            IsAllChecked = true;
        }
      
        private async void saveReturnedProductsToDatabase()
        {
            var productsTableToReturn = GetCheckedProductsToReturnAsDataTable();

            if(AccessToClassLibraryBackendProject.ProcessProductReturns(_saleID, productsTableToReturn))
            {
                await ShowMessageBoxDialog.Handle("تمت العملية بنجاح");
            }

            else { await ShowMessageBoxDialog.Handle("حصل خطأ ما"); }
        }
    }
}
