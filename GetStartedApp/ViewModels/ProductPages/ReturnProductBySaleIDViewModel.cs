using GetStartedApp.Models;
using GetStartedApp.Models.Objects;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStartedApp.ViewModels.ProductPages
{
    public class ReturnProductBySaleIDViewModel : ViewModelBase
    {

      ProductSoldInfos productsSoldInfo;
      DataTable ProductsSoldTable;

      private List<ReturnedProduct> _productsToReturnList;
      public List<ReturnedProduct> ProductsToReturnList { get => _productsToReturnList; set => this.RaiseAndSetIfChanged(ref _productsToReturnList, value); }

     
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
                  product.IsCheckedForReturn = value;
              }
          }
      }
        public ReturnProductBySaleIDViewModel(int saleID) {

             productsSoldInfo = AccessToClassLibraryBackendProject.LoadProductSoldInfoFromReader(saleID);
             ProductsToReturnList = ConvertDataTableToProductSoldList(productsSoldInfo.ProductsBoughtInThisOperation);
        }

        public List<ReturnedProduct> ConvertDataTableToProductSoldList(DataTable productsDataTable)
        {
            List<ReturnedProduct> returnedProductsList = new List<ReturnedProduct>();

            foreach (DataRow row in productsDataTable.Rows)
            {
                // Create a new ProductSold object using values from the DataRow
                var productSold = new ReturnedProduct(
                    productId: row.Field<long>("ProductID"),
                    productName: row.Field<string>("ProductName"),
                    originalPrice: Convert.ToSingle(row.Field<decimal>("UnitPrice")),  // Convert from decimal to float
                    soldPrice: Convert.ToSingle(row.Field<decimal>("UnitSoldPrice")),  // Convert from decimal to float
                    quantitiy: row.Field<int>("QuantitySold"),
                    Image: null
                );


                // Add the ProductSold object to the list
                returnedProductsList.Add(productSold);
            }

            return returnedProductsList;
        }
    }
}
