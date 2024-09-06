using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStartedApp.Helpers
{
    using System.Globalization;

   
        public static class CultureHelper
        {
            // Static method to set the language and customize culture settings
            public static void SetLanguageSystem(string cultureString = "en-US")
            {

            // this is for setting invariante culture which is making the system not get affected by lanuage changing which cause issues
            // for example in US local or language teh comma is . while in french is , so that create issues in my application that was built 
            // based on US lanauge computer

            // this culture normally cames with "," comma for decimal number i've modifed this option to dot "." because my system was built on that us form

            // Set the specified culture for resources and culture info
                CultureInfo Culture = Assets.Languages.Resources.Culture = new CultureInfo(cultureString);
                CultureInfo DynamicCulture = Culture;

                // Clone the culture and modify its number formatting settings
                CultureInfo customCulture = (CultureInfo)DynamicCulture.Clone();
                customCulture.NumberFormat.NumberDecimalSeparator = ".";
                customCulture.NumberFormat.NumberGroupSeparator = ",";

                // Apply the customized culture to both thread-specific and UI-specific cultures
                CultureInfo.DefaultThreadCurrentCulture = customCulture;
                CultureInfo.DefaultThreadCurrentUICulture = customCulture;
            }
        }
    }


