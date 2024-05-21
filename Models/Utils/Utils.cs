using CMS_Brian.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace CMS_Brian.Models
{
    public class Utils : IUtils
    {
        #region Generate Randoms
        //Genereate a New Token
        public string GenerateToken()
        {
            string token = Guid.NewGuid().ToString();
            return token;
        }

        public string RandomCode(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        #endregion

        #region Get Time

        public string getTime(DateTime dt)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = new TimeSpan(DateTime.Now.Ticks - dt.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds <= 1 ? "One second ago" : ts.Seconds + " seconds ago";

            if (delta < 2 * MINUTE)
                return "a minute ago";

            if (delta < 45 * MINUTE)
                return ts.Minutes + " minutes ago";

            if (delta < 90 * MINUTE)
                return "An hour ago";

            if (delta < 24 * HOUR)
                return ts.Hours + " hours ago";

            if (delta < 48 * HOUR)
                return "Yesterday";

            if (delta < 30 * DAY)
                return ts.Days + " days ago";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "One month ago" : months + " months ago";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "One year ago" : years + " years ago";
            }
        }

        #endregion

        #region Set and Get Cookies Value
        public void setCookiesValue(object value, string cookiename)
        {
            HttpCookie cartCookie = new HttpCookie(cookiename, value.ToString());
            cartCookie.Expires = DateTime.Now.AddMonths(1);
            HttpContext.Current.Response.Cookies.Set(cartCookie);
        }

        public string getStringCookiesValue(string cookiename)
        {
            if (HttpContext.Current.Request.Cookies[cookiename] != null)
            {
                return HttpContext.Current.Request.Cookies[cookiename].Value;
            }
            else
            {
                return "";
            }
        }

        public int getIntCookiesValue(string cookiename)
        {
            try
            {
                if (HttpContext.Current.Request.Cookies[cookiename] != null)
                {
                    return Convert.ToInt32(HttpContext.Current.Request.Cookies[cookiename].Value);
                }
                else
                {
                    return 0;
                }

            }
            catch (Exception)
            {
                return 0;
            }
        }

        public void setCurrentUser(string UserId, string Username, string FullName, string Role, TokenResponse user)
        {
            setCookiesValue(UserId, Constants.UserIdSession);
            setCookiesValue(Username, Constants.UsernameSession);
            setCookiesValue(FullName, Constants.UserFullnameSession);
            setCookiesValue(Role, Constants.UserRoleSession);
            setCookiesValue(user.AccessToken, Constants.UserTokenSession);
            setCookiesValue(user.RefreshToken, Constants.UserRefreshTokenSession);
        }

        public void setCurrentUserInfo(TokenResponse user)
        {
            setCookiesValue(user.AccessToken, Constants.UserTokenSession);
            setCookiesValue(user.RefreshToken, Constants.UserRefreshTokenSession);
        }

        public void removeCurrentUser()
        {
            RemoveCookiesValue(Constants.UserIdSession);
            RemoveCookiesValue(Constants.UsernameSession);
            RemoveCookiesValue(Constants.UserFullnameSession);
            RemoveCookiesValue(Constants.UserRoleSession);
            RemoveCookiesValue(Constants.UserTokenSession);
            RemoveCookiesValue(Constants.UserRefreshTokenSession);
        }

        public void RemoveCookiesValue(string cookiename)
        {
            HttpCookie currentCookie = HttpContext.Current.Request.Cookies[cookiename];
            if (currentCookie != null)
            {
                currentCookie.Expires = DateTime.Now.AddDays(-10);
                currentCookie.Value = null;
                HttpContext.Current.Response.SetCookie(currentCookie);
            }
        }

        #endregion

        #region ContainsInt
        public bool ContainsInt(string str, int value)
        {
            return str.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.Parse(x.Trim()))
                .Contains(value);
        }
        #endregion

        #region Get IP
        public string GetIpUser()
        {
            /*if (HttpContext.Current.Request.IsLocal)
            {
                return "103.203.136.50"; //static ip address because server variables not provide local ip, so put any default ip.
            }
            else
            {
                return HttpContext.Current.Request.ServerVariables["REMOTE_HOST"].ToString();
            }*/
            return "";
        }
        #endregion

        #region Get Random String

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            //ABCDEFGHIJKLMNOPQRSTUVWXYZ
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        #endregion

        static string key { get; set; } = "A!9HHhi%XjjYY4YP2@Nob009X";

        public static string Encrypt(string text)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateEncryptor())
                    {
                        byte[] textBytes = UTF8Encoding.UTF8.GetBytes(text);
                        byte[] bytes = transform.TransformFinalBlock(textBytes, 0, textBytes.Length);
                        return Convert.ToBase64String(bytes, 0, bytes.Length);
                    }
                }
            }
        }

        public static string Decrypt(string cipher)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateDecryptor())
                    {
                        byte[] cipherBytes = Convert.FromBase64String(cipher);
                        byte[] bytes = transform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                        return UTF8Encoding.UTF8.GetString(bytes);
                    }
                }
            }
        }

        public static int GetWeek_MonthRemainingDays(DateTime sDate, string TargetCriteria)
        {
            int day = 0;
            if (TargetCriteria == "Weekly")
            {
                var dayofweek = (int)sDate.DayOfWeek;
                if (dayofweek == 1)
                    day = 6;
                else if (dayofweek > 1)
                    day = (7 - dayofweek);
            }
            else
            {
                var dayofMonth = sDate.Day;
                if (dayofMonth > 1)
                {
                    //DateTime firstOfNextMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1);
                    //DateTime lastOfThisMonth = firstOfNextMonth.AddDays(-1);
                    //day = (lastOfThisMonth.AddDays(-sDate.Day)).Day;

                    var daysInMonth = DateTime.DaysInMonth(sDate.Year, sDate.Month);
                    day = daysInMonth - sDate.Day;
                }
                else
                {
                    day = DateTime.DaysInMonth(sDate.Year, sDate.Month) - 1;
                }
            }

            return day;
        }

        /// <summary>
        /// Get total number of months between two dates.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns>Return number of months.</returns>
        public static int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            var months = (endDate.Year - startDate.Year) * 12 + endDate.Month - startDate.Month;
            return Math.Abs(months);
        }
    }
}