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

        SetLanguageSystemAsArabicMoroccoCustomized();

        AvaloniaXamlLoader.Load(this);
    }

    private void SetLanguageSystemAsArabicMoroccoCustomized()
    {
        // this is for setting invariante culture which is making the system not get affected by lanuage changing which cause issues
        // for example in US local or language teh comma is . while in french is , so that create issues in my application that was built 
        // based on US lanauge computer

        // this culture normally cames with "," comma for decimal number i've modifed this option to dot "." because my system was built on that us form

        CultureInfo arabicCulture = new CultureInfo("ar-MA");

        CultureInfo customArabicCulture = (CultureInfo)arabicCulture.Clone();
        customArabicCulture.NumberFormat.NumberDecimalSeparator = ".";
        customArabicCulture.NumberFormat.NumberGroupSeparator = ",";
        // Apply to both culture and UI culture
        CultureInfo.DefaultThreadCurrentCulture = customArabicCulture;
        CultureInfo.DefaultThreadCurrentUICulture = customArabicCulture;
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