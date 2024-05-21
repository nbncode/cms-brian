﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS_Brian.Models
{
    public interface IAuthManagement
    {
        TokenResponse GetToken(string Username, string Password);
        TokenResponse GetTokenFromRefreshToken(string RefeshToken);
    }
}