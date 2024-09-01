using Avalonia.Controls;
using Avalonia.ReactiveUI;
using GetStartedApp.ViewModels.ProductPages;
using System;
using System.Diagnostics;
using ReactiveUI;

using System.Reactive.Disposables;
using Avalonia.Interactivity;


namespace GetStartedApp.Views
{
    public partial class ShowMessageBoxContainer : Window
    {
        

    

        public ShowMessageBoxContainer(string messageToShow, bool areButtonsVisible=false)
        {
            InitializeComponent();

            var messageTextBlock = this.FindControl<TextBlock>("MessageTextBlock");
            var yesButton = this.FindControl<Button>("YesButton");
            var noButton = this.FindControl<Button>("NoButton");

            messageTextBlock.Text = messageToShow;
            yesButton.IsVisible = areButtonsVisible;
            noButton.IsVisible = areButtonsVisible;

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }


        public void OnYesBtnClicked(object sender, RoutedEventArgs e)
        {
            
                Close(true);
            
        }

        public void OnNoBtnClicked(object sender, RoutedEventArgs e)
        {
           
                Close(false);
            
        }

       
    }
}
