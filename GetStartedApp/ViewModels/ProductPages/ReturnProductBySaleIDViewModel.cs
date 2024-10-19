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

        private List<ProductSold> _productsToReturnList;
        public List<ProductSold> ProductsToReturnList { get => _productsToReturnList; set => this.RaiseAndSetIfChanged(ref _productsToReturnList, value); }

        public ReturnProductBySaleIDViewModel(int saleID) {

             productsSoldInfo = AccessToClassLibraryBackendProject.LoadProductSoldInfoFromReader(saleID);
             ProductsToReturnList = ConvertDataTableToProductSoldList(productsSoldInfo.ProductsBoughtInThisOperation);
        }

        public List<ProductSold> ConvertDataTableToProductSoldList(DataTable productsDataTable)
        {
            List<ProductSold> productSoldList = new List<ProductSold>();

            foreach (DataRow row in productsDataTable.Rows)
            {
                // Create a new ProductSold object using values from the DataRow
                var productSold = new ProductSold(
                    productId: row.Field<long>("ProductID"),
                    productName: row.Field<string>("ProductName"),
                    originalPrice: Convert.ToSingle(row.Field<decimal>("UnitPrice")),  // Convert from decimal to float
                    soldPrice: Convert.ToSingle(row.Field<decimal>("UnitSoldPrice")),  // Convert from decimal to float
                    quantity: row.Field<int>("QuantitySold"),
                    image: null
                );


                // Add the ProductSold object to the list
                productSoldList.Add(productSold);
            }

            return productSoldList;
        }
    }
}
