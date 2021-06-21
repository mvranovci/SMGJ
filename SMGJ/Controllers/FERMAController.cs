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
    [Authorize]
    public class FermaController : BaseController
    {
        SMGJDB db = new SMGJDB();
        // GET: Temperatura
        public async Task<ActionResult> Index()
        {
            var user = await GetUser();
            List<FERMA> model = db.FERMAs.ToList();

            return View(model);

        }
        public async Task<ActionResult> Create()
        {
            var user = await GetUser();
            FERMA model = new FERMA();
            ViewBag.KomunaID = await loadKomuna(null);
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteFerma(FERMA model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();

            try
            {
                FERMA menu = db.FERMAs.Find(model.ID);

                db.FERMAs.Remove(menu);
                await db.SaveChangesAsync();
                returnmodel.status = true;
                returnmodel.Mesazhi = "Ferma eshte fshire me sukses";
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
            FERMA model = new FERMA();
            if (id != null)
            {
                model = db.FERMAs.Find(id.Value);
            }
            ViewBag.KomunaID = await loadKomuna(model.KomunaID);
            // ViewBag.InstitucioniID = await loadKomuna(user.KomunaID);
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Create(FERMA model)
        {

            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();

            var exists = db.FERMAs.Any(t => t.Emri == model.Emri);
            var existsKrijuar = db.FERMAs.Any(t => t.KrijuarNga == user.ID);
            if (exists || existsKrijuar)
            {
                returnmodel.status = false;
                returnmodel.Mesazhi = "Nuk mund ta regjistroni kete ferme!";
                return Json(returnmodel, JsonRequestBehavior.DenyGet);
            }


            if (ModelState.IsValid)
            {

                try
                {
                    FERMA new_model = new FERMA();

                    new_model.Emri = model.Emri;
                    new_model.KomunaID = model.KomunaID;
                    new_model.KrijuarNga = user.ID;
                    new_model.Krijuar = DateTime.Now;
                    db.FERMAs.Add(new_model);
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "Ferma u regjistrua me sukses";
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

            ViewBag.KomunaID = await loadKomuna(model.KomunaID);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(FERMA model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();

            var exists = db.FERMAs.Any(t => t.Emri == model.Emri);
            if (exists)
            {
                returnmodel.status = false;
                returnmodel.Mesazhi = "Nuk mund ta editoni kete ferme, sepse ekziston!";
                return Json(returnmodel, JsonRequestBehavior.DenyGet);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    FERMA new_model = db.FERMAs.Find(model.ID);

                    new_model.Emri = model.Emri;
                    new_model.KomunaID = model.KomunaID;
                    new_model.KrijuarNga = user.ID;
                    new_model.Krijuar = DateTime.Now;
                    //bone update
                    db.Entry(new_model).State = EntityState.Modified;
                    //ruaj te dhenat
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "FERMA u editua me sukses";
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
            ViewBag.KomunaID = await loadKomuna(model.KomunaID);
        }

    }
}
