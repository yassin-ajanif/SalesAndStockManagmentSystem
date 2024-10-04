using GetStartedApp.Models;
using ReactiveUI;
using System;
using System.Reactive;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Concurrent;
using Avalonia.Threading;

namespace GetStartedApp.ViewModels.DashboardPages
{
    public class FinancesViewModel:ViewModelBase
    {
       // private DateTimeOffset _startDate = new DateTimeOffset(DateTime.Now);
       // 
       // public DateTimeOffset StartDate
       // {
       //     get => _startDate;
       //     set => this.RaiseAndSetIfChanged(ref _startDate, value);
       // }
       //
       // private DateTimeOffset _endDate= new DateTimeOffset(DateTime.Now);
       // public DateTimeOffset EndDate
       // {
       //     get => _endDate;
       //     set => this.RaiseAndSetIfChanged(ref _endDate, value);
       // }

        //private bool _isPageActivated = true;

        //public bool IsPageActivated 
        //{
        //    get => _isPageActivated;
        //    set => this.RaiseAndSetIfChanged(ref _isPageActivated, value);
        //}
        private bool _isLoadingMessageIsVisible=false;

        public bool IsLoadingMessageIsVisible
        {
            get => _isLoadingMessageIsVisible;
            set => this.RaiseAndSetIfChanged(ref _isLoadingMessageIsVisible, value);
        }

        private Task<decimal> _revenue;
       
        public Task<decimal> Revenue
        {
            get => _revenue;
            set => this.RaiseAndSetIfChanged(ref _revenue, value);
        }

 
        private Task<decimal> _profit;
        public Task<decimal> Profit
        {
            get => _profit;
            set => this.RaiseAndSetIfChanged(ref _profit, value);
        }

    //  private string _isClickedOrNotColorOfThisDayBtn ;
    //  public string IsClickedOrNotColorOfThisDayBtn
    //  {
    //      get => _isClickedOrNotColorOfThisDayBtn;
    //      set => this.RaiseAndSetIfChanged(ref _isClickedOrNotColorOfThisDayBtn, value);
    //  }
    //
    //  private string _isClickedOrNotColorOfThisWeekBtn ;
    //  public string IsClickedOrNotColorOfThisWeekBtn
    //  {
    //      get => _isClickedOrNotColorOfThisWeekBtn;
    //      set => this.RaiseAndSetIfChanged(ref _isClickedOrNotColorOfThisWeekBtn, value);
    //  }
    //
    //  private string _isClickedOrNotColorOfThisMonthBtn ;
    //  public string IsClickedOrNotColorOfThisMonthBtn
    //  {
    //      get => _isClickedOrNotColorOfThisMonthBtn;
    //      set => this.RaiseAndSetIfChanged(ref _isClickedOrNotColorOfThisMonthBtn, value);
    //  }
    //
    //
    //  public ReactiveCommand<Unit, Unit> ThisDayBtnCommand { get; set; }
    //  public ReactiveCommand<Unit, Unit> ThisWeekBtnCommand { get; set; }
    //  public ReactiveCommand<Unit, Unit> ThisMonthBtnCommand { get; set; }

        //public enum eButtonTypeClicked
        //{
        //    ThisDayBtn,
        //    ThisWeekBtn,
        //    ThisMonthBtn
        //}

        public FinancesViewModel()
        {
            // we set the time minima of the firt item product was inserted as startdate to simplify research for a user

            //setTheGrayColorOfAllBtns();
            WhenUserPickTime_DisplayTheColorBtnThatIndicate_IfIsDayOrThisWeekOrThisMonth();
            WhenUserPickTime_GetTheTotalRevenueAndProfit();

            ThisDayBtnCommand = ReactiveCommand.Create(DisplayTotalRevenueAndProfitOfThisDay);
            ThisWeekBtnCommand = ReactiveCommand.Create(DisplayTotalRevenueAndProfitOfThisWeek);
            ThisMonthBtnCommand = ReactiveCommand.Create(DisplayTotalRevenueAndProfitOfThisMonth);
  
          
        }

        DateTimeOffset getTheTimeOfFirstInsertedItemFromDatabase()
        {
            return new DateTimeOffset(DateTime.Now);
        }

         private void WhenUserPickTime_GetTheTotalRevenueAndProfit()
        {
            this.WhenAnyValue(x => x.StartDate, x => x.EndDate)
               .Subscribe(_ =>
               {

                   DisplayTotalRevenueAndProfit();
                 //  ChangeTheColorOfBtnIfIsDayOrThisWeekOrThisMonth();

               }
                   );
        }

    

        public async void DisplayTotalRevenueAndProfit()
        {
            BlockThePage_And_Show_Loading_Message();

            // Execute async methods on a separate thread
            Revenue = Task.FromResult(await Task.Run(() => AccessToClassLibraryBackendProject.GetTotalRevenue(StartDate.DateTime.Date, EndDate.DateTime.Date)));
            Profit =  Task.FromResult(await Task.Run(() => AccessToClassLibraryBackendProject.GetTotalProfit(StartDate.DateTime.Date, EndDate.DateTime.Date )));

            DeBlockThePage_And_Hide_Loading_Message();

        }


      

        //public void ChangeTheColorOfBtnIfIsDayOrThisWeekOrThisMonth()
        //{
        //    // Reset all button colors to Gainsboro
        //     setTheGrayColorOfAllBtns();

