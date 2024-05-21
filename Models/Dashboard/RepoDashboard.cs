using System.Linq;

namespace CMS_Brian.Models.Dashboard
{
    public class RepoDashboard
    {
        #region Variables
        CMSBrianEntities db = new CMSBrianEntities();
        #endregion

        public DashboardModel GetDashboard(DashboardModel model)
        {
            int administrator=0, instructor = 0, judge = 0, client = 0, classes = 0;
            judge = db.Judges.Count();
            client = db.Clients.Count();
            if (model.Role == Constants.SuperAdmin || model.Role == Constants.Administrators)
            {
                administrator = (from a in db.UserDetails
                                 where a.IsDeleted.Value == false
                                 && a.AspNetUser.AspNetRoles.Where(r => r.Name == Constants.Administrators).Count() > 0
                                 select a).Count();
                instructor = (from a in db.UserDetails
                              where a.IsDeleted.Value == false
                              && a.AspNetUser.AspNetRoles.Where(r => r.Name == Constants.Instructors).Count() > 0
                              select a).Count();
                classes = db.ClassTables.Count();
            }
            else if (model.Role == Constants.Instructors)
            {
                classes = db.ClassTables.Where(c => c.InsAdminId == model.UserId).Count();
            }
            // prepare model 
            model.TotalAdministrators = administrator;
            model.TotalInstructors = instructor;
            model.TotalJudges = judge;
            model.TotalClients = client;
            model.TotalClasses = classes;

            return model;
        }
    }
}