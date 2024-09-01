using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesProductsManagmentSystemBusinessLayer
{
    public static class ClsCategoryProductManager
    {


     
        public static bool InsertNewCategory(string CategoryNameToInsert)
        {

            return ClsDataAccessLayer.InsertNewCategory(CategoryNameToInsert);
        }


        public static bool updateCategoryName(string perviousCategoryName, string newCategoryName)
        {
            return ClsDataAccessLayer.UpdateProductCategory(perviousCategoryName, newCategoryName) ;
        }

        public static bool DeleteCategory(string categoryName)
        {
            return ClsDataAccessLayer.DeleteCategory(categoryName) ;
        }


    }
}