        //    // Change the color of the clicked button to red based on the condition
        //    if (StartAndEndDate_Are_ThisDay())
        //    {
        //        IsClickedOrNotColorOfThisDayBtn = "Red";
        //    }
        //    else if (StartAndEndDate_Are_ThisWeek())
        //    {
        //        IsClickedOrNotColorOfThisWeekBtn = "Red";
        //    }
        //    else if (StartAndEndDate_Are_ThisMonth())
        //    {
        //        IsClickedOrNotColorOfThisMonthBtn = "Red";
        //    }
        //}


        //protected void setTheGrayColorOfAllBtns()
        //{
        //    IsClickedOrNotColorOfThisDayBtn = "Gainsboro";
        //    IsClickedOrNotColorOfThisWeekBtn = "Gainsboro";
        //    IsClickedOrNotColorOfThisMonthBtn = "Gainsboro";
        //}
       
        //protected void setStartAndDateOfToday_WhenTodayBtnIsClicked()
        //{
        //   StartDate = EndDate = DateTime.Today.Date;
        //}

        //protected void setStartAndDateThisWeek_WhenWeekBtnIsClicked()
        //{
        //    // Set EndDate to today's date
        //    EndDate = DateTime.Today.Date;

        //    // Set StartDate to seven days ago
        //    StartDate = EndDate.AddDays(-7);
        //}


        //protected void setStartAndDateOfThisMonth_WhenThisMonthBtnIsClicked()
        //{
        //    // Set EndDate to today's date
        //    EndDate = DateTime.Today.Date;

        //    // Set StartDate to thirty days ago
        //    StartDate = EndDate.AddDays(-30);
        //}

        private async void BlockThePage_And_Show_Loading_Message()
        {
            IsLoadingMessageIsVisible = true;
            IsPageActivated = false;
           // this hep this function to win the race confition hwich is updating the properties before the next function win the race and 
           // finnaly not seing the properties in ui updated which are messages
            
        }

        private async void DeBlockThePage_And_Hide_Loading_Message()
        {
            IsLoadingMessageIsVisible = false;
            IsPageActivated = true;
           
           
        }
        private async void DisplayTotalRevenueAndProfitOfThisDay()
      {
            
            setStartAndDateOfToday_WhenTodayBtnIsClicked();
            BlockThePage_And_Show_Loading_Message();

            // Execute async methods on a separate thread
            Revenue = Task.FromResult(await Task.Run(() => AccessToClassLibraryBackendProject.GetTotalRevenue(StartDate.DateTime.Date, EndDate.DateTime.Date)));
            Profit = Task.FromResult(await Task.Run(() => AccessToClassLibraryBackendProject.GetTotalProfit(StartDate.DateTime.Date, EndDate.DateTime.Date)));

            DeBlockThePage_And_Hide_Loading_Message();

        }

 
        private async void DisplayTotalRevenueAndProfitOfThisWeek()
        {
            setStartAndDateThisWeek_WhenWeekBtnIsClicked();

            BlockThePage_And_Show_Loading_Message();

            // Execute async methods on a separate thread
            Revenue = Task.FromResult(await Task.Run(() => AccessToClassLibraryBackendProject.GetTotalRevenue(StartDate.DateTime.Date, EndDate.DateTime.Date)));
            Profit = Task.FromResult(await Task.Run(() => AccessToClassLibraryBackendProject.GetTotalProfit(StartDate.DateTime.Date, EndDate.DateTime.Date)));

            DeBlockThePage_And_Hide_Loading_Message();
        }



        private async void DisplayTotalRevenueAndProfitOfThisMonth()
        {
            
            setStartAndDateOfThisMonth_WhenThisMonthBtnIsClicked();
            BlockThePage_And_Show_Loading_Message();

            // Execute async methods on a separate thread
            Revenue = Task.FromResult(await Task.Run(() => AccessToClassLibraryBackendProject.GetTotalRevenue(StartDate.DateTime.Date, EndDate.DateTime.Date)));
            Profit = Task.FromResult(await Task.Run(() => AccessToClassLibraryBackendProject.GetTotalProfit(StartDate.DateTime.Date, EndDate.DateTime.Date)));

            DeBlockThePage_And_Hide_Loading_Message();
        }

        //private bool StartAndEndDate_Are_ThisDay()
        //{
        //    return StartDate.Date == EndDate.Date &&  StartDate.Date == DateTime.Today.Date &&  EndDate.Date == DateTime.Today.Date;
        //}

        //private bool StartAndEndDate_Are_ThisWeek()
        //{
        //    // Calculate the start date of the week containing StartDate
        //    DateTime sevenDaysAgo = DateTime.Today.Date.AddDays(-7);


        //    // Check if StartDate is exactly 7 days ago and EndDate is today (both dates without time component)
        //    bool isWithinThisWeek = StartDate.Date == sevenDaysAgo && EndDate.Date == DateTime.Today.Date;

        //    return isWithinThisWeek;
        //}


        //private bool StartAndEndDate_Are_ThisMonth()
        //{
        //    // Calculate the difference in days between StartDate and EndDate
        //    TimeSpan difference = EndDate - StartDate;
        //    bool StartDateIsThisYear = StartDate.Year == DateTime.Today.Year;
        //    bool EndDateIsThisYear = EndDate.Year == DateTime.Today.Year;

        //    // Check if the difference is exactly 30 days
        //    return StartDateIsThisYear && EndDateIsThisYear && difference.Days == 30;
        //}




    }
}
