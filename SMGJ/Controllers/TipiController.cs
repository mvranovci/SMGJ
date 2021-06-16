using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using SMGJ.Models;

namespace SMGJ.Controllers
{
    public class TipiController : BaseController
    {
        SMGJDB db = new SMGJDB();
        // GET: Tipi
        public ActionResult Index()
        {
            List<TIPI> model = db.TIPIs.ToList();
            return View(model);
        }

        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new TIPI());
            else
                    return View(db.TIPIs.Where(x => x.ID == id).FirstOrDefault<TIPI>());
        }

        [HttpPost]
        public async Task<ActionResult> AddOrEdit(TIPI emp)
        {
            var user = await GetUser();
            TIPI tip = new TIPI();
            if(emp.ID != 0)
            {
                tip = db.TIPIs.Find(emp.ID);
            }
            tip.Emertimi = emp.Emertimi;
            tip.Krijuar = DateTime.Now;
            tip.KrijuarNga = user.ID;
                if (emp.ID == 0)
                {
                    db.TIPIs.Add(tip);
                    db.SaveChanges();
                    return Json(new { success = true}, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    db.Entry(tip).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true}, JsonRequestBehavior.AllowGet);
                }


        }
        [HttpPost]
        public async Task<ActionResult> Delete(TIPI model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            try
            {
                TIPI tipi = db.TIPIs.Find(model.ID);
                db.TIPIs.Remove(tipi);
                await db.SaveChangesAsync();
                returnmodel.status = true;
                returnmodel.Mesazhi = "DELETED FROM DATABASE";
                return Json(returnmodel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                returnmodel.status = false;
                returnmodel.Mesazhi = "Ka ndodhur nje gabim";
                return Json(returnmodel, JsonRequestBehavior.DenyGet);
            }
        }

    }
}