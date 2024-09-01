using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using MsBox.Avalonia.Base;
using System.Text.RegularExpressions;

namespace GetStartedApp.Helpers.CustomUIErrorAttributes
{

    public class PositiveFloatRange : ValidationAttribute
    {
        public float Minimum { get; set; }
        public float Maximum { get; set; }

        public PositiveFloatRange(float minimum, float maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }

        public override bool IsValid(object value)
        {
            string stringValue = value as string;


            if (string.IsNullOrEmpty(stringValue))
            {
                // If the string is null or empty, it's considered valid.
                return true;
            }

            // if a string contains e which is exponotial value it parsable but it is not welcomed in our app 
            // for example writing 1e3 will not throw any error but in our case we will not allow that 
            if (Regex.IsMatch(stringValue, "e")) return false;

            if (float.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out float number))
            {
                return number >= Minimum && number <= Maximum;
            }
            return false;
        }
    }



}

