﻿using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReactiveUI;
using System.Windows.Input;
using Avalonia.Media.Imaging;
using GetStartedApp.Helpers;
using GetStartedApp.Helpers.CustomUIErrorAttributes;
using GetStartedApp.Models;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.ReactiveUI;
using MsBox.Avalonia.Base;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Globalization;
using GetStartedApp.Models.Objects;
using GetStartedApp.Models.Enums;
using GetStartedApp.ViewModels.DashboardPages;
using System.Collections.ObjectModel;
using DynamicData.Binding;
//using Microsoft.CodeAnalysis.CSharp.Syntax;



namespace GetStartedApp.ViewModels.ProductPages
{
    public class AddProductViewModel : ViewModelBase
    {
       
        public Func<Task<string>> PickProductImageFunc { get; set; }

        public Interaction<string, Unit> ShowMessageBoxDialog { get; set; }

        public Interaction<string, bool> ShowMessageBoxDialog_For_BarCodePrintingPersmission { get; set; }

        public Interaction<Unit,Unit>  ShowBarCodePrinterPage { get; set; }

        private string _SelectedProductImagePath;

        protected Bitmap _SelectedImageToDisplay;
        public Bitmap SelectedImageToDisplay { get { return _SelectedImageToDisplay; }  
            set { this.RaiseAndSetIfChanged(ref _SelectedImageToDisplay, value); } }

        protected const long maxNumberOfProductsCanSystemHold = 1_000_000_000_000_000_000;
        protected const float maxPriceProductCanHold = 1_000_000;
        protected const float maxBenifitFromProduct = 100_000;

       
       
        private List<string> _Productcategories;
        public List<string> ProductCategories
        {
            get { return _Productcategories; }
            set { this.RaiseAndSetIfChanged(ref _Productcategories, value); }
        }

       
        
        private long _ProductID;
       
        private string _EntredProductID;
        [PositiveIntRange(1, maxNumberOfProductsCanSystemHold, ErrorMessage = "ادخل رقم موجب وبدون فاصلة ")]
        [ProductIdAlreadyExists("هذا الرقم تم تسجيله من قبل")]
        public virtual string EntredProductID
        {
            get { return _EntredProductID; }

            set
            {

                 this.RaiseAndSetIfChanged(ref _EntredProductID, value);

                
                DataEntryPropertyLoader.ConvertStringToIntAndLoadPrivateProperty(value, ref _ProductID);

            }
        }


        //private string _EntredProductName;
        private string  _ProductName;
        [CheckForInvalidCharacters]
        [StringMustHaveAtLeast_3_Letters(ErrorMessage = "اسم المنتج يجب ان يحتوي على الاقل ثلاث حروف")]
        [MaxStringLengthAttribute_IS(50, "هذه الجملة طويلة جدا")]
        public string EntredProductName
        {
            get { return _ProductName; }
            set { this.RaiseAndSetIfChanged(ref _ProductName, value); }
        }


        //private string _EntredProductDescription;
        protected string _ProductDescription;

        [CheckForInvalidCharacters]
        [MaxStringLengthAttribute_IS(100, "هذه الجملة طويلة جدا")]
        public string EnteredProductDescription
        {
            get {

                
                    return _ProductDescription;

            }
            set { 
                
                this.RaiseAndSetIfChanged(ref _ProductDescription, value);

               
            }
        }


        protected string _EntredPrice;
        protected float _Price;
        [PositiveFloatRange(1, maxNumberOfProductsCanSystemHold, ErrorMessage = "ادخل رقم موجب فقط")]
        public string EnteredPrice
        {
            get { return _EntredPrice; }
            set
            {

                this.RaiseAndSetIfChanged(ref _EntredPrice, value);

                DataEntryPropertyLoader.ConvertStringToFloatAndLoadPrivateProperty(value, ref _Price);

                DisplayTheBenefitFromPriceAndCost();
            }
        }


        protected string _EntredCost;
        protected float _Cost;
        [PositiveFloatRange(1, maxPriceProductCanHold, ErrorMessage = "ادخل رقم موجب فقط")]
        public string EntredCost
        {
            get { return _EntredCost; }

            set
            {

                this.RaiseAndSetIfChanged(ref _EntredCost, value);

                DataEntryPropertyLoader.ConvertStringToFloatAndLoadPrivateProperty(value, ref _Cost);

                DisplayTheBenefitFromPriceAndCost();

            }
        }


        protected string _CalculatedBenefit;
        protected float  _Benefit;
        [PositiveFloatRange(1, maxBenifitFromProduct, ErrorMessage = "الربح يجب ان يكون على الاقل درهم واحد")]
        public string CalculatedBenefit
        {
            get { return _CalculatedBenefit; }
            set { this.RaiseAndSetIfChanged(ref _CalculatedBenefit, value); }
        }

      
        protected string  _EntredStockQuantity;
        protected int     _StockQuantity;
        [PositiveIntRange(0, maxNumberOfProductsCanSystemHold, ErrorMessage = "ادخل رقم موجب وبدون فاصلة ")]
        public string EntredStockQuantity
        {
            get { return _EntredStockQuantity; }

            set
            {
                this.RaiseAndSetIfChanged(ref _EntredStockQuantity, value);
                DataEntryPropertyLoader.ConvertStringToIntAndLoadPrivateProperty(value, ref _StockQuantity);
            }

        }

