using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStartedApp.Models
{
    public class ClientInfo
    {
        public int ClientID { get; set; }
        public string ClientName { get; set; }

        public string PhoneNumber { get; set; }
        public string email {  get; set; }

        public ClientInfo(int clientID, string clientName, string phoneNumber, string email)
        {
            ClientID = clientID;
            ClientName = clientName;
            PhoneNumber = phoneNumber;
            this.email = email;
        }
    }
}
