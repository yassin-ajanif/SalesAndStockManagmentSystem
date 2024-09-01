using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStartedApp.Helpers
{
    internal class DataEntryValidation
    {
        private static bool TryParseInt(string input)
        {
            if (int.TryParse(input, out int result))
            {
                return true;
            }
            
            return false;
        }

        private static bool TryParselong(string input)
        {
            if (long.TryParse(input, out long result))
            {
                return true;
            }

            return false;
        }

        private static bool TryParseFloat(string input)
        {
            
           return float.TryParse(input, out float result);
            
        }
   

        public static bool IsThisStringValidInteger(string input)
        {

            if (TryParseInt(input)) return true;

            return false;
        }

        public static bool IsThisStringValidLongInteger(string input)
        {

            if (TryParselong(input)) return true;

            return false;
        }

        public static bool IsThisStringValidPositiveInteger(string input)
        {
            if (int.TryParse(input, out int result) && result > 0)
            {
                return true;
            }
            return false;
        }


        public static bool IsThisStringValidFloat(string input)
        {

            if (TryParseFloat(input)) return true;

            return false;
        }

        public static int StringConvertedToInteger(string input){     return int.Parse(input); }
        public static long StringConvertedTolong(string input){     return long.Parse(input); }

        public static float StringConvertedToFloat(string input) { return float.Parse(input); }

   

    }
}
