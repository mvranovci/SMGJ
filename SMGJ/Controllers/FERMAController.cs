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

            if (!User.IsInRole("Administrator") && hasFarm(user.ID))
            {
                return RedirectToAction("Edit", "Ferma");
            }

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

                if (exists != null)
                {
                    returnmodel.status = false;
                    returnmodel.Mesazhi = "Ka gjedha ne kete ferme!";
                    return Json(returnmodel, JsonRequestBehavior.DenyGet);
                }

                if (existsQumsht != null)
                {
                    returnmodel.status = false;
                    returnmodel.Mesazhi = "Ka qumesht detaje te regjistruar ne kete ferme!";
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
                return View(ferma);
            }
            else
            {
                FERMA model = new FERMA();
                if(id != null)
                {
                    model = db.FERMAs.Find(id.Value);

                }

                ViewBag.KomunaID = await loadKomuna(model.KomunaID);
                return View(model);
            }

        }
        [HttpPost]
        public async Task<ActionResult> Create([Bind(Include = "Emri,KomunaID,KrijuarNga")] FERMA model)
        {
            bool result = User.IsInRole("Administrator");
            var user = await GetUser();

            MessageJs returnmodel = new MessageJs();

            bool exists_ferma = db.FERMAs.Any(t => t.Emri.ToLower().Trim() == model.Emri.ToLower().Trim() && t.KomunaID == model.KomunaID);
            bool exists_ferma_perdoruesi = user.FermaID != null ? true : false;

            int newValue = result ? model.KrijuarNga.Value : user.ID;

            if (exists_ferma)
            {
                returnmodel.status = false;
                returnmodel.Mesazhi = "Nuk mund ta regjistroni kete ferme sepse ekziston nje e tille!";
                return Json(returnmodel, JsonRequestBehavior.DenyGet);
            }
            if (exists_ferma_perdoruesi && !result)
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
                    new_model.ID = model.ID;
                    new_model.Emri = model.Emri;
                    new_model.KomunaID = model.KomunaID;
                    new_model.KrijuarNga = newValue;
                    new_model.Krijuar = DateTime.Now;
                    db.FERMAs.Add(new_model);

                   
                    var user_admin = db.USERs.Find(newValue);
                    user_admin.FermaID = new_model.ID;
                    db.Entry(user_admin).State = EntityState.Modified;

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
                var errors = ModelState.Select(x => x.Value.Errors)
                        .Where(y => y.Count > 0)
                        .ToList();

                returnmodel.status = false;
                returnmodel.Mesazhi = "Modeli nuk eshte valid";
                return Json(returnmodel, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit(FERMA model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();

                try
                {
                    FERMA new_model = db.FERMAs.Find(model.ID);

                    new_model.Emri = model.Emri;
                    new_model.KomunaID = model.KomunaID;
                    new_model.KrijuarNga = model.KrijuarNga != null ? model.KrijuarNga : user.ID;
                    new_model.Krijuar = DateTime.Now;
                    db.Entry(new_model).State = EntityState.Modified;
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

        }


    }

