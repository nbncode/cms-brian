using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CMS_Brian.Models.Admin.Class
{
    #region Response
    #region ClsRoster
    public class ClsRoster
    {
        public int? RosterId { get; set; }
        [Required]
        public int? ClassId { get; set; }
        [Required]
        public int? ClientId { get; set; }
        [Required]
        public int? JudgeId { get; set; }
        [Required]
        public DateTime? StartDate { get; set; }
        public string Note { get; set; }
        public string ClassName { get; set; }
        public string ClientName { get; set; }
    }
    #endregion

    public class ClsClassRoster
    {
        public int RosterId { get; set; }
        public string ClassName { get; set; }
        public string ClientName { get; set; }
        public string DateOfBirth { get; set; }
        public string JudgeName { get; set; }
        public DateTime? StartDate { get; set; }
        public decimal? Fees { get; set; }
        public string PaidAmount { get; set; }
        public string Note { get; set; }
        public int ClientClassId { get; set; }
        public int ClassId { get; set; }
        public int ClientId { get; set; }
    }
    #endregion
    public class RepoClassRoster
    {
        #region Variables
        CMSBrianEntities db = new CMSBrianEntities();
        #endregion

        #region Get All Roster Class By ClassId
        public List<ClsClassRoster> GetAllRosterClassByClassId(int classId)
        {
            var Rosterlist = new List<ClsClassRoster>();
            var rosters = db.ClassRosters.Where(r => r.ClassId == classId).ToList();
            var data = (from c in db.ClientClasses
                        join cl in db.Clients on c.ClientId equals cl.ClientId
                        where c.ClassId == classId
                        select new ClsClassRoster()
                        {
                            ClassName = c.ClassId != null ? c.ClassTable.ClassName : "",
                            ClientName = cl.FirstName + " " + cl.LastName,
                            DateOfBirth = cl.DateofBirth,
                            ClassId = c.ClassId ?? 0,
                            ClientId = cl.ClientId,
                            ClientClassId = c.Id,
                            Fees= c.ClassId != null ? c.ClassTable.ClassFees : null,
                            JudgeName = cl.JudgeId!=null?cl.Judge.JudgeName:"",
                        }).ToList();
            var payments = db.ClientPaymentLogs.Where(p => p.ClassId == classId).ToList();
            foreach (var item in data)
            {
                var roster = rosters.Where(r => r.ClientId == item.ClientId && r.ClassId == item.ClassId).FirstOrDefault();
                var paidAmount = payments.Where(p => p.ClientId == item.ClientId)?.Sum(p=>p.PaidAmount);
              
                Rosterlist.Add(
                    new ClsClassRoster()
                    {
                        RosterId = roster != null ? roster.RosterId : 0,
                        ClassName = item.ClassName,
                        ClientName = item.ClientName,
                        DateOfBirth = item.DateOfBirth,
                        JudgeName = item.JudgeName,
                        StartDate = roster != null ? roster.StartDate:null,
                        Note = roster != null ? roster.Note : "",
                        ClientClassId = item.ClientClassId,
                        ClassId = item.ClassId,
                        ClientId = item.ClientId,
                        PaidAmount = paidAmount!=null?paidAmount.ToString():"",
                        Fees=item.Fees,
                    });
            }

            return Rosterlist;
        }
        #endregion

        #region Get Class Roster by RosterId

        public ClsRoster GetClassRosterByRosterId(int rosterId)
        {
            var Model = new ClsRoster();
            var data = db.ClassRosters.Where(c => c.RosterId == rosterId).FirstOrDefault();
            if (data != null)
            {
                Model.RosterId = data.RosterId;
                Model.ClassId = data.ClassId;
                Model.JudgeId = data.JudgeId;
                Model.StartDate = data.StartDate;
                Model.ClientId = data.ClientId;
                Model.Note = data.Note;
            }

            return Model;
        }
        #endregion

        #region Add Or Update Class Roster
        public bool AddOrUpdateClassRoster(ClsRoster model)
        {
            var old = db.ClassRosters.Where(c => c.RosterId == model.RosterId).FirstOrDefault();
            if (old != null)
            {
                old.ClassId = model.ClassId;
                old.JudgeId = model.JudgeId;
                old.ClientId = model.ClientId;
                old.StartDate = model.StartDate;
                old.Note = model.Note;
            }
            else
            {
                db.ClassRosters.Add(new ClassRoster()
                {
                    ClassId = model.ClassId,
                    JudgeId = model.JudgeId,
                    ClientId = model.ClientId,
                    StartDate = model.StartDate,
                    Note = model.Note,
                });
            }
            return db.SaveChanges() > 0;
        }
        #endregion
    }
}