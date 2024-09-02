using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Avalonia.Media.Imaging;
using GetStartedApp.Helpers;
using GetStartedApp.Helpers.CustomUIErrorAttributes;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Subjects;
using GetStartedApp.Models;
using System.Diagnostics;


namespace GetStartedApp.ViewModels.ProductPages
{
    public class ReturnProductViewModel : EditAllProductInfoViewModel
    {
        public override string ProductBtnOperation => " ارجاع منتج ";

        const int maxNumberOfProductsCanBeReturnedAtOnce = 1000;
        const int maxNumberPriceOfProductCanWorth = 100_000;

        private string _numberOfProductsReturned;

        [PositiveIntRange(1, maxNumberOfProductsCanBeReturnedAtOnce, ErrorMessage = "ادخل رقم موجب وبدون فاصلةاقل من 1000")]
        
        public string NumberOfProductsReturned
        {
            get { return _numberOfProductsReturned; }
            set { 
                this.RaiseAndSetIfChanged(ref _numberOfProductsReturned, value);
                ActivateTheRetrunProductBtnCommandIfUserHasEntredInputCorrectly();
            }
        }
       
        
        private string _priceSoldOfProductsReturned;
        [PositiveFloatRange(1, maxNumberPriceOfProductCanWorth, ErrorMessage = "ادخل رقم موجب اقل من 10000")]
        public string PriceSoldOfProductsReturned
        {
            get { return _priceSoldOfProductsReturned; }
            set {   
                
                this.RaiseAndSetIfChanged(ref _priceSoldOfProductsReturned, value);
                ActivateTheRetrunProductBtnCommandIfUserHasEntredInputCorrectly();
            }
        }

        private float _profitOfProductReturned;

        private Subject<bool> _canReturnProduct = new Subject<bool>();

       

        public ReturnProductViewModel
            (Bitmap productImage,long productid,string productName,string description,float cost,float price,float profit,int stockQuanity,string productCategory,ProductsListViewModel productListTable) 
            : base
            (productImage,productid,productName,description,cost,price,profit,stockQuanity,productCategory,productListTable)
        {
            _profitOfProductReturned = profit;
            DisableAllUserInputs_Except_Returned_ProudctsUnits_And_PriceSold();
            DisableImagePickerBtns();

            // set the btn as disbaled of return product
            _canReturnProduct.OnNext(false);

           AddOrEditOrDeleteProductCommand = ReactiveCommand.Create(AddProductReturnedToDatabse, _canReturnProduct);

            // this section is for testing

        //  returnEachDayOf_2024_ProductId_1_And_2_And_See_If_EachDay_Of_2024_Has_This_ProductId_Returned();

        }

        private void ActivateTheRetrunProductBtnCommandIfUserHasEntredInputCorrectly()
        {
            if (checkIfUserHasEntredDataOfReturnedProductCorrectly()) _canReturnProduct.OnNext(true);
            else _canReturnProduct.OnNext(false);
        }
        private async void AddProductReturnedToDatabse()
        {
           
           bool ReturnProductOperationStateSucedded =  AccessToClassLibraryBackendProject.InsertIntoReturnedProducts
                (_ProductID, _ProductName,int.Parse(NumberOfProductsReturned),float.Parse(PriceSoldOfProductsReturned), _profitOfProductReturned);

            if (ReturnProductOperationStateSucedded) { 

                await ShowMessageBoxDialog.Handle("لقد تمت العملية بنجاح ");
                ProductsListViewModel.ReloadProductListIntoSceen();
            }

            else { await ShowMessageBoxDialog.Handle("لقد حصل خطأ ما"); }
        }
        private bool checkIfUserHasEntredDataOfReturnedProductCorrectly()
        {
           
            // we do this before to check if vairable are initialize if not we should go out becuase going futher and try to check them is going to throw null execption
            // because the object is not yet constructed 
            if(PriceSoldOfProductsReturned==null || NumberOfProductsReturned==null) return false;



            // we check if the attribute of EntredstockQuanity is not rasing an error due to wrong input
            // like non valid character such letters or like * , / and so on
            return 
                UiAttributeChecker.AreThesesAttributesPropertiesValid(this, nameof(NumberOfProductsReturned), nameof(PriceSoldOfProductsReturned))
                &&
                 !string.IsNullOrEmpty(NumberOfProductsReturned)
                 &&
                 !string.IsNullOrEmpty(PriceSoldOfProductsReturned)
                ;
                         

        }

        private void DisableAllUserInputs_Except_Returned_ProudctsUnits_And_PriceSold()
        {
            // ReadOnly is mean Enabled  i was rushed and didnt have a time to edit this property in all parts of the view and viemodel
            IsProductIdReadOnly = false;
            IsProductNameReadOnly = false;
            IsProductDescriptionReadOnly = false;
            IsPriceReadOnly = false;
            IsCostReadOnly = false;
            IsStockQuantityReadOnly = false;
           


        }

        private void DisableImagePickerBtns()
        {

            PickImageCommand = ReactiveCommand.CreateFromTask(PickImageProduct, Observable.Return(false));

            DeleteImageCommand = ReactiveCommand.CreateFromTask(DisplayNoImage, Observable.Return(false));
        }


        // this section below is dedicated for tests 



        void returnEachDayOf_2024_ProductId_1_And_2_And_See_If_EachDay_Of_2024_Has_This_ProductId_Returned()
        {
            long productId1 = 1;
            string productName1 = "Product 1";
            long productId2 = 2;
            string productName2 = "Product 2";
            int numberOfProductsReturned = 1; // Assuming one product returned each time
            float priceSoldOfProductsReturned = 100; // Example price sold
            float profitOfProductReturned = 50; // Example profit

            for (int month = 1; month <= 12; month++)
            {
                for (int day = 1; day <= DateTime.DaysInMonth(2024, month); day++)
                {
                    DateTime returnDate = new DateTime(2024, month, day);

                    // Return ProductId 1
                    bool return1Succeeded = AccessToClassLibraryBackendProject.InsertIntoReturnedProducts(
                        productId1,
                        productName1,
                        numberOfProductsReturned,
                        priceSoldOfProductsReturned,
                        profitOfProductReturned
                    );

                    // Return ProductId 2
                    bool return2Succeeded = AccessToClassLibraryBackendProject.InsertIntoReturnedProducts(
                        productId2,
                        productName2,
                        numberOfProductsReturned,
                        priceSoldOfProductsReturned,
                        profitOfProductReturned
                    );

                    // Check if returns were successful
                    if (return1Succeeded)
                    {
                        Debug.WriteLine($"Product with ID {productId1} returned successfully on {returnDate.ToShortDateString()}");
                    }
                    else
                    {
                        Debug.WriteLine($"Failed to return product with ID {productId1} on {returnDate.ToShortDateString()}");
                    }

                    if (return2Succeeded)
                    {
                        Debug.WriteLine($"Product with ID {productId2} returned successfully on {returnDate.ToShortDateString()}");
                    }
                    else
                    {
                        Debug.WriteLine($"Failed to return product with ID {productId2} on {returnDate.ToShortDateString()}");
                    }
                }
            }
        }
        }
}
