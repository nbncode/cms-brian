﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS_Brian.Models
{
    public class TokenResult
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string userName { get; set; }
        public string error { get; set; }
        public string refresh_token { get; set; }
    }
}