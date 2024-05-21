using CMS_Brian.Models.Admin.UserResponse;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CMS_Brian.Models.Admin.AvailableDays
{
    public class RepoAvailableDays
    {
        #region Variables
        CMSBrianEntities db = new CMSBrianEntities();
        #endregion

        #region Add Or Update Users Available Days
        public bool AddOrUpdateUserAvailableDays(clsAvailableDays model)
        {
            var availableDaysIds = model.AvailableDaysIds != null ? model.AvailableDaysIds.Split(',') : null;
            
                var listAvailableDays = new List<UserAvailableDay>();
                var availDays = db.UserAvailableDays.Where(x => x.UserId == model.UserId).ToList();
                if (availDays.Count > 0)
                {
                    db.UserAvailableDays.RemoveRange(availDays);
                }
                if (availableDaysIds != null)
                {
                    foreach (var id in availableDaysIds)
                    {
                        listAvailableDays.Add(new UserAvailableDay()
                        {
                            UserId = model.UserId,
                            AvailableDayId = Convert.ToInt32(id)
                        });
                    }
                }
                db.UserAvailableDays.AddRange(listAvailableDays);
           
            return db.SaveChanges() > 0;
        }
        #endregion

        #region Get AvailableDays by UserId

        public List<UserAvailableDay> GetAvailableDaysByUserId(string userId)
        {
            return db.UserAvailableDays.Where(d => d.UserId == userId).ToList();
        }
        #endregion

        #region Get All AvailableDays

        public List<AvailableDay> GetAllAvailableDays()
        {
            return db.AvailableDays.ToList();
        }
        #endregion
       
    }
}