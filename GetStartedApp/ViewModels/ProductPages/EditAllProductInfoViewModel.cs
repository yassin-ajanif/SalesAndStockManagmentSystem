using Avalonia.Media.Imaging;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using System.Reactive.Linq;
using GetStartedApp.Helpers;
using GetStartedApp.Models;
using GetStartedApp.Helpers.CustomUIErrorAttributes;
using System.Diagnostics;
using System.IO.Pipelines;
using GetStartedApp.Models.Objects;
using GetStartedApp.Models.Enums;

namespace GetStartedApp.ViewModels.ProductPages
{
    public class EditAllProductInfoViewModel : EditPriceProductViewModel

    {
        public override string ProductBtnOperation => " تعديل المنتج ";

        private Bitmap _originalProductImage;
        private string _originalProductName;
        private string _originalProductDescription;
        private string _originalProductCost;
        private string _originalProductPrice;
        private string _originalProductQuantity;
        private string _originalProductSelectedCategory;

       

        public EditAllProductInfoViewModel(Bitmap productImage, long productID,
           string productName, string Description,
           float cost, float price, float profit,
           int stockQuantity,int stockQuantity2, int stockQuantity3,
           string selectedCategory, ProductsListViewModel productsList)

            : base(productImage, productID, productName, Description, cost, price, profit, stockQuantity, stockQuantity2, stockQuantity3,selectedCategory, productsList)
        {

           
            EnableAllUserInputs_ExceptProductID();

            EnableImagePickerBtns();

            // we recond all the product info or store the values so we can copare and deect if the user made any changes
            // so we enable the edit button 
            RecordAllOriginalProductInfoBeforeEdition();

            AddOrEditOrDeleteProductCommand = ReactiveCommand.Create(EditAllInfoProduct, checkIfEditedAllInfoProductAreCorrectlyEdited());


        }

        private bool IsTheOriginalImageDifferentFromCurrent(Bitmap Image1 , Bitmap Image2) {

            //if(Image1==null || Image2==null) return false;
            //
            //return Image1.Size != Image2.Size;

            return ImageHelper.IsTheOriginalImageDifferentFromCurrent(Image1, Image2);
        }
        private void RecordAllOriginalProductInfoBeforeEdition() {

            _originalProductImage = SelectedImageToDisplay;
            _originalProductName = EntredProductName;
            _originalProductDescription = EnteredProductDescription;
            _originalProductCost = EntredCost;
            _originalProductPrice = EnteredPrice;
            _originalProductQuantity = EntredStockQuantity;
            _originalProductSelectedCategory = SelectedCategory;
        
        
        }

        private bool UserHasEdited_AtLeast_OneSingle_ProductInfo()
        {
            return (
                   
                    _originalProductImage != SelectedImageToDisplay||
                    _originalProductName != EntredProductName ||
                    _originalProductDescription != EnteredProductDescription ||
                    _originalProductCost != EntredCost ||
                    _originalProductPrice != EnteredPrice ||
                    _originalProductQuantity != EntredStockQuantity ||
                    _originalProductSelectedCategory != SelectedCategory||
                     IsTheOriginalImageDifferentFromCurrent(_originalProductImage, SelectedImageToDisplay) 
                    );
        }

