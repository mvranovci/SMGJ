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
    
    public partial class NJOFTIMET_USER
    {
        public int ID { get; set; }
        public int NjoftimetID { get; set; }
        public int PrejID { get; set; }
        public int TekID { get; set; }
    
        public virtual NJOFTIMET NJOFTIMET { get; set; }
        public virtual USER USER { get; set; }
        public virtual USER USER1 { get; set; }
    }
}
