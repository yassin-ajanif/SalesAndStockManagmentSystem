using ReactiveUI;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ExCSS;
using GetStartedApp.Models.Enums;

namespace GetStartedApp.ViewModels;

/// <summary>
/// public class ViewModelBase : ReactiveObject
/// </summary>
public class ViewModelBase : ReactiveObject ,INotifyDataErrorInfo
{
 
    public static eLoginMode AppLoginMode { get; set; }


  private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
 
  public bool HasErrors => _errors.Count > 0;
 
  public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

   private List<string> propertyAttributeErrors;

    // we get error from custom attributes
    private List<string> GetAttributeErrors(string propertyName)
    {
        var errors = new List<string>();
        var property = GetType().GetProperty(propertyName);

        if (property == null) return errors;

        // Get the value of the property
        var propertyValue = property.GetValue(this);

        // Simplify retrieval and processing of attributes
        var attributes = property.GetCustomAttributes(typeof(ValidationAttribute), true);

        foreach (ValidationAttribute attribute in attributes)
        {
            var validationResult = attribute.GetValidationResult(propertyValue, new ValidationContext(this));
            if (validationResult != ValidationResult.Success)
            {
                errors.Add(validationResult.ErrorMessage);
            }
        }

        return errors;
    }

    public IEnumerable GetErrors(string propertyName)
  {
      
      
        if (string.IsNullOrEmpty(propertyName) || !_errors.ContainsKey(propertyName))
      {
          return null;
      }
      return _errors[propertyName];
  }
 
  protected void OnErrorsChanged(string propertyName)
  {
      ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
  }
 
     private void GetActuallAttributesErros_And_LoadThem(string propertyName,string errorMessage)
    {
        propertyAttributeErrors = GetAttributeErrors(propertyName);
        
        _errors[propertyName] = propertyAttributeErrors;
    }
     protected void ShowUiError(string propertyName, string errorMessage)
     {
           // Clear existing errors for the property
        

           GetActuallAttributesErros_And_LoadThem(propertyName,errorMessage);
           propertyAttributeErrors.Add(errorMessage);
     
           OnErrorsChanged(propertyName);
        
    }

    protected void DeleteUiError(string propertyName, string errorMessage)
    {
        
            GetActuallAttributesErros_And_LoadThem(propertyName, errorMessage);

        if (_errors.ContainsKey(propertyName))
       {
     
          // Remove the specific error message if it exists
           if (propertyAttributeErrors.Contains(errorMessage))
           {
                _errors.Remove(errorMessage);
               
            }

            OnErrorsChanged(propertyName);
        }
    
    }


}
