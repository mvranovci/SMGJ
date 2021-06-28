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
    public class LAGESHTIA_AJRITController : BaseController
    {
        SMGJDB db = new SMGJDB();

        public async Task<ActionResult> Index()
        {
            var user = await GetUser();
            List<LAGESHTIA_AJRIT> model = db.LAGESHTIA_AJRIT.ToList();
            return View(model);
        }
        public async Task<ActionResult> Create()
        {
            var user = await GetUser();
            LAGESHTIA_AJRIT model = new LAGESHTIA_AJRIT();
            return View();
        }
        // DeleteMenu
        [HttpPost]
        public async Task<ActionResult> DeleteRecord(LAGESHTIA_AJRIT model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            try
            {
                LAGESHTIA_AJRIT lageshtia = db.LAGESHTIA_AJRIT.Find(model.ID);


                var exists = db.GJEDHAT_PARAMETRAT.Where(e => e.LageshtiaID == model.ID).Any();
                if (exists)
                {
                    returnmodel.status = false;
                    returnmodel.Mesazhi = "Nuk mund ta fshini kete lageshti te ajrit, sepse ekzistojne Gjedha qe permbajne kete parameter ";
                    return Json(returnmodel, JsonRequestBehavior.DenyGet);
                }


                db.LAGESHTIA_AJRIT.Remove(lageshtia);
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
            LAGESHTIA_AJRIT model = new LAGESHTIA_AJRIT();
            if (id != null)
            {
                model = db.LAGESHTIA_AJRIT.Find(id.Value);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Create(LAGESHTIA_AJRIT model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            if (ModelState.IsValid)
            {
                try
                {
                    LAGESHTIA_AJRIT new_model = new LAGESHTIA_AJRIT();

                    new_model.Vlera = model.Vlera;
                    new_model.Krijuar = DateTime.Now;
                    new_model.KrijuarNga = user.ID;


                    var exists = db.LAGESHTIA_AJRIT.Where(e => e.Vlera == model.Vlera).Any();
                    if (exists)
                    {
                        returnmodel.status = false;
                        returnmodel.Mesazhi = "Keto vlera te lageshtise se ajrit ekzistojne";
                        return Json(returnmodel, JsonRequestBehavior.DenyGet);
                    }
                    db.LAGESHTIA_AJRIT.Add(new_model);
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "Te dhenat u regjistruan me sukses";
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
        public async Task<ActionResult> Edit(LAGESHTIA_AJRIT model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            if (ModelState.IsValid)
            {


                try
                {
                    LAGESHTIA_AJRIT new_model = db.LAGESHTIA_AJRIT.Find(model.ID);

                    new_model.ID = model.ID;
                    new_model.Vlera = model.Vlera;
                    new_model.Krijuar = DateTime.Now;
                    new_model.KrijuarNga = user.ID;
                    var exist = db.LAGESHTIA_AJRIT.Where(e => e.Vlera == model.Vlera && e.ID != model.ID).Any();
                    if (exist)
                    {
                        returnmodel.status = false;
                        returnmodel.Mesazhi = "Keto vlera te lageshtise se ajrit ekzistojne";
                        return Json(returnmodel, JsonRequestBehavior.DenyGet);
                    }
                    var exists = db.GJEDHAT_PARAMETRAT.Where(e => e.LageshtiaID == model.ID).Any();
                    if (exists)
                    {
                        returnmodel.status = false;
                        returnmodel.Mesazhi = "Nuk mund te ndryshoni kete lageshti te ajrit, sepse ekziston nje e tille";
                        return Json(returnmodel, JsonRequestBehavior.DenyGet);
                    }

                    db.Entry(new_model).State = EntityState.Modified;
                    //ruaj te dhenat
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "Keto vlera te lageshtise se ajrit u ndryshuan me sukses";
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
