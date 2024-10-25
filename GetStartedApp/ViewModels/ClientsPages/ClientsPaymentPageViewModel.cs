using GetStartedApp.ViewModels.ProductPages;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStartedApp.ViewModels.ClientsPages
{
    public class ClientsPaymentPageViewModel: ReturnProductBySaleIDViewModel
    {

        public string ChangingPaymentOperation { get; }
        public ClientsPaymentPageViewModel(int SaleID, string ChangingPaymentOperation):base(SaleID)
        {
            this.ChangingPaymentOperation = ChangingPaymentOperation;
        }

        
    }
}
