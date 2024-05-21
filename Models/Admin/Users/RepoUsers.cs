using CMS_Brian.Models.Admin.UserResponse;
using System.Collections.Generic;
using System.Linq;

namespace CMS_Brian.Models.Admin.Users
{
    public class RepoUsers
    {
        #region Variables
        CMSBrianEntities db = new CMSBrianEntities();
        #endregion

        #region Get
        public UserDetail GetUserDetailByUserId(string userId)
        {
            return db.UserDetails.Where(a => a.IsDeleted.Value == false && a.UserId == userId).FirstOrDefault();
        }

        public string GetUserFullName(string UserId, IList<string> roles)
        {
            string UserName = "";
            if (roles.Any())
            {
                var user = db.UserDetails.Where(a => a.UserId == UserId).FirstOrDefault();
                UserName = user != null ? $"{user.FirstName} {user.LastName}" : string.Empty;
            }
            return UserName;
        }
        #endregion

        #region Administrator

        #region Get All Administrator
        public List<ClsAdministratorResponse> GetAllAdministrators()
        {
            var administrators = new List<ClsAdministratorResponse>();

            var administratoruser = db.UserDetails.Where(a => a.IsDeleted.Value == false && a.AspNetUser.AspNetRoles.Where(b => b.Name == Constants.Administrators).Count() > 0).Select(u => new ClsAdministratorResponse()
            {
                UserId = u.UserId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Address = u.Address,
                Status = u.Status,
                UserImage = u.UserImage,
                PhoneNumber = u.PhoneNumber,
                Email = u.AspNetUser.Email,
                IsApproved = u.AspNetUser.EmailConfirmed
            }).ToList();
            foreach (var u in administratoruser)
            {
                var days = "";
                var availaDays = (from d in db.AvailableDays
                                  join av in db.UserAvailableDays on d.Id equals av.AvailableDayId
                                  where av.UserId == u.UserId
                                  select d).ToList();
                foreach (var d in availaDays)
                {
                    if (days == "") { days = d.DayName; }
                    else { days = days + ", " + d.DayName; }
                }


                u.AvailableDays = days;

                administrators.Add(u);

            }

            return administrators;
        }
        #endregion

        #region Get Administrator by UserId
        public ClsAdministrator GetAdministratorByUserId(string userId)
        {
            var administrator = new ClsAdministrator();

            administrator = (from u in db.UserDetails
                             where u.UserId == userId
                             select new ClsAdministrator()
                             {
                                 UserId = userId,
                                 FirstName = u.FirstName,
                                 LastName = u.LastName,
                                 Address = u.Address,
                                 Status = u.Status,
                                 UserImage = u.UserImage,
                                 PhoneNumber = u.PhoneNumber
                             }).FirstOrDefault();

            return administrator;
        }
        #endregion

        #region Get Administrator or Instractor Details by UserId
        public ClsAdministratorResponse GetAdminOrInsDetailsByUserId(string userId)
        {
            var administrator = new ClsAdministratorResponse();

            administrator = (from u in db.UserDetails
                             where u.UserId == userId
                             select new ClsAdministratorResponse()
                             {
                                 UserId = userId,
                                 FirstName = u.FirstName,
                                 LastName = u.LastName,
                                 Address = u.Address,
                                 Status = u.Status,
                                 UserImage = u.UserImage,
                                 PhoneNumber = u.PhoneNumber
                             }).FirstOrDefault();

            var ab = db.UserDetails.Where(a => a.UserId == userId).FirstOrDefault();
            if (administrator != null)
            {
                var classNames = "";
                var DayName = "";
                var userclass = db.ClassTables.Where(c => c.InsAdminId == administrator.UserId).ToList();
                foreach (var c in userclass)
                {
                    if (classNames != "") { classNames = classNames + ", " + c.ClassName; }
                    else { classNames = c.ClassName; }
                }
                var availableDays = (from d in db.AvailableDays
                                     join u in db.UserAvailableDays on d.Id equals u.AvailableDayId
                                     where u.UserId == userId
                                     select d).ToList();
                foreach (var d in availableDays)
                {
                    if (DayName != "") { DayName = DayName + ", " + d.DayName; }
                    else { DayName = d.DayName; }
                }
                administrator.AvailableDays = DayName;
                administrator.AssignClasses = classNames;
            }

            return administrator;
        }
        #endregion

