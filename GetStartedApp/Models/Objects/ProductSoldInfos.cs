using GetStartedApp.Helpers;
using GetStartedApp.Models.Objects;
using System.Data;

public class ProductSoldInfos
{
    public int SaleID { get; set; }
    public int? ClientID { get; set; } // Changed to nullable
    public int? CompanyID { get; set; } // Changed to nullable
    public DataTable ProductsBoughtInThisOperation { get; set; }
    public string SelectedPaymentMethodInFrench { get; set; }
    public ChequeInfo UserChequeInfo { get; set; }


    public ProductSoldInfos(int saleID, int? clientID, int? companyID, DataTable productsBoughtInThisOperation, string selectedPaymentMethodInEnglish, ChequeInfo userChequeInfo)
    {
        SaleID = saleID;
        ClientID = clientID;
        CompanyID = companyID;
        ProductsBoughtInThisOperation = productsBoughtInThisOperation;
        SelectedPaymentMethodInFrench = WordTranslation.TranslatePaymentIntoTargetedLanguage(selectedPaymentMethodInEnglish,"fr");
        UserChequeInfo = userChequeInfo;
     
    }
}
