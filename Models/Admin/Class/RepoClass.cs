using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace CMS_Brian.Models.Admin.Class
{

    #region Class response
    public class ClassResponse
    {
        public int? ClassId { get; set; }
        [Required]
        public string ClassName { get; set; }
        public string ClassDescription { get; set; }
        [Required]
        public string LocationName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Status { get; set; }
        //[Required]
        public string DaysOfWeek { get; set; }
        [Required]
        public string InstructorId { get; set; }
        public decimal? ClassFees { get; set; }
    }
    #endregion

    #region Available Days Id response
    public class AvailableDaysId
    {
        public int Id { get; set; }
    }
    #endregion

    #region Class Details
    public class ClassDetails
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string ClassDescription { get; set; }
        public string LocationName { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public string AvailableDays { get; set; }
        public string InstructorName { get; set; }
        public decimal? ClassFees { get; set; }
    }
    #endregion

    #region Class Details
    public class ClassList
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string ClassLocation { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public int Roster { get; set; }
        public string AvailableDays { get; set; }
        public string InstructorName { get; set; }
        public string ClassFees { get; set; }
    }
    #endregion

    public class RepoClass
    {
        #region Variables
        CMSBrianEntities db = new CMSBrianEntities();
        #endregion

        #region Add Or Update Class
        public bool AddOrUpdateClass(ClassResponse model)
        {
            var old = db.ClassTables.Where(c => c.ClassId == model.ClassId).FirstOrDefault();
            if (old != null)
            {
                old.ClassName = model.ClassName;
                old.ClassDescription = model.ClassDescription;
                old.LocationName = model.LocationName;
                old.Address = model.Address;
                old.InsAdminId = model.InstructorId;
                old.DaysOfWeek = model.DaysOfWeek;
                old.Status = model.Status;
                old.ClassFees = model.ClassFees;
            }
            else
            {
                db.ClassTables.Add(new ClassTable()
                {
                    ClassName = model.ClassName,
                    ClassDescription = model.ClassDescription,
                    LocationName = model.LocationName,
                    Address = model.Address,
                    Status = model.Status,
                    DaysOfWeek = model.DaysOfWeek,
                    InsAdminId = model.InstructorId,
                    ClassFees=model.ClassFees
                });
            }
            return db.SaveChanges() > 0;
        }
        #endregion

        #region Get Class by ClassId

        public ClassResponse GetClassByClassId(int classId)
        {
            var classModel = new ClassResponse();
            var data = db.ClassTables.Where(c => c.ClassId == classId).FirstOrDefault();
            if (data != null)
            {
                classModel.ClassId = data.ClassId;
                classModel.ClassName = data.ClassName;
                classModel.LocationName = data.LocationName;
                classModel.Address = data.Address;
                classModel.ClassDescription = data.ClassDescription;
                classModel.Status = data.Status;
                classModel.DaysOfWeek = data.DaysOfWeek;
                classModel.InstructorId = data.InsAdminId;
                classModel.ClassFees = data.ClassFees;
            }

            return classModel;
        }
        #endregion

        #region Get All Classes
        public List<ClassList> GetAllClasses(string role, string userId)
        {
            var classes = new List<ClassTable>();
            var data = new List<ClassList>();
            if (role == Constants.SuperAdmin || role == Constants.Administrators)
            {
                classes = db.ClassTables.ToList();
            }
            else
            {
                classes = db.ClassTables.Where(c => c.InsAdminId == userId).ToList();
            }
            foreach (var c in classes)
            {
                var days = "";
                var user = db.UserDetails.Where(a => a.UserId == c.InsAdminId).FirstOrDefault();
                var availDays = db.AvailableDays.Where(a => c.DaysOfWeek.Contains(a.Id.ToString())).ToList();
                foreach (var d in availDays)
                {
                    if (days == "") { days = d.DayName; }
                    else { days = days + " ," + d.DayName; }
                }

                data.Add(new ClassList()
                {
                    ClassId = c.ClassId,
                    ClassName = c.ClassName,
                    ClassLocation = c.LocationName,
                    Address = c.Address,
                    Status = c.Status,
                    Roster = db.ClientClasses.Where(a => a.ClassId == c.ClassId).ToList().Count,
                    AvailableDays = days,
                    InstructorName = user != null ? user.FirstName + " ," + user.LastName : "",
                    ClassFees=c.ClassFees.ToString()
                });
            }
            return data;
        }
        #endregion

        #region Get All Assign Classes
        public List<ClassTable> GetAssignClasses(bool IsAll, string userId)
        {
            var classes = new List<ClassTable>();
            if (IsAll == true)
            {
                classes = db.ClassTables.ToList();
            }
            else
            {
                classes = db.ClassTables.Where(c => c.InsAdminId == userId).ToList();
            }
            return classes;
        }
        #endregion

        #region Change Class Status by Id
        //public bool ChangeClassStatusById(ClassResponse model)
        //{
        //    var old = db.ClassTables.Where(c => c.ClassId == model.ClassId).FirstOrDefault();
        //    if (old != null)
        //    {
        //        old.Status = model.Status;
        //    }
        //    return db.SaveChanges() > 0;
        //}
        #endregion

        #region Get All Active Classes
        public List<ClassTable> GetAllActiveClasses(string role, string userId)
        {
            if (role == Constants.SuperAdmin)
            {
                return db.ClassTables.Where(c => c.Status == "Active").ToList();
            }
            return db.ClassTables.Where(c => c.Status == "Active" && c.InsAdminId == userId).ToList();

        }
        #endregion

        #region Administrator and Instractor List
        public SelectList AdminOrInstractorSelectList()
        {
            // Create select list
            var selListItem = new SelectListItem() { Value = "", Text = "-- Select --" };
            var newList = new List<SelectListItem> { selListItem };

            var users = (from a in db.UserDetails
                         where a.IsDeleted.Value == false
                         && a.AspNetUser.AspNetRoles.Where(r => r.Name ==Constants.Administrators || r.Name == Constants.Instructors).Count() > 0
                         select a).ToList(); ;

            foreach (var u in users)
            {
                newList.Add(new SelectListItem() { Value = u.UserId.ToString(), Text = u.FirstName + " " + u.LastName });
            }
            return new SelectList(newList, "Value", "Text", null);
        }
        #endregion

        #region Get Class AvailableDays by ClassId

        public List<AvailableDaysId> GetAvailableDaysByClassId(int classId)
        {
            var listAvailableDays = new List<AvailableDaysId>();
            var Ids = db.ClassTables.Where(d => d.ClassId == classId).FirstOrDefault();
            var availableDaysIds = Ids != null ? Ids.DaysOfWeek.Split(',') : null;
            if (availableDaysIds != null)
            {
                foreach (var id in availableDaysIds)
                {
                    listAvailableDays.Add(new AvailableDaysId()
                    {
                        Id = Convert.ToInt32(id)
                    });
                }
            }
            return listAvailableDays;
        }
        #endregion

        #region Class List for client
        public SelectList ClassSelectListByClientId(int clientId)
        {
            // Create select list
            var classlist = new SelectListItem() { Value = "", Text = "-- Select --" };
            var newList = new List<SelectListItem> { classlist };

            var clientClass = (from a in db.ClassTables
                               join c in db.ClientClasses on a.ClassId equals c.ClassId
                               where c.ClientId == clientId
                               select a).ToList(); ;

            foreach (var c in clientClass)
            {
                newList.Add(new SelectListItem() { Value = c.ClassId.ToString(), Text = c.ClassName });
            }
            return new SelectList(newList, "Value", "Text", null);
        }
        #endregion

        #region Get Class Details by ClassId

        public ClassDetails GetClassDetailsByClassId(int classId)
        {
            var classModel = new ClassDetails();

            var data = db.ClassTables.Where(c => c.ClassId == classId).FirstOrDefault();
            if (data != null)
            {
                var instractor = db.UserDetails.Where(c => c.UserId == data.InsAdminId).FirstOrDefault();
                var DaysName = "";
                if (data.DaysOfWeek != null)
                {
                    var availableDays = db.AvailableDays.Where(c => data.DaysOfWeek.Contains(c.Id.ToString())).ToList();

                    foreach (var d in availableDays)
                    {
                        if (DaysName != "") { DaysName = DaysName + ", " + d.DayName; }
                        else { DaysName = d.DayName; }
                    }
                }

                classModel.ClassId = data.ClassId;
                classModel.ClassName = data.ClassName;
                classModel.LocationName = data.LocationName;
                classModel.Address = data.Address;
                classModel.ClassDescription = data.ClassDescription;
                classModel.Status = data.Status;
                classModel.AvailableDays = DaysName;
                classModel.InstructorName = instractor != null ? instractor.FirstName + " " + instractor.LastName : "";
                classModel.ClassFees = data.ClassFees;
            }

            return classModel;
        }
        #endregion

        #region Get All Assign Classes by ClientId
        public List<ClassTable> GetAssignClassesByClientId(int ClientId)
        {
            var classes = new List<ClassTable>();

            classes = (from c in db.ClassTables
                       join clc in db.ClientClasses on c.ClassId equals clc.ClassId
                       where clc.ClientId == ClientId
                       select c
                     ).ToList();
            return classes;
        }
        #endregion

        #region Get User Class Id by UserId
        public List<int> GetUserClassIdByUserId(string userId)
        {
            return db.ClassTables.Where(c => c.InsAdminId == userId)?.Select(cc => cc.ClassId).ToList();
        }
        #endregion

        #region Assign Classes to User
        public bool AssignClassesToUser(string userId, string assignClassIds)
        {
            var result = false;
            // remove old assign classes
            var old = db.ClassTables.Where(c => c.InsAdminId == userId).ToList();
            foreach (var c in old)
            {
                c.InsAdminId = null;
                db.SaveChanges();
                result = true;
            }
            // add current assign classes
            string[] classIdsArr = null;
            if (!string.IsNullOrEmpty(assignClassIds))
                classIdsArr = assignClassIds.Split(',');
           
            if (classIdsArr != null)
            {
                var allClasses = db.ClassTables.Where(c=> classIdsArr.Contains(c.ClassId.ToString())).ToList();
                foreach (var cid in classIdsArr)
                {
                    var data = allClasses.Where(c => c.ClassId == Convert.ToInt32(cid)).FirstOrDefault();
                    if(data!=null)
                    {
                        data.InsAdminId = userId;
                        db.SaveChanges();
                        result = true;
                    }
                }
            }
            return result;
        }
        #endregion

        #region Get Class Name by ClassId

        public string GetClassNameByClassId(int classId)
        {
            var className = "";
            var data = db.ClassTables.Where(c => c.ClassId == classId).FirstOrDefault();
            if (data != null)
            {
                className = data.ClassName;
            }

            return className;
        }
        #endregion
    }
}