//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CMS_Brian.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Client
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Client()
        {
            this.ClassRosters = new HashSet<ClassRoster>();
            this.ClientPaymentLogs = new HashSet<ClientPaymentLog>();
        }
    
        public int ClientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string DateofBirth { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string SignedForm { get; set; }
        public Nullable<int> JudgeId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClassRoster> ClassRosters { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClientPaymentLog> ClientPaymentLogs { get; set; }
        public virtual Judge Judge { get; set; }
    }
}