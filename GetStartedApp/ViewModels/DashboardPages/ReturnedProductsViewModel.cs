using DynamicData;
using GetStartedApp.Models;
using GetStartedApp.Models.Objects;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStartedApp.ViewModels.DashboardPages
{
    public class ReturnedProductsViewModel : SoldProductsViewModel
    {


        public ReturnedProductsViewModel()
        {


            Title = "قائمة المنتوجات المسترجعة";

            WhenUserPickTime_GetTheSoldProducts();
            ThisDayBtnCommand = ReactiveCommand.Create(GetReturnedProductsOfThisDay);
            ThisWeekBtnCommand = ReactiveCommand.Create(GetReturnedProductsOfThisWeek);
            ThisMonthBtnCommand = ReactiveCommand.Create(GetReturnedProductsOfThisMonth);
        }

        private void WhenUserPickTime_GetTheSoldProducts()
        {
            this.WhenAnyValue(x => x.StartDate, x => x.EndDate)
               .Subscribe(_ => { GetReturnedProductsList(StartDate, EndDate); ChangeTheColorOfBtnIfIsDayOrThisWeekOrThisMonth(); });
        }
        private void GetReturnedProductsOfThisDay()
        {
            setStartAndDateOfToday_WhenTodayBtnIsClicked();
            GetReturnedProductsList(StartDate, EndDate);
        }

        private void GetReturnedProductsOfThisWeek()
        {
            setStartAndDateThisWeek_WhenWeekBtnIsClicked();
            GetReturnedProductsList(StartDate, EndDate);
        }

        private void GetReturnedProductsOfThisMonth()
        {
            setStartAndDateOfThisMonth_WhenThisMonthBtnIsClicked();
            GetReturnedProductsList(StartDate, EndDate);
        }

        private void GetReturnedProductsList(DateTimeOffset StartDate, DateTimeOffset EndDate)
        {
            List<ReturnedProduct> productsReturned = AccessToClassLibraryBackendProject.GetReturnedProductsList(StartDate.DateTime, EndDate.DateTime);

            // we load the list of prdocut returned to sold products list becuase this porperty is the one bound to ui
            // while this viewmodel is inheriting form soldproduct , the same thing for a view
            LoadingReturnedProductsList_To_SoldProductList_BoundToView(productsReturned);

          

        }

        private void LoadingReturnedProductsList_To_SoldProductList_BoundToView(List<ReturnedProduct> productsReturned)
        {
            SoldProductList.Clear();

            foreach (var productReturned in productsReturned)
            {
                SoldProductList.Add(productReturned);
            }
        }




    }
}
