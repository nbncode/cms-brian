using CMS_Brian.Models;
using CMS_Brian.Models.Accounts;
using CMS_Brian.Models.Admin.Users;
using CMS_Brian.Models.Response;
//using CMS_Brian.Models.Accounts;
using CMS_Brian.Models.Services;
using CMS_Brian.Models.UserRegister;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CMS_Brian.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        #region Variables
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public IRoleManagement _roleManagement;
        public IUserManagement _userManagement;
        private ApplicationRoleManager _roleManager;
        #endregion

        #region Construtors
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        #endregion

        #region Uses Manger

        public ApplicationSignInManager SignInManager
        {
            get
            {
                if (_signInManager == null)
                {
                    _signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
                }
                return _signInManager;
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                if (_userManager == null)
                {
                    _userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                }
                return _userManager;
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                if (_roleManager == null)
                {
                    _roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
                }
                return _roleManager;
            }
            private set
            {
                _roleManager = value;
            }
        }

        #endregion

        #region Login

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel()) ;
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var systemController = new SystemController();
            var result = systemController.LoginUser(new clsAuth
            {
                //Username = model.PhoneNumber,
                Username = model.EmailId,
                Password = model.Password
            });

            if (result.ResultFlag == 1)
            {

                var user = await UserManager.FindAsync(model.EmailId, model.Password).ConfigureAwait(true);
                await SignInAsync(user, model.RememberMe).ConfigureAwait(true);

                return RedirectToLocal(returnUrl, (TokenResponse)result.Data);
            }

            TempData["error"] = result.Message;
            return View(model);
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        #endregion

        #region Register

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(ClsUserRegister model, FormCollection frm)
        {
            if (ModelState.IsValid)
            {
                var radio = frm["RegisterType"] != null ? frm["RegisterType"].ToString() : "";
                SystemController systemController = new SystemController();
                if (radio == "Clients")
                {
                    var result = systemController.UserRegisterOrUpdate(model);
                    if (result.ResultFlag == 1)
                    {
                        TempData["success"] = result.Message;
                        return RedirectToAction("RegisterSuccess", "Account");
                    }
                    else
                    {
                        TempData["error"] = result.Message;
                    }
                }
                else if(radio == "Instructors")
                {
                  //  ToDO:
                }
            }
            else
            {
                TempData["error"] = "Please fill all required fields.";
            }
            return View(model);
        }

        //
        // GET: /Account/RegisterSuccess
        [AllowAnonymous]
        public ActionResult RegisterSuccess()
        {
            return View();
        }
      
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        #endregion

        #region Forgot Password
        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user != null)
                {
                    if (user.EmailConfirmed)
                    {
                        // Don't reveal that the user does not exist or is not confirmed
                        return View("ResetPassword");
                    }
                    else { TempData["error"] = "Your account not apporved."; }
                }
                else { TempData["error"] = "Please enter valid email."; }
                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Forgot Password Confirmation

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        #endregion

        #region Reset Password
        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword()
        {
            return View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                ModelState.AddModelError("Email", "user not found.");
                return View(model);
            }

            RepoUsers repoUsers = new RepoUsers();
            string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var result = await UserManager.ResetPasswordAsync(user.Id, code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View(model);
        }

        #endregion

        #region Reset Password Confirmation
        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        #endregion

        #region LogOff
        public ActionResult LogOff()
        {
            Session.Abandon();
            Utils utils = new Utils();
            //utils.removeCurrentUser();
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }
        #endregion

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        public void SetService()
        {
            _userManagement = new UserManagement(UserManager, SignInManager, RoleManager);
            _roleManagement = new RoleManagement(UserManager, SignInManager, _userManagement, RoleManager);
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl, TokenResponse user)
        {
            SetService();

            var utils = new Utils();
            var controllerName = "home";
            if (user.Roles.Contains(Models.Constants.SuperAdmin))
            {
                utils.setCurrentUser(user.UserId, user.Username, user.FullName, Models.Constants.SuperAdmin, user);
                controllerName = "Admin";
            }
            else if (user.Roles.Contains(Models.Constants.Administrators))
            {
                utils.setCurrentUser(user.UserId, user.Username, user.FullName, Models.Constants.Administrators, user);
                controllerName = "Admin";
            }
            else if (user.Roles.Contains(Models.Constants.Instructors))
            {
                utils.setCurrentUser(user.UserId, user.Username, user.FullName, Models.Constants.Instructors, user);
                controllerName = "Admin";
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // By default open dashboard page
            return RedirectToAction("index", controllerName);
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}