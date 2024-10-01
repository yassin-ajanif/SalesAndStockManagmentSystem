using GetStartedApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStartedApp.ViewModels.DashboardPages
{
    public class BonLivraisonViewModel : DevisViewModel
    {

        public BonLivraisonViewModel(MainWindowViewModel mainWindowViewModel) :base(mainWindowViewModel) { 
        
        
              
        
        }

        public async void MakeBlOperation(int SaleID)
        {
            DataTable TableOfProductsInfoScanned = LoadProductBoughtFromScannedListIntoADataTable();
            string SelectedPaymentMethodInFrench = TranslateTheSelectedPaymentMethodInFrench();
            decimal TVA = decimal.Parse(TaxValue);

            if (SelectedClientType == "زبون عادي") AccessToClassLibraryBackendProject.GenerateBonLivraison_ForClient(TableOfProductsInfoScanned, ClientID, SelectedPaymentMethodInFrench, TVA, SaleID);

        //    else if (SelectedClientType == "شركة") AccessToClassLibraryBackendProject.GenerateDevis_ForCompany(CompanyID, TableOfProductsInfoScanned, SelectedPaymentMethodInFrench, TVA);

        }
    }
}
