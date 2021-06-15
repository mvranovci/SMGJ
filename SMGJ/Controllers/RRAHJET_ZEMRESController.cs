using SMGJ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Data.Entity;

namespace SMGJ.Controllers
{
    public class RRAHJET_ZEMRESController : BaseController
    {
        SMGJDB db = new SMGJDB();
        // GET: RRAHJET_ZEMRES
        public async Task<ActionResult> Index()
        {
            var user = await GetUser();
            List<RRAHJET_ZEMRES> model = db.RRAHJET_ZEMRES.ToList();
            return View(model);
        }
        public async Task<ActionResult> Create()
        {
            var user = await GetUser();
            RRAHJET_ZEMRES model = new RRAHJET_ZEMRES();
            return View(model);
        }
        //DeleteMenu
        [HttpPost]
        public async Task<ActionResult> DeleteRrahjetZemres(RRAHJET_ZEMRES model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            try
            {
                RRAHJET_ZEMRES rrahjet_zemres = db.RRAHJET_ZEMRES.Find(model.ID);

                if (await db.GJEDHAT_PARAMETRAT.AnyAsync(x => x.RrahjeteZemresID == model.ID))
                {
                    returnmodel.status = false;
                    returnmodel.Mesazhi = "Kete vlere nuk mund ta fshini sepse gjendet ne tabelen GJEDHAT PARAMETRAT!";

                    return Json(returnmodel, JsonRequestBehavior.DenyGet);
                }
                
                db.RRAHJET_ZEMRES.Remove(rrahjet_zemres);
                await db.SaveChangesAsync();
                returnmodel.status = true;
                returnmodel.Mesazhi = "Vlera eshte fshire!";
                return Json(returnmodel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                returnmodel.status = false;
                returnmodel.Mesazhi = "Ka ndodhur nje gabim";
                return Json(returnmodel, JsonRequestBehavior.DenyGet);
            }
        }
        public async Task<ActionResult> Edit(int? id)
        {
            var user = await GetUser();
            RRAHJET_ZEMRES model = new RRAHJET_ZEMRES();
            if (id != null)
            {
                model = db.RRAHJET_ZEMRES.Find(id.Value);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Create(RRAHJET_ZEMRES model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            var exists = db.GJEDHAT_PARAMETRAT.Any(gj => gj.RrahjeteZemresID == model.ID);
            var existsEmertimi = db.RRAHJET_ZEMRES.Any(x => x.Vlera == model.Vlera);
            if (exists || existsEmertimi)
            {
                returnmodel.status = false;
                returnmodel.Mesazhi = "Vlera ekziston te gjedhi parametrat ose te rrahjet e zemres!";
                return Json(returnmodel, JsonRequestBehavior.DenyGet);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    RRAHJET_ZEMRES new_model = new RRAHJET_ZEMRES();

                    new_model.Vlera = model.Vlera;
                    new_model.Krijuar = DateTime.Now;
                    new_model.KrijuarNga = user.ID;

                    db.RRAHJET_ZEMRES.Add(new_model);
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "Rrahjet e zemres u regjistruan me sukses";
                    return Json(returnmodel, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    returnmodel.status = false;
                    returnmodel.Mesazhi = "Ka ndodhur nje gabim";
                    return Json(returnmodel, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                returnmodel.status = false;
                returnmodel.Mesazhi = "Modeli nuk eshte valid";
                return Json(returnmodel, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit(RRAHJET_ZEMRES model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();

            var exists = db.GJEDHAT_PARAMETRAT.Any(gj => gj.RrahjeteZemresID == model.ID);
            var existsEmertimi = db.RRAHJET_ZEMRES.Any(x => x.Vlera == model.Vlera);
            if (exists || existsEmertimi)
            {
                returnmodel.status = false;
                returnmodel.Mesazhi = "Vlera ekziston te gjedhi parametrat ose te rrahjet e zemres!";
                return Json(returnmodel, JsonRequestBehavior.DenyGet);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    RRAHJET_ZEMRES new_model = db.RRAHJET_ZEMRES.Find(model.ID);

                    new_model.Vlera = model.Vlera;
                    new_model.Krijuar = DateTime.Now;
                    new_model.KrijuarNga = user.ID;
                    //bone update
                    db.Entry(new_model).State = EntityState.Modified;
                    //ruaj te dhenat
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "Rrahjet e zemres u edituan me sukses";
                    return Json(returnmodel, JsonRequestBehavior.AllowGet);
                    //return Json(returnmodel, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    returnmodel.status = false;
                    returnmodel.Mesazhi = "Ka ndodhur nje gabim";
                    return Json(returnmodel, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                returnmodel.status = false;
                returnmodel.Mesazhi = "Modeli nuk eshte valid";
                return Json(returnmodel, JsonRequestBehavior.DenyGet);
            }
        }

    }
}