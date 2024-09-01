using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using GetStartedApp.ViewModels;

namespace GetStartedApp;

public class ViewLocator : IDataTemplate
{

    public Control? Build(object? data)
    {
        if (data is null)
            return null;

  
        var name = data.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);

        var type = Type.GetType(name);

        if (type != null)
        {
            var control = (Control)Activator.CreateInstance(type)!;
            control.DataContext = data;
            return control;
        }
        
        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }

    public string ExtractViewModelNameFromPath(string fullTypeName)
    {
        // Split the full type name by '.' separator
        string[] parts = fullTypeName.Split('.');

        // Find the last part which should be the ViewModel class name
        string viewModelName = parts[parts.Length - 1];

        // Remove any trailing curly braces or other characters
        viewModelName = viewModelName.Trim('{', '}');

        return viewModelName;
    }
}
