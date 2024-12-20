﻿using GetStartedApp.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GetStartedApp.ViewModels.DashboardPages
{
    public class DashboardViewModel : ViewModelBase
    {

        private MainWindowViewModel MainWindowViewModel { get; set; }

        public ICommand GoToProductsPageCommand { get; private set; }
        public ICommand GoToBarCodePageCommand { get; private set; }
        public ICommand GoToCategoryProductsPageCommand { get; private set; }
        public ICommand GoToMakeSalePageCommand { get; private set; }
        public ICommand GoToReturnProductPageCommand { get; private set; }
        public ICommand GoToFinancesPageCommand { get; private set; }
        public ICommand GoToSoldItemsPageCommand { get; private set; }
        public ICommand GoToLoginPageCommand { get; private set; }


        // this funtion is responsibe for enabling the commands that will be sending through btns
        // for example button command="Binding GoToProductsPageCommand"
        // this command is the one that is going to trigger the function GoToProductPage in  mainviewModel as pricipal windows

        private void EnableBtnCommands()
        {

          GoToProductsPageCommand = ReactiveCommand.Create(MainWindowViewModel.GoToProductPage);
          GoToBarCodePageCommand = ReactiveCommand.Create(MainWindowViewModel.GoToBarCodeProductPage);
          GoToCategoryProductsPageCommand = ReactiveCommand.Create(MainWindowViewModel.GoToCategoriesProductPage);
          GoToMakeSalePageCommand = ReactiveCommand.Create(MainWindowViewModel.GoToMakeSalePage);
          GoToReturnProductPageCommand = ReactiveCommand.Create(MainWindowViewModel.GoToRetrunProductPage);
          GoToFinancesPageCommand = ReactiveCommand.Create(MainWindowViewModel.GoToFinancesPage,canGoToFinancesPage());
          GoToSoldItemsPageCommand = ReactiveCommand.Create(MainWindowViewModel.GoToSoldItemsPage);
          GoToLoginPageCommand = ReactiveCommand.Create(MainWindowViewModel.GoToLoginPage);
        }

        public DashboardViewModel(MainWindowViewModel MainWindowViewPage)
        {
            MainWindowViewModel = MainWindowViewPage;

           if(MainWindowViewModel!=null) EnableBtnCommands();

          

        }

        public IObservable<bool> canGoToFinancesPage() {


            if (ViewModelBase.AppLoginMode == eLoginMode.Admin) return Observable.Return(true);

            return Observable.Return(false);
    }


}
}
