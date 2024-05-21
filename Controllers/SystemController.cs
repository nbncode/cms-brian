using CMS_Brian.Models;
using CMS_Brian.Models.Accounts;
using CMS_Brian.Models.Admin.AvailableDays;
using CMS_Brian.Models.Admin.Class;
using CMS_Brian.Models.Admin.UserResponse;
using CMS_Brian.Models.Admin.Users;
using CMS_Brian.Models.judgeModel;
using CMS_Brian.Models.Others;
using CMS_Brian.Models.Response;
using CMS_Brian.Models.Services;
using CMS_Brian.Models.UserLogs;
using CMS_Brian.Models.UserRegister;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace CMS_Brian.Controllers
{
    [Authorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SystemController : ApiController
    {
        #region Variables
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public IRoleManagement _roleManagement;
        public IUserManagement _userManagement;
        private ApplicationRoleManager _roleManager;
        #endregion

        #region Construtor
        public SystemController()
        {
            if (_userManager == null)
            {
                setService();
            }
        }
        #endregion

        #region Uses Manger

        public ApplicationSignInManager SignInManager
        {
            get
            {
                if (_signInManager == null)
                {
                    _signInManager = HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
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
                    _userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
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
                    _roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
                }
                return _roleManager;
            }
            private set
            {
                _roleManager = value;
            }
        }

        #endregion

        #region User Management

        [HttpPost]
        public ResultModel ApproveUser(ClsAdministratorResponse model)
        {
            try
            {
                var result = _userManagement.SetApproved(model.UserId, model.IsApproved);
                if (result && model.IsApproved)
                {
                    return new ResultModel()
                    {
                        Message = "User Approved Successfully.",
                        ResultFlag = 1
                    };
                }
                else if (result && !model.IsApproved)
                {
                    return new ResultModel()
                    {
                        Message = "User UnApproved Successfully.",
                        ResultFlag = 1
                    };
                }
                else
                {
                    return new ResultModel()
                    {
                        Message = "Something goes wrong!!.",
                        ResultFlag = 0
                    };
                }
            }
            catch (Exception ex)
            {
                return RepoUserLogs.SendExceptionMailFromController(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), ex.Message, ex.StackTrace);
            }
        }

        [HttpPost]
        public ResultModel DeleteUser(ClsAdministratorResponse model)
        {
            try
            {
                RepoUsers repoUsers = new RepoUsers();
                _userManagement.LockUserAccount(model.UserId);
                var result = repoUsers.deleteUser(model.UserId);
                if (result)
                {

                    return new ResultModel()
                    {
                        Message = "User Deleted Successfully.",
                        ResultFlag = 1
                    };
                }
                else
                {
                    return new ResultModel()
                    {
                        Message = "User Not Deleted.",
                        ResultFlag = 0
                    };
                }
            }
            catch (Exception ex)
            {
                return RepoUserLogs.SendExceptionMailFromController(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), ex.Message, ex.StackTrace);
            }
        }

        //[HttpPost]
        //public ResultModel LockUser(ClsUserResponse model)
        //{
        //    try
        //    {
        //        var result = false;
        //        if (model.Locked)
        //        {
        //            result = _userManagement.LockUserAccount(model.UserId);
        //        }
        //        else
        //        {
        //            result = _userManagement.UnlockUserAccount(model.UserId);
        //        }

        //        if (result && model.Locked)
        //        {
        //            return new ResultModel()
        //            {
        //                Message = "User Locked Successfully.",
        //                ResultFlag = 1
        //            };
        //        }
        //        else if (result && !model.Locked)
        //        {
        //            return new ResultModel()
        //            {
        //                Message = "User UnLocked Successfully.",
        //                ResultFlag = 1
        //            };
        //        }
        //        else
        //        {
        //            return new ResultModel()
        //            {
        //                Message = "Something goes wrong!!.",
        //                ResultFlag = 0
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return RepoUserLogs.SendExceptionMailFromController(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), ex.Message, ex.StackTrace);
        //    }
        //}

        //[AllowAnonymous]
        //[HttpPost]
        //public ResultModel ChangePassword(clsApproveUser model)
        //{
        //    try
        //    {
        //        var result = false;
        //        if (!string.IsNullOrEmpty(model.Code))
        //        {
        //            result = _userManagement.ChangePassword(model.UserId, model.NewPassword, model.Code);
        //        }
        //        else
        //        {
        //            result = _userManagement.ChangePassword(model.UserId, model.NewPassword);
        //        }

        //        // Update password of user
        //        var userDetails = new UserDetail()
        //        {
        //            UserId = model.UserId,
        //            Password = Utils.Encrypt(model.NewPassword)
        //        };
        //        var repoUsers = new RepoUsers();
        //        repoUsers.UpdatePassword(userDetails);

        //        if (result)
        //        {
        //            return new ResultModel()
        //            {
        //                Message = "Password Changed Successfully.",
        //                ResultFlag = 1
        //            };
        //        }
        //        else
        //        {
        //            return new ResultModel()
        //            {
        //                Message = "Password not changed. Password must be at least 6 characters. Password must have at least one non letter or digit character. Password must have at least one lowercase ('a'-'z'). Password must have at least one uppercase ('A'-'Z').",
        //                ResultFlag = 0
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return RepoUserLogs.SendExceptionMailFromController(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), ex.Message, ex.StackTrace);
        //    }
        //}

        [HttpPost]
        public ResultModel GetRoles()
        {
            try
            {
                return new ResultModel()
                {
                    Data = _roleManagement.GetAllRoles(),
                    Message = "Data Found Successfully.",
                    ResultFlag = 1
                };
            }
            catch (Exception ex)
            {
                return RepoUserLogs.SendExceptionMailFromController(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), ex.Message, ex.StackTrace);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ResultModel ChangePassword(clsApproveUser model)
        {
            try
            {
                var result = false;
                if (!string.IsNullOrEmpty(model.UserId))
                {
                    result = _userManagement.ChangePassword(model.UserId, model.NewPassword);
                }

                // Update password of user
                var userDetails = new UserDetail()
                {
                    UserId = model.UserId,
                    Password = Utils.Encrypt(model.NewPassword)
                };
                var repoUsers = new RepoUsers();
                repoUsers.UpdatePassword(userDetails);

                if (result)
                {
                    return new ResultModel()
                    {
                        Message = "Password Changed Successfully.",
                        ResultFlag = 1
                    };
                }
                else
                {
                    return new ResultModel()
                    {
                        Message = "Password not changed. Password must be at least 6 characters. Password must have at least one non letter or digit character. Password must have at least one lowercase ('a'-'z'). Password must have at least one uppercase ('A'-'Z').",
                        ResultFlag = 0
                    };
                }
            }
            catch (Exception ex)
            {
                return RepoUserLogs.SendExceptionMailFromController(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), ex.Message, ex.StackTrace);
            }
        }

        #endregion

        #region Users Register and update

        #region Add Or Update SuperAdmin API
        [AllowAnonymous]
        [HttpPost]
        public ResultModel UserRegisterOrUpdate(ClsUserRegister model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new ResultModel()
                    {
                        Message = "Please enter all required fields.",
                        ResultFlag = 0
                    };
                }
                else
                {
                    RepoUsers repoUsers = new RepoUsers();

                    if (!string.IsNullOrEmpty(model.UserId))
                    {
                        //Update user
                        var userDetails = new UserDetail()
                        {
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            IsDeleted = false,
                            UserId = model.UserId,
                            Gender = model.Gender,
                            Address = model.Address,
                        };

                        var resultDetails = repoUsers.AddOrUpdateUsers(userDetails);
                        return new ResultModel()
                        {
                            Message = "User details updated successfully.",
                            ResultFlag = 1
                        };
                    }
                    else
                    {
                        string UserId = "";
                        var roles = new[] { Models.Constants.SuperAdmin };

                        //Register user
                        var result = _userManagement.RegisterUser(model.PhoneNumber, model.Email, model.Password, roles, model.IsApproved, out UserId);
                        if (result.Succeeded)
                        {
                            var userDetails = new UserDetail()
                            {
                                FirstName = model.FirstName,
                                LastName = model.LastName,
                                IsDeleted = false,
                                UserId = UserId,
                                Gender = model.Gender,
                                Address = model.Address,
                                PhoneNumber = model.PhoneNumber,
                                Password = Utils.Encrypt(model.Password)
                            };

                            var resultDetails = repoUsers.AddOrUpdateUsers(userDetails);
                            if (resultDetails)
                            {
                                return new ResultModel()
                                {
                                    Message = "Registration Successfully.",
                                    ResultFlag = 1
                                };
                            }
                            else
                            {
                                return new ResultModel()
                                {
                                    Message = "Registration failed!!",
                                    ResultFlag = 0
                                };
                            }
                        }
                        else
                        {
                            return new ResultModel()
                            {
                                Message = result.Errors.Count() > 0 ? result.Errors.FirstOrDefault() : "Registration failed!!",
                                ResultFlag = 0
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return RepoUserLogs.SendExceptionMailFromController(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), ex.Message, ex.StackTrace);
            }
        }

        #endregion

        #region Add Or Update Administrator API
        [AllowAnonymous]
        [HttpPost]
        public ResultModel AdministratorRegisterOrUpdate(ClsAdministrator model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new ResultModel()
                    {
                        Message = "Please enter all required fields.",
                        ResultFlag = 0
                    };
                }
                else
                {
                    RepoUsers repoUsers = new RepoUsers();

                    if (!string.IsNullOrEmpty(model.UserId))
                    {
                        //Update user
                        var userDetails = new UserDetail()
                        {
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            IsDeleted = false,
                            UserId = model.UserId,
                            Address = model.Address,
                            Status = model.Status,
                            UserImage = model.UserImage,
                            PhoneNumber = model.PhoneNumber,
                        };

                        var resultDetails = repoUsers.AddOrUpdateUsers(userDetails);

                        if (resultDetails)
                        {
                            RepoAvailableDays availableDays = new RepoAvailableDays();
                            availableDays.AddOrUpdateUserAvailableDays(new clsAvailableDays()
                            {
                                UserId = model.UserId,
                                AvailableDaysIds = model.AvailableDaysIds
                            });
                            return new ResultModel()
                            {
                                Message = "User details updated successfully.",
                                ResultFlag = 1
                            };
                        }
                        return new ResultModel()
                        {
                            Message = "User details updated successfully.",
                            ResultFlag = 1
                        };
                    }
                    else
                    {
                        string UserId = "";
                        var roles = new[] { Models.Constants.Administrators };

                        //Register user
                        var result = _userManagement.RegisterUser(model.Email, model.Email, model.Password, roles, model.IsApproved, out UserId);
                        if (result.Succeeded)
                        {
                            var userDetails = new UserDetail()
                            {
                                FirstName = model.FirstName,
                                LastName = model.LastName,
                                IsDeleted = false,
                                UserId = UserId,
                                Address = model.Address,
                                Status = model.Status,
                                UserImage = model.UserImage,
                                PhoneNumber = model.PhoneNumber,
                                Password = Utils.Encrypt(model.Password)
                            };

                            var resultDetails = repoUsers.AddOrUpdateUsers(userDetails);
                            if (resultDetails)
                            {
                                RepoAvailableDays availableDays = new RepoAvailableDays();
                                availableDays.AddOrUpdateUserAvailableDays(new clsAvailableDays()
                                {
                                    UserId = UserId,
                                    AvailableDaysIds = model.AvailableDaysIds
                                });

                                // Send mail to Administrators
                                clsEmailMgt mail = new clsEmailMgt();
                                string msg = "Registration Successfully.";
                                string subject = "Registration";
                                var Data = new System.Threading.Thread(() => mail.SendMailRegisterUser(msg, model.FirstName+" "+model.LastName, model.Email, model.Password,subject))
                                { IsBackground = true };
                                Data.Start();

                                return new ResultModel()
                                {
                                    Message = "Registration Successfully.",
                                    ResultFlag = 1
                                };
                            }
                            else
                            {
                                return new ResultModel()
                                {
                                    Message = "Registration failed!!",
                                    ResultFlag = 0
                                };
                            }
                        }
                        else
                        {
                            return new ResultModel()
                            {
                                Message = result.Errors.Count() > 0 ? result.Errors.FirstOrDefault() : "Registration failed!!",
                                ResultFlag = 0
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return RepoUserLogs.SendExceptionMailFromController(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), ex.Message, ex.StackTrace);
            }
        }

        #endregion

        #region Add Or Update Instructor API
        [AllowAnonymous]
        [HttpPost]
        public ResultModel InstructorRegisterOrUpdate(ClsInstructor model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new ResultModel()
                    {
                        Message = "Please enter all required fields.",
                        ResultFlag = 0
                    };
                }
                else
                {
                    RepoUsers repoUsers = new RepoUsers();

                    if (!string.IsNullOrEmpty(model.UserId))
                    {
                        //Update user
                        var userDetails = new UserDetail()
                        {
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            IsDeleted = false,
                            UserId = model.UserId,
                            Address = model.Address,
                            Status = model.Status,
                            UserImage = model.UserImage,
                            PhoneNumber = model.PhoneNumber,
                        };

                        var resultDetails = repoUsers.AddOrUpdateUsers(userDetails);

                        if (resultDetails)
                        {
                            RepoAvailableDays availableDays = new RepoAvailableDays();
                            availableDays.AddOrUpdateUserAvailableDays(new clsAvailableDays()
                            {
                                UserId = model.UserId,
                                AvailableDaysIds = model.AvailableDaysIds
                            });

                            return new ResultModel()
                            {
                                Message = "User details updated successfully.",
                                ResultFlag = 1
                            };
                        }
                        return new ResultModel()
                        {
                            Message = "User details updated successfully.",
                            ResultFlag = 1
                        };
                    }
                    else
                    {
                        string UserId = "";
                        var roles = new[] { Models.Constants.Instructors };

                        //Register user
                        var result = _userManagement.RegisterUser(model.Email, model.Email, model.Password, roles, model.IsApproved, out UserId);
                        if (result.Succeeded)
                        {
                            var userDetails = new UserDetail()
                            {
                                FirstName = model.FirstName,
                                LastName = model.LastName,
                                IsDeleted = false,
                                UserId = UserId,
                                Address = model.Address,
                                Status = model.Status,
                                UserImage = model.UserImage,
                                PhoneNumber = model.PhoneNumber,
                                Password = Utils.Encrypt(model.Password)
                            };

                            var resultDetails = repoUsers.AddOrUpdateUsers(userDetails);
                            if (resultDetails)
                            {
                                RepoAvailableDays availableDays = new RepoAvailableDays();
                                availableDays.AddOrUpdateUserAvailableDays(new clsAvailableDays()
                                {
                                    UserId = UserId,
                                    AvailableDaysIds = model.AvailableDaysIds
                                });

                                // Send mail to Instructors
                                clsEmailMgt mail = new clsEmailMgt();
                                string msg = "Registration Successfully.";
                                string subject = "Registration";
                                var Data = new System.Threading.Thread(() => mail.SendMailRegisterUser(msg, model.FirstName + " " + model.LastName, model.Email, model.Password, subject))
                                { IsBackground = true };
                                Data.Start();

                                return new ResultModel()
                                {
                                    Message = "Registration Successfully.",
                                    ResultFlag = 1
                                };
                            }
                            else
                            {
                                return new ResultModel()
                                {
                                    Message = "Registration failed!!",
                                    ResultFlag = 0
                                };
                            }
                        }
                        else
                        {
                            return new ResultModel()
                            {
                                Message = result.Errors.Count() > 0 ? result.Errors.FirstOrDefault() : "Registration failed!!",
                                ResultFlag = 0
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return RepoUserLogs.SendExceptionMailFromController(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), ex.Message, ex.StackTrace);
            }
        }

        #endregion

        #region Add Or Update Client API
        [AllowAnonymous]
        [HttpPost]
        public ResultModel ClientRegisterOrUpdate(ClsClient model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new ResultModel()
                    {
                        Message = "Please enter all required fields.",
                        ResultFlag = 0
                    };
                }
                else
                {
                    RepoUsers repoUsers = new RepoUsers();
                       
                        var clientDetails = new Client()
                        {
                            ClientId=model.ClientId??0,
                            FirstName = model.FirstName,
                            LastName=model.LastName,
                            Gender = model.Gender,
                            DateofBirth = model.DateOfBirth,
                            Address = model.Address,
                            PhoneNumber=model.PhoneNumber,
                            Description = model.Description,
                            JudgeId=model.JudgeId,
                            Status=model.Status,
                            SignedForm=model.SignForm,
                        };

                        var resultDetails = repoUsers.AddOrUpdateClient(clientDetails);

                        return new ResultModel()
                        {
                            Message = "Record added & updated successfully",
                            ResultFlag = 1
                        };
                }
            }
            catch (Exception ex)
            {
                return RepoUserLogs.SendExceptionMailFromController(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), ex.Message, ex.StackTrace);
            }
        }

        #endregion

        #region Add Or Update Judge API
        [AllowAnonymous]
        [HttpPost]
        public ResultModel JudgeRegisterOrUpdate(ClsJudge model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new ResultModel()
                    {
                        Message = "Please enter all required fields.",
                        ResultFlag = 0
                    };
                }
                else
                {
                    RepoJudge repoJudge = new RepoJudge();

                    var judgeDetails = new Judge()
                    {
                        JudgeId = model.JudgeId ?? 0,
                        JudgeName = model.JudgeName,
                        Address = model.Address,
                        PhoneNumber = model.PhoneNumber,
                        Email=model.Email,
                    };

                    var resultDetails = repoJudge.AddOrUpdateJudge(judgeDetails);

                    return new ResultModel()
                    {
                        Message = "Record added & updated successfully",
                        ResultFlag = 1
                    };
                }
            }
            catch (Exception ex)
            {
                return RepoUserLogs.SendExceptionMailFromController(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), ex.Message, ex.StackTrace);
            }
        }

        #endregion
        #endregion

        #region Helper

        public void setService()
        {
            _userManagement = new UserManagement(UserManager, SignInManager, RoleManager);
            _roleManagement = new RoleManagement(UserManager, SignInManager, _userManagement, RoleManager);
        }

        #endregion

        #region GetUserRole
        public string GetUserRole()
        {
            return ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role).OrderByDescending(o => o.Value).Select(c => c.Value).FirstOrDefault();
        }

        /// <summary>
        /// Get current logged in user's roles.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetUserRoles()
        {
            return ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role).OrderByDescending(o => o.Value).Select(c => c.Value);
        }

        #endregion

        #region GetCurrentUserId
        public string GetCurrentUserId()
        {
            var userid = User.Identity.GetUserId();
            if(string.IsNullOrEmpty(userid))
            {
                AccountController ac = new AccountController();
                ac.LogOff();
            }
            return userid;
        }
        #endregion

        #region Auth Management
        [AllowAnonymous]
        [HttpPost]
        public ResultModel LoginUser(clsAuth model)
        {
            //try
            //{
                AuthManagement authManagement = new AuthManagement();

            var result = SignInManager.PasswordSignIn(model.Username, model.Password, false, shouldLockout: true);

            switch (result)
            {
                case SignInStatus.Success:
                    var user = UserManager.FindByNameAsync(model.Username);
                    user.Wait();
                    if (!user.Result.EmailConfirmed)
                    {
                        return new ResultModel()
                        {
                            Message = "Your account is not approved. Please contact administrator.",
                            ResultFlag = 0
                        };
                    }
                    else
                    {
                        var token = authManagement.GetToken(model.Username, model.Password);
                        if (string.IsNullOrEmpty(token.Error))
                        {
                            var general = new General();
                            RepoUsers repoUsers = new RepoUsers();
                            var userDetail = repoUsers.GetUserDetailByUserId(user.Result.Id);
                            token.UserId = user.Result.Id;
                            var roles = _userManager.GetRolesAsync(token.UserId);
                            roles.Wait();
                            token.Roles = roles.Result.OrderByDescending(o => o.ToString()).ToList();
                            token.Profile = general.CheckImageUrl(!string.IsNullOrEmpty(userDetail?.UserImage) ? userDetail?.UserImage : "/assets/images/NoPhotoAvailable.png");
                            token.FullName = repoUsers.GetUserFullName(token.UserId, token.Roles);

                            return new ResultModel()
                            {
                                Message = "Login successfully.",
                                ResultFlag = 1,
                                Data = token
                            };
                        }
                        else
                        {
                            return new ResultModel()
                            {
                                Message = "Your email address is not approved.",
                                ResultFlag = 0
                            };
                        }
                    }
                case SignInStatus.LockedOut:
                    return new ResultModel()
                    {
                        Message = "Your are locked out. Please contact administrator.",
                        ResultFlag = 0
                    };
                case SignInStatus.Failure:
                default:
                    return new ResultModel()
                    {
                        Message = "Username password do not match",
                        ResultFlag = 0
                    };
            }
        //}
        //catch (Exception ex)
        //{
        //    return RepoUserLogs.SendExceptionMailFromController(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), ex);
        //}
    }

        [AllowAnonymous]
        [HttpPost]
        public ResultModel GetTokenByRefreshToken(clsAuth model)
        {
            try
            {
                AuthManagement authManagement = new AuthManagement();

                var token = authManagement.GetTokenFromRefreshToken(model.RefreshToken);
                if (string.IsNullOrEmpty(token.Error))
                {

                    RepoUsers repoUsers = new RepoUsers();
                    token.UserId = model.UserId;

                    var roles = _userManager.GetRolesAsync(token.UserId);
                    roles.Wait();
                    token.Roles = roles.Result;
                    token.FullName = repoUsers.GetUserFullName(token.UserId, token.Roles);
                    return new ResultModel()
                    {
                        Message = "Login successfully.",
                        ResultFlag = 1,
                        Data = token
                    };
                }
                else
                {
                    return new ResultModel()
                    {
                        Message = token.Error,
                        ResultFlag = 0
                    };
                }
            }
            catch (Exception)
            {
                return new ResultModel()
                {
                    Message = "Error",
                    ResultFlag = 0
                };
            }
        }

        #endregion

        #region Login Without Password
        [HttpPost]
        public ResultModel LoginWithoutPassword(clsAuth modal)
        {
            try
            {
                if (string.IsNullOrEmpty(modal.Username))
                {
                    return new ResultModel()
                    {
                        Message = "User Name Required",
                        ResultFlag = 0
                    };
                }
                var userId = _userManagement.GetUserIdByUsername(modal.Username);
                AuthManagement authManagement = new AuthManagement();
                var user = UserManager.FindByNameAsync(modal.Username);
                user.Wait();
                if (!user.Result.EmailConfirmed)
                {
                    return new ResultModel()
                    {
                        Message = "Your account is not approved by administrator.",
                        ResultFlag = 0
                    };
                }
                else
                {
                    RepoUsers repoUsers = new RepoUsers();
                    var userDetail = repoUsers.GetUserDetailByUserId(userId);
                    var password = Utils.Decrypt(userDetail.Password);
                    var token = authManagement.GetToken(modal.Username, Utils.Decrypt(userDetail.Password));
                    if (string.IsNullOrEmpty(token.Error))
                    {
                        var general = new General();
                        token.UserId = user.Result.Id;
                        var roles = _userManager.GetRolesAsync(token.UserId);
                        roles.Wait();
                        token.Roles = roles.Result.OrderByDescending(o => o.ToString()).ToList();
                        token.FullName = repoUsers.GetUserFullName(token.UserId, token.Roles);
                        token.Profile = general.CheckImageUrl(!string.IsNullOrEmpty(userDetail?.UserImage) ? userDetail?.UserImage : "/assets/images/NoPhotoAvailable.png");
                        return new ResultModel()
                        {
                            Message = "Login successfully.",
                            ResultFlag = 1,
                            Data = token
                        };
                    }
                    else
                    {
                        return new ResultModel()
                        {
                            Message = "Something goes wrong!! Please try again",
                            ResultFlag = 0
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return RepoUserLogs.SendExceptionMailFromController(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), ex.Message, ex.StackTrace);
            }
        }
        #endregion

        #region User Profile Api       
        [HttpPost]
        public ResultModel UserProfile()
        {
            try
            {
                string role = GetUserRole();
                var cls = new UserProfiles();
                var result = cls.GetUserProfile(User.Identity.GetUserId(), role);
                if (result != null)
                {
                    return new ResultModel()
                    {
                        Message = "Data found successfully.",
                        ResultFlag = 1,
                        Data = result
                    };
                }
                else
                {
                    return new ResultModel()
                    {
                        Message = "Data not found.",
                        ResultFlag = 0,
                        Data = result
                    };
                }
            }
            catch (Exception ex)
            {
                return RepoUserLogs.SendExceptionMailFromController(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), ex.Message, ex.StackTrace);
            }
        }

        #endregion

        #region User Profile Update Api       
        [HttpPost]
        public ResultModel UpdateUserProfile(ResponseUserProfile model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new ResultModel()
                    {
                        Message = "Please enter required fields.",
                        ResultFlag = 0
                    };
                }
                var cls = new UserProfiles();
                bool status = cls.UpdateUserProfile(model, User.Identity.GetUserId());
                if (status)
                {
                   var username= ChangeUserName(model);
                    if(username.ResultFlag==1)
                    { 
                    return new ResultModel()
                    {
                        Message = "Profile successfully updated.",
                        ResultFlag = 1,
                        Data = null
                    };
                    }
                    else
                    {
                        return username;
                    }
                }
                else
                {
                    return new ResultModel()
                    {
                        Message = "Profile not update.",
                        ResultFlag = 0,
                        Data = null
                    };
                }
            }
            catch (Exception ex)
            {
                return RepoUserLogs.SendExceptionMailFromController(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), ex.Message, ex.StackTrace);
            }
        }

        #endregion

        #region Change Class Status
        [HttpPost]
        public ResultModel ClassStatusChange(ClassResponse model)
        {
            try
            {
                RepoClass repoClass = new RepoClass();
                // var result = repoClass.ChangeClassStatusById(model);
                var result = true;
                if (result)
                {

                    return new ResultModel()
                    {
                        Message = "Class status change successfully.",
                        ResultFlag = 1
                    };
                }
                else
                {
                    return new ResultModel()
                    {
                        Message = "Failled to change class status.",
                        ResultFlag = 0
                    };
                }
            }
            catch (Exception ex)
            {
                return RepoUserLogs.SendExceptionMailFromController(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), ex.Message, ex.StackTrace);
            }
        }

        #endregion

        #region Delete Client
        [HttpPost]
        public ResultModel DeleteClient(ClsClient model)
        {
            try
            {
                RepoUsers repoUsers = new RepoUsers();
                
                var result = repoUsers.deleteClient(model.ClientId??0);
                if (result)
                {

                    return new ResultModel()
                    {
                        Message = "Client Deleted Successfully.",
                        ResultFlag = 1
                    };
                }
                else
                {
                    return new ResultModel()
                    {
                        Message = "Client Not Deleted.",
                        ResultFlag = 0
                    };
                }
            }
            catch (Exception ex)
            {
                return RepoUserLogs.SendExceptionMailFromController(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), ex.Message, ex.StackTrace);
            }
        }

        #endregion

        #region Get ChangeUserName
        [HttpPost]
        public ResultModel ChangeUserName(ResponseUserProfile model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new ResultModel()
                    {
                        Message = "Please enter all required fields.",
                        ResultFlag = 0
                    };
                }
                string userId = User.Identity.GetUserId();
                var result = _userManagement.UpdateUserName(userId, model.Emailaddress);
                string msg = "";
                if (result.Errors.Any())
                {
                    msg = result.Errors.FirstOrDefault();
                }
                else
                {
                    General cls = new General();
                    cls.UpdateUserName(model.Emailaddress);
                    msg = "UserName updated successfully.";
                }
                return new ResultModel()
                {
                    Data = msg,
                    Message = result.Succeeded ? "UserName updated successfully." : msg,
                    ResultFlag = result.Succeeded ? 1 : 0
                };
            }
            catch (Exception ex)
            {
                return RepoUserLogs.SendExceptionMailFromController(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), ex.Message, ex.StackTrace);
            }
        }
        #endregion
    }
}
