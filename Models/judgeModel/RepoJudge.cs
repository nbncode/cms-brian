using CMS_Brian.Models.Admin.UserResponse;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace CMS_Brian.Models.judgeModel
{
    public class RepoJudge
    {
        #region Variables
        CMSBrianEntities db = new CMSBrianEntities();
        #endregion

        #region Get All Judges
        public List<Judge> GetAllJudges()
        {
            return db.Judges.ToList();
        }
        #endregion

        #region Get Judge by Id
        public ClsJudge GetjudgeById(int JudgeId)
        {
            var judge = new ClsJudge();

            judge = (from u in db.Judges
                      where u.JudgeId == JudgeId
                      select new ClsJudge()
                      {
                          JudgeId = JudgeId,
                          Email = u.Email,
                          JudgeName = u.JudgeName,
                          PhoneNumber = u.PhoneNumber,
                          Address = u.Address,
                      }).FirstOrDefault();
            return judge;
        }
        #endregion

        #region Update Insert
        public bool AddOrUpdateJudge(Judge model)
        {
            var old = db.Judges.Where(a => a.JudgeId == model.JudgeId).FirstOrDefault();
            if (old == null)
            {
                db.Judges.Add(model);
                return db.SaveChanges() > 0;
            }
            else
            {
                old.Email = model.Email;
                old.Address = model.Address;
                old.PhoneNumber = model.PhoneNumber;
                old.JudgeName = model.JudgeName;
                db.SaveChanges();
                return true;
            }
        }
        #endregion

        #region Delete Judge

        public bool deleteJudge(int JudgeId)
        {
            var old = db.Judges.Where(a => a.JudgeId == JudgeId).FirstOrDefault();
            if (old == null)
            {
                return false;
            }
            db.Judges.Remove(old);
            return db.SaveChanges() > 0;
        }

        #endregion

        #region  Judge List
        public SelectList JudgeSelectList()
        {
            // Create select list
            var selListItem = new SelectListItem() { Value = "", Text = "-- Select --" };
            var newList = new List<SelectListItem> { selListItem };

            var judges = db.Judges.ToList();

            foreach (var u in judges)
            {
                newList.Add(new SelectListItem() { Value = u.JudgeId.ToString(), Text = u.JudgeName});
            }
            return new SelectList(newList, "Value", "Text", null);
        }
        #endregion

        #region Assign Clients To Judge
        public bool AssignClientsToJudge(int judgeId, string assignClientIds)
        {
            var result = false;
            // remove old assign clients
            var old = db.Clients.Where(c => c.JudgeId == judgeId).ToList();
            foreach (var c in old)
            {
                c.JudgeId = null;
                db.SaveChanges();
                result = true;
            }
            // add current assign clients
            string[] clientIdsArr = null;
            if (!string.IsNullOrEmpty(assignClientIds))
                clientIdsArr = assignClientIds.Split(',');

            if (clientIdsArr != null)
            {
                var allClients = db.Clients.Where(c => clientIdsArr.Contains(c.ClientId.ToString())).ToList();
                foreach (var cid in clientIdsArr)
                {
                    var data = allClients.Where(c => c.ClientId == Convert.ToInt32(cid)).FirstOrDefault();
                    if (data != null)
                    {
                        data.JudgeId = judgeId;
                        db.SaveChanges();
                        result = true;
                    }
                }
            }
            return result;
        }
        #endregion
    }
}