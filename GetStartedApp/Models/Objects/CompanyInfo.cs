using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using GetStartedApp.Helpers;


namespace GetStartedApp.Models.Objects
{

    public class CompanyInfo
    {
        public int CompanyId { get; set; }
        public Bitmap CompanyLogo { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLocation { get; set; }
        public string ICE { get; set; }
        public string IFS { get; set; }
        public string Email { get; set; }
        public string Patente { get; set; }
        public string RC { get; set; }
        public string CNSS { get; set; }

        public CompanyInfo(int companyId, byte[] companyLogo, string companyName, string companyLocation,
                       string ice, string ifs, string email, string patente, string rc, string cnss)
        {
            CompanyId = companyId;
            CompanyLogo = ImageConverter.ByteArrayToBitmap(companyLogo);
            CompanyName = companyName;
            CompanyLocation = companyLocation;
            ICE = ice;
            IFS = ifs;
            Email = email;
            Patente = patente;
            RC = rc;
            CNSS = cnss;
        }
    }

}
