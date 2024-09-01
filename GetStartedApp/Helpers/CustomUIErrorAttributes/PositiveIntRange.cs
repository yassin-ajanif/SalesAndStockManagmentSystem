using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStartedApp.Helpers.CustomUIErrorAttributes
{
    public class PositiveIntRange : ValidationAttribute
    {
        public long Minimum { get; set; }
        public long Maximum { get; set; }

        public PositiveIntRange(long minimum, long maximum)
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

            if (long.TryParse(stringValue, out long number))
            {
                return number >= Minimum && number <= Maximum;
            }
            return false;
        }
    }

}