        private bool Is_UiError_Raised_If_TheProductNameToEdit_Is_AlreadyExistInDb()
        {
            // we use this function to detect if the productname is already existing to prevent duplicated productnaem we use eproductmode to set a specif algorithm in stored procedure
            // there is another mode wich is edit mode widht requie another algorith to treat that becuase when we edit a product that proudct is already existing in the db
            // so that cause issue and wont allow us to edit the product other elements like price and other stuff in this case we set
            // eproductmode.editmode and he will ignoe the product that we are in so we get rid of the product alredy existing message and still check that we don't inlcude 
            // or add a productname that exist in the db

            // this is to prevent the error becuase the property sometimes cannot be yet initialized becuase the code who calls this function is in the set which get exectued befreo 
            // a constructor

            _isTheProductNameToEditAlreadyExistInDb = AccessToClassLibraryBackendProject.DoesProductNameAlreadyExist(EntredProductName, (int)eProductMode.EditProductMode, _ProductID);

            if (_isTheProductNameToEditAlreadyExistInDb) ShowUiError(nameof(EntredProductName), "هذا الاسم موجود من قبل");
            else DeleteUiError(nameof(EntredProductName), "هذا الاسم موجود من قبل");

            return _isTheProductNameToEditAlreadyExistInDb;
        }
        private async void EditAllInfoProduct()
        {
           
                ProductInfo ProductInfoEditedByUser =
                    new ProductInfo(_ProductID, _ProductName, _ProductDescription, _StockQuantity,_StockQuantity2,_StockQuantity3, _Price, _Cost, _SelectedImageToDisplay, _SelectedCategory);

               if (AccessToClassLibraryBackendProject.UpdateAllInfoProduct(ProductInfoEditedByUser))
               {
           
                   await ShowMessageBoxDialog.Handle("تمت تعديل المنتج بنجاح");
                   ProductsListViewModel.ReloadProductListIntoSceen();
                  Show_MessageBoxOfPrintingBarcode_Proposition_If_UserHasIncreasedStockQuantity();
            }
           
               else await ShowMessageBoxDialog.Handle("هناك مشكلة في تعديل هذا المنتج");
           
        }

      
        private IObservable<bool> checkIfEditedAllInfoProductAreCorrectlyEdited()
        {

            var canEditProductInfo = this.WhenAnyValue(
                             x => x.EntredProductName,
                             x => x.EnteredProductDescription,
                             x => x.EntredCost,
                             x => x.EnteredPrice,
                             x => x.EntredStockQuantity,
                             x => x.SelectedCategory,
                             x => x.SelectedImageToDisplay,
                             x => x.CalculatedBenefit,
                             (EntredProductName, EnteredProductDescription, EntredCost, EnteredPrice, EntredStockQuantity, SelectedCategory, SelectedImageToDisplay, CalculatedBenefit) =>
                             !string.IsNullOrEmpty(EntredProductID) &&
                             !string.IsNullOrEmpty(EntredProductName) && !Is_UiError_Raised_If_TheProductNameToEdit_Is_AlreadyExistInDb()&&
                             EnteredProductDescription != null &&
                             !string.IsNullOrEmpty(EntredCost) &&
                             !string.IsNullOrEmpty(EnteredPrice) &&
                             !string.IsNullOrEmpty(EntredStockQuantity) &&
                             !string.IsNullOrEmpty(SelectedCategory) &&
                             !string.IsNullOrEmpty(CalculatedBenefit) &&
                             UserHasEdited_AtLeast_OneSingle_ProductInfo() &&
                            

                             // we check if the attribute are not rasing an error due to wrong input
                             // like non valid character such letters or like * , / and so on
                             UiAttributeChecker.AreThesesAttributesPropertiesValid
                                                                                  (this, nameof(EntredProductName),
                                                                                         nameof(EnteredProductDescription),
                                                                                         nameof(EntredCost),
                                                                                         nameof(EnteredPrice),
                                                                                         nameof(EntredStockQuantity),
                                                                                         nameof(SelectedCategory),
                                                                                         nameof(CalculatedBenefit)));



            // Combine both observables using CombineLatest
            return canEditProductInfo;
            
        }

        // this funciton will make all the inputs editable
        private void EnableAllUserInputs_ExceptProductID()
        {
            // ReadOnly is mean Enabled  i was rushed and didnt have a time to edit this property in all parts of the view and viemodel
            IsProductIdReadOnly = false;
            IsProductNameReadOnly = !false;
            IsProductDescriptionReadOnly = !false;
            IsPriceReadOnly = !false;
            IsCostReadOnly = !false;
            IsStockQuantityReadOnly = !false;
            IsStockQuantityReadOnly2 = !false;
            IsStockQuantityReadOnly3 = !false;
            IsSelectedCategoryEnabled = true;

            
        }

        private void EnableImagePickerBtns()
        {

            PickImageCommand = ReactiveCommand.CreateFromTask(PickImageProduct, Observable.Return(true));

            DeleteImageCommand = ReactiveCommand.CreateFromTask(DisplayNoImage, Observable.Return(true));
        }

        // this section is for testing 

        //async void Edit_RandomValidProduct_From_0_To_10_000_MainFunction()
        //{
        //    for (long i = 1; i < 10_000; i++)
        //    {
        //        // Assuming _ProductID, _ProductName, _ProductDescription,
        //        // _StockQuantity, _Price, _Cost, _SelectedImageToDisplay, _SelectedCategory are defined somewhere

        //        //  ProductInfo ProductInfoEditedByUser =
        //        //   new ProductInfo(_ProductID, _ProductName, _ProductDescription, _StockQuantity, _Price, _Cost, _SelectedImageToDisplay, _SelectedCategory);
        //        //
        //        //  if (AccessToClassLibraryBackendProject.UpdateAllInfoProduct(ProductInfoEditedByUser))
        //        //
        //        EntredProductName = GenerateRandomValid_ProductName();
        //        EnteredProductDescription = GenerateRandomValid_ProductDescription();
        //        EntredCost = GenerateRandomValid_ProductCost();
        //        EnteredPrice = GenerateRandomValid_ProductPrice();
        //        EntredStockQuantity = GenerateRandomValid_StockQuantity();

