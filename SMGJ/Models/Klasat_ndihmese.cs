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
     public class gjedhi_qumshti
    {
        public string   emri { get; set; }
        public int ID { get; set; }
        public decimal sasia { get; set; }

        public string vathi { get; set; }
    }
    public class Menute
    {
       public string Url { get; set; }
       public string Pershkrimi { get; set; }

        public string css { get; set; }
    }

    public class Njoftimet_e_reja
    {
        public string Emri { get; set; }
        public string Mesazhi { get; set; }
        public DateTime Koha { get; set; }
        public int GjedhiId { get; set; }

    }
}
