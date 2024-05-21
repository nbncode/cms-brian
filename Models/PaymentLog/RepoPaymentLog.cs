using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CMS_Brian.Models.PaymentLog
{
    public class RepoPaymentLog
    {
        #region Variables
        CMSBrianEntities db = new CMSBrianEntities();
        #endregion

        #region Get All Payment Logs By ClientId
        public List<PaymentResponse> GetAllPaymentLogsByClientId(int clientId)
        {
            var paymentLogslist = new List<PaymentResponse>();

            paymentLogslist = (from p in db.ClientPaymentLogs
                               join cl in db.Clients on p.ClientId equals cl.ClientId
                               where p.ClientId == clientId
                               select new PaymentResponse()
                               {
                                   ClassName = p.ClassId != null ? db.ClassTables.Where(c => c.ClassId == p.ClassId).Select(c => c.ClassName).FirstOrDefault() : "",
                                   FirstName = cl.FirstName,
                                   LastName = cl.LastName,
                                   ClassId = p.ClassId ?? 0,
                                   ClientId = cl.ClientId,
                                   PaymentMode = p.PaymentMode,
                                   PaidAmount = p.PaidAmount,
                                   PaymentDate =p.CreatedDate,
                                   PaymentLogId = p.PaymentLogId,
                                   Fees = p.ClassId != null ? db.ClassTables.Where(c => c.ClassId == p.ClassId).Select(c => c.ClassFees).FirstOrDefault() : null,
                               }).ToList();

            return paymentLogslist;
        }
        #endregion

        #region Get payment log by PaymentLogId

        public ClsPaymentLog GetPaymentLogId(int paymentLogId)
        {
            var Model = new ClsPaymentLog();
            var data = db.ClientPaymentLogs.Where(c => c.PaymentLogId == paymentLogId).FirstOrDefault();
            if (data != null)
            {
                Model.PaymentLogId = data.PaymentLogId;
                Model.ClassId = data.ClassId;
                Model.ClientId = data.ClientId;
                Model.PaymentMode = data.PaymentMode;
                Model.PaidAmount = data.PaidAmount ?? 0;
                Model.TotalAmount = data.TotalAmount ?? 0;
                Model.Description = data.Description;
                Model.CreatedDate = data.CreatedDate;
                //Model.CreatedDate = Convert.ToDateTime(data.CreatedDate?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
            }

            return Model;
        }
        #endregion

        #region Add Or Update PaymentLog
        public bool AddOrUpdatePaymentLog(ClsPaymentLog model)
        {
            var old = db.ClientPaymentLogs.Where(c => c.PaymentLogId == model.PaymentLogId).FirstOrDefault();
            if (old != null)
            {
                old.ClassId = model.ClassId;
                old.ClientId = model.ClientId;
                old.PaymentMode = model.PaymentMode;
                old.PaidAmount = model.PaidAmount;
                old.TotalAmount = model.TotalAmount;
                old.Description = model.Description;
            }
            else
            {
                db.ClientPaymentLogs.Add(new ClientPaymentLog()
                {
                    ClassId = model.ClassId,
                    ClientId = model.ClientId,
                    PaymentMode = model.PaymentMode,
                    PaidAmount = model.PaidAmount,
                    TotalAmount = model.TotalAmount,
                    Description=model.Description,
                    CreatedDate=DateTime.Now.Date,
                    //CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)),
                });
            }
            return db.SaveChanges() > 0;
        }
        #endregion

        #region Get Total Payment by ClientId

        public decimal GetTotalPaidAmountByClientId(int clientId)
        {
            decimal totalPayment = 0;
            var payments = db.ClientPaymentLogs.Where(p => p.ClientId == clientId).ToList();
            if (payments.Count > 0)
            {
                totalPayment = payments.Sum(p => p.PaidAmount.Value);
            }
            return totalPayment;
        }
        #endregion
    }
}