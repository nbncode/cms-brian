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
    
    public partial class ClassTable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ClassTable()
        {
            this.ClientClasses = new HashSet<ClientClass>();
        }
    
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string ClassDescription { get; set; }
        public string LocationName { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public string DaysOfWeek { get; set; }
        public string InsAdminId { get; set; }
        public Nullable<decimal> ClassFees { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClientClass> ClientClasses { get; set; }
    }
}