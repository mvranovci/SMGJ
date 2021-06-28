using SMGJ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Data.Entity;

namespace SMGJ.Controllers
{   [CustomAuthorizeAttribute]

    public class YNDYRAController : BaseController
    {
        SMGJDB db = new SMGJDB();
        // GET: MENU
        public async Task<ActionResult> Index()
        {
            var user = await GetUser();
            List<YNDYRA> model = db.YNDYRAs.ToList();
            return View(model);
        }
        public async Task<ActionResult> Create()
        {
            var user = await GetUser();
            YNDYRA model = new YNDYRA();
            return View(model);
        }
        // DeleteMenu
        [HttpPost]
        public async Task<ActionResult> DeleteRecord(YNDYRA model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            try
            {
                YNDYRA yndyra = db.YNDYRAs.Find(model.ID);


                var exists = db.QUMESHTIs.Where(e => e.YndyraID == model.ID).Any();
                if (exists)
                {
                    returnmodel.status = false;
                    returnmodel.Mesazhi = "Nuk mund te perfundohet ky veprim";
                    return Json(returnmodel, JsonRequestBehavior.DenyGet);
                }


                db.YNDYRAs.Remove(yndyra);
                await db.SaveChangesAsync();
                returnmodel.status = true;
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
            YNDYRA model = new YNDYRA();
            if (id != null)
            {
                model = db.YNDYRAs.Find(id.Value);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Create(YNDYRA model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            if (ModelState.IsValid)
            {
                try
                {
                    YNDYRA new_model = new YNDYRA
                    {
                        Perqindja = model.Perqindja,
                        Emertimi = model.Emertimi,
                        Krijuar = DateTime.Now,
                        KrijuarNga = user.ID
                    };

                    var exists = db.YNDYRAs.Where(e => e.Emertimi == model.Emertimi).Any();
                    if (exists)
                    {
                        returnmodel.status = false;
                        returnmodel.Mesazhi = "Kjo e dhene ekziston";
                        return Json(returnmodel, JsonRequestBehavior.DenyGet);
                    }
                    db.YNDYRAs.Add(new_model);
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "Recordi u regjistrua me sukses";
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
        public async Task<ActionResult> Edit(YNDYRA model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            if (ModelState.IsValid)
            {
                try
                {
                    YNDYRA new_model = db.YNDYRAs.Find(model.ID);

                    new_model.ID = model.ID;
                    new_model.Perqindja = model.Perqindja;
                    new_model.Emertimi = model.Emertimi;
                    new_model.Krijuar = DateTime.Now;
                    new_model.KrijuarNga = user.ID;
                    var exists = db.QUMESHTIs.Where(e => e.YndyraID == model.ID).Any();
                    if (exists)
                    {
                        returnmodel.status = false;
                        returnmodel.Mesazhi = "Nuk mund te perfundohet ky veprim";
                        return Json(returnmodel, JsonRequestBehavior.DenyGet);
                    }

                    var exist = db.YNDYRAs.Where(e => e.Perqindja == model.Perqindja && e.ID != model.ID).Any();
                    if (exist)
                    {
                        returnmodel.status = false;
                        returnmodel.Mesazhi = "Kjo e dhene ekziston";
                        return Json(returnmodel, JsonRequestBehavior.DenyGet);
                    }
                    //bone update
                    db.Entry(new_model).State = EntityState.Modified;
                    //ruaj te dhenat
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "Yndyra-ja u editua me sukses";
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

    }
}