        //        _SelectedCategory = "didi";
        //        ProductInfo productInfoFilledByUser = new ProductInfo(
        //            i,
        //            _ProductName,
        //            _ProductDescription,
        //            _StockQuantity,
        //            _Price,
        //            _Cost,
        //            _SelectedImageToDisplay,
        //            _SelectedCategory);

        //        bool TheValidRecordIsFailedAtUi = !await checkIfEditedAllInfoProductAreCorrectlyEdited().FirstAsync();

        //        if (TheValidRecordIsFailedAtUi)
        //        {

        //            Debug.WriteLine($"Product failed at Ui Stage:");
        //            Debug.WriteLine($"Product ID: {_ProductID}");
        //            Debug.WriteLine($"Product Name: {_ProductName}");
        //            Debug.WriteLine($"Product Description: {_ProductDescription}");
        //            Debug.WriteLine($"Stock Quantity: {_StockQuantity}");
        //            Debug.WriteLine($"Price: {_Price}");
        //            Debug.WriteLine($"Cost: {_Cost}");
        //            Debug.WriteLine($"Selected Image to Display: {_SelectedImageToDisplay}");
        //            Debug.WriteLine($"Selected Category: {_SelectedCategory}");

        //            return;
        //        }


        //        if (!AccessToClassLibraryBackendProject.UpdateAllInfoProduct(productInfoFilledByUser))
        //        {
        //            Debug.WriteLine($" DATABASE STAGE : Product failed to be added :");
        //            Debug.WriteLine($"Product ID: {_ProductID}");
        //            Debug.WriteLine($"Product Name: {_ProductName}");
        //            Debug.WriteLine($"Product Description: {_ProductDescription}");
        //            Debug.WriteLine($"Stock Quantity: {_StockQuantity}");
        //            Debug.WriteLine($"Price: {_Price}");
        //            Debug.WriteLine($"Cost: {_Cost}");
        //            Debug.WriteLine($"Selected Image to Display: {_SelectedImageToDisplay}");
        //            Debug.WriteLine($"Selected Category: {_SelectedCategory}");

        //            return;
        //        }

        //    }

        //    Debug.WriteLine("operation of valid all product info succeded ");
        //}

        //async void Edit_RandomInValidProduct_From_0_To_10_000_MainFunction()
        //{
        //    for (long i = 0; i <= 10_000; i++)
        //    {
        //        // Assuming _ProductID, _ProductName, _ProductDescription,
        //        // _StockQuantity, _Price, _Cost, _SelectedImageToDisplay, _SelectedCategory are defined somewhere


        //        EntredProductName = GenerateRandomInvalid_ProductName();
        //        EnteredProductDescription = GenerateRandomInvalid_ProductDescription();
        //        EntredCost = GenerateRandomInvalid_ProductCost();
        //        EnteredPrice = GenerateRandomInvalid_ProductPrice();
        //        EntredStockQuantity = GenerateRandomInvalid_StockQuantity();

        //        _SelectedCategory = "didi";
        //        ProductInfo productInfoFilledByUser = new ProductInfo(
        //            i,
        //            _ProductName,
        //            _ProductDescription,
        //            _StockQuantity,
        //            _Price,
        //            _Cost,
        //            _SelectedImageToDisplay,
        //            _SelectedCategory);

        //        bool TheInValidRecordIsSuccedAtUi = await checkIfEditedAllInfoProductAreCorrectlyEdited().FirstAsync();

        //        if (TheInValidRecordIsSuccedAtUi)
        //        {

        //            Debug.WriteLine($"invalid product is passed at Ui Stage:");
        //            Debug.WriteLine($"Product ID: {_ProductID}");
        //            Debug.WriteLine($"Product Name: {_ProductName}");
        //            Debug.WriteLine($"Product Description: {_ProductDescription}");
        //            Debug.WriteLine($"Stock Quantity: {_StockQuantity}");
        //            Debug.WriteLine($"Price: {_Price}");
        //            Debug.WriteLine($"Cost: {_Cost}");
        //            Debug.WriteLine($"Selected Image to Display: {_SelectedImageToDisplay}");
        //            Debug.WriteLine($"Selected Category: {_SelectedCategory}");

        //            return;
        //        }


               

        //    }

        //    Debug.WriteLine("operation of non valid all product edition succeded ");
        //}


    }
}
