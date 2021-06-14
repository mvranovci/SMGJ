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
    public class MENUController : BaseController
    {
        SMGJDB db = new SMGJDB();
        // GET: MENU
        public async Task<ActionResult> Index()
        {
            var user = await GetUser();
            List<MENU> model = db.MENUs.ToList();
            return View(model);
        }
        public async Task<ActionResult> Create()
        {
            var user = await GetUser();
            MENU model = new MENU();
            return View(model);
        }
        // DeleteMenu
        [HttpPost]
        public async Task<ActionResult> DeleteMenu(MENU model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            try
            {
                MENU menu =   db.MENUs.Find(model.ID);
                var exist = db.MENU_ROLI.Any(q => q.MenuID == model.ID);
                if(exist)
                {
                    returnmodel.status = false;
                    returnmodel.Mesazhi = "Nuk mund te fshihet menu-ja, egziston tek rolet"; 
                    return Json(returnmodel, JsonRequestBehavior.DenyGet);

                }
                db.MENUs.Remove(menu);
                await db.SaveChangesAsync();
                returnmodel.status = true;
                returnmodel.Mesazhi = "Menu-ja eshte fshire nga sistemi";
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
            MENU model = new MENU();
            if (id != null)
            {
                model = db.MENUs.Find(id.Value);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Create(MENU model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            if (ModelState.IsValid)
            {
                try
                {
                    MENU new_model = new MENU();  

                    new_model.Controller = model.Controller;
                    new_model.Action_Metoda = model.Action_Metoda;
                    new_model.Aktiv = model.Aktiv;
                    new_model.Emertimi = model.Emertimi;
                    new_model.Renditja = model.Renditja;
                    db.MENUs.Add(new_model);
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "Menu-ja u regjistrua me sukses";
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
        public async Task<ActionResult> Edit(MENU model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            if (ModelState.IsValid)
            {
                try
                {
                    MENU new_model = db.MENUs.Find(model.ID);

                    new_model.Controller = model.Controller;
                    new_model.Action_Metoda = model.Action_Metoda;
                    new_model.Aktiv = model.Aktiv;
                    new_model.Emertimi = model.Emertimi;
                    new_model.Renditja = model.Renditja;
                    //bone update
                    db.Entry(new_model).State = EntityState.Modified;
                    //ruaj te dhenat
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "Menu-ja u editua me sukses";
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