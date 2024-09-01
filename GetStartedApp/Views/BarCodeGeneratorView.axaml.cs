using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace GetStartedApp.Views;

public partial class BarCodeGeneratorView : UserControl
{
    public BarCodeGeneratorView()
    {
        InitializeComponent();
        barCodeViewerImage.Width = 350;
    }

    private void ZoomIn(object sender, RoutedEventArgs e)
    {
        if (barCodeViewerImage.Width > 800) return;
        barCodeViewerImage.Width +=50;
    }

    private void ZoomOut(object sender, RoutedEventArgs e)
    {
        if (barCodeViewerImage.Width < 400) return;
        barCodeViewerImage.Width -= 50;
    }

}