        #region Get AdministratorId by Instructor Id
        //public int GetAdministratorIdByInstructorId(int? Id)
        //{
        //    int administratorId = 0;
        //    var instructor = db.Instructors.Where(a => a.InstructorId == Id).FirstOrDefault();
        //    if (instructor != null) { administratorId = instructor.AdministratorId ?? 0; }
        //    return administratorId;
        //}
        #endregion
        #endregion

        #region Get All Instructor 
        public List<ClsInstructorResponse> GetAllInstructor()
        {
            var Instructors = new List<ClsInstructorResponse>();

            var instructorsuser = db.UserDetails.Where(a => a.IsDeleted.Value == false && a.AspNetUser.AspNetRoles.Where(b => b.Name == Constants.Instructors).Count() > 0).Select(u => new ClsInstructorResponse()
            {
                UserId = u.UserId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Address = u.Address,
                Status = u.Status,
                UserImage = u.UserImage,
                Email = u.AspNetUser.Email,
                PhoneNumber = u.PhoneNumber,
                IsApproved = u.AspNetUser.EmailConfirmed
            }).ToList();
            foreach (var u in instructorsuser)
            {
                var days = "";
                var availaDays = (from d in db.AvailableDays
                                  join av in db.UserAvailableDays on d.Id equals av.AvailableDayId
                                  where av.UserId == u.UserId
                                  select d).ToList();
                foreach (var d in availaDays)
                {
                    if (days == "") { days = d.DayName; } else { days = days + ", " + d.DayName; }

                }


                u.AvailableDays = days;

                Instructors.Add(u);

            }

            return Instructors;
        }

        public ClsInstructor GetInstructorByUserId(string userId)
        {
            var instructor = new ClsInstructor();

            instructor = (from u in db.UserDetails
                          where u.UserId == userId
                          select new ClsInstructor()
                          {
                              UserId = userId,
                              FirstName = u.FirstName,
                              LastName = u.LastName,
                              Address = u.Address,
                              Status = u.Status,
                              UserImage = u.UserImage,
                              PhoneNumber = u.PhoneNumber,
                          }).FirstOrDefault();

            return instructor;
        }


        #endregion

        #region Clients

        #region Get All Clients by ClassId
        public List<Client> GetAllClients(int? classId)
        {
            var clients = new List<Client>();
            if (classId == null)
            {
                clients = db.Clients.ToList();
            }
            else
            {
                clients = (from c in db.Clients
                           join cc in db.ClientClasses on c.ClientId equals cc.ClientId
                           where cc.ClassId == classId
                           select c
                         ).ToList();
            }
            return clients;
        }
        #endregion

        #region Get All Clients List
        public List<ClientResponseList> GetAllClientList()
        {
            var data = new List<ClientResponseList>();
            var clients = db.Clients.ToList();
            foreach (var client in clients)
            {
                decimal totalPayment = 0;
                var payments = db.ClientPaymentLogs.Where(p => p.ClientId == client.ClientId).ToList();
                if (payments.Count > 0)
                {
                    totalPayment = payments.Sum(p => p.PaidAmount.Value);
                }
                data.Add(new ClientResponseList()
                {
                    ClientId = client.ClientId,
                    ClientName = client.FirstName + " " + client.LastName,
                    DateofBirth = client.DateofBirth,
                    JudgeName = client.JudgeId != null ? client.Judge.JudgeName : "",
                    Status = client.Status,
                    Address = client.Address,
                    Classes = db.ClientClasses.Where(a => a.ClientId == client.ClientId).Select(a => a.ClientId).ToList().Count,
                    SignedForm = client.SignedForm,
                    Gender = client.Gender,
                    TotalPayment = totalPayment,
                });
            }
            return data;
        }
        #endregion

        #region Get Client by Id
        public ClsClient GetClientById(int clientId)
        {
            var client = new ClsClient();

            client = (from u in db.Clients
                      where u.ClientId == clientId
                      select new ClsClient()
                      {
                          ClientId = clientId,
                          FirstName = u.FirstName,
                          LastName = u.LastName,
                          Gender = u.Gender,
                          DateOfBirth = u.DateofBirth,
                          Address = u.Address,
                          JudgeId = u.JudgeId,
                          PhoneNumber = u.PhoneNumber,
                          Description = u.Description,
                          Status = u.Status,
                          SignForm = u.SignedForm,
                      }).FirstOrDefault();
            return client;
        }
        #endregion

