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
    
    public partial class LAGESHTIA_AJRIT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LAGESHTIA_AJRIT()
        {
            this.GJEDHAT_PARAMETRAT = new HashSet<GJEDHAT_PARAMETRAT>();
        }
    
        public int ID { get; set; }
        public Nullable<decimal> Vlera { get; set; }
        public Nullable<System.DateTime> Krijuar { get; set; }
        public Nullable<int> KrijuarNga { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GJEDHAT_PARAMETRAT> GJEDHAT_PARAMETRAT { get; set; }
    }
}
