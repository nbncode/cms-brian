using CMS_Brian.Models;
using CMS_Brian.Models.Admin.Class;
using CMS_Brian.Models.Admin.UserResponse;
using CMS_Brian.Models.Admin.Users;
using CMS_Brian.Models.Dashboard;
using CMS_Brian.Models.judgeModel;
using CMS_Brian.Models.Others;
using CMS_Brian.Models.PaymentLog;
using CMS_Brian.Models.UserLogs;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CMS_Brian.Controllers
{
    [Authorize(Roles = "SuperAdmin,Administrators,Instructors,Clients")]
    public class AdminController : Controller
    {
        #region Dashboard
        public ActionResult Index()
        {
            DashboardModel model = new DashboardModel();
            SystemController system = new SystemController();
            model.Role = system.GetUserRole();
            model.UserId = system.GetCurrentUserId();
            RepoDashboard dashboard = new RepoDashboard();
            var data = dashboard.GetDashboard(model);
            return View(data);
        }
        #endregion

        #region Administrator
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Administrator()
        {
            return View();
        }
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult AdministratorPartialView()
        {
            RepoUsers db = new RepoUsers();
            return PartialView(db.GetAllAdministrators());
        }
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult AddAdministrator(string mode, string id)
        {
            SystemController systemController = new SystemController();

            if (!string.IsNullOrEmpty(mode) && mode == "edit")
            {
                RepoUsers db = new RepoUsers();
                var data = db.GetAdministratorByUserId(id);

                return View(data);
            }

            return View();
        }

        [HttpPost]
        public ActionResult AddAdministrator(ClsAdministrator model)
        {
            try
            {
                SystemController systemController = new SystemController();

                if (!string.IsNullOrEmpty(model.UserId))
                {
                    ViewBag.id = model.UserId;
                    model.PhoneNumber = "123123";
                    model.Password = "123123";
                }
                model.IsApproved = true;
                var result = systemController.AdministratorRegisterOrUpdate(model);

                if (result.ResultFlag == 1)
                {
                    TempData["success"] = result.Message;
                    return RedirectToAction("Administrator", "Admin");
                }

                TempData["error"] = result.Message;
            }
            catch (Exception exc)
            {
                TempData["error"] = "Record Added & Updated Unsuccessfully";
                RepoUserLogs.LogException(exc);
            }

            return View(model);
        }

        public ActionResult AdministratorDetails(string UserId)
        {
            var cls = new RepoUsers();
            var result = cls.GetAdminOrInsDetailsByUserId(UserId);
            return View(result);
        }
        #endregion

        #region Instructor
        [Authorize(Roles = "SuperAdmin,Administrators")]
        public ActionResult Instructor()
        {
            return View();
        }
        [Authorize(Roles = "SuperAdmin,Administrators")]
        public ActionResult InstructorPartialView()
        {
            RepoUsers db = new RepoUsers();
            return PartialView(db.GetAllInstructor());
        }
        [Authorize(Roles = "SuperAdmin,Administrators")]
        public ActionResult AddInstructor(string mode, string id)
        {
            SystemController systemController = new SystemController();
            if (!string.IsNullOrEmpty(mode) && mode == "edit")
            {
                RepoUsers db = new RepoUsers();
                var data = db.GetInstructorByUserId(id);

                return View(data);
            }

            return View();
        }

        [HttpPost]
        public ActionResult AddInstructor(ClsInstructor model)
        {
            try
            {
                SystemController systemController = new SystemController();

                if (!string.IsNullOrEmpty(model.UserId))
                {
                    ViewBag.id = model.UserId;
                    model.PhoneNumber = "123123";
                    model.Password = "123123";
                }
                model.IsApproved = true;
                var result = systemController.InstructorRegisterOrUpdate(model);

                if (result.ResultFlag == 1)
                {
                    TempData["success"] = result.Message;
                    return RedirectToAction("Instructor", "Admin");
                }

                TempData["error"] = result.Message;
            }
            catch (Exception exc)
            {
                TempData["error"] = "Record Added & Updated Unsuccessfully";
                RepoUserLogs.LogException(exc);
            }

            return View(model);
        }

        public ActionResult InstructorDetails(string UserId)
        {
            var cls = new RepoUsers();
            var result = cls.GetAdminOrInsDetailsByUserId(UserId);
            return View(result);
        }
        #endregion

        #region Assign Classes to Instructor View
        public ActionResult AssignClassesToInstructorView(string UserId)
        {
            var rc = new RepoClass();
            ViewBag.ClassList = rc.GetAllActiveClasses(Constants.SuperAdmin, "");
            var repoUser = new RepoUsers();
            var user = repoUser.GetUserDetailByUserId(UserId);
            var data = new clsAssignClassToInstructor
            {
                UserName = user != null ? user.FirstName + " " + user.LastName : "",
                UserId = UserId,
                ClassIds = new List<int>()
            };
            foreach (var cId in rc.GetUserClassIdByUserId(UserId))
            {
                data.ClassIds.Add(cId);
            }
            return View(data);
        }

        [HttpPost]
        public ActionResult SaveUserClasses(string userId, string assignClassIds)
        {
            object data = null;
            try
            {
                var ru = new RepoClass();
                bool status = ru.AssignClassesToUser(userId, assignClassIds);

                data = new
                {
                    Message = status ? "Classes saved successfully." : "Failed to save Classes.",
                    ResultFlag = status ? 1 : 0
                };
            }
            catch (Exception exc)
            {
                data = new
                {
                    exc.Message,
                    ResultFlag = 0
                };
                RepoUserLogs.LogException(exc);
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Client
        [Authorize(Roles = "SuperAdmin,Administrators,Instructors")]
        public ActionResult Client()
        {
            return View();
        }
        [Authorize(Roles = "SuperAdmin,Administrators,Instructors")]
        public ActionResult ClientPartialView()
        {
            RepoUsers db = new RepoUsers();
            return PartialView(db.GetAllClientList());
        }
        [Authorize(Roles = "SuperAdmin,Administrators,Instructors")]
        public ActionResult AddClient(string mode, int? id)
        {
            RepoJudge rj = new RepoJudge();
            ViewBag.JudgeList = rj.JudgeSelectList();
            if (!string.IsNullOrEmpty(mode) && mode == "edit")
            {
                RepoUsers db = new RepoUsers();
                var data = db.GetClientById(id.Value);
                return View(data);
            }
            return View(new ClsClient());
        }

        [HttpPost]
        public ActionResult AddClient(ClsClient model)
        {
            try
            {
                RepoJudge rj = new RepoJudge();
                ViewBag.JudgeList = rj.JudgeSelectList();
                RepoUsers db = new RepoUsers();
                SystemController systemController = new SystemController();

                var result = systemController.ClientRegisterOrUpdate(model);
                if (result.ResultFlag == 1)
                {
                    TempData["success"] = result.Message;
                    return RedirectToAction("Client", "Admin");
                }

                TempData["error"] = result.Message;
            }
            catch (Exception exc)
            {
                TempData["error"] = "Record Added & Updated Unsuccessfully";
                RepoUserLogs.LogException(exc);
            }

            return View(model);
        }

        public ActionResult ClientDetails(int ClientId)
        {
            try
            {
                RepoUsers db = new RepoUsers();
                var result = db.GetClientDetailById(ClientId);
                return View(result);
            }
            catch (Exception exc)
            {
                TempData["error"] = "Some unexpected error.";
                RepoUserLogs.LogException(exc);
            }
            return View();
        }
        #endregion

        #region Assign Clients for Class
        [Authorize(Roles = "SuperAdmin,Administrators,Instructors")]
        public ActionResult AddClientsClass(int clientId)
        {
            var repoUser = new RepoUsers();
            var client = repoUser.GetClientById(clientId);

            var rc = new RepoClass();
            ViewBag.ClientId = clientId;

            SystemController system = new SystemController();
            var role = system.GetUserRole();
            var currentUserId = system.GetCurrentUserId();

            ViewBag.ClassList = rc.GetAllActiveClasses(role, currentUserId);

            var data = new clsAddClientClass
            {
                ClientName = client != null ? client.FirstName + " " + client.LastName : "",
                ClientId = clientId,
                ClassIds = new List<int>()
            };
            var clientClass = new RepoClientClass();
            foreach (var cId in clientClass.GetClientClassIdByClientId(clientId))
            {
                data.ClassIds.Add(cId);
            }
            return View(data);
        }

        [HttpPost]
        public ActionResult SaveClientsClass(int clientId, string classIds)
        {
            object data = null;

            try
            {
                string[] classIdsArr = null;
                if (!string.IsNullOrEmpty(classIds))
                    classIdsArr = classIds.Split(',');

                var lstClientClasses = new List<ClientClass>();
                if (classIdsArr != null)
                {
                    foreach (var cid in classIdsArr)
                    {
                        lstClientClasses.Add(new ClientClass()
                        {
                            ClientId = clientId,
                            ClassId = Convert.ToInt32(cid),
                            CreatedDate = DateTime.Now
                        });
                    }
                }

                var ru = new RepoClientClass();
                bool status = ru.AddOrUpdateClassForClient(lstClientClasses, clientId);

                data = new
                {
                    Message = status ? "Classes saved for client successfully." : "Failed to save Classes.",
                    ResultFlag = status ? 1 : 0
                };
            }
            catch (Exception exc)
            {
                data = new
                {
                    exc.Message,
                    ResultFlag = 0
                };
                RepoUserLogs.LogException(exc);
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Assign Classes for Client Partial View
        public ActionResult ClientClassesPartialView(int ClientId)
        {
            RepoClass db = new RepoClass();
            return PartialView(db.GetAssignClassesByClientId(ClientId));
        }
        #endregion

        #region Payment Logs
        public ActionResult ClientPaymentLog(int ClientId)
        {
            ViewBag.ClientId = ClientId;
            return View();
        }
        public ActionResult ClientPaymentLogPartialView(int ClientId)
        {
            RepoUsers repoUser = new RepoUsers();
            var client = repoUser.GetClientById(ClientId);
            ViewBag.ClientName = client != null ? client.FirstName + " " + client.LastName : "";
            RepoPaymentLog rp = new RepoPaymentLog();
            ViewBag.TotalPaidAmount =rp.GetTotalPaidAmountByClientId(ClientId);
            ViewBag.ClientId = ClientId;
            RepoPaymentLog pm = new RepoPaymentLog();
            var data = pm.GetAllPaymentLogsByClientId(ClientId);
            return PartialView(data);
        }

        public ActionResult AddPaymentLog(int? id,string mode, int ClientId, string classId)
        {
            ClsPaymentLog model = new ClsPaymentLog();
            RepoClass db = new RepoClass();
            ViewBag.ClassList = db.ClassSelectListByClientId(ClientId);
            General gn = new General();
            ViewBag.PaymentModeList = gn.PaymentModeList();
            if (!string.IsNullOrEmpty(mode) && mode == "edit")
            {
                RepoPaymentLog pm = new RepoPaymentLog();
                var data = pm.GetPaymentLogId(id.Value);
                return View(data);
            }
            model.ClientId = ClientId;
            model.PaidAmount = null;
            if (!string.IsNullOrEmpty(classId))
            {
                model.ClassId = Convert.ToInt32(classId);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult AddPaymentLog(ClsPaymentLog model)
        {
            RepoClass rc = new RepoClass();
            General gn = new General();
            try
            {
                RepoPaymentLog db = new RepoPaymentLog();
                if (!ModelState.IsValid)
                {
                    ViewBag.ClassList = rc.ClassSelectListByClientId(model.ClientId??0);
                    ViewBag.PaymentModeList = gn.PaymentModeList();
                    return View(model);
                }
                var result = db.AddOrUpdatePaymentLog(model);

                if (result)
                {
                   // string urlName = Request.UrlReferrer.ToString();
                    TempData["success"] = "Record Added & Updated Successfully";
                    return RedirectToAction("ClassRoster", "Admin", new { ClassId = model.ClassId });
                }
            }
            catch (Exception exc)
            {
                TempData["error"] = "Record Added & Updated Unsuccessfully";
                RepoUserLogs.LogException(exc);
            }
            ViewBag.ClassList = rc.ClassSelectListByClientId(model.ClientId ?? 0);
            ViewBag.PaymentModeList = gn.PaymentModeList();
            return View(model);
        }
        #endregion

        #region Class
        [Authorize(Roles = "SuperAdmin,Administrators,Instructors")]
        public ActionResult Class()
        {
            return View();
        }
        [Authorize(Roles = "SuperAdmin,Administrators,Instructors")]
        public ActionResult ClassPartialView()
        {
            SystemController system = new SystemController();
            var role = system.GetUserRole();
            var userId = system.GetCurrentUserId();
            RepoClass db = new RepoClass();
            return PartialView(db.GetAllClasses(role, userId));
        }
        [Authorize(Roles = "SuperAdmin,Administrators,Instructors")]
        public ActionResult AddClass(string mode, int? id)
        {
            RepoClass db = new RepoClass();
            ViewBag.UserList = db.AdminOrInstractorSelectList();

            if (!string.IsNullOrEmpty(mode) && mode == "edit")
            {
                var data = db.GetClassByClassId(id.Value);
                return View(data);
            }

            return View();
        }

        [HttpPost]
        public ActionResult AddClass(ClassResponse model)
        {
            try
            {
                RepoClass db = new RepoClass();
                ViewBag.UserList = db.AdminOrInstractorSelectList();

                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var result = db.AddOrUpdateClass(model);

                if (result)
                {
                    TempData["success"] = "Record Added & Updated Successfully";
                    return RedirectToAction("Class", "Admin");
                }
            }
            catch (Exception exc)
            {
                TempData["error"] = "Record Added & Updated Unsuccessfully";
                RepoUserLogs.LogException(exc);
            }

            return View(model);
        }

        public ActionResult ClassDetails(int ClassId)
        {
            try
            {
                RepoClass db = new RepoClass();
            var result = db.GetClassDetailsByClassId(ClassId);
            return View(result);
            }
            catch (Exception exc)
            {
                TempData["error"] = "Some unexpected error.";
                RepoUserLogs.LogException(exc);
            }
            return View();
        }

        #endregion

        #region Class Roster
        public ActionResult ClassRoster(int ClassId)
        {
            ViewBag.classId = ClassId;
            return View();
        }
        public ActionResult ClassRosterPartialView(int ClassId)
        {
            RepoClass repoCls = new RepoClass();
            ViewBag.className = repoCls.GetClassNameByClassId(ClassId);
            ViewBag.classId = ClassId;
            RepoClassRoster rcl = new RepoClassRoster();
            var data = rcl.GetAllRosterClassByClassId(ClassId);
            return PartialView(data);
        }

        public ActionResult SaveRoster(int rosterId,int classId, int clientId)
        {
            RepoJudge rj = new RepoJudge();
            ViewBag.JudgeList = rj.JudgeSelectList();
            RepoUsers repoUser = new RepoUsers();
            var client = repoUser.GetClientById(clientId);
            RepoClass rc = new RepoClass();
            var cl = rc.GetClassByClassId(classId);
            var model = new ClsRoster();
            if (rosterId>0)
            {
                RepoClassRoster db = new RepoClassRoster();
                var data = db.GetClassRosterByRosterId(rosterId);
                data.ClientName = client != null ? client.FirstName + " " + client.LastName : "";
                data.ClassName = cl != null ? cl.ClassName : "";
                return View(data);
            }
            else
            {
                model.ClassId = classId;
                model.ClientId = clientId;
                model.ClientName = client != null ? client.FirstName + " " + client.LastName:"";
                model.ClassName = cl!=null?cl.ClassName:"";
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult SaveRoster(ClsRoster model)
        {
            RepoJudge rj = new RepoJudge();
            ViewBag.JudgeList = rj.JudgeSelectList();
            try
            {
                RepoClassRoster db = new RepoClassRoster();
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var result = db.AddOrUpdateClassRoster(model);

                if (result)
                {
                    TempData["success"] = "Record Added & Updated Successfully";
                    return RedirectToAction("ClassRoster", "Admin", new { ClassId = model.ClassId });
                }
            }
            catch (Exception exc)
            {
                TempData["error"] = "Record Added & Updated Unsuccessfully";
                RepoUserLogs.LogException(exc);
            }

            return View(model);
        }
        #endregion

        #region Dashboard Instructor PartialView
        public ActionResult DashboardInstructorPartialView()
        {
            RepoUsers db = new RepoUsers();
            return PartialView(db.GetAllInstructor());
        }
        #endregion

        #region Dashboard Client PartialView
        public ActionResult DashboardClientPartialView(int? classId)
        {
            RepoUsers db = new RepoUsers();
            return PartialView(db.GetAllClients(classId));
        }
        #endregion

        #region Assign Clients for Class
        [Authorize(Roles = "SuperAdmin,Administrators,Instructors")]
        public ActionResult AssignClassToClient(int ClassId)
        {
            var repoClass = new RepoClass();
            var cl = repoClass.GetClassByClassId(ClassId);

            ViewBag.ClassId = ClassId;
            var ru = new RepoUsers();
            ViewBag.ClientList = ru.GetAllClients(null);

            var data = new clsAddClassClients
            {
                ClassName = cl != null ? cl.ClassName : "",
                ClassId = ClassId,
                ClientIds = new List<int>()
            };
            var clientClass = new RepoClientClass();
            foreach (var cId in clientClass.GetClientListByClassId(ClassId))
            {
                data.ClientIds.Add(cId);
            }
            return View(data);
        }

        [HttpPost]
        public ActionResult SaveClientForClass(int ClassId, string ClientIds)
        {
            object data = null;

            try
            {
                string[] clientIdsArr = null;
                if (!string.IsNullOrEmpty(ClientIds))
                    clientIdsArr = ClientIds.Split(',');

                var lstClientClasses = new List<ClientClass>();
                if (clientIdsArr != null)
                {
                    foreach (var cid in clientIdsArr)
                    {
                        lstClientClasses.Add(new ClientClass()
                        {
                            ClientId = Convert.ToInt32(cid),
                            ClassId = ClassId,
                            CreatedDate = DateTime.Now
                        });
                    }
                }

                var ru = new RepoClientClass();
                bool status = ru.AddOrUpdateClientsForClass(lstClientClasses, ClassId);

                data = new
                {
                    Message = status ? "Client saved for class successfully." : "Failed to save Client.",
                    ResultFlag = status ? 1 : 0
                };
            }
            catch (Exception exc)
            {
                data = new
                {
                    exc.Message,
                    ResultFlag = 0
                };
                RepoUserLogs.LogException(exc);
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Judge
        [Authorize(Roles = "SuperAdmin,Administrators,Instructors")]
        public ActionResult Judge()
        {
            return View();
        }
        [Authorize(Roles = "SuperAdmin,Administrators,Instructors")]
        public ActionResult JudgePartialView()
        {
            RepoJudge db = new RepoJudge();
            return PartialView(db.GetAllJudges());
        }
        [Authorize(Roles = "SuperAdmin,Administrators,Instructors")]
        public ActionResult AddJudge(string mode, int? id)
        {
            if (!string.IsNullOrEmpty(mode) && mode == "edit")
            {
                RepoJudge db = new RepoJudge();
                var data = db.GetjudgeById(id.Value);
                return View(data);
            }
            return View(new ClsJudge());
        }

        [HttpPost]
        public ActionResult AddJudge(ClsJudge model)
        {
            try
            {
                RepoJudge db = new RepoJudge();
                SystemController systemController = new SystemController();

                var result = systemController.JudgeRegisterOrUpdate(model);
                if (result.ResultFlag == 1)
                {
                    TempData["success"] = result.Message;
                    return RedirectToAction("Judge", "Admin");
                }

                TempData["error"] = result.Message;
            }
            catch (Exception exc)
            {
                TempData["error"] = "Record Added & Updated Unsuccessfully";
                RepoUserLogs.LogException(exc);
            }

            return View(model);
        }

        public ActionResult JudgeDetails(int id)
        {
            var cls = new RepoJudge();
            var result = cls.GetjudgeById(id);
            return View(result);
        }
        #endregion

        #region Assign Clients to Judge
        public ActionResult AssignClientsToJudgeView(int JudgeId)
        {
            var ru = new RepoUsers();
            ViewBag.ClientList = ru.GetAllClientList();
            var repoJudge = new RepoJudge();
            var user = repoJudge.GetjudgeById(JudgeId);
            var data = new clsAssignClientToJudge
            {
                UserName = user != null ? user.JudgeName: "",
                JudgeId = JudgeId,
                ClientIds = new List<int>()
            };
            foreach (var cId in ru.GetClientIdsByJudgeId(JudgeId))
            {
                data.ClientIds.Add(cId);
            }
            return View(data);
        }

        [HttpPost]
        public ActionResult SaveJudgeClients(int JudgeId, string assignClientIds)
        {
            object data = null;
            try
            {
                var rj = new RepoJudge();
                bool status = rj.AssignClientsToJudge(JudgeId, assignClientIds);

                data = new
                {
                    Message = status ? "Client saved successfully." : "Failed to save Client.",
                    ResultFlag = status ? 1 : 0
                };
            }
            catch (Exception exc)
            {
                data = new
                {
                    exc.Message,
                    ResultFlag = 0
                };
                RepoUserLogs.LogException(exc);
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Assign Clients Partial For Judge 
        public ActionResult AssignClientsForJudge(int JudgeId)
        {
            RepoUsers db = new RepoUsers();
            return PartialView(db.GetClientsByJudgeId(JudgeId));
        }
        #endregion
    }
}
