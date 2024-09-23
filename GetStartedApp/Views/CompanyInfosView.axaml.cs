using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GetStartedApp.ViewModels;
using System.Threading.Tasks;
using System;
using Avalonia.ReactiveUI;

namespace GetStartedApp.Views;

public partial class CompanyInfosView : ReactiveUserControl<CompanyInfosViewModel>
{
    public CompanyInfosView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is CompanyInfosViewModel myMainWindowViewModel)
        {
            myMainWindowViewModel.PickCompanyLogoImageFunc = OnPickImageButtonClick;
        }
    }
    public async Task<string> OnPickImageButtonClick()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Title = "Select Image";
        openFileDialog.Filters.Add(new FileDialogFilter() { Name = "Image Files", Extensions = { "png", "jpg", "jpeg", "gif" } });

        var result = await openFileDialog.ShowAsync((Window)this.VisualRoot);

        if (result != null && result.Length > 0)
        {
            // Handle the selected image file
            string selectedImagePath = result[0];
            // Do something with the selected image path, e.g., display it in an Image control

            return selectedImagePath;
        }

        return "";
    }
}