        protected string _EntredStockQuantity2;
        protected int _StockQuantity2;
        [PositiveIntRange(0, maxNumberOfProductsCanSystemHold, ErrorMessage = "ادخل رقم موجب وبدون فاصلة ")]
        public string EntredStockQuantity2
        {
            get { return _EntredStockQuantity2; }

            set
            {
                this.RaiseAndSetIfChanged(ref _EntredStockQuantity2, value);
                DataEntryPropertyLoader.ConvertStringToIntAndLoadPrivateProperty(value, ref _StockQuantity2);
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

        
        private string _ProductBtnOperation = "اضافة منتج";
        public virtual string ProductBtnOperation { get=> _ProductBtnOperation; set=> this.RaiseAndSetIfChanged(ref _ProductBtnOperation, value); } 


        private bool _isProductIdReadOnly;
        public bool IsProductIdReadOnly
        {
            get { return _isProductIdReadOnly; }
            set { this.RaiseAndSetIfChanged(ref _isProductIdReadOnly, value); }
        }


        private bool _isProductNameReadOnly;
        public bool IsProductNameReadOnly
        {
            get { return _isProductNameReadOnly; }
            set { this.RaiseAndSetIfChanged(ref _isProductNameReadOnly, value); }
        }


        private bool _isProductDescriptionReadOnly;
        public bool IsProductDescriptionReadOnly
        {
            get { return _isProductDescriptionReadOnly; }
            set { this.RaiseAndSetIfChanged(ref _isProductDescriptionReadOnly, value); }
        }


        private bool _isCostReadOnly;
        public bool IsCostReadOnly
        {
            get { return _isCostReadOnly; }
            set { this.RaiseAndSetIfChanged(ref _isCostReadOnly, value); }
        }


        private bool _isPriceReadOnly;
        public bool IsPriceReadOnly
        {
            get { return _isPriceReadOnly; }
            set { this.RaiseAndSetIfChanged(ref _isPriceReadOnly, value); }
        }


        private bool _isStockQuantityReadOnly;
        public bool IsStockQuantityReadOnly
        {
            get { return _isStockQuantityReadOnly; }
            set { this.RaiseAndSetIfChanged(ref _isStockQuantityReadOnly, value); }
        }


        private bool _IsSelectedCategoryEnabled;
        public bool IsSelectedCategoryEnabled
        {
            get { return _IsSelectedCategoryEnabled; }
            set { this.RaiseAndSetIfChanged(ref _IsSelectedCategoryEnabled, value); }
        }

        private bool _isStockQuantityReadOnly2;
        public bool IsStockQuantityReadOnly2
        {
            get { return _isStockQuantityReadOnly2; }
            set { this.RaiseAndSetIfChanged(ref _isStockQuantityReadOnly2, value); }
        }

        private bool _isStockQuantityReadOnly3;
        public bool IsStockQuantityReadOnly3
        {
            get { return _isStockQuantityReadOnly3; }
            set { this.RaiseAndSetIfChanged(ref _isStockQuantityReadOnly3, value); }
        }

     

        private bool _isTheProductNameToAddAlreadyExistInDb;

        public string BarCodeSerieNumber { get; set; }
        public int    NumberOfBarcodes { get; set; }

        protected ProductsListViewModel ProductsListViewModel;
       
        private BonReceptionViewModel BonReceptionViewModel;

        public ICommand PickImageCommand { get; set; }

        public ICommand DeleteImageCommand { get; set; }

        public ICommand AddOrEditOrDeleteProductCommand { get; set; }

        private string _currentProductID;
        private string _currentProductName;


        // this function generate automatic product id from database
        // if the last product id was for example 10 then this function will
        //generate 11 as the next to it
        private void getNewProductIdGeneratedFromDatabase()
        {
            EntredProductID = AccessToClassLibraryBackendProject.GetNewProductIDFromDatabase().ToString();
        }

        public AddProductViewModel(ProductsListViewModel productsListViewModel)
        {
            // Call the shared initialization method
            Initialize();

            // Make the object global to this class
            this.ProductsListViewModel = productsListViewModel;
        }

        // Constructor that add new product
        public AddProductViewModel(BonReceptionViewModel bonReceptionViewModel)
        {         
            // Make the object global to this class
            this.BonReceptionViewModel = bonReceptionViewModel;

            // Call the shared initialization method
            Initialize_For_ProductScanned_ToRecive_AddProductMode();

        }

        // Constructor that edit product

        public AddProductViewModel(ProductScannedInfo_ToRecieve productScannedInfo_ToRecieve, BonReceptionViewModel bonReceptionViewModel, bool IsthisProductExistInDb=false)
        {
            
            BonReceptionViewModel = bonReceptionViewModel;

            // load all info of product added to edit at the form
            EntredProductID = productScannedInfo_ToRecieve.ProductInfo.id.ToString();
            SelectedImageToDisplay = productScannedInfo_ToRecieve.ProductInfo.SelectedProductImage;
            EntredProductName = productScannedInfo_ToRecieve.ProductInfo.name;
            EnteredProductDescription = productScannedInfo_ToRecieve.ProductInfo.description;
            EntredCost = productScannedInfo_ToRecieve.ProductInfo.cost.ToString();
            EnteredPrice = productScannedInfo_ToRecieve.ProductInfo.price.ToString();
            SelectedCategory = productScannedInfo_ToRecieve.ProductInfo.selectedCategory;
            EntredStockQuantity = productScannedInfo_ToRecieve.ProductsUnitsToReduce_From_Stock1.ToString();
            EntredStockQuantity2 = productScannedInfo_ToRecieve.ProductsUnitsToReduce_From_Stock2.ToString();
            EntredStockQuantity3 = productScannedInfo_ToRecieve.ProductsUnitsToReduce_From_Stock3.ToString();
            ProductBtnOperation = "تعديل منتج";

            _currentProductID   = EntredProductID;
            _currentProductName = EntredProductName;

            // Call the shared initialization method
            Initialize_For_ProductScanned_ToRecive_EditProductMode(IsthisProductExistInDb);


        }


        // Shared initialization logic
        private void Initialize()
        {
            // I added this modification to make a generation of product id automatically not 
            // manually as it was but with the ability to go back to the manual mode later 
            // if I wanted
            IsProductIdReadOnly = true;

            // We set description to empty string letting the user the choice of not adding anything 
            // because null is creating problems in other parts of code and requires to alter a lot of 
            // functions so this solution was a quick turnaround
            EnteredProductDescription = "";

            getNewProductIdGeneratedFromDatabase();

            DisplayNoImage();

            // Set the list of products categories a user will choose among
            ProductCategories = GetProductsCategoryFromDatabase();

            PickImageCommand = ReactiveCommand.CreateFromTask(PickImageProduct);
            DeleteImageCommand = ReactiveCommand.CreateFromTask(DisplayNoImage);
            AddOrEditOrDeleteProductCommand = ReactiveCommand.Create(AddProductToBonReceptionList, CheckIfFormIsFilledCorreclty());

            // This is an initialization of command that is going to open a message box 
            // when adding productOperation is submitted
            ShowMessageBoxDialog = new Interaction<string, Unit>();
            ShowMessageBoxDialog_For_BarCodePrintingPersmission = new Interaction<string, bool>();
            ShowBarCodePrinterPage = new Interaction<Unit, Unit>();

            EnableAllInputsExceptID();
        }

        private void Initialize_For_ProductScanned_ToRecive_AddProductMode()
        {
            // I added this modification to make a generation of product id automatically not 
            // manually as it was but with the ability to go back to the manual mode later 
            // if I wanted
            IsProductIdReadOnly = true;

            // We set description to empty string letting the user the choice of not adding anything 
            // because null is creating problems in other parts of code and requires to alter a lot of 
            // functions so this solution was a quick turnaround
            EnteredProductDescription = "";

            getProductID_Automatically(BonReceptionViewModel.ProductsListScanned_To_Recive);

            DisplayNoImage();

            // Set the list of products categories a user will choose among
            ProductCategories = GetProductsCategoryFromDatabase();

            PickImageCommand = ReactiveCommand.CreateFromTask(PickImageProduct);
            DeleteImageCommand = ReactiveCommand.CreateFromTask(DisplayNoImage);
            AddOrEditOrDeleteProductCommand = ReactiveCommand.Create(AddProductToBonReceptionList, CheckIfFormIsFilledCorreclty_WhenWeAddNewProduct());

            // This is an initialization of command that is going to open a message box 
            // when adding productOperation is submitted
            ShowMessageBoxDialog = new Interaction<string, Unit>();
            ShowMessageBoxDialog_For_BarCodePrintingPersmission = new Interaction<string, bool>();
            ShowBarCodePrinterPage = new Interaction<Unit, Unit>();

            EnableAllInputsExceptID();
        }

        private void Set_TheRight_AddOrEditOrDeleteProductCommand_Based_If_The_Product_ExistInDb_Or_Its_New(bool isThisProduct_To_Edit_ExistInDb)
        {
            if(isThisProduct_To_Edit_ExistInDb) AddOrEditOrDeleteProductCommand = ReactiveCommand.Create(EditedProductAddedToBonReceptionList, CheckIfFormIsFilledCorreclty_WhenWeEdit_TheExistingProductInDb_Added());

            else AddOrEditOrDeleteProductCommand = ReactiveCommand.Create(EditedProductAddedToBonReceptionList, CheckIfFormIsFilledCorreclty_WhenWeEdit_TheNewProductAdded());
        }
        // this function dosent get new product id from datbase because the info of proudcts are exsitng to  be edited
        private void Initialize_For_ProductScanned_ToRecive_EditProductMode(bool isThisProduct_To_Edit_ExistInDb)
        {


            if (isThisProduct_To_Edit_ExistInDb) EnableAllInputsExceptID_ProductName();
            else EnableAllInputs();

            DisplayNoImage();


            // Set the list of products categories a user will choose among
            ProductCategories = GetProductsCategoryFromDatabase();

            PickImageCommand = ReactiveCommand.CreateFromTask(PickImageProduct);
            DeleteImageCommand = ReactiveCommand.CreateFromTask(DisplayNoImage);

            Set_TheRight_AddOrEditOrDeleteProductCommand_Based_If_The_Product_ExistInDb_Or_Its_New(isThisProduct_To_Edit_ExistInDb);

            // This is an initialization of command that is going to open a message box 
            // when adding productOperation is submitted
            ShowMessageBoxDialog = new Interaction<string, Unit>();
            ShowMessageBoxDialog_For_BarCodePrintingPersmission = new Interaction<string, bool>();
            ShowBarCodePrinterPage = new Interaction<Unit, Unit>();

           
        }

        private void getProductID_Automatically(ObservableCollection<ProductScannedInfo_ToRecieve> ProductsListScanned_To_Recive)
        {
            if (AreThereNewProductAddedToBonReceptionList(ProductsListScanned_To_Recive)) {
               
                EntredProductID = getTheSmallestProductID_That_Doesnt_ExistInDb_NeitherIn_ProductListScannedToRecieve(ProductsListScanned_To_Recive).ToString(); }
            
            else getNewProductIdGeneratedFromDatabase();
        }

        private bool AreThereNewProductAddedToBonReceptionList(ObservableCollection<ProductScannedInfo_ToRecieve> ProductsListScanned_To_Recive)
        {
            // Check if any product in the list is new (ThisProductIsExistingInDB is false)
            return ProductsListScanned_To_Recive.Any(product => !product.ThisProductIsExistingInDB);
        }

        private long getTheSmallestProductID_That_Doesnt_ExistInDb_NeitherIn_ProductListScannedToRecieve(ObservableCollection<ProductScannedInfo_ToRecieve> ProductsListScanned_To_Recive)
        {
            // Retrieve the smallest non-existing product ID from the database
            long smallestNonExistingProductIdFromDb = AccessToClassLibraryBackendProject.GetNewProductIDFromDatabase();

            long newUniqueProductID_That_Dosent_ExistNeitherIn_Db_Or_ProductListScannedToRecive = smallestNonExistingProductIdFromDb;
            // Get a list of all product IDs from the ProductsListScanned_To_Recieve collection
            var productIdsInList = ProductsListScanned_To_Recive.Select(product => product.ProductInfo.id).ToList();

            // Increment the smallestNonExistingProductIdFromDb until it finds one not in the list
            while (productIdsInList.Contains(newUniqueProductID_That_Dosent_ExistNeitherIn_Db_Or_ProductListScannedToRecive) || 
                AccessToClassLibraryBackendProject.IsThisProductIdAlreadyExist(newUniqueProductID_That_Dosent_ExistNeitherIn_Db_Or_ProductListScannedToRecive))
            {
                newUniqueProductID_That_Dosent_ExistNeitherIn_Db_Or_ProductListScannedToRecive++;
            }

            // Return the first product ID that is not in the database and not in the list
            return newUniqueProductID_That_Dosent_ExistNeitherIn_Db_Or_ProductListScannedToRecive;
        }

        private bool IsProductIDAlreadyInBonReceptionList(long productID, ObservableCollection<ProductScannedInfo_ToRecieve> ProductsListScanned_To_Recive)
        {
            // Check if any product in the list has the same ProductID
            return ProductsListScanned_To_Recive.Any(product => product.ProductInfo.id == productID);
        }

        private bool IsProductNameAlreadyInBonReceptionList(string productName, ObservableCollection<ProductScannedInfo_ToRecieve> productsListScannedToReceive)
        {
            // Check if the product name already exists in the list of scanned products
            return productsListScannedToReceive.Any(product => product.ProductInfo.name.Equals(productName, StringComparison.OrdinalIgnoreCase));
        }

        private void EnableAllInputsExceptID()
        {
            // i made miskate to set readonly but it must be entable or disabled in tis case it must be isProductIdEnabled 
            bool isEnabled = true;
            bool isDisabled = false;

          //  IsProductIdReadOnly = isEnabled;
            IsProductNameReadOnly = isEnabled;
            IsProductDescriptionReadOnly = isEnabled;
            IsCostReadOnly = isEnabled;
            IsPriceReadOnly = isEnabled;
            IsStockQuantityReadOnly = isEnabled;
            IsStockQuantityReadOnly2 = isEnabled;
            IsStockQuantityReadOnly3 = isEnabled;
            IsSelectedCategoryEnabled = isEnabled;
           

        }
       
        private void EnableAllInputsExceptID_ProductName()
        {
            EnableAllInputsExceptID();
            IsProductNameReadOnly = false;
        }

        private void EnableAllInputs()
        {
            EnableAllInputsExceptID();
            IsProductIdReadOnly = false;
        }
        protected void DisplayTheBenefitFromPriceAndCost()
        {

            // we get the benefit by doing this operation Benifit = _Price - _Cost that a user enter

            string BenefitCalculated = ((decimal)_Price - (decimal)_Cost).ToString();

            _Benefit = float.Parse(BenefitCalculated);

            CalculatedBenefit = BenefitCalculated;

        }
        // this function will display a no image Image that indicate a user haven't set the productImage yet 
        protected async Task DisplayNoImage()
        {

            SelectedImageToDisplay = ImageHelper.LoadFromResource(new Uri("avares://GetStartedApp/Assets/Icons/NoImageImage.png"));
   
        }

        public async Task PickImageProduct()
        {
            

            if (PickProductImageFunc != null)
            {
                // save the path of image selected 
                _SelectedProductImagePath = await PickProductImageFunc.Invoke();
                
                // we check if the user has selected an image path becuase sometimes it can close the window and not selectged an image
                // which can throw error and crash the app
                if (_SelectedProductImagePath !="") {
                // display the image selected after resizing it to reduce it memory taken in space
                // this is for the sake of improving performance
                SelectedImageToDisplay = await ImageResizerHelper.ResizeImageAsync(_SelectedProductImagePath);

             }

            }
          
        }

        private bool AreAllPropertiesAttributeValid( )
        {
            // this function checks the attribute of each property bound to ui
            // if one property rasing an error uiAttribueckher will return false which will block the button add product command
            // for exampe EntredProductID has a property PositiveRange so it checks if the property is valid 
            // the same thing applies to all property names we pass
            return UiAttributeChecker.AreThesesAttributesPropertiesValid
                (this, nameof(EntredProductID), 
                nameof(EnteredProductDescription),
                nameof(EntredProductName), 
                nameof(EnteredPrice),
                nameof(EntredCost),
                nameof(CalculatedBenefit),
                nameof(EntredStockQuantity),
                nameof(EntredStockQuantity2),
                nameof(EntredStockQuantity3));
        }

        private bool AreAllPropertiesAttributeValid_ExcludeProductID_And_ProductName()
        {
            // this function checks the attribute of each property bound to ui
            // if one property rasing an error uiAttribueckher will return false which will block the button add product command
            // for exampe EntredProductID has a property PositiveRange so it checks if the property is valid 
            // the same thing applies to all property names we pass
            DeleteAllUiErrorsProperty(nameof(EntredProductID));
            DeleteAllUiErrorsProperty(nameof(EntredProductName));
            return UiAttributeChecker.AreThesesAttributesPropertiesValid
                (this,
                nameof(EnteredProductDescription),
                nameof(EnteredPrice),
                nameof(EntredCost),
                nameof(CalculatedBenefit),
                nameof(EntredStockQuantity),
                nameof(EntredStockQuantity2),
                nameof(EntredStockQuantity3));
        }

        private bool Is_UiError_Raised_If_TheProduct_Name_ToAdd_Is_AlreadyExistInDb()
        {
            // we use this function to detect if the productname is already existing to prevent duplicated productnaem we use eproductmode to set a specif algorithm in stored procedure
            // there is another mode wich is edit mode widht requie another algorith to treat that becuase when we edit a product that proudct is already existing in the db
            // so that cause issue and wont allow us to edit the product other elements like price and other stuff in this case we set
            // eproductmode.editmode and he will ignoe the product that we are in so we get rid of the product alredy existing message and still check that we don't inlcude 
            // or add a productname that exist in the db

            _isTheProductNameToAddAlreadyExistInDb = AccessToClassLibraryBackendProject.DoesProductNameAlreadyExist(EntredProductName, (int)eProductMode.addProductMode, _ProductID);

            if (_isTheProductNameToAddAlreadyExistInDb) ShowUiError(nameof(EntredProductName), "هذا الاسم موجود من قبل");
            else DeleteUiError(nameof(EntredProductName), "هذا الاسم موجود من قبل");

            return _isTheProductNameToAddAlreadyExistInDb;
        }

        private bool Is_UiError_Raised_If_TheProduct_ID_ToAdd_Is_AlreadyExist_In_ReceptionList()
        {
            // Try to parse the EntredProductID (string) to a long
            if (long.TryParse(EntredProductID, out long parsedProductID))
            {
                // Check if the product ID already exists in the BonReceptionList
                bool isProductIDAlreadyInList = IsProductIDAlreadyInBonReceptionList(parsedProductID, BonReceptionViewModel.ProductsListScanned_To_Recive);

                // Show or delete the UI error based on the result
                if (isProductIDAlreadyInList) ShowUiError(nameof(EntredProductID), "رقم هذا المنتج موجود في قائمة الاستلام");
                else DeleteUiError(nameof(EntredProductID), "رقم هذا المنتج موجود في قائمة الاستلام");

                return isProductIDAlreadyInList;
            }

            // If parsing fails, return false and clear any UI error
            DeleteUiError(nameof(EntredProductID), "رقم هذا المنتج موجود في قائمة الاستلام");
            return false;
        }

        private bool Is_UiError_Raised_If_TheProduct_Name_ToAdd_Is_AlreadyExist_In_ReceptionList()
        {
            // Check if the product name already exists in the BonReceptionList
            bool isProductNameAlreadyInList = IsProductNameAlreadyInBonReceptionList(EntredProductName, BonReceptionViewModel.ProductsListScanned_To_Recive);

            // Show or delete the UI error based on the result
            if (isProductNameAlreadyInList) ShowUiError(nameof(EntredProductName), "اسم هذا المنتج موجود في قائمة الاستلام");
            else DeleteUiError(nameof(EntredProductName), "اسم هذا المنتج موجود في قائمة الاستلام");

            return isProductNameAlreadyInList;
        }

        private bool Is_UiError_Raised_If_TheProduct_ID_Is_AlreadyExistInBonReceptionList_WhenEditing()
        {
            // If the entered product ID matches the current product being edited, no need to flag an error
            if (EntredProductID == _currentProductID)
            {
                // Clear the UI error for the current product ID (since it's the one being edited)
                DeleteUiError(nameof(EntredProductID), "رقم هذا المنتج موجود في قائمة الاستلام");
                return false;
            }

            // Try to parse the EntredProductID (string) to a long
            if (long.TryParse(EntredProductID, out long parsedProductID))
            {
                // Check if the product ID already exists in the BonReceptionList (excluding the current product)
                if (IsProductIDAlreadyInBonReceptionList(parsedProductID, BonReceptionViewModel.ProductsListScanned_To_Recive))
                {
                    // Show an error if the product ID already exists
                    ShowUiError(nameof(EntredProductID), "رقم هذا المنتج موجود في قائمة الاستلام");
                    return true;
                }
                else
                {
                    // Remove the error if the product ID does not exist
                    DeleteUiError(nameof(EntredProductID), "رقم هذا المنتج موجود في قائمة الاستلام");
                    return false;
                }
            }

            // If parsing fails, return false (no error)
            return false;
        }

        private bool Is_UiError_Raised_If_TheProduct_Name_Is_AlreadyExistInBonReceptionList_WhenEditing()
        {
            // If the entered name matches the current product being edited, no need to flag an error
            if (EntredProductName == _currentProductName)
            {
                // Clear the UI error for the current product name (since it's the one being edited)
                DeleteUiError(nameof(EntredProductName), "اسم هذا المنتج موجود في قائمة الاستلام");
                return false;
            }

            // Check if the product name already exists in the BonReceptionList (excluding the current product)
            if (IsProductNameAlreadyInBonReceptionList(EntredProductName, BonReceptionViewModel.ProductsListScanned_To_Recive))
            {
                // Show an error if the product name already exists
                ShowUiError(nameof(EntredProductName), "اسم هذا المنتج موجود في قائمة الاستلام");
                return true;
            }
            else
            {
                // Remove the error if the product name does not exist
                DeleteUiError(nameof(EntredProductName), "اسم هذا المنتج موجود في قائمة الاستلام");
                return false;
            }
        }
   
        private IObservable<bool> CheckIfFormIsFilledCorreclty( )
        {
            
                var canAddProduct1 = this.WhenAnyValue(
                    x => x.EntredProductID,
                    x => x.EntredProductName,
                    x => x.EnteredProductDescription,
                    x => x.EnteredPrice,
                    x => x.EntredCost,
                    x => x.CalculatedBenefit,
                    x => x.EntredStockQuantity,
                    x => x.SelectedCategory,
                    (EntredProductID, EntredProductName, EnteredProductDescription, EnteredPrice, EntredCost, CalculatedBenefit, EntredStockQuantity, SelectedCategory) =>
                        !string.IsNullOrEmpty(EntredProductName) &&
                        !Is_UiError_Raised_If_TheProduct_Name_ToAdd_Is_AlreadyExistInDb() &&
                        !string.IsNullOrEmpty(EntredProductID) && !string.IsNullOrWhiteSpace(EntredProductID) &&
                        !string.IsNullOrWhiteSpace(EntredProductName) &&
                        !string.IsNullOrEmpty(EnteredPrice) && !string.IsNullOrWhiteSpace(EnteredPrice) &&
                        !string.IsNullOrEmpty(CalculatedBenefit) && !string.IsNullOrWhiteSpace(CalculatedBenefit) &&
                        !string.IsNullOrEmpty(EntredStockQuantity) && !string.IsNullOrWhiteSpace(EntredStockQuantity) &&
                        EnteredProductDescription != null &&
                        !string.IsNullOrEmpty(EntredCost) &&
                        !string.IsNullOrWhiteSpace(EntredCost) &&
                        !string.IsNullOrEmpty(SelectedCategory) && !string.IsNullOrWhiteSpace(SelectedCategory) &&
                        AreAllPropertiesAttributeValid()
                    );

                var canAddProduct2 = this.WhenAnyValue(
                    x => x.EntredStockQuantity2,
                    x => x.EntredStockQuantity3,
                    (EntredStockQuantity2, EntredStockQuantity3) =>
                        !string.IsNullOrEmpty(EntredStockQuantity2) && !string.IsNullOrWhiteSpace(EntredStockQuantity2) &&
                        !string.IsNullOrEmpty(EntredStockQuantity3) && !string.IsNullOrWhiteSpace(EntredStockQuantity3) &&
                        AreAllPropertiesAttributeValid()

                );

                // Combine the two observables using CombineLatest
                return canAddProduct1.CombineLatest(canAddProduct2, (isValid1, isValid2) => isValid1 && isValid2);
            }

        // this function add the 
        private IObservable<bool> CheckIfFormIsFilledCorreclty_WhenWeAddNewProduct()
        {
            var canAddProduct1 = this.WhenAnyValue(
                      x => x.EntredProductID,
                      x => x.EntredProductName,
                      x => x.EnteredProductDescription,
                      x => x.EnteredPrice,
                      x => x.EntredCost,
                      x => x.CalculatedBenefit,
                      x => x.EntredStockQuantity,
                      x => x.SelectedCategory,
                      (EntredProductID, EntredProductName, EnteredProductDescription, EnteredPrice, EntredCost, CalculatedBenefit, EntredStockQuantity, SelectedCategory) =>
                          !string.IsNullOrEmpty(EntredProductID) && !string.IsNullOrWhiteSpace(EntredProductID) &&
                          !Is_UiError_Raised_If_TheProduct_ID_ToAdd_Is_AlreadyExist_In_ReceptionList() &&
                          !string.IsNullOrEmpty(EntredProductName) &&!string.IsNullOrWhiteSpace(EntredProductName) &&
                          !Is_UiError_Raised_If_TheProduct_Name_ToAdd_Is_AlreadyExistInDb() &&
                          !Is_UiError_Raised_If_TheProduct_Name_ToAdd_Is_AlreadyExist_In_ReceptionList() &&
                          !string.IsNullOrEmpty(EnteredPrice) && !string.IsNullOrWhiteSpace(EnteredPrice) &&
                          !string.IsNullOrEmpty(CalculatedBenefit) && !string.IsNullOrWhiteSpace(CalculatedBenefit) &&
                          !string.IsNullOrEmpty(EntredStockQuantity) && !string.IsNullOrWhiteSpace(EntredStockQuantity) &&
                          EnteredProductDescription != null &&
                          !string.IsNullOrEmpty(EntredCost) &&
                          !string.IsNullOrWhiteSpace(EntredCost) &&
                          !string.IsNullOrEmpty(SelectedCategory) && !string.IsNullOrWhiteSpace(SelectedCategory) &&
                          AreAllPropertiesAttributeValid()
                      );

            var canAddProduct2 = this.WhenAnyValue(
                x => x.EntredStockQuantity2,
                x => x.EntredStockQuantity3,
                (EntredStockQuantity2, EntredStockQuantity3) =>
                    !string.IsNullOrEmpty(EntredStockQuantity2) && !string.IsNullOrWhiteSpace(EntredStockQuantity2) &&
                    !string.IsNullOrEmpty(EntredStockQuantity3) && !string.IsNullOrWhiteSpace(EntredStockQuantity3) &&
                    AreAllPropertiesAttributeValid()

            );

            // Combine the two observables using CombineLatest
            return canAddProduct1.CombineLatest(canAddProduct2, (isValid1, isValid2) => isValid1 && isValid2);
        }

        private IObservable<bool> CheckIfFormIsFilledCorreclty_WhenWeEdit_TheNewProductAdded()
        {
            var canAddProduct1 = this.WhenAnyValue(
                      x => x.EntredProductID,
                      x => x.EntredProductName,
                      x => x.EnteredProductDescription,
                      x => x.EnteredPrice,
                      x => x.EntredCost,
                      x => x.CalculatedBenefit,
                      x => x.EntredStockQuantity,
                      x => x.SelectedCategory,
                      (EntredProductID, EntredProductName, EnteredProductDescription, EnteredPrice, EntredCost, CalculatedBenefit, EntredStockQuantity, SelectedCategory) =>
                          !string.IsNullOrEmpty(EntredProductID) && !string.IsNullOrWhiteSpace(EntredProductID) &&
                          !Is_UiError_Raised_If_TheProduct_ID_Is_AlreadyExistInBonReceptionList_WhenEditing() &&
                          !string.IsNullOrEmpty(EntredProductName) && !string.IsNullOrWhiteSpace(EntredProductName) &&
                          !Is_UiError_Raised_If_TheProduct_Name_ToAdd_Is_AlreadyExistInDb() &&
                          !Is_UiError_Raised_If_TheProduct_Name_Is_AlreadyExistInBonReceptionList_WhenEditing() &&
                          !string.IsNullOrEmpty(EnteredPrice) && !string.IsNullOrWhiteSpace(EnteredPrice) &&
                          !string.IsNullOrEmpty(CalculatedBenefit) && !string.IsNullOrWhiteSpace(CalculatedBenefit) &&
                          !string.IsNullOrEmpty(EntredStockQuantity) && !string.IsNullOrWhiteSpace(EntredStockQuantity) &&
                          EnteredProductDescription != null &&
                          !string.IsNullOrEmpty(EntredCost) &&
                          !string.IsNullOrWhiteSpace(EntredCost) &&
                          !string.IsNullOrEmpty(SelectedCategory) && !string.IsNullOrWhiteSpace(SelectedCategory) &&
                          AreAllPropertiesAttributeValid()
                      );

            var canAddProduct2 = this.WhenAnyValue(
                x => x.EntredStockQuantity2,
                x => x.EntredStockQuantity3,
                (EntredStockQuantity2, EntredStockQuantity3) =>
                    !string.IsNullOrEmpty(EntredStockQuantity2) && !string.IsNullOrWhiteSpace(EntredStockQuantity2) &&
                    !string.IsNullOrEmpty(EntredStockQuantity3) && !string.IsNullOrWhiteSpace(EntredStockQuantity3) &&
                    AreAllPropertiesAttributeValid()

            );

            // Combine the two observables using CombineLatest
            return canAddProduct1.CombineLatest(canAddProduct2, (isValid1, isValid2) => isValid1 && isValid2);
        }

        // in this cde we excluded the check of productName and productid from attribute checker so the productid and product name is existing indb not raise 
        // because in this case we're going to disable the productid and productname cases
        private IObservable<bool> CheckIfFormIsFilledCorreclty_WhenWeEdit_TheExistingProductInDb_Added()
        {
            var canAddProduct1 = this.WhenAnyValue(
                      x => x.EntredProductID,
                      x => x.EntredProductName,
                      x => x.EnteredProductDescription,
                      x => x.EnteredPrice,
                      x => x.EntredCost,
                      x => x.CalculatedBenefit,
                      x => x.EntredStockQuantity,
                      x => x.SelectedCategory,
                      (EntredProductID, EntredProductName, EnteredProductDescription, EnteredPrice, EntredCost, CalculatedBenefit, EntredStockQuantity, SelectedCategory) =>
                          !string.IsNullOrEmpty(EntredProductID) && !string.IsNullOrWhiteSpace(EntredProductID) &&
                          !Is_UiError_Raised_If_TheProduct_ID_Is_AlreadyExistInBonReceptionList_WhenEditing() &&
                          !string.IsNullOrEmpty(EntredProductName) && !string.IsNullOrWhiteSpace(EntredProductName) &&
                          !Is_UiError_Raised_If_TheProduct_Name_Is_AlreadyExistInBonReceptionList_WhenEditing() &&
                          !string.IsNullOrEmpty(EnteredPrice) && !string.IsNullOrWhiteSpace(EnteredPrice) &&
                          !string.IsNullOrEmpty(CalculatedBenefit) && !string.IsNullOrWhiteSpace(CalculatedBenefit) &&
                          !string.IsNullOrEmpty(EntredStockQuantity) && !string.IsNullOrWhiteSpace(EntredStockQuantity) &&
                          EnteredProductDescription != null &&
                          !string.IsNullOrEmpty(EntredCost) &&
                          !string.IsNullOrWhiteSpace(EntredCost) &&
                          !string.IsNullOrEmpty(SelectedCategory) && !string.IsNullOrWhiteSpace(SelectedCategory)                         
                          &&AreAllPropertiesAttributeValid_ExcludeProductID_And_ProductName()
                      );

            var canAddProduct2 = this.WhenAnyValue(
                x => x.EntredStockQuantity2,
                x => x.EntredStockQuantity3,
                (EntredStockQuantity2, EntredStockQuantity3) =>
                    !string.IsNullOrEmpty(EntredStockQuantity2) && !string.IsNullOrWhiteSpace(EntredStockQuantity2) &&
                    !string.IsNullOrEmpty(EntredStockQuantity3) && !string.IsNullOrWhiteSpace(EntredStockQuantity3)
                    && AreAllPropertiesAttributeValid_ExcludeProductID_And_ProductName()

            );

            // Combine the two observables using CombineLatest
            return canAddProduct1.CombineLatest(canAddProduct2, (isValid1, isValid2) => isValid1 && isValid2);
        }

        public async void AddProductToBonReceptionList()
        {
            // Create the product info object
            ProductInfo productInfoFilledByUser = CreateProductInfoFromUserInput();

            // Product is new, not existing in the database
            bool thisProductIsExistingInDB = false;

            // Add the product to the list
            AddProductToReceptionList(productInfoFilledByUser, thisProductIsExistingInDB);

            // Show success message
            await ShowMessageBoxDialog.Handle("تمت اضافة المنتج بنجاح");
        }

        public async void EditedProductAddedToBonReceptionList()
        {
            // Create the product info object
            ProductInfo productInfoFilledByUser = CreateProductInfoFromUserInput();

            // Product is being edited, so remove the existing product
            RemoveExistingProductFromList(productInfoFilledByUser.id);

            // Product is being edited, not a new one in the database
            bool thisProductIsExistingInDB = false;

            // Add the edited product to the list
            AddProductToReceptionList(productInfoFilledByUser, thisProductIsExistingInDB);

            // Show success message
            await ShowMessageBoxDialog.Handle("تم تعديل المنتج بنجاح");
        }

        // Creates a ProductInfo object from user input
        private ProductInfo CreateProductInfoFromUserInput()
        {
            return new ProductInfo(
                _ProductID, _ProductName, _ProductDescription, _StockQuantity,
                _StockQuantity2, _StockQuantity3, _Price, _Cost,
                _SelectedImageToDisplay, _SelectedCategory
            );
        }

        // Adds a product to the reception list with relevant stock information
        private void AddProductToReceptionList(ProductInfo productInfo, bool isExistingInDB)
        {
            ProductScannedInfo_ToRecieve newProductToAdd_Plus_PriceAndUnitsOfSoldProduct =
                new ProductScannedInfo_ToRecieve(productInfo, BonReceptionViewModel, isExistingInDB);

            newProductToAdd_Plus_PriceAndUnitsOfSoldProduct.ProductsUnits =
                (_StockQuantity + _StockQuantity2 + _StockQuantity3).ToString();

            newProductToAdd_Plus_PriceAndUnitsOfSoldProduct.ProductsUnitsToReduce_From_Stock1 = _StockQuantity.ToString();
            newProductToAdd_Plus_PriceAndUnitsOfSoldProduct.ProductsUnitsToReduce_From_Stock2 = _StockQuantity2.ToString();
            newProductToAdd_Plus_PriceAndUnitsOfSoldProduct.ProductsUnitsToReduce_From_Stock3 = _StockQuantity3.ToString();

            BonReceptionViewModel.ProductsListScanned_To_Recive.Add(newProductToAdd_Plus_PriceAndUnitsOfSoldProduct);
        }

        // Removes an existing product from the reception list by product ID
        private void RemoveExistingProductFromList(long productId)
        {
            var existingProduct = BonReceptionViewModel.ProductsListScanned_To_Recive
                .FirstOrDefault(p => p.ProductInfo.id == productId);

            if (existingProduct != null)
            {
                BonReceptionViewModel.ProductsListScanned_To_Recive.Remove(existingProduct);
            }
        }


        protected async void AskUserIfHeWantToPrintBarCodes()
        {
            bool DoesUserWantToPrintBarcodes = await ShowMessageBoxDialog_For_BarCodePrintingPersmission.Handle("هل تريد طباعة الباركود");

            if(DoesUserWantToPrintBarcodes)
            {
                setDefaultBarCodeParameters();

                await ShowBarCodePrinterPage.Handle(Unit.Default);
            }
        }

        // we set the default barcode parameters we're going to pass to barcode generator
        protected virtual void setDefaultBarCodeParameters()
        {
            BarCodeSerieNumber = EntredProductID;
            NumberOfBarcodes = _StockQuantity;
    }

        public virtual bool isUserAllowedToPrintBarCodes()
        {

            // in this case it will be alowyas true i mean when a user is adding product so it's allowed to printbarcode
            // but in the case of edit productinfo user must comply one condtion
            // for this reason we're going to override this method add the derived class editproduct writing our condition

            //  later one i found that should disable this feature because is not useful so the quick way is to make this function always returning false
            // later one i can refactore the code
            return false;

        }

        protected List<String> GetProductsCategoryFromDatabase()
        {

            return AccessToClassLibraryBackendProject.GetProductsCategoryFromDatabase();

        }


        // this section is for testing ///////////

        //string costGlobalVriable;

        //protected string GenerateRandomValid_ProductName()
        //{
        //    string validCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ" +
        //                             "0123456789" +
        //                             "ءاأإآابتثجحخدذرزسشصضطظعغفقكلمنهوية" +
        //                             " _-@()"; // Include space, underscore, hyphen, at symbol, and parentheses

        //    string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ" +
        //                     "ءاأإآابتثجحخدذرزسشصضطظعغفقكلمنهوية";

        //    Random random = new Random();
        //    string stringToGenerate;

        //    do
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        int lengthOfWordToGenerate = random.Next(3, 20); // Generate a random length between 3 and 20 characters 

        //        for (int i = 0; i < lengthOfWordToGenerate; i++)
        //        {
        //            int randomNumberBetween_0_And_validCharactersLength = random.Next(0, validCharacters.Length);
        //            char randomChar = validCharacters[randomNumberBetween_0_And_validCharactersLength];
        //            sb.Append(randomChar);
        //        }

        //        stringToGenerate = sb.ToString();

        //    } while (stringToGenerate.Count(c => letters.Contains(c)) < 3);

        //    return stringToGenerate;
        //}

        //protected string GenerateRandomInvalid_ProductName()
        //{
        //    string invalidCharacters = @"'""$%&/\\;!?|<>+=\[\]{}:,'\\.*^`#~";
        //    string validCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ" +
        //                             "0123456789" +
        //                             "ءاأإآابتثجحخدذرزسشصضطظعغفقكلمنهوية" +
        //                             " _-@()";

        //    Random random = new Random();
        //    StringBuilder sb = new StringBuilder();
        //    int lengthOfWordToGenerate = random.Next(3, 20); // Generate a random length between 3 and 20 characters 

        //    // Ensure at least one valid character is included
        //    int randomNumberBetween_0_And_validCharactersLength = random.Next(validCharacters.Length);
        //    char randomValidChar = validCharacters[randomNumberBetween_0_And_validCharactersLength];
        //    sb.Append(randomValidChar);

        //    // Ensure at least one invalid character is included
        //    int randomNumberBetween_0_And_invalidCharactersLength = random.Next(invalidCharacters.Length);
        //    char randomInvalidChar = invalidCharacters[randomNumberBetween_0_And_invalidCharactersLength];
        //    sb.Append(randomInvalidChar);

        //    // Fill the rest of the string with random characters from the combined set
        //    string allCharacters = validCharacters + invalidCharacters;
        //    for (int i = 2; i < lengthOfWordToGenerate; i++) // Starting from 2 as we already added one valid and one invalid character
        //    {
        //        int randomNumberBetween_0_And_allCharactersLength = random.Next(allCharacters.Length);
        //        char randomChar = allCharacters[randomNumberBetween_0_And_allCharactersLength];
        //        sb.Append(randomChar);
        //    }

        //    string invalidStringToGenerate = sb.ToString();

        //    return invalidStringToGenerate;
        //}

        //protected string GenerateRandomValid_ProductDescription()
        //{
        //    string validCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ" +
        //                             "0123456789" +
        //                             "ءاأإآابتثجحخدذرزسشصضطظعغفقكلمنهوية" +
        //                             " _-@()"; // Include space, underscore, hyphen, at symbol, and parentheses

        //    string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ" +
        //                     "ءاأإآابتثجحخدذرزسشصضطظعغفقكلمنهوية";

        //    Random random = new Random();
        //    string stringToGenerate;

        //    do
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        int lengthOfWordToGenerate = random.Next(5, 50); // Generate a random length between 3 and 20 characters 

        //        for (int i = 0; i < lengthOfWordToGenerate; i++)
        //        {
        //            int randomNumberBetween_0_And_validCharactersLength = random.Next(0, validCharacters.Length);
        //            char randomChar = validCharacters[randomNumberBetween_0_And_validCharactersLength];
        //            sb.Append(randomChar);
        //        }

        //        stringToGenerate = sb.ToString();

        //    } while (stringToGenerate.Count(c => letters.Contains(c)) < 3);

        //    return stringToGenerate;
        //}

        //protected string GenerateRandomInvalid_ProductDescription()
        //{
        //    string invalidCharacters = @"'""$%&/\\;!?|<>+=\[\]{}:,'\\.*^`#~";
        //    string validCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ" +
        //                             "0123456789" +
        //                             "ءاأإآابتثجحخدذرزسشصضطظعغفقكلمنهوية" +
        //                             " _-@()";

        //    Random random = new Random();
        //    StringBuilder sb = new StringBuilder();
        //    int lengthOfWordToGenerate = random.Next(3, 50); // Generate a random length between 3 and 20 characters 

        //    // Ensure at least one valid character is included
        //    int randomNumberBetween_0_And_validCharactersLength = random.Next(validCharacters.Length);
        //    char randomValidChar = validCharacters[randomNumberBetween_0_And_validCharactersLength];
        //    sb.Append(randomValidChar);

        //    // Ensure at least one invalid character is included
        //    int randomNumberBetween_0_And_invalidCharactersLength = random.Next(invalidCharacters.Length);
        //    char randomInvalidChar = invalidCharacters[randomNumberBetween_0_And_invalidCharactersLength];
        //    sb.Append(randomInvalidChar);

        //    // Fill the rest of the string with random characters from the combined set
        //    string allCharacters = validCharacters + invalidCharacters;
        //    for (int i = 2; i < lengthOfWordToGenerate; i++) // Starting from 2 as we already added one valid and one invalid character
        //    {
        //        int randomNumberBetween_0_And_allCharactersLength = random.Next(allCharacters.Length);
        //        char randomChar = allCharacters[randomNumberBetween_0_And_allCharactersLength];
        //        sb.Append(randomChar);
        //    }

        //    string invalidStringToGenerate = sb.ToString();

        //    return invalidStringToGenerate;
        //}

        //protected string GenerateRandomInvalid_ProductCost()
        //{
        //    Random random = new Random();

        //    // Generate a random cost that is clearly invalid
        //    // For example, a negative cost or a cost exceeding a realistic upper limit
        //    double cost = Math.Round(random.NextDouble() * 10000.0 - 5000.0, 2); // Range: -5000.00 to 5000.00

        //    // Convert the cost to string
        //    string costString = cost.ToString("F2");

        //    // Append some random characters to the cost string to make it invalid
        //    string invalidCharacters = @"'""$%&/\\;!?|<>+=\[\]{}:,'\\.*^`#~";
        //    string validCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ" +
        //                             "0123456789" +
        //                             "ءاأإآابتثجحخدذرزسشصضطظعغفقكلمنهوية" +
        //                             " _-@()";

        //    StringBuilder sb = new StringBuilder();

        //    // Ensure at least one valid character is included in the cost string
        //    int randomNumberBetween_0_And_validCharactersLength = random.Next(validCharacters.Length);
        //    char randomValidChar = validCharacters[randomNumberBetween_0_And_validCharactersLength];
        //    sb.Append(randomValidChar);

        //    // Ensure at least one invalid character is included in the cost string
        //    int randomNumberBetween_0_And_invalidCharactersLength = random.Next(invalidCharacters.Length);
        //    char randomInvalidChar = invalidCharacters[randomNumberBetween_0_And_invalidCharactersLength];
        //    sb.Append(randomInvalidChar);

        //    // Append the generated cost string
        //    sb.Append(costString);

        //    // Add more random characters to the string
        //    int lengthOfWordToGenerate = random.Next(3, 20); // Generate a random length between 3 and 20 characters 
        //    string allCharacters = validCharacters + invalidCharacters;
        //    for (int i = 2; i < lengthOfWordToGenerate; i++) // Starting from 2 as we already added one valid and one invalid character
        //    {
        //        int randomNumberBetween_0_And_allCharactersLength = random.Next(allCharacters.Length);
        //        char randomChar = allCharacters[randomNumberBetween_0_And_allCharactersLength];
        //        sb.Append(randomChar);
        //    }

        //    string invalidStringToGenerate = sb.ToString();

        //    return invalidStringToGenerate;
        //}

        //protected string GenerateRandomValid_ProductCost()
        //{
        //    // Define the maximum and minimum values for a valid cost
        //    float minCost = 1f;
        //    float maxCost = 1000.00f;

        //    // Generate a random float value within the defined range
        //    Random random = new Random();
        //    float cost = (float)(random.NextDouble() * (maxCost - minCost) + minCost);
        //    string costString = cost.ToString("F2");

        //    costGlobalVriable = costString;
        //    return costString;
        //}

        //protected string GenerateRandomValid_ProductPrice()
        //{
        //    Random random = new Random();
        //    double price;

        //    do
        //    {
        //        // Generate a random price between 0.01 and 2000.00
        //        price = Math.Round(random.NextDouble() * 2000.0, 2) + 0.01;

        //    } while (price - float.Parse(costGlobalVriable) < 1.0 && price <= 100_000);

        //    return price.ToString();
        //}

        //protected string GenerateRandomInvalid_ProductPrice()
        //{
        //    string invalidCharacters = @"'""$%&/\\;!?|<>+=\[\]{}:,'\\.*^`#~";

        //    Random random = new Random();
        //    StringBuilder sb = new StringBuilder();
        //    int lengthOfPriceString = random.Next(3, 20); // Generate a random length between 3 and 20 characters 

        //    // Ensure at least one valid character is included
        //    int randomNumberBetween_0_And_invalidCharactersLength = random.Next(invalidCharacters.Length);
        //    char randomInvalidChar = invalidCharacters[randomNumberBetween_0_And_invalidCharactersLength];
        //    sb.Append(randomInvalidChar);

        //    // Fill the rest of the string with random digits and invalid characters
        //    for (int i = 1; i < lengthOfPriceString; i++) // Starting from 1 as we already added one invalid character
        //    {
        //        if (i % 3 == 0)
        //        {
        //            sb.Append(randomInvalidChar); // Add invalid character every 3rd character
        //        }
        //        else
        //        {
        //            sb.Append(random.Next(0, 10)); // Add random digit
        //        }
        //    }

        //    string invalidPriceString = sb.ToString();

        //    return invalidPriceString;
        //}

        //protected string GenerateRandomValid_StockQuantity()
        //{
        //    // Define the maximum value for a valid stock quantity
        //    int maxStockQuantity = 1_000_000; // 1 billion

        //    // Generate a random integer value within the defined range
        //    Random random = new Random();
        //    int stockQuantity = random.Next(1, maxStockQuantity + 1); // Generate between 1 and maxStockQuantity (inclusive)
        //    string stockQuantityString = stockQuantity.ToString();

        //    return stockQuantityString;
        //}

        //protected string GenerateRandomInvalid_StockQuantity()
        //{
        //    Random random = new Random();
        //    StringBuilder sb = new StringBuilder();

        //    // Generate a random length between 1 and 10 characters for variety
        //    int length = random.Next(1, 11);

        //    // Add a mix of letters, special characters, and numbers
        //    string invalidCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ" +
        //                               "!@#$%^&*()_+-=[]{};:'\"\\|,.<>/?`~" +
        //                               "1234567890";

        //    bool hasNumber = false;
        //    for (int i = 0; i < length; i++)
        //    {
        //        int index = random.Next(0, invalidCharacters.Length);
        //        char selectedChar = invalidCharacters[index];

        //        if (char.IsDigit(selectedChar))
        //        {
        //            hasNumber = true;
        //        }

        //        sb.Append(selectedChar);
        //    }

        //    // Ensure that the generated string is not purely numeric
        //    if (hasNumber && sb.Length == sb.ToString().Count(char.IsDigit))
        //    {
        //        // If the string is purely numeric, add an invalid character to make it invalid
        //        sb.Append(invalidCharacters[random.Next(0, invalidCharacters.Length - 10)]);
        //    }

        //    return sb.ToString();
        //}


        //protected void GenerateRandomValid_ProductRecordToAdd()
        //{
        //    //  productid is auto generate and selected category is paints already set in the constructor and existing in the datbase
        //    string productName = GenerateRandomValid_ProductName();
        //    string productDescription = GenerateRandomValid_ProductDescription();
        //    string cost = GenerateRandomValid_ProductCost();
        //    string price = GenerateRandomValid_ProductPrice();
        //    string stockQuantity = GenerateRandomValid_StockQuantity();

        //    EntredProductName = productName;
        //    EnteredProductDescription = productDescription;
        //    EntredCost = cost;
        //    EnteredPrice = price;
        //    EntredStockQuantity = stockQuantity;
        //}

        //protected void GenerateRandomInValidProductRecordToAdd()
        //{
        //    string productName = GenerateRandomInvalid_ProductName();
        //    string productDescription = GenerateRandomInvalid_ProductDescription();
        //    string cost = GenerateRandomInvalid_ProductCost();
        //    string price = GenerateRandomInvalid_ProductPrice();
        //    string stockQuantity = GenerateRandomInvalid_StockQuantity();

        //    EntredProductName = productName;
        //    EnteredProductDescription = productDescription;
        //    EntredCost = cost;
        //    EnteredPrice = price;
        //    EntredStockQuantity = stockQuantity;
        //}

        //async void Insert_RandomValidProduct_10_000_Times_MainFunction()
        //{
        //    for (int i = 0; i < 10_000; i++)
        //    {
        //        // Assuming _ProductID, _ProductName, _ProductDescription,
        //        // _StockQuantity, _Price, _Cost, _SelectedImageToDisplay, _SelectedCategory are defined somewhere

        //        getNewProductIdGeneratedFromDatabase();
        //        GenerateRandomValid_ProductRecordToAdd();

        //        _SelectedCategory = "didi";
        //        ProductInfo productInfoFilledByUser = new ProductInfo(
        //             _ProductID,
        //            _ProductName,
        //            _ProductDescription,
        //            _StockQuantity,
        //            _Price,
        //            _Cost,
        //            _SelectedImageToDisplay,
        //            _SelectedCategory);

        //        bool TheValidRecordIsFailedAtUi = !await CheckIfFormIsFilledCorreclty().FirstAsync();

        //       if (TheValidRecordIsFailedAtUi) {

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


        //        if (!AccessToClassLibraryBackendProject.AddProductToDataBase(productInfoFilledByUser))
        //        {
        //            Debug.WriteLine($"Product failed to be added :");
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

        //    Debug.WriteLine("operation succeded ");
        //}

        //async void Insert_RandomInValidProduct_10_000_Times_MainFunction()
        //{
        //    for (int i = 1; i < 10_000; i++)
        //    {
        //        // Assuming _ProductID, _ProductName, _ProductDescription,
        //        // _StockQuantity, _Price, _Cost, _SelectedImageToDisplay, _SelectedCategory are defined somewhere

        //        getNewProductIdGeneratedFromDatabase();
        //        GenerateRandomInValidProductRecordToAdd();

        //        _SelectedCategory = "paints";
        //        ProductInfo productInfoFilledByUser = new ProductInfo(
        //            _ProductID,
        //            _ProductName,
        //            _ProductDescription,
        //            _StockQuantity,
        //            _Price,
        //            _Cost,
        //            _SelectedImageToDisplay,
        //            _SelectedCategory);

        //        bool TheInValidRecordIsSuccedAtUi = await CheckIfFormIsFilledCorreclty().FirstAsync();

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

        //    Debug.WriteLine("operation succeded ");
        //}
   

        //public async Task <bool> AddProductPartOfEndToEndTest( string productname, string productdescription,string stockquantity, string price, string cost,string Theselectedcategory)
        //{
        //     getNewProductIdGeneratedFromDatabase();
        //    EntredProductName = productname;
        //    EnteredProductDescription = productdescription;
        //    EntredStockQuantity = stockquantity;
        //    _EntredPrice = price;
        //    _EntredCost = cost;
        //    SelectedCategory = Theselectedcategory;
        //    CalculatedBenefit = (double.Parse(price) - double.Parse(cost)).ToString();

        //    if (!await CheckIfFormIsFilledCorreclty().FirstAsync()) return false;

        //    ProductInfo ProductInfoFilledByUser =
        //          new ProductInfo(_ProductID, _ProductName, _ProductDescription, _StockQuantity, float.Parse(_EntredPrice), float.Parse(_EntredCost), _SelectedImageToDisplay, _SelectedCategory);

        //    return AccessToClassLibraryBackendProject.AddProductToDataBase(ProductInfoFilledByUser);
        //}
    }
}



