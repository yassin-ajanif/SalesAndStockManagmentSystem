using GetStartedApp.Models.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStartedApp.ViewModels
{
    public class EditNewAddedCheckViewModel : AddNewChequeInfoViewModel
    {

        public EditNewAddedCheckViewModel(ref ChequeInfo checkInfoFilledByUser) : base( ref checkInfoFilledByUser)
        {
            displayThePreviousChequeInfoEntredByuser(checkInfoFilledByUser);
        }


        private void displayThePreviousChequeInfoEntredByuser(ChequeInfo previousChequeInfoEntredByUser)
        {
            this.ChequeNumber = previousChequeInfoEntredByUser.ChequeNumber;
            this.Amount = previousChequeInfoEntredByUser.Amount.ToString();
            this.ChequeDate = previousChequeInfoEntredByUser.ChequeDate;
        }
    }
}
