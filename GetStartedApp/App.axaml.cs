using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GetStartedApp.ViewModels.DashboardPages;
using GetStartedApp.ViewModels.ProductPages;
using GetStartedApp.Views;
using System.Globalization;

namespace GetStartedApp;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

 

    public override void OnFrameworkInitializationCompleted()
    {
      
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
                
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}