using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CMS_Brian.Models.PaymentLog
{
    public class PaymentResponse
    {
        public int PaymentLogId { get; set; }
        public string ClassName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PaymentMode { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? Fees { get; set; }
        public DateTime? PaymentDate { get; set; }
        public int ClassId { get; set; }
        public int ClientId { get; set; }
    }
    #region Cls paymentLog
    public class ClsPaymentLog
    {
        public int? PaymentLogId { get; set; }
        public int? ClassId { get; set; }
        [Required]
        public int? ClientId { get; set; }
        [Required]
        public string PaymentMode { get; set; }
        [Required]
        public Nullable<decimal> PaidAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Description { get; set; }
    }
    #endregion
}