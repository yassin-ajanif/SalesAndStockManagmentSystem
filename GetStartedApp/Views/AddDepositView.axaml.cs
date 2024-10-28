using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using GetStartedApp.ViewModels;
using ReactiveUI;
using System.Reactive;

namespace GetStartedApp.Views;

public partial class AddDepositView : ReactiveUserControl<AddDepositViewModel>
{
    public AddDepositView()
    {
        InitializeComponent();
        RegisterShowDialogSalesEvents();
    }

    private void RegisterShowDialogSalesEvents()
    {

        this.WhenActivated(action => action(ViewModel!.AddDepositInteraction.RegisterHandler(UserHasSubmittedCorrectChequeInfo)));

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