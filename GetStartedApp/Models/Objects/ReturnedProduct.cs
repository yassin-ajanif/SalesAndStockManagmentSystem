
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using ReactiveUI;
using GetStartedApp.Helpers.CustomUIErrorAttributes;
using ReactiveUI;
using System;
using System.Reactive.Linq;



namespace GetStartedApp.Models.Objects
{
    public class ReturnedProduct : ProductSold
    {
        private const int maxProductNumberCanBeReturned = 10_000;

        private bool _isCheckedForReturn = false;
        public bool IsCheckedForReturn
        {
            get => _isCheckedForReturn;
            set => SetField(ref _isCheckedForReturn, value);
        }

        private string _quanityToReturn = "0";
        [PositiveIntRangeNotNullAllowed(0, maxProductNumberCanBeReturned, ErrorMessage = "ادخل رقم موجب")]
        public string QuanityToReturn
        {
            get => _quanityToReturn;

            set {

                SetField(ref _quanityToReturn, value);
                RaiseAnUiError_If_UserEntredInvalid_ProudctsNumber_To_Return(value);
                CheckTheBoxIfProductQuantityToReturnIsEdited(value);
            }
        }

        private string _previousReturnedQuantity;
        public string PreviousReturnedQuantity
        {
            get => _previousReturnedQuantity;
            set{

                SetField(ref _previousReturnedQuantity, value);
               
            }
        }

        private bool _isProductReturnable;

        // if user has return the full quanity sold equal to previous return products the product won't be returnable
        public bool IsProductReturnable
        {
            get => _isProductReturnable;
            set => SetField(ref _isProductReturnable, value);   
        }

        public int maximumProductsUserCanReturn => Quantity - int.Parse(PreviousReturnedQuantity);

        public int SoldItemID { get; }

        // this constructor is made when we return product without introducing the saleid normally is made for unkown clients
        public ReturnedProduct(long productId, string productName, float originalPrice, float soldPrice, int quantitiy, Bitmap Image)
           : base(productId, productName, originalPrice, soldPrice, quantitiy, Image) { }
       

        // this constructor is made for know saleid operations so in this case we need to have solditemid that we want to return
        public ReturnedProduct(int soldItemID, long productId, string productName, float originalPrice, float soldPrice, int quantitiy, int PreviousReturnedUnits, Bitmap Image)
            : base(productId, productName, originalPrice, soldPrice, quantitiy, Image)
        {
            
            PreviousReturnedQuantity = PreviousReturnedUnits.ToString();
            SoldItemID = soldItemID;
            IsProductReturnable = IsThisProductReturnable();

        }


        private void RaiseAnUiError_If_UserEntredInvalid_ProudctsNumber_To_Return(string AcutalValue_QuantityToReturn)
        {
            if (int.TryParse(AcutalValue_QuantityToReturn, out int quantityToReturn) &&
                int.TryParse(PreviousReturnedQuantity, out int previousQuantityReturned))
            {
                int quantitySold = Quantity;
               // int maximumProductsUserCanReturn = quantitySold - previousQuantityReturned;

                // Ensure returned quantity does not exceed the sold quantity
                if (quantityToReturn > maximumProductsUserCanReturn)
                {
                    ShowUiError(nameof(QuanityToReturn),
                        $"يمكنك إرجاع فقط {maximumProductsUserCanReturn} منتجات");
                }
                else
                {
                    DeleteUiError(nameof(QuanityToReturn),
                        $"يمكنك إرجاع فقط {maximumProductsUserCanReturn} منتجات");
                }
            }
            
        }


        public bool IsThisProductReturnable()
        {
            int QuantitySold = Quantity;
            int PreviousQuantity = int.Parse(PreviousReturnedQuantity);

            return QuantitySold > PreviousQuantity;
        }


        private void CheckTheBoxIfProductQuantityToReturnIsEdited(string quantityToReturn)
        {
            // Try to parse the string input to an integer
            if (int.TryParse(quantityToReturn, out int quantity))
            {
                // Check if the quantity is greater than zero and doesn't exceed the maximum allowed return quantity
                if (quantity > 0 && quantity <= maximumProductsUserCanReturn)
                {
                    IsCheckedForReturn = true;
                }
                else
                {
                    IsCheckedForReturn = false; // Quantity is either zero or exceeds allowed quantity
                }
            }
            else
            {
                // Invalid input (e.g., not a number)
                IsCheckedForReturn = false;
            }
        }


    }
}
