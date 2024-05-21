using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using CMS_Brian.Models;
using CMS_Brian.Models.Others;

namespace CMS_Brian.Models
{
    public class clsEmailMgt
    {
        #region Variables
        CMSBrianEntities db = new CMSBrianEntities();
        General general = new General();
        RepoMailSender sendMail = new RepoMailSender();

        #endregion

        #region Send mail to user
        public string SendMailRegisterUser(string msg, string Username, string Email, string password,string subject)
        {
            string html = sendMail.GetHtml(Username, string.Empty, msg);
           
                var str = sendMail.MailSender(Email, html, subject);
                return str;
        }
        #endregion
       
       
    }
}