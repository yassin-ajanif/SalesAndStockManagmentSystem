using System;
using System.Collections.Generic;
using Avalonia.Media.Imaging;

namespace GetStartedApp.Models
{
    public class ReturnedProduct : ProductSold
    {
        public ReturnedProduct(long productId , string productName , float originalPrice , float soldPrice, int quantitiy, Bitmap Image ) 
            : base(productId,productName,originalPrice,soldPrice,quantitiy,Image)
        { }
    }
}
