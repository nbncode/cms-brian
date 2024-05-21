using CMS_Brian.Models;
using CMS_Brian.Models.Admin.Class;
using CMS_Brian.Models.Admin.UserResponse;
using CMS_Brian.Models.Admin.Users;
using CMS_Brian.Models.UserLogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace CMS_Brian.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Login", "Account");
        }

        #region Image Upload
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase qqfile)
        {
            JsonResult json = null;

            if (qqfile != null)
            {
                // this works for IE
                string path = Guid.NewGuid() + "_" + Path.GetFileName(qqfile.FileName);
                var filename = Path.Combine(Server.MapPath("~/Content/attachment/"), path);
                string filePath = Path.Combine(("~/Content/attachment/"), path);
                qqfile.SaveAs(filename);
                List<string> lst = new List<string>();
                lst.Add(filePath);
                json = Json(new { success = true, list = lst, filename = filePath.Replace("~", "") }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // this works for Firefox, Chrome
                var filename = Request["qqfile"];
                string path = Guid.NewGuid() + "_" + Path.GetFileName(qqfile.FileName);
                if (!string.IsNullOrEmpty(filename))
                {
                    filename = Path.Combine(Server.MapPath("~/Content/attachment/"), path);
                    string filePath = Path.Combine(("~/Content/attachment/"), path);
                    using (var output = System.IO.File.Create(path))
                    {
                        Request.InputStream.CopyTo(output);
                    }

                    json = Json(new { success = true, filename = filePath.Replace("~", "") }, JsonRequestBehavior.AllowGet);
                }
            }

            return json ?? Json(new { success = false });
        }
        #endregion

        #region Change Password

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            SystemController systemController = new SystemController();
            var data = systemController.ChangePassword(new clsApproveUser()
            {
                UserId = model.UserId,
                NewPassword = model.NewPassword
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region User Profile
        public ActionResult ProfileDetail()
        {
            SystemController systemController = new SystemController();
            var data = systemController.UserProfile();
            return View(data.Data);
        }
        public new ActionResult Profile()
        {

            SystemController systemController = new SystemController();
            var data = systemController.UserProfile();
            return View(data.Data);
        }

        [HttpPost]
        public new ActionResult Profile(ResponseUserProfile model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                try
                {
                    SystemController systemController = new SystemController();
                    var result = systemController.UpdateUserProfile(model);
                    if (result.ResultFlag == 1)
                    {
                        TempData["Success"] = result.Message;
                        return RedirectToAction("ProfileDetail");
                    }
                    else
                    {
                        TempData["error"] = result.Message;
                    }
                }
                catch (Exception exc)
                {
                    TempData["error"] = "Record Not Updated";
                    RepoUserLogs.LogException(exc);
                }
                return View(model);
            }
        }
        #endregion

        #region Class Partial View
        public ActionResult ClassPartialView(string userId)
        {
            bool isAll = false;
            SystemController system = new SystemController();
            var role = system.GetUserRole();
            var currentUserId = system.GetCurrentUserId();

            // check show class all or assigned
            if(role==Constants.SuperAdmin)
            { 
            if(userId != currentUserId) { isAll = false; }
                else { isAll = true; }
            }
            RepoClass db = new RepoClass();
            return PartialView(db.GetAssignClasses(isAll, userId));
        }      
        #endregion
    }
}