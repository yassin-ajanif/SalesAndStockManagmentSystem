
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
            }
        }

        private string _previousReturnedQuantity = "1";
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
            set
                {
                if (IsThisProductReturnable()) SetField(ref _isProductReturnable, true);
                else SetField(ref _isProductReturnable, false);
                }
        }


        public ReturnedProduct(long productId, string productName, float originalPrice, float soldPrice, int quantitiy, Bitmap Image)
            : base(productId, productName, originalPrice, soldPrice, quantitiy, Image)
        {



        }


        private void RaiseAnUiError_If_UserEntredInvalid_ProudctsNumber_To_Return(string AcutalValue_QuantityToReturn)
        {
            if (int.TryParse(AcutalValue_QuantityToReturn, out int quantityToReturn) &&
                int.TryParse(PreviousReturnedQuantity, out int previousQuantityReturned))
            {
                int quantitySold = Quantity;
                int maximumProductsUserCanReturn = quantitySold - previousQuantityReturned;

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


        private bool IsThisProductReturnable()
        {
            int QuantitySold = Quantity;
            int PreviousQuantity = int.Parse(PreviousReturnedQuantity);

            return QuantitySold > PreviousQuantity;
        }
    }
}
