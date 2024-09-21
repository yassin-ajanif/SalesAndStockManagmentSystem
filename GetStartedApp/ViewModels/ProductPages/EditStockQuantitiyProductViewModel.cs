//using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using GetStartedApp.Helpers.CustomUIErrorAttributes;
using GetStartedApp.Helpers;
using ReactiveUI;
using System.Reactive.Linq;
using System.Diagnostics;
using System.Reactive;
using GetStartedApp.Models;

namespace GetStartedApp.ViewModels.ProductPages
{
    public class EditStockQuantitiyProductViewModel : AddProductViewModel
    {


     protected long _ProductID;
   
     private string _EntredProductID; 
     public string EntredProductID
       {
           get { return _EntredProductID; }
   
           set
           {
   
               this.RaiseAndSetIfChanged(ref _EntredProductID, value);
   
               DataEntryPropertyLoader.ConvertStringToIntAndLoadPrivateProperty(value, ref _ProductID);
   
   
           }
       }


       
        protected string _ProductName;
        [CheckForInvalidCharacters]
        [StringMustHaveAtLeast_3_Letters(ErrorMessage = "اسم المنتج يجب ان يحتوي على الاقل ثلاث حروف")]
        [MaxStringLengthAttribute_IS(50, "هذه الجملة طويلة جدا")]
     
        public string EntredProductName
        {
            get { return _ProductName; }
            set {  this.RaiseAndSetIfChanged(ref _ProductName, value); }
        }



        //private string _EntredProductDescription;
        protected string _ProductDescription;
        [CheckForInvalidCharacters]
        [MaxStringLengthAttribute_IS(100, "هذه الجملة طويلة جدا")]
        public string EnteredProductDescription
        {
            get { return _ProductDescription; }
            set { this.RaiseAndSetIfChanged(ref _ProductDescription, value); }
        }


        protected string _EntredStockQuantity;
        protected int _StockQuantity;

        [PositiveIntRange(0, maxNumberOfProductsCanSystemHold, ErrorMessage = "ادخل رقم موجب وبدون فاصلة ")]
        public string EntredStockQuantity
        {
            get { return _EntredStockQuantity; }

            set
            {
                //this.RaiseAndSetIfChanged(ref _EntredStockQuantity, value);
                //DataEntryPropertyLoader.ConvertStringToIntAndLoadPrivateProperty(value, ref _StockQuantity);
            }

        }

        protected string _EntredStockQuantity2;
        protected int _StockQuantity2;

        [PositiveIntRange(0, maxNumberOfProductsCanSystemHold, ErrorMessage = "ادخل رقم موجب وبدون فاصلة ")]
        public string EntredStockQuantity2
        {
            get { return _EntredStockQuantity; }

            set
            {
                this.RaiseAndSetIfChanged(ref _EntredStockQuantity2, value);
            //    DataEntryPropertyLoader.ConvertStringToIntAndLoadPrivateProperty(value, ref _StockQuantity2);
            }

        }

        protected string _EntredStockQuantity3;
        protected int _StockQuantity3;

        [PositiveIntRange(0, maxNumberOfProductsCanSystemHold, ErrorMessage = "ادخل رقم موجب وبدون فاصلة ")]
        public string EntredStockQuantity3
        {
            get { return _EntredStockQuantity3; }

            set
            {
                this.RaiseAndSetIfChanged(ref _EntredStockQuantity3, value);
                DataEntryPropertyLoader.ConvertStringToIntAndLoadPrivateProperty(value, ref _StockQuantity3);
            }

        }



        protected string _SelectedCategory;
        public string SelectedCategory
        {
            get { return _SelectedCategory; }
            set { this.RaiseAndSetIfChanged(ref _SelectedCategory, value); }
        }

        public override string ProductBtnOperation => "تعديل كمية المنتج";

        private string OriginalProductQuantityStock;

        protected bool _isTheProductNameToEditAlreadyExistInDb;
        // this is teh user when is going to edit product quantitiy 


