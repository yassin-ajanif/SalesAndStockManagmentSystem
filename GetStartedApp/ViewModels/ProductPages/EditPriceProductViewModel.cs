using Avalonia.Media.Imaging;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetStartedApp.Helpers;
using System.Reactive.Linq;
using GetStartedApp.Models;
using System.Diagnostics;

namespace GetStartedApp.ViewModels.ProductPages
{
    public class EditPriceProductViewModel : EditStockQuantitiyProductViewModel
    {
        public override string ProductBtnOperation => " تعديل ثمن المنتج ";

        private string originalProductPrice;
        private string originalProductCost;

        public EditPriceProductViewModel(Bitmap productImage, long productID,
            string productName, string Description,
            float cost, float price, float profit,
            int stockQuantity, int stockQuantity2, int stockQuantity3,
            string selectedCategory,ProductsListViewModel productsList)
            : base(productImage, productID, productName, Description, cost, price, profit, 
                  stockQuantity,stockQuantity2,stockQuantity3, selectedCategory,productsList)
        {
            
            DisableAllInputsAndLet_ProductCostAndProductPrice_Only();

            // we set Entredproduct price and cost displayed values as original refrences 
            // we check them later to enable a btn command of editing price and cost 
            originalProductPrice = EnteredPrice;
            originalProductCost = EntredCost;

            // we override our property command to make it this time to edit product price and cost 
            AddOrEditOrDeleteProductCommand = ReactiveCommand.Create(EditProductPrice, checkIfProductPriceAndCost_AreEditedCorreclty());


            // this section below is for testing 

           // EditProductPrice_WithValidPrice();

           // EditProductPrice_WithInValidPrice();
        }


        private void DisableAllInputsAndLet_ProductCostAndProductPrice_Only()
        {
            // ReadOnly is mean Enabled  i was rushed and didnt have a time to edit this property in all parts of the view and viemodel
         
            IsStockQuantityReadOnly = !true;
            IsStockQuantityReadOnly2 = !true;
            IsStockQuantityReadOnly3 = !true;
            IsPriceReadOnly = !false;
            IsCostReadOnly = !false;

            // the rest of UI properties are already set in the base class 
        }


        // if  a user changes the product cost or price then we enable a ProductBtnOperation command to do that change action
        private bool userHasChanged_ProductPrice_Or_ProductCost() {


            return originalProductPrice != EnteredPrice || originalProductCost != EntredCost;


        }
        
        // to consider if product is edit correctly we must meet theses conditons:
        // the product price or cost ui case must be a valid string positive number not empty case
        // the value of the price or cost must be different from the old OriginalProductQuantityStock one
        private IObservable<bool> checkIfProductPriceAndCost_AreEditedCorreclty()
        {

            var canEditProductQuantity = this.WhenAnyValue(
                             x => x.EnteredPrice,
                             x => x.EntredCost,
                             x => x.CalculatedBenefit,
                             (EnteredPrice, EntredCost, CalculatedBenefit) =>

                             !string.IsNullOrEmpty(EnteredPrice) &&
                             !string.IsNullOrEmpty(EntredCost) &&
                             !string.IsNullOrEmpty(CalculatedBenefit) &&
                              userHasChanged_ProductPrice_Or_ProductCost()&& 
                             // we check if the attribute of entred product price and cost and also benift calculated from them
                             // if they're not rasing an error due to wrong input or
                             // not enough benefit
                             // like non valid character such letters or like * , / and so on
                             UiAttributeChecker.AreThesesAttributesPropertiesValid(this,nameof(EnteredPrice),nameof(EntredCost),nameof(CalculatedBenefit))

                                                                  );
            return canEditProductQuantity;


        }

        
         private async void EditProductPrice()
        {
            long productChosen = _ProductID;
            float newProductCost = _Cost;
            float newProductPrice = _Price;

           if(AccessToClassLibraryBackendProject.UpdateProductPriceOrCost(productChosen, newProductCost, newProductPrice))
            { 
                await ShowMessageBoxDialog.Handle("تم تعديل تمن المنتج بنجاح");
                
                ProductsListViewModel.ReloadProductListIntoSceen();
            }
               

            else await ShowMessageBoxDialog.Handle("هناك مشكلة في تعديل هذا المنتج");
        }
   
    
    // this section is for testing 


        //async void EditProductPrice_WithValidPrice()
        //{
        //   for(long i=1; i<=10_000; i++) { 

        //    EntredCost = GenerateRandomValid_ProductCost();
        //    EnteredPrice = GenerateRandomValid_ProductPrice();

        //    long productChosen = i;
        //    float newProductCost = _Cost;
        //    float newProductPrice = _Price;

        //    bool isPriceEditedValid = await checkIfProductPriceAndCost_AreEditedCorreclty().FirstAsync();

        //        if (!isPriceEditedValid) { Debug.WriteLine($" UI STAGE : this product should be edit at productid{_ProductID} at cost {_Cost} and price {_Price}"); return; }

        //        if (!AccessToClassLibraryBackendProject.UpdateProductPriceOrCost(productChosen, newProductCost, newProductPrice))
        //        {
        //            Debug.WriteLine($" DATABSE STAGE : this product should be edit at productid{_ProductID} at cost {_Cost} and price {_Price}");
        //            return;
        //        }
        //    }


        //    Debug.WriteLine("operation of valid price edition succeded");
        
        //}

        //async void EditProductPrice_WithInValidPrice()
        //{
        //    for (int i = 1; i <= 10_000; i++)
        //    {
      
        //        int productChosen = i;
        //        EntredCost = GenerateRandomInvalid_ProductCost();
        //        EnteredPrice = GenerateRandomInvalid_ProductPrice();
  
        //        bool isPriceEditedValid = await checkIfProductPriceAndCost_AreEditedCorreclty().FirstAsync();

        //        if (isPriceEditedValid)
        //        {
        //            Debug.WriteLine($" UI STAGE : this product should not be edit at productid{_ProductID} at cost {EntredCost} and price {EnteredPrice}");
        //            return;
        //        }

        //    }


        //    Debug.WriteLine("operation of invalid price editing succeded");

        //}





    }

}
