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
    
    public partial class NJOFTIMET
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NJOFTIMET()
        {
            this.NJOFTIMET_USER = new HashSet<NJOFTIMET_USER>();
        }
    
        public int ID { get; set; }
        public Nullable<int> Gjedhi_ParametriID { get; set; }
        public Nullable<System.DateTime> Data { get; set; }
        public string Mesazhi { get; set; }
    
        public virtual NJOFTIMET NJOFTIMET1 { get; set; }
        public virtual NJOFTIMET NJOFTIMET2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NJOFTIMET_USER> NJOFTIMET_USER { get; set; }
    }
}
