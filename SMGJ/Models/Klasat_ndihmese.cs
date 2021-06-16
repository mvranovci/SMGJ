using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMGJ.Models
{
     public class Klasat_ndihmese
    {

    }
    public class menu_roli
    {
        public int RoleID { get; set; }
        public IList<MENU> menu
        {
            get; set;
        }
       
    }
    public class Menute
    {
       public string Url { get; set; }
       public string Pershkrimi { get; set; }

        public string css { get; set; }
    }


}