        public EditStockQuantitiyProductViewModel(
          
            Bitmap productImage, long productID,
            string productName, string Description,
            float cost,float price,float profit,
            int stockQuantity,string selectedCategory,
            ProductsListViewModel ProductsListViewModel

           ) : base(ProductsListViewModel)
        
        {
            // WE NEED to retivece all products categories becuase the selectedCategory variable 
            // checks if the name sent is matching the list of productCategories
            // you can't set productCategory as a random string , the string must be in the productlistCategory binded with xaml code

            ProductCategories = GetProductsCategoryFromDatabase();

           SelectedImageToDisplay = productImage;
           EntredProductID = productID.ToString();
           EntredProductName = productName;
           EnteredProductDescription = Description;
           EntredCost = cost.ToString();
           EnteredPrice = price.ToString();
           EntredStockQuantity = stockQuantity.ToString();
           SelectedCategory = selectedCategory;
          


            // this is first value of quality product stock we use it as refrence to know if the user has edit the productquanity or not 
            // so we can enable or disable to edit product quantity command
           OriginalProductQuantityStock = EntredStockQuantity;

           // this method disable all inputs an their attributes including productname , id , description and others
           // it allows only for a user to edit a quantity
           // which is the EditProductQuantity mode allows

           DisableAllInputsAndLet_StockQuantityInputAlone();

            disableImagePickerBtns();

            // calculate the benefit of product
            DisplayTheBenefitFromPriceAndCost();


            AddOrEditOrDeleteProductCommand = ReactiveCommand.Create(EditProductQuantity, checkIfProductQuantityEditedCorreclty());

            // this part of code is for testing 

          //  EditStockQuantity_WithValid_StockInput();

           // EditStockQuantity_With_InValid_StockInput();

        }

        // this function role is to check if user has incread the quantity 
        // in this case we're going to show barcode printing operation to user
        // if he decreazsed the user for courese will not need to print barcodes
        protected bool CheckIfUserHasIncreasedStockQuantity()
        {
            return _StockQuantity - int.Parse(OriginalProductQuantityStock) > 0;
        }


        protected override void setDefaultBarCodeParameters()
        {
            BarCodeSerieNumber = EntredProductID;

            int NumberOfProductsUserHasAdded = _StockQuantity - int.Parse(OriginalProductQuantityStock);

            NumberOfBarcodes = NumberOfProductsUserHasAdded;
        }

        public override bool isUserAllowedToPrintBarCodes()
        {
            return CheckIfUserHasIncreasedStockQuantity();
        }

        protected void Show_MessageBoxOfPrintingBarcode_Proposition_If_UserHasIncreasedStockQuantity()
        {
            if (CheckIfUserHasIncreasedStockQuantity())
            {
                setDefaultBarCodeParameters();

                AskUserIfHeWantToPrintBarCodes();
            }
        }


       private async void EditProductQuantity()
        {
            long porductIdChosen = _ProductID;
            int newQuantityOfProduct = _StockQuantity;
            int newQuantityOfProduct_Stock2 = _StockQuantity2;
            int newQuantityOfProduct_Stock3 = _StockQuantity3;

            if (AccessToClassLibraryBackendProject.UpdateProductQuantity(porductIdChosen, newQuantityOfProduct, newQuantityOfProduct_Stock2, newQuantityOfProduct_Stock3))

            { await ShowMessageBoxDialog.Handle("تم تعديل كمية المنتج بنجاح");
                ProductsListViewModel.ReloadProductListIntoSceen();

                Show_MessageBoxOfPrintingBarcode_Proposition_If_UserHasIncreasedStockQuantity();
            }

            else await ShowMessageBoxDialog.Handle("هناك مشكلة في تعديل هذا المنتج");

           
        }

        //  when a user edit the product quantity the originaplproductQuantiyStock will not be equal to newest one
        // so this method wil return true
        private bool UserHasEditedTheStockQuantityProduct() {


            return OriginalProductQuantityStock != EntredStockQuantity;
        
        }

        

