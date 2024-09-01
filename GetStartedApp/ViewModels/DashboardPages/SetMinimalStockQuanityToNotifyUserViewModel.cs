using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using System.Reactive;
using GetStartedApp.Helpers.CustomUIErrorAttributes;
using System.Reactive.Linq;
using GetStartedApp.Models;

namespace GetStartedApp.ViewModels.DashboardPages
{
    public class SetMinimalStockQuanityToNotifyUserViewModel : ViewModelBase
    {
        private string _minimalStockQuantity;

        public SetMinimalStockQuanityToNotifyUserViewModel()
        {
     //       var canSaveStockNumber = this.WhenAnyValue(x => x.MinimalStockQuantity)
      //                                       .Select(x => int.TryParse(x, out _));

       //     SaveCommand = ReactiveCommand.Create(Save, canSaveStockNumber);

            MinimalStockQuantity = AccessToClassLibraryBackendProject.GetMinimalStockValue().ToString();
        }

        [StringNotEmpty(ErrorMessage = "ليس هناك رقم ادخل الرقم")]
        [PositiveIntRange(1, int.MaxValue, ErrorMessage = "ادخل رقم موجب وبدون فاصلة ")]
        public string MinimalStockQuantity
        {
            get => _minimalStockQuantity;
            set => this.RaiseAndSetIfChanged(ref _minimalStockQuantity, value);
        }

       // public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        public void Save()
        {
            AccessToClassLibraryBackendProject.UpdateMinimalStockValue(int.Parse(MinimalStockQuantity));
        }
    }
}
