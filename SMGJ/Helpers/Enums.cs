using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SMGJ.Helpers
{
    public class Enums
    {
        public enum Gjinia
        {
            [Description("Mashkull ")]
            Mashkull = 1,
            [Description("Femer")]
            Femer = 2,
            [Description("Asnje")]
            Asnje = 3

        }

        public enum Roli
        {
            
            Administrator = 1,        
            Fermer = 2

        }
    }
}