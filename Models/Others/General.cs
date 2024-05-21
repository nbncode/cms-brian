using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CMS_Brian.Models.Others
{
    public class General
    {
        #region Is SuperAdmin
        public static bool IsSuperAdmin()
        {
            return GetUserRole() == Constants.SuperAdmin;
        }
        #endregion

        #region Is Administrators
        public static bool IsAdministrators()
        {
            return GetUserRole() == Constants.Administrators;
        }
        #endregion

        #region Is Instructors
        public static bool IsInstructors()
        {
            return GetUserRole() == Constants.Instructors;
        }
        #endregion

        #region Get UserId
        public static string GetUserId()
        {
            Utils utils = new Utils();
            return utils.getStringCookiesValue(Constants.UserIdSession);
        }
        #endregion

        #region Get Username
        public static string GetUsername()
        {
            Utils utils = new Utils();
            return utils.getStringCookiesValue(Constants.UsernameSession);
        }
        #endregion

        #region Get Refresh Token
        public static string GetRefreshToken()
        {
            Utils utils = new Utils();

            return utils.getStringCookiesValue(Constants.UserRefreshTokenSession);
        }
        #endregion

        #region Get Token
        public static string GetToken()
        {
            Utils utils = new Utils();

            return utils.getStringCookiesValue(Constants.UserTokenSession);
        }
        #endregion

        #region Get UserRole
        public static string GetUserRole()
        {
            if (HttpContext.Current.Request.IsAuthenticated)
            {
                Utils utils = new Utils();
                return utils.getStringCookiesValue(Constants.UserRoleSession);
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region Web Request
        public static string GetToken(string url, string postData)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            var data = Encoding.ASCII.GetBytes(postData);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.8.1.3) Gecko/20070309 Firefox/2.0.0.3";
            request.ProtocolVersion = HttpVersion.Version10;
            request.KeepAlive = true;
            request.AllowAutoRedirect = false;
            request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            request.Accept = "text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";
            request.Timeout = 15000;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    // log errorText
                }
                throw;
            }
        }
        #endregion

        #region check Image url Local or Online
        public string CheckImageUrl(string img)
        {
            if (string.IsNullOrEmpty(img) || !img.StartsWith("http") && !File.Exists(HttpContext.Current.Server.MapPath(img)))
            {
                return ConfigurationManager.AppSettings["WebUrl"] + "/assets/images/NoPhotoAvailable.png";
            }

            return img.StartsWith("http") ? img : ConfigurationManager.AppSettings["WebUrl"] + img;
        }
        #endregion
      
        #region Update UserName
        public bool UpdateUserName(string email)
        {
            Utils utils = new Utils();
            utils.setCookiesValue(email, "UsernameSession");
            return true;
        }
        #endregion

        #region Class List for client
        public SelectList PaymentModeList()
        {
            // Create select list
            var paymentModelist = new SelectListItem() { Value = "", Text = "-- Select --" };
            var newList = new List<SelectListItem> { paymentModelist };
            newList.Add(new SelectListItem() { Value = Constants.Cash, Text = Constants.Cash });
            newList.Add(new SelectListItem() { Value = Constants.Card, Text = Constants.Card });
            newList.Add(new SelectListItem() { Value = Constants.Other, Text = Constants.Other });
            return new SelectList(newList, "Value", "Text", null);
        }
        #endregion
    }
}