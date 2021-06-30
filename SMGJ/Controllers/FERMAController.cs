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
    [CustomAuthorizeAttribute]
    public class FermaController : BaseController
    {
        SMGJDB db = new SMGJDB();
        // GET: Ferma
        public async Task<ActionResult> Index()
        
        {
            var user = await GetUser();
            List<FERMA> model = db.FERMAs.ToList();
            ViewBag.KomunaID = await loadKomuna(null);

            bool result = User.IsInRole("Administrator");
            if (!result)
            {
                if (hasFarm(user.ID))
                {
                    return RedirectToAction("Edit", "Ferma");

                    //return View("~/Views/FERMA/Edit.cshtml");

                }
                else
                    {
                    return View("~/Views/FERMA/Create.cshtml");
                }
            }
            else
            {
                return View(model);
            }
        }

        public async Task<ActionResult> Create()
        {
            var user = await GetUser();
            FERMA model = new FERMA();
            ViewBag.KomunaID = await loadKomuna(null);
            ViewBag.UserID = await loadUser(null);
            return View(model);
        }


        [HttpPost]
        public async Task<ActionResult> DeleteFerma(FERMA model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            try
            {
                FERMA ferma = db.FERMAs.Find(model.ID);
                var exists = (from gj in db.GJEDHIs where gj.FermaID == model.ID select gj).FirstOrDefault();
                var existsQumsht = (from gj in db.QUMESHTI_DETAJET where gj.FermaID == model.ID select gj).FirstOrDefault();
                if (exists != null || existsQumsht != null)
                {
                    returnmodel.status = false;
                    returnmodel.Mesazhi = "Ka gjedha ne kete ferme!";
                    return Json(returnmodel, JsonRequestBehavior.DenyGet);
                }
                List<USER> lista = (from a in db.USERs where a.FermaID == ferma.ID select a).ToList();
                if (lista.Any())
                {
                    for (int i = 0; i < lista.Count(); i++)
                    {
                        var user_ferma = lista[i];
                        user_ferma.FermaID = null;
                        db.Entry(user_ferma).State = EntityState.Modified;
                    }
                    db.FERMAs.Remove(ferma);
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "Ferma eshte fshire  me sukses!";
                    return Json(returnmodel, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    db.FERMAs.Remove(ferma);
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "Ferma eshte fshire me sukses!";
                    return Json(returnmodel, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                returnmodel.status = false;
                returnmodel.Mesazhi = "Ka ndodhur nje gabim!";
                return Json(returnmodel, JsonRequestBehavior.DenyGet);
            }
        }

        //[NoDirectAccess]
        public async Task<ActionResult> Edit(int? id)
        {
            var user = await GetUser();
            bool result = User.IsInRole("Administrator");

            if (!result)
            {

                var ferma = db.FERMAs.Find(user.FermaID);

                ViewBag.KomunaID = await loadKomuna(ferma.KomunaID);
                // ViewBag.UserID = await loadUser(f);

                return View(ferma);
            }
            else
            {
                FERMA model = new FERMA();
                if(id != null)
                {
                    model = db.FERMAs.Find(id.Value);

                }

                //ViewBag.UserID = await loadUser(model.UserID);
                ViewBag.KomunaID = await loadKomuna(model.KomunaID);
                return View(model);
            }

        }
        [HttpPost]
        public async Task<ActionResult> Create(FERMA model)
        {
            bool result = User.IsInRole("Administrator");
            var user = await GetUser();
            var u = db.USERs.Find(model.KrijuarNga);

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

                    //u.FermaID = model.ID;
                    if (result)
                    {
                        new_model.KrijuarNga = model.KrijuarNga;
                    }
                    else
                    {
                        new_model.KrijuarNga = user.ID;
                    }
                    new_model.Krijuar = DateTime.Now;
                    //db.Entry(new_model).State = EntityState.Modified;

                    db.FERMAs.Add(new_model);
                    await db.SaveChangesAsync();

                    u.FermaID = new_model.ID;
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
            ViewBag.UserID = await loadUser(model.KrijuarNga);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(FERMA model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();

            var exists = db.FERMAs.Any(t => t.Emri.ToLower().Trim() == model.Emri.ToLower().Trim());

            var exists1 = db.FERMAs.Any(t => t.KrijuarNga == user.ID);
            bool result = User.IsInRole("Administrator");
            var existsEmri = db.FERMAs.Find(model.ID).Emri.ToLower().Trim() != model.Emri.ToLower().Trim();

            if (exists && existsEmri)
            {
                returnmodel.status = false;
                returnmodel.Mesazhi = "Nuk mund ta editoni kete ferme, sepse ekziston!";
                return Json(returnmodel, JsonRequestBehavior.DenyGet);
            }
            //if (ModelState.IsValid)
            //{
            if (result)
            {
                try
                {
                    FERMA new_model = db.FERMAs.Find(model.ID);

                    new_model.Emri = model.Emri;
                    new_model.KomunaID = model.KomunaID;
                    new_model.KrijuarNga = model.KrijuarNga;
                    new_model.Krijuar = DateTime.Now;
                    //bone update
                    db.Entry(new_model).State = EntityState.Modified;
                    //ruaj te dhenat
                    ViewBag.KomunaID = await loadKomuna(model.KomunaID);

                    ViewBag.UserID = await loadUser(model.KrijuarNga);
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "Te dhenat e fermes u edituan me sukses";
                    return Json(returnmodel, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    returnmodel.status = false;
                    returnmodel.Mesazhi = "Ka ndodhur nje gabim";
                    return Json(returnmodel, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                try
                {
                    FERMA new_model = db.FERMAs.Find(model.ID);

                    new_model.Emri = model.Emri;
                    new_model.KomunaID = model.KomunaID;
                    new_model.KrijuarNga = user.ID;
                    new_model.Krijuar = DateTime.Now;
                    //bone update
                    ViewBag.KomunaID = await loadKomuna(model.KomunaID);

                    db.Entry(new_model).State = EntityState.Modified;
                    //ruaj te dhenat
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "Te dhenat e fermes u edituan me sukses";
                    return Json(returnmodel, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    returnmodel.status = false;
                    returnmodel.Mesazhi = "Ka ndodhur nje gabim";
                    return Json(returnmodel, JsonRequestBehavior.AllowGet);
                }
            }


            ViewBag.Useri = user.ID;
        }


    }
}
