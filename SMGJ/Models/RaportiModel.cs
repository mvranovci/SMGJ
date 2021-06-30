using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMGJ.Models
{
    public class RaportiModel
    {
        public int Raportet { get; set; }
        public int FermaID { get; set; }
        public DateTime? Prej { get; set; }
        public DateTime? Deri { get; set; }
    }
}