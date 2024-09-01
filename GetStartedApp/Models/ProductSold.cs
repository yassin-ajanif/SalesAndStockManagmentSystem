using Avalonia.Media.Imaging;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace GetStartedApp.Models
{
    public class ProductSold : INotifyPropertyChanged
    {
        private long _productId;
        private string _productName;
        private float _originalPrice;
        private float _soldPrice;
        private int _quantity;
        private Bitmap _image;

        public event PropertyChangedEventHandler PropertyChanged;

        public long ProductId
        {
            get => _productId;
            set => SetField(ref _productId, value);
        }

        public string ProductName
        {
            get => _productName;
            set => SetField(ref _productName, value);
        }

        public float OriginalPrice
        {
            get => _originalPrice;
            set => SetField(ref _originalPrice, value);
        }

        public float SoldPrice
        {
            get => _soldPrice;
            set => SetField(ref _soldPrice, value);
        }

        public int Quantity
        {
            get => _quantity;
            set => SetField(ref _quantity, value);
        }

        public Bitmap Image
        {
            get => _image;
            set => SetField(ref _image, value);
        }

        public ProductSold(long productId, string productName, float originalPrice, float soldPrice, int quantity, Bitmap image)
        {
            _productId = productId;
            _productName = productName;
            _originalPrice = originalPrice;
            _soldPrice = soldPrice;
            _quantity = quantity;
            _image = image;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Debug.WriteLine($"Property changed: {propertyName}");
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
