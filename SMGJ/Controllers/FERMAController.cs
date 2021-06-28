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
        // GET: Ferma
        public async Task<ActionResult> Index()
        {
            var user = await GetUser();
            List<FERMA> model = db.FERMAs.ToList();
            ViewBag.KomunaID = await loadKomuna(null);

            bool result = User.IsInRole("Administrator");
            if (!result)
            {
                if (!hasFarm(user.ID))
                {
                    return View("~/Views/FERMA/Create.cshtml");
                }
                else
                {
                    // return View("Edit","Ferma");
                     return RedirectToAction("Edit", "Ferma");
                    // return View("~/Views/FERMA/Edit.cshtml");
                }
            }
            else
            {
                return View(model);
            }
    }

    [NoDirectAccess]
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

                var u = db.USERs.Find(ferma.KrijuarNga);
                u.FermaID = null;
                db.Entry(u).State = EntityState.Modified;
                db.FERMAs.Remove(ferma);

                await db.SaveChangesAsync();
                returnmodel.status = true;
                returnmodel.Mesazhi = "Ferma eshte fshire me sukses!";
                return Json(returnmodel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                returnmodel.status = false;
                returnmodel.Mesazhi = "Ka ndodhur nje gabim!";
                return Json(returnmodel, JsonRequestBehavior.DenyGet);
            }
    }

    [NoDirectAccess]
    public async Task<ActionResult> Edit(int? id)
    {
        var user = await GetUser();
        bool result = User.IsInRole("Administrator");

            if (!result)
            {

                var model = db.FERMAs.Where(x => x.KrijuarNga == user.ID).Single();
                
                ViewBag.KomunaID = await loadKomuna(model.KomunaID);
                ViewBag.UserID = await loadUser(model.UserID);

                return View(model);
            }
            else
            {
                FERMA model = new FERMA();
                if (id != null)
                {
                    model = db.FERMAs.Find(id.Value);

                    

                }

                ViewBag.UserID = await loadUser(model.UserID);
                ViewBag.KomunaID = await loadKomuna(model.KomunaID);
                return View(model);
            }

        }
    [HttpPost]
    public async Task<ActionResult> Create(FERMA model)
    {

      
        bool result = User.IsInRole("Administrator");
        var user = await GetUser();
        MessageJs returnmodel = new MessageJs();

        var exists = db.FERMAs.Any(t => t.Emri.ToLower().Trim() == model.Emri.ToLower().Trim());
        var existsKrijuar = db.FERMAs.Any(t => t.KrijuarNga == user.ID);
         if (exists ||  existsKrijuar)
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
                if (result)
                    {
                        new_model.KrijuarNga = model.KrijuarNga;
                    }
                    else {
                        new_model.KrijuarNga = user.ID;
                    }
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
        
            if (exists && db.FERMAs.Find(model.ID).Emri.ToLower().Trim() != model.Emri.ToLower().Trim())
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
