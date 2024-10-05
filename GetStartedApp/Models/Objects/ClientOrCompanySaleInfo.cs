using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStartedApp.Models.Objects
{
    public class ClientOrCompanySaleInfo
    {
        public int SaleID { get; set; }
        public string TimeOfOperation { get; set; }  // String representation of date and time with day name
        public decimal TotalPrice { get; set; }
        public int PaymentID { get; set; }
        public string PaymentName { get; set; }
        public int? ClientOrCompanyID { get; set; }  // Can be either ClientID or CompanyID
        public string ClientOrCompanyName { get; set; }  // Can be either ClientName or CompanyName

        // Constructor
        public ClientOrCompanySaleInfo(int saleID, DateTime timeOfOperation, decimal totalPrice, int paymentID, string paymentName, int? clientOrCompanyID, string clientOrCompanyName)
        {
            SaleID = saleID;
            TimeOfOperation = timeOfOperation.ToString("dddd dd/MM/yyyy HH:mm:ss");  // Include day name in date string
            TotalPrice = totalPrice;
            PaymentID = paymentID;
            PaymentName = paymentName;
            ClientOrCompanyID = clientOrCompanyID;
            if(clientOrCompanyName == "unknownClient") ClientOrCompanyName= "زبون مكتبي";
        }
    }

}
