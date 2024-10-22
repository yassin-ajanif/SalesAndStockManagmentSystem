using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStartedApp.Helpers.CustomUIErrorAttributes
{
    public class PositiveIntRangeNotNullAllowed : ValidationAttribute
    {   
            public int Minimum { get; set; }
            public int Maximum { get; set; }

            public PositiveIntRangeNotNullAllowed(int minimum, int maximum)
            {
                Minimum = minimum;
                Maximum = maximum;
            }

            public override bool IsValid(object value)
            {
                string stringValue = value as string;
                if (string.IsNullOrWhiteSpace(stringValue))
                {
                    // If the string is null or empty, it's considered valid.
                    return false;
                }

                if (int.TryParse(stringValue, out int number))
                {
                    return number >= Minimum && number <= Maximum;
                }
                return false;
            }
        }
    
}
