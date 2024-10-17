using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStartedApp.Helpers
{
    public static class TimeHelper
    {
        public static string FormatDateWithDayInArabic(DateTime operationTime)
        {
            var culture = new CultureInfo("ar-MA");
            return operationTime.ToString("dddd, dd MMMM yyyy HH:mm", culture);
        }
    }
}
