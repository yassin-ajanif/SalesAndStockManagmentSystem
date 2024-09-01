using GetStartedApp.ViewModels.DashboardPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;


public class TwoPropertiesAreEqualAttribute : ValidationAttribute
{
    private readonly string _originalProperty;
    private readonly string _propertyToCompareWith;

    public TwoPropertiesAreEqualAttribute(string originalProperty, string propertyToCompareWith)
    {
        _originalProperty = originalProperty;
        _propertyToCompareWith = propertyToCompareWith;
    }

    public ValidationResult GetValidationResult(object value, ValidationContext validationContext)
    {
        return IsValid(value, validationContext);
    }

   protected override ValidationResult IsValid(object DataUserIsEntering, ValidationContext InfoAboutTheObjectInputUserIsEntering)
   {
       bool PropertyIsNotYetInitialized = DataUserIsEntering == null || InfoAboutTheObjectInputUserIsEntering == null;
  
       if (PropertyIsNotYetInitialized) return new ValidationResult(ErrorMessage);

        var originalProperty = InfoAboutTheObjectInputUserIsEntering.ObjectType.GetProperty(_originalProperty);
       var comparisonProperty = InfoAboutTheObjectInputUserIsEntering.ObjectType.GetProperty(_propertyToCompareWith);
  
       if (originalProperty == null || comparisonProperty == null)
           throw new ArgumentException("Properties with these names not found");
  
       var originalValue = originalProperty.GetValue(InfoAboutTheObjectInputUserIsEntering.ObjectInstance);
       var comparisonValue = comparisonProperty.GetValue(InfoAboutTheObjectInputUserIsEntering.ObjectInstance);
  
       if (DataUserIsEntering != null && originalValue != null && comparisonValue != null && originalValue.Equals(comparisonValue))
           return ValidationResult.Success;
  
       return new ValidationResult(ErrorMessage);
         
   }

  



}
