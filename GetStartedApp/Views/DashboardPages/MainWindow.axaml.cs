using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using GetStartedApp.ViewModels.DashboardPages;
using GetStartedApp.ViewModels.ProductPages;
using GetStartedApp.Views.ProductPages;
using ReactiveUI;
using System.Threading.Tasks;
using GetStartedApp.Models;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
using SalesProductsManagmentSystemBusinessLayer;
using System.IO;
using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Diagnostics;
using System.Reactive;
using GetStartedApp.Views.DashboardPages;
using Avalonia.VisualTree;
using Microsoft.VisualBasic;
using GetStartedApp.ViewModels;
using QuestPDF.Elements;


namespace GetStartedApp.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{

 
    public MainWindow()
    {
       
        InitializeComponent();
        
    }

    private async void ShowSetMinimalStockViewBoundToViewmodel_()
    {
        var dialog = new DialogContainerView
        {
            Title = "المخزن",
            MaxWidth = 300,
            MaxHeight = 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };
        // CurrentDialogOpened = dialog;
        // Create an instance of AddOrEditOrKnowProductView and set it as the Content of the dialog
        dialog.Content = new SetMinimalStockQuanityToNotifyUserView()
        {
            DataContext = new SetMinimalStockQuanityToNotifyUserViewModel(),
           
        };

        var window = this.GetVisualRoot() as Window;
        if (window == null)
        {
            throw new InvalidOperationException("Cannot show dialog because this control is not contained within a Window.");
        }


        var result = await dialog.ShowDialog<Unit>(window);

    }
    private async void OnEditMinStockValue_Before_RaisingBellNotification(object sender, RoutedEventArgs e)
    {

        ShowSetMinimalStockViewBoundToViewmodel_();
        
    }

   
}