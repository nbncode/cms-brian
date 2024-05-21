using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS_Brian.Models.Admin.Class
{
    public class clsAssignClientToJudge
    {
        public int JudgeId { get; set; }
        public List<int> ClientIds { get; set; }
        public string UserName { get; set; }
    }
    public class clsAssignClassToInstructor
    {
        public string UserId { get; set; }
        public List<int> ClassIds { get; set; }
        public string UserName { get; set; }
    }
    public class clsAddClientClass
    {
        public int ClientId { get; set; }
        public List<int> ClassIds { get; set; }
        public string ClientName { get; set; }
    }

    public class clsAddClassClients
    {
        public int ClassId { get; set; }
        public List<int> ClientIds { get; set; }
        public string ClassName { get; set; }
    }

    public class RepoClientClass
    {
        #region Variables
        CMSBrianEntities db = new CMSBrianEntities();
        #endregion

        #region Get Client Class Id by UserId
        public List<int> GetClientClassIdByClientId(int clientId)
        {
            return db.ClientClasses.Where(c => c.ClientId == clientId).Select(cc => cc.ClassId??0).ToList();
        }
        #endregion

        #region Add or update Class for Client
        public bool AddOrUpdateClassForClient(List<ClientClass> clientClasses, int clientId)
        {
            var result = false;

            // First check class already assign remove existing classes
            var existingClientClass = db.ClientClasses.Where(se => se.ClientId == clientId).ToList();

            // Filter already assign classes
            var sameClassIds = (from old in existingClientClass
                               join newcl in clientClasses on old.ClassId equals newcl.ClassId
                               select old.ClassId).ToList();

            existingClientClass = existingClientClass.Where(e=> !sameClassIds.Contains(e.ClassId)).ToList();

            clientClasses = clientClasses.Where(e => !sameClassIds.Contains(e.ClassId)).ToList();

            if (existingClientClass?.Count() > 0)
            {
                db.ClientClasses.RemoveRange(existingClientClass);
                db.SaveChanges();
                result = true;
            }

            if (clientClasses.Count > 0)
            {
                // Then save the selected one
                db.ClientClasses.AddRange(clientClasses);
                result = db.SaveChanges() > 0;
            }

            return result;
        }
        #endregion

        #region Get Client Id list by ClassId
        public List<int> GetClientListByClassId(int classId)
        {
            return db.ClientClasses.Where(c => c.ClassId == classId).Select(cc => cc.ClientId ?? 0).ToList();
        }
        #endregion

        #region Add or update Clients for Class
        public bool AddOrUpdateClientsForClass(List<ClientClass> clientClasses, int classId)
        {
            var result = false;

            // First check class already assign remove existing classes
            var existingClientClass = db.ClientClasses.Where(se => se.ClassId == classId).ToList();

            // Filter already assign client
            var sameClientIds = (from old in existingClientClass
                                join newcl in clientClasses on old.ClientId equals newcl.ClientId
                                select old.ClientId).ToList();

            existingClientClass = existingClientClass.Where(e => !sameClientIds.Contains(e.ClientId)).ToList();

            clientClasses = clientClasses.Where(e => !sameClientIds.Contains(e.ClientId)).ToList();

            if (existingClientClass?.Count() > 0)
            {
                db.ClientClasses.RemoveRange(existingClientClass);
                db.SaveChanges();
                result = true;
            }

            if (clientClasses.Count > 0)
            {
                // Then save the selected one
                db.ClientClasses.AddRange(clientClasses);
                result = db.SaveChanges() > 0;
            }

            return result;
        }
        #endregion
    }
}