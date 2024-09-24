using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GetStartedApp.ViewModels;
using System.Threading.Tasks;
using System;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using ReactiveUI;
using System.Reactive;

namespace GetStartedApp.Views;

public partial class CompanyInfosView : ReactiveUserControl<CompanyInfosViewModel>
{
    public CompanyInfosView()
    {
        InitializeComponent();
        RegisterShowMessageBoxDialogEvents();
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

    private void RegisterShowMessageBoxDialogEvents()
    {
        this.WhenActivated(action => {

            action(ViewModel!.ShowMessageBoxDialog.RegisterHandler(ShowMessageBox));   
        }
        );
    }

    private async Task ShowMessageBox(InteractionContext<string, Unit> context)
    {


        var window = this.GetVisualRoot() as Window;
        var dialog = new ShowMessageBoxContainer(context.Input);

        await dialog.ShowDialog(window);

        //  window.Close();          
        context.SetOutput(Unit.Default);

        // we won't close windows if the user is going to print barcode , becuase from this windows were going to open barcode widnows
        // closing that will cause crash in the program

        window.Close();

    }

}