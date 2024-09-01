using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesProductsManagmentSystemBusinessLayer
{
    public class ClsLoginManager
    {
        public static bool InsertFirstRunDateAndSetTrialOrPaidMode(bool isPaid)
        {
            return ClsDataAccessLayer.InsertFirstRunDateAndSetTrialOrPaidMode(isPaid);
        }
      
        public static DateTime? GetFirstRunDate()
        {
            return ClsDataAccessLayer.GetFirstRunDate();
        }

        public static bool IsApplicationUsersInPaidMode()
        {
            return ClsDataAccessLayer.IsApplicationUsersInPaidMode();
        }
        public static bool CheckIfLogNamesIncludeAdminAndUser()
        {
           return ClsDataAccessLayer.CheckIfLogNamesIncludeAdminAndUser();
            
        }

        public static bool InsertOrUpdateAdminAndUserLogins(string logNameAdmin, string passwordAdmin, string nationalIDOFAdmin,
    string logNameUser, string passwordUser, string nationalIDOFUser)
        {
            return ClsDataAccessLayer.InsertOrUpdateAdminAndUserLogins(logNameAdmin,passwordAdmin, nationalIDOFAdmin, logNameUser, passwordUser, nationalIDOFUser);
        }

        public static bool isThisNationalidBelongstoadmin(string AdminNationalID) { 
        
         return ClsDataAccessLayer.isThisNationalidBelongstoadmin(AdminNationalID);
        }

        public static bool CheckIfUserOrAdminPasswordIsCorrect(string userType, string password) {
        
        
        return ClsDataAccessLayer.CheckIfUserOrAdminPasswordIsCorrect(userType, password);

        }


        public static bool CheckIfProductAlreadyExistingInSoldItemsList(long productid)
        {
            return ClsDataAccessLayer.CheckIfProductAlreadyExistingInSoldItemsList(productid);
        }

        public static bool IsCompanyLogoExisting()
        {
            
                return ClsDataAccessLayer.CheckIfLogoCompanyIsExisting();
            
        }

       public static DataTable GetCompanyDetails()
        {
            return ClsDataAccessLayer.GetCompanyDetails();
        }

        public static async Task<bool> UpdateCompanyDetailsAsync(byte[] companyLogo, string companyName, string companyLocation,string ICE, string professionalTaxID, string TaxID)
        {
            return await ClsDataAccessLayer.UpdateCompanyDetailsAsync(companyLogo, companyName, companyLocation,ICE,professionalTaxID,TaxID);
        }

    }
}
