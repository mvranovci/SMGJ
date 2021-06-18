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
    public class RACAController : BaseController
    {
        SMGJDB db = new SMGJDB();
        // GET: RACA
        public ActionResult Index()
        {
            List<RACA> model = db.RACAs.ToList();
            return View(model);
        }

        [HttpGet]
        public ActionResult ADDorEDIT(int id = 0)
        {
            if (id == 0)
                return View(new RACA());
            else
                return View(db.RACAs.Where(x => x.ID == id).FirstOrDefault<RACA>());
        }

        [HttpPost]
        public async Task<ActionResult> ADDorEDIT(RACA emp)
        {
            var user = await GetUser();
            RACA ra = new RACA();
            if (emp.ID != 0)
            {
                ra = db.RACAs.Find(emp.ID);
            }
            ra.Emertimi = emp.Emertimi;
            ra.Krijuar = DateTime.Now;
            ra.KrijuarNga = user.ID;
            if (emp.ID == 0)
            {
                db.RACAs.Add(ra);
                db.SaveChanges();
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                db.Entry(ra).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }


        }
        [HttpPost]
        public async Task<ActionResult> Delete(RACA model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            try
            {
                RACA RACA = db.RACAs.Find(model.ID);
                db.RACAs.Remove(RACA);
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