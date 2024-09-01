using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStartedApp.Helpers
{

    // the goal of the class is to transfer a data entred by user which is a string by :
    // first check if it can be convertable like from string to int or float 
    // convert it then load the property we want into view model to send to data layer like database or somewhere else
    public static class DataEntryPropertyLoader
    {
        // Userinput is the data entred by user
        // PropertyToLoad  is the property refrence that we want to load that it exist in our viewmodel
        public static void ConvertStringToIntAndLoadPrivateProperty(string UserInput, ref long PropertyToLoad)
        {

           
                if (DataEntryValidation.IsThisStringValidLongInteger(UserInput))
            {
                long EntredProductIDConvertedToInt = DataEntryValidation.StringConvertedTolong(UserInput);

                // entredproductid must be positive 
                if (EntredProductIDConvertedToInt >= 0)

                    PropertyToLoad = EntredProductIDConvertedToInt;

                Debug.WriteLine(PropertyToLoad);

            }
        }

        public static void ConvertStringToIntAndLoadPrivateProperty(string UserInput, ref int PropertyToLoad)
        {

            if (DataEntryValidation.IsThisStringValidInteger(UserInput))
            {
                int EntredProductIDConvertedToInt = DataEntryValidation.StringConvertedToInteger(UserInput);

                // entredproductid must be positive 
                if (EntredProductIDConvertedToInt >= 0)

                    PropertyToLoad = EntredProductIDConvertedToInt;

                Debug.WriteLine(PropertyToLoad);

            }
        }

        public static void ConvertStringToIntAndLoadPrivateProperty_Allowing_ZeroValue(string UserInput, ref int PropertyToLoad)
        {

            if (DataEntryValidation.IsThisStringValidInteger(UserInput))
            {
                int EntredProductIDConvertedToInt = DataEntryValidation.StringConvertedToInteger(UserInput);

                // entredproductid must be positive 
                if (EntredProductIDConvertedToInt >= 0)

                    PropertyToLoad = EntredProductIDConvertedToInt;

                Debug.WriteLine(PropertyToLoad);

            }
        }

        public static void ConvertStringToFloatAndLoadPrivateProperty(string UserInput, ref float PropertyToLoad)
        {
            if (DataEntryValidation.IsThisStringValidFloat(UserInput))
            {
                float EntredProductIDConvertedToInt = DataEntryValidation.StringConvertedToFloat(UserInput);

                // entredproductid must be positive 
                if (EntredProductIDConvertedToInt >= 0)

                    PropertyToLoad = EntredProductIDConvertedToInt;

                Debug.WriteLine(PropertyToLoad);

            }
        }
            public static void ConvertStringToFloatAndLoadPrivateProperty_Allowing_ZeroValue(string UserInput, ref float PropertyToLoad)
            {
                if (DataEntryValidation.IsThisStringValidFloat(UserInput))
                {
                    float EntredProductIDConvertedToInt = DataEntryValidation.StringConvertedToFloat(UserInput);

                    // entredproductid must be positive 
                    if (EntredProductIDConvertedToInt >= 0)

                        PropertyToLoad = EntredProductIDConvertedToInt;

                    Debug.WriteLine(PropertyToLoad);

                }
            }

    }
}
