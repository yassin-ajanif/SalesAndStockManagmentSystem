
using Avalonia.Media.Imaging;
using ReactiveUI;


namespace GetStartedApp.Models.Objects
{
    public class ReturnedProduct : ProductSold
    {
       
        private bool _isCheckedForReturn = false;
        public bool IsCheckedForReturn
        {
            get => _isCheckedForReturn;
            set => this.RaiseAndSetIfChanged(ref _isCheckedForReturn, value);
        }

        public ReturnedProduct(long productId, string productName, float originalPrice, float soldPrice, int quantitiy, Bitmap Image)
            : base(productId, productName, originalPrice, soldPrice, quantitiy, Image)
        { }
    }
}
