using SMGJ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SMGJ.Controllers
{
    public class CHARTSController : BaseController
    {
        SMGJDB db = new SMGJDB();
        // GET: CHARTS
        public async Task<ActionResult> Index()
        {
            var tipet = db.TIPIs.ToList();
            Dictionary<string, int> count = new Dictionary<string, int>();
            List<string> emratTipit = new List<string>();
            List<int> values = new List<int>();
            string tipi = "";
            string vlera = "";

            foreach (var item in tipet)
            {
                var gjedhitipi = db.GJEDHIs.Where(g => g.TipiID == item.ID).Count();
                tipi += "'" + item.Emertimi + "',";
                vlera += gjedhitipi + ",";
                values.Add(gjedhitipi);
            }
            ViewBag.KeyValueListTipi = count;

            ViewBag.Tipet = subStringVlera(tipi);
            ViewBag.Vlerat = subStringVlera(vlera);

            return View();

        }
        private string subStringVlera(string tipi)
        {
            tipi = tipi.Substring(0, tipi.Length - 1);
            return tipi;
        }
    }

    }