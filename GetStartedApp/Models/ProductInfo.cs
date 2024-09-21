using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace GetStartedApp.Models
{
    public class ProductInfo : INotifyPropertyChanged
    {
        private long _id;
        private string _name;
        private string _description;
        private int _stockQuantity;
        private int _stockQuantity2;
        private int _stockQuantity3;
        private float _price;
        private float _cost;
        private float _profit;
        private string _selectedCategory;
        private Bitmap _selectedProductImage;

        public event PropertyChangedEventHandler PropertyChanged;

        public long id
        {
            get => _id;
            set => SetField(ref _id, value);
        }

        public string name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        public string description
        {
            get => _description;
            set => SetField(ref _description, value);
        }

        public int StockQuantity
        {
            get => _stockQuantity;
            set => SetField(ref _stockQuantity, value);
        }

        public int StockQuantity2
        {
            get => _stockQuantity2;
            set => SetField(ref _stockQuantity, value);
        }

        public int StockQuantity3
        {
            get => _stockQuantity3;
            set => SetField(ref _stockQuantity, value);
        }

        public float price
        {
            get => _price;
            set => SetField(ref _price, value);
        }

        public float cost
        {
            get => _cost;
            set => SetField(ref _cost, value);
        }

        public float profit
        {
            get => _profit;
            set => SetField(ref _profit, value);
        }

        public string selectedCategory
        {
            get => _selectedCategory;
            set => SetField(ref _selectedCategory, value);
        }

        public Bitmap SelectedProductImage
        {
            get => _selectedProductImage;
            set => SetField(ref _selectedProductImage, value);
        }

        public ProductInfo(long id, string name, string description, int stockQuantity,int stockQuantity2,int stockQuantity3, float price, float cost, Bitmap selectedProductImage, string selectedCategory)
        {
            _id = id;
            _name = name;
            _description = description;
            _stockQuantity = stockQuantity;
            _stockQuantity2 = stockQuantity2;
            _stockQuantity3 = stockQuantity3;
            _price = price;
            _cost = cost;
            _selectedProductImage = selectedProductImage;
            _selectedCategory = selectedCategory;
            _profit = price - cost; // Assuming profit is calculated from price and cost
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Debug.WriteLine($"property changed : {propertyName}");
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
