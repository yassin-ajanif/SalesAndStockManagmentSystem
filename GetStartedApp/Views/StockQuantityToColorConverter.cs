using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media;
using GetStartedApp.Models;

namespace GetStartedApp.Views
{

    // this class is used to change the color of items where their stock values is less than 10 
    // to give a user a warning to fill up the stock 
    public class StockQuantityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Check if the value is a ProductInfo object
            if (value is ProductInfo productInfo)
            {
                // Access the StockQuantity property and check if it meets the condition
                if (productInfo.StockQuantity <= AccessToClassLibraryBackendProject.GetMinimalStockValue())
                {
                    // Return red color if stock quantity is less than 10
                    return Brushes.Red;
                   // return new Tuple<IBrush, IBrush>(Brushes.Red, Brushes.White);
                }
            }

            // Return default color if condition is not met
            return Brushes.Black; // Or any other default color you want
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // This method is not used in one-way binding
            throw new NotImplementedException();
        }
    }
}
