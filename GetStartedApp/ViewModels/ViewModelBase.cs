using ReactiveUI;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GetStartedApp.Models;
using System;
using System.Collections.Generic;
using System.Collections;

namespace GetStartedApp.ViewModels;

/// <summary>
/// public class ViewModelBase : ReactiveObject
/// </summary>
public class ViewModelBase : ReactiveObject ,INotifyDataErrorInfo
{
 
    public static eLoginMode AppLoginMode { get; set; }


  private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
 
  public bool HasErrors => _errors.Count > 0;
 
  public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
 
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
 
  protected void ShowError(string propertyName, string errorMessage, bool hasError)
  {
      // Clear existing errors for the property
      if (_errors.ContainsKey(propertyName))
      {
          _errors.Remove(propertyName);
      }
 
      if (hasError)
      {
          List<string> propertyErrors = new List<string> { errorMessage };
          _errors[propertyName] = propertyErrors;
      }
 
      OnErrorsChanged(propertyName);
  }

}