        #region Update Insert
        public bool AddOrUpdateClient(Client model)
        {
            var old = db.Clients.Where(a => a.ClientId == model.ClientId).FirstOrDefault();
            if (old == null)
            {
                db.Clients.Add(model);
                return db.SaveChanges() > 0;
            }
            else
            {
                old.FirstName = model.FirstName;
                old.LastName = model.LastName;
                old.Gender = model.Gender;
                old.Address = model.Address;
                old.DateofBirth = model.DateofBirth;
                old.JudgeId = model.JudgeId;
                old.Description = model.Description;
                old.PhoneNumber = model.PhoneNumber;
                old.Status = model.Status;
                old.SignedForm = model.SignedForm;

                db.SaveChanges();
                return true;
            }
        }
        #endregion

        #region Delete Client

        public bool deleteClient(int clientId)
        {
            var old = db.Clients.Where(a => a.ClientId == clientId).FirstOrDefault();
            if (old == null)
            {
                return false;
            }
            db.Clients.Remove(old);
            return db.SaveChanges() > 0;
        }

        #endregion

        #region Get Client Details By Id
        public ClientDetail GetClientDetailById(int id)
        {
            var data = (from cl in db.Clients
                        where cl.ClientId == id
                        select new ClientDetail()
                        {
                            ClientId = cl.ClientId,
                            FirstName = cl.FirstName,
                            LastName = cl.LastName,
                            DateofBirth = cl.DateofBirth,
                            JudgeName = cl.JudgeId != null ? cl.Judge.JudgeName : "",
                            Status = cl.Status,
                            Address = cl.Address,
                            SignedForm = cl.SignedForm,
                            Gender = cl.Gender,
                            PhoneNumber = cl.PhoneNumber,
                        }).FirstOrDefault();
            return data;
        }
        #endregion

        #region Get Clients By JudegeId
        public List<RepoJudgeClients> GetClientsByJudgeId(int? judgeId)
        {
            var clientList = new List<RepoJudgeClients>();
            var clients = db.Clients.Where(c => c.JudgeId == judgeId).ToList();
            var clientclasses = db.ClientClasses.ToList();
            foreach (var item in clients)
            {
                var clientclass = clientclasses?.Where(c => c.ClientId == item.ClientId).OrderByDescending(c=>c.Id).ToList();
                clientList.Add(new RepoJudgeClients()
                {
                    ClientName = item.FirstName + " " + item.LastName,
                    DateOfBirth = item.DateofBirth,
                    Address = item.Address,
                    PhoneNumber = item.PhoneNumber,
                    Status = item.Status,
                    ClassName = clientclass != null ? clientclass.FirstOrDefault().ClassTable?.ClassName : "",
                    ClassCount = clientclass.Count,
                });
            }
            return clientList;
        }
        #endregion

        #region Get ClientIds By JudegeId
        public List<int> GetClientIdsByJudgeId(int? judgeId)
        {
            return db.Clients.Where(c => c.JudgeId == judgeId)?.Select(cc => cc.ClientId).ToList();
        }
        #endregion
        #endregion

        #region Update Insert
        public bool AddOrUpdateUsers(UserDetail model)
        {
            var old = db.UserDetails.Where(a => a.UserId == model.UserId).FirstOrDefault();
            if (old == null)
            {
                model.IsDeleted = false;
                db.UserDetails.Add(model);
                return db.SaveChanges() > 0;
            }
            else
            {
                old.FirstName = model.FirstName;
                old.LastName = model.LastName;
                old.Gender = model.Gender;
                old.Address = model.Address;
                old.DateofBirth = model.DateofBirth;
                old.JudgeName = model.JudgeName;
                old.Description = model.Description;
                old.UserImage = model.UserImage;
                old.Status = model.Status;
                db.SaveChanges();
                return true;
            }
        }
        #endregion

        #region Delete

        public bool deleteUser(string id)
        {
            var old = db.UserDetails.Where(a => a.UserId == id).FirstOrDefault();
            if (old == null)
            {
                return false;
            }
            old.IsDeleted = true;
            return db.SaveChanges() > 0;
        }

        #endregion

        #region Update password
        public bool UpdatePassword(UserDetail model)
        {
            var ud = db.UserDetails.Where(a => a.UserId == model.UserId).FirstOrDefault();
            if (ud != null)
            {
                ud.Password = model.Password;
                return db.SaveChanges() > 0;
            }
            else
                return false;
        }
        #endregion
    }
}