using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS_Brian.Models
{
    public class clsAuth
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string RefreshToken { get; set; }
        public string UserId { get; set; }
        public int? OTP { get; set; }
    }
}