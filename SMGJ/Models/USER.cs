//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SMGJ.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class USER
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public USER()
        {
            this.NJOFTIMET_USER = new HashSet<NJOFTIMET_USER>();
            this.NJOFTIMET_USER1 = new HashSet<NJOFTIMET_USER>();
            this.QUMESHTI_DETAJET = new HashSet<QUMESHTI_DETAJET>();
            this.USER_IN_FERMA = new HashSet<USER_IN_FERMA>();
        }
    
        public int ID { get; set; }
        public string UserId { get; set; }
        public string Emri { get; set; }
        public string Mbiemri { get; set; }
        public int RoleID { get; set; }
        public Nullable<System.DateTime> Datelindja { get; set; }
        public string NrTelefonit { get; set; }
        public string NrLeternjoftimit { get; set; }
        public string Email { get; set; }
        public int KomunaID { get; set; }
        public Nullable<int> MediaID { get; set; }
        public Nullable<bool> Aktiv { get; set; }
        public Nullable<bool> NjoftoEmail { get; set; }
    
        public virtual KOMUNA KOMUNA { get; set; }
        public virtual Medium Medium { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NJOFTIMET_USER> NJOFTIMET_USER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NJOFTIMET_USER> NJOFTIMET_USER1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QUMESHTI_DETAJET> QUMESHTI_DETAJET { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USER_IN_FERMA> USER_IN_FERMA { get; set; }
    }
}
