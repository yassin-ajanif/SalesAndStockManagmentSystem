using ExCSS;
using GetStartedApp.ViewModels.DashboardPages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStartedApp.Helpers
{
    public static class UiAttributeChecker
    {
        public static bool AreThesesAttributesPropertiesValid(object obj, params string[] propertyNames)
        {
            var type = obj.GetType();
            foreach (var propertyName in propertyNames)
            {
                var property = type.GetProperty(propertyName);
                if (property != null)
                {
                    var attributes = property.GetCustomAttributes(typeof(ValidationAttribute), true);


                    foreach (ValidationAttribute attribute in attributes)
                    {
                        var value = property.GetValue(obj);
                        if (!attribute.IsValid(value))
                        {
                            return false;
                        }
                    }

                }
            }

            return true;
        }

    
    }
}