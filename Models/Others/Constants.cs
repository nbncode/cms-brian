using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace CMS_Brian.Models
{
    public class Constants
    {
        #region Roles
        public static string SuperAdmin = "SuperAdmin";
        public static string Administrators = "Administrators";
        public static string Instructors = "Instructors";
        //public static string Clients = "Clients";
        #endregion

        #region Client status
        public static string Active = "Active";
        public static string Completed = "Completed";
        public static string BalanceOwed = "Balance Owed";
        public static string Inactive = "Inactive";
        #endregion

        #region Payment mode
        public static string Cash = "Cash";
        public static string Card = "Card";
        public static string Other = "Other";
        #endregion

        #region Cookies Id
        public const string UsernameSession = "UsernameSession";
        public const string UserFullnameSession = "UserFullnameSession";
        public const string UserIdSession = "UserIdSession";
        public const string UserRoleSession = "UserRoleSession";
        public const string UserRefreshTokenSession = "UserRefreshTokenSession";
        public const string UserTokenSession = "UserTokenSession";
        #endregion

        #region Other
        public static string TokenApi = ConfigurationManager.AppSettings["WebUrl"] + "/token";
        public static string Sign = "₹";
        public static string RupeeSign = "₨";
        #endregion
        
    }
}