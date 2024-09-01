using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using GetStartedApp.ViewModels.DashboardPages;
using GetStartedApp.ViewModels.ProductPages;

namespace GetStartedApp;

public partial class SetMinimalStockQuanityToNotifyUserView : ReactiveUserControl<SetMinimalStockQuanityToNotifyUserViewModel>
{
    public SetMinimalStockQuanityToNotifyUserView()
    {
        InitializeComponent();
    }

    private async void OnSaveCommand(object sender, RoutedEventArgs e)
    {
        if (DataContext is not SetMinimalStockQuanityToNotifyUserViewModel myViewModel) return;

        myViewModel.Save();

        // Find the parent window and close it
        var window = this.VisualRoot as Window;
        window?.Close();
    }

}