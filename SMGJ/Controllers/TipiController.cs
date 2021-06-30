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
{   [CustomAuthorizeAttribute]
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
            var tipi = new TIPI();
            if (id == 0)
                return View(tipi);
            else
                    return View(db.TIPIs.Where(x => x.ID == id).FirstOrDefault<TIPI>());
        }

        [HttpPost]
        public async Task<ActionResult> AddOrEdit(TIPI emp)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            TIPI tip = new TIPI();
            if (emp.ID != 0)
            {
                tip = db.TIPIs.Find(emp.ID);
            }
            tip.ID = emp.ID;
            tip.Emertimi = emp.Emertimi;
            tip.Krijuar = DateTime.Now;
            tip.KrijuarNga = user.ID;


            var exist = db.TIPIs.Where(e => e.Emertimi.ToLower().Trim() == emp.Emertimi.ToLower().Trim()).Any();
            if (exist)
            {
                returnmodel.status = false;
                returnmodel.Mesazhi = "Këto tipe ekzistojnë!";
                return Json(returnmodel, JsonRequestBehavior.DenyGet);
            }  

            if (emp.ID == 0)
            {
                db.TIPIs.Add(tip);
            }
            else
            {
                db.Entry(tip).State = EntityState.Modified;
            }
                
                await db.SaveChangesAsync();
                returnmodel.status = true;
                returnmodel.Mesazhi = "Të dhënat e tipit u regjistruan me sukses";
                return Json(returnmodel, JsonRequestBehavior.AllowGet);
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
                returnmodel.Mesazhi = "Tipi i gjedhit eshte fshire me sukses";
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