        // to consider if product is edit correctly we must meet theses conditons:
        // the productquantity ui case must be a valid string positive number not empty case
        // the value of the productquantity stock must be different from the old OriginalProductQuantityStock one
        private IObservable<bool> checkIfProductQuantityEditedCorreclty()
        {

            var canEditProductQuantity = this.WhenAnyValue(
                             x => x.EntredStockQuantity,
                             (EntredStockQuantity) =>

                             !string.IsNullOrEmpty(EntredStockQuantity) &&
                             UserHasEditedTheStockQuantityProduct() &&
                             // we check if the attribute of EntredstockQuanity is not rasing an error due to wrong input
                             // like non valid character such letters or like * , / and so on
                             UiAttributeChecker.AreThesesAttributesPropertiesValid(this,nameof(EntredStockQuantity))&&
                             !_isTheProductNameToEditAlreadyExistInDb

                                                                  ) ;
                return canEditProductQuantity;

            
        }
       protected void DisableAllInputsAndLet_StockQuantityInputAlone()
       {
            // ReadOnly is mean Enabled  i was rushed and didnt have a time to edit this property in all parts of the view and viemodel
           IsProductIdReadOnly = false;
           IsProductNameReadOnly = false;
           IsProductDescriptionReadOnly = false;
           IsCostReadOnly = false;
           IsPriceReadOnly = false;
           IsStockQuantityReadOnly = true; // This is set to true
           IsSelectedCategoryEnabled = false;
    
       }

       protected void disableImagePickerBtns()
        {

            PickImageCommand = ReactiveCommand.CreateFromTask(PickImageProduct, Observable.Return(false));

            DeleteImageCommand = ReactiveCommand.CreateFromTask(DisplayNoImage, Observable.Return(false));
        }


        /* this section is for testing the edit product operation */

        

      
        //async void EditStockQuantity_WithValid_StockInput()
        //{
        //    long numberOfRecordAtDatabase = 10_000;

        //    for(long i=1; i<=numberOfRecordAtDatabase; i++)
        //    {
        //        long porductIdChosen = 1;
        //       // int newQuantityOfProduct = int.Parse(GenerateRandomValid_StockQuantity());
        //          EntredStockQuantity = GenerateRandomValid_StockQuantity();

        //        bool isStockEditedValidAtUiStage = await checkIfProductQuantityEditedCorreclty().FirstAsync();

        //        if (!isStockEditedValidAtUiStage)
        //        {
        //            Debug.WriteLine($" UI STAGE : the stock value {EntredStockQuantity} should be edited at productid {porductIdChosen}");
        //            return;
        //        }

        //        if (!AccessToClassLibraryBackendProject.UpdateProductQuantity(porductIdChosen, int.Parse(EntredStockQuantity)))
        //        {
        //            Debug.WriteLine($" DB STAGE : the stock value {EntredStockQuantity} should be edited at productid {porductIdChosen}");
        //            return;
        //        }

        //    }

        //     Debug.WriteLine("operation of edition succeded");
        //}

        //async void EditStockQuantity_With_InValid_StockInput()
        //{
        //    int numberOfRecordAtDatabase = 10_000;

        //    for (int i = 1; i <= numberOfRecordAtDatabase; i++)
        //    {
        //        int porductIdChosen = 1;
        //         EntredStockQuantity = GenerateRandomInvalid_StockQuantity();

        //        bool isStockEditedValidAtUiStage = await checkIfProductQuantityEditedCorreclty().FirstAsync();

        //        if (isStockEditedValidAtUiStage)
        //        {
        //            Debug.WriteLine($" UI STAGE : the stock value {EntredStockQuantity} should not be edited at productid {porductIdChosen}");
        //            return;
        //        }

        //    }

        //    Debug.WriteLine("operation of invalid edition succeded");
        //}
   
      
    
    }
}
