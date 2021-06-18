﻿using System;
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
            Mashkull = 0,
            [Description("Femer")]
            Femer = 1
        }
        public enum Roli
        { 
            Administrator = 1,        
            Fermer = 2

        }
        
        public enum Nivelet_e_kontaminimit
        {
            [Description("I ulët")]
            I_ulet = 1,
            [Description("I mesëm")]
            I_mesem = 2,
            [Description("I lartë")]
            I_larte = 3,
        }
    }
}
