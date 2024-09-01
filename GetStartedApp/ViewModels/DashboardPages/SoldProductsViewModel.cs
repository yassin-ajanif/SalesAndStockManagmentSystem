using GetStartedApp.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStartedApp.ViewModels.DashboardPages
{
    public class SoldProductsViewModel: FinancesViewModel
    {
        private ObservableCollection<ProductSold> _soldProductList = new ObservableCollection<ProductSold>();

        public ObservableCollection<ProductSold> SoldProductList
        {
            get { return _soldProductList; }
            set { this.RaiseAndSetIfChanged(ref _soldProductList, value); }
        }

        private string _title = "قائمة المبيعات";

        public string Title
        {
            get { return _title; }
            set { this.RaiseAndSetIfChanged(ref _title, value); }
        }




        public SoldProductsViewModel()
        {
            
            WhenUserPickTime_GetTheSoldProducts();
            ThisDayBtnCommand = ReactiveCommand.Create(GetSoldProductsOfThisDay);
            ThisWeekBtnCommand = ReactiveCommand.Create(GetSoldProductsOfThisWeek);
            ThisMonthBtnCommand = ReactiveCommand.Create(GetSoldProductsOfThisMonth);

        }

         private void WhenUserPickTime_GetTheSoldProducts()
         {
            this.WhenAnyValue(x => x.StartDate, x => x.EndDate)
               .Subscribe( _ => { 

                   GetSoldProductsList(StartDate, EndDate);
                   ChangeTheColorOfBtnIfIsDayOrThisWeekOrThisMonth(); 
               });
         }

        private void GetSoldProductsList(DateTimeOffset StartDate, DateTimeOffset EndDate)
        {
            List<ProductSold> productSolds = AccessToClassLibraryBackendProject.GetSoldProductsList(StartDate.DateTime, EndDate.DateTime);

            SoldProductList.Clear();
            foreach (var productSold in productSolds)
            {
                SoldProductList.Add(productSold);
            }
        }

        private void GetSoldProductsOfThisDay()
        {
            setStartAndDateOfToday_WhenTodayBtnIsClicked();
                  
         


        }

        private void GetSoldProductsOfThisWeek()
        {
            setStartAndDateThisWeek_WhenWeekBtnIsClicked();
        
       

        }

        private void GetSoldProductsOfThisMonth()
        {  
            setStartAndDateOfThisMonth_WhenThisMonthBtnIsClicked();

     


        }
    }
}
