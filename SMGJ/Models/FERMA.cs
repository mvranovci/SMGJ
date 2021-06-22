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
    
    public partial class FERMA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FERMA()
        {
            this.GJEDHIs = new HashSet<GJEDHI>();
            this.QUMESHTI_DETAJET = new HashSet<QUMESHTI_DETAJET>();
            this.USERs = new HashSet<USER>();
        }
    
        public int ID { get; set; }
        public string Emri { get; set; }
        public Nullable<int> KomunaID { get; set; }
        public Nullable<System.DateTime> Krijuar { get; set; }
        public Nullable<int> KrijuarNga { get; set; }
       
    
        public virtual KOMUNA KOMUNA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GJEDHI> GJEDHIs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QUMESHTI_DETAJET> QUMESHTI_DETAJET { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USER> USERs { get; set; }
        public int? UserID { get; internal set; }
    }
}
