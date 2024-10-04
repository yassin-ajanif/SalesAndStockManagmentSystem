using GetStartedApp.Models;
using GetStartedApp.Models.Objects;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStartedApp.ViewModels.DashboardPages
{
    public class BonLivraisonsViewModel : DevisViewModel
    {

        private List<ClientOrCompanySaleInfo> _clientOrCompanySaleInfos;
        
        public List<ClientOrCompanySaleInfo> clientOrCompanySaleInfos{get =>  _clientOrCompanySaleInfos; set => this.RaiseAndSetIfChanged(ref _clientOrCompanySaleInfos, value); }

        public BonLivraisonsViewModel(MainWindowViewModel mainWindowViewModel):base(mainWindowViewModel)
        {
            ThisDayBtnCommand = ReactiveCommand.Create(test1);
            ThisWeekBtnCommand = ReactiveCommand.Create(test2);
            ThisMonthBtnCommand = ReactiveCommand.Create(test3);

            clientOrCompanySaleInfos = AccessToClassLibraryBackendProject.LoadSalesForClientOrCompany("Client", StartDate.DateTime, EndDate.DateTime, 1);
        }

        public void test1() { 

            setStartAndDateOfToday_WhenTodayBtnIsClicked();
            clientOrCompanySaleInfos = AccessToClassLibraryBackendProject.LoadSalesForClientOrCompany("Client", StartDate.DateTime, EndDate.DateTime, 1);

        }
        public void test2() { setStartAndDateThisWeek_WhenWeekBtnIsClicked(); }

        public void test3() { setStartAndDateOfThisMonth_WhenThisMonthBtnIsClicked(); }

    }
}
