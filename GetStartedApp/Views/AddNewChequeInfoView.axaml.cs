using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using ReactiveUI;
using System.Threading.Tasks;
using System.Reactive ;
using Avalonia.ReactiveUI;
using GetStartedApp.ViewModels;
using Avalonia.Interactivity;

namespace GetStartedApp.Views;

public partial class AddNewChequeInfoView : ReactiveUserControl<AddNewChequeInfoViewModel>
{
    public AddNewChequeInfoView()
    {
        InitializeComponent();
        RegisterShowDialogSalesEvents();
    }

    private void RegisterShowDialogSalesEvents()
    {
    
       this.WhenActivated(action=>action(ViewModel!.addNewChequeInfoInteraction.RegisterHandler(UserHasSubmittedCorrectChequeInfo)));

    }

    private async void UserHasSubmittedCorrectChequeInfo(InteractionContext<Unit, Unit> interaction)
    {
        var parentWindow = this.GetVisualRoot() as Window;
        bool userHasSubmitedChequeInfoCorrectly = true;

        if (parentWindow != null)
        {
            // this function trigger the dialog function and returns true indicating that user didn't leave the page and has entred corect info
            parentWindow.Close(userHasSubmitedChequeInfoCorrectly); // Pass true to indicate successful completion
        }

        interaction.SetOutput(Unit.Default);
    }
}