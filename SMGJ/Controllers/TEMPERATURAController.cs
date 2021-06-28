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
    public class TemperaturaController : BaseController
    {
        SMGJDB db = new SMGJDB();
        // GET: Temperatura
        public async Task<ActionResult> Index()
        {
            var user = await GetUser();
            List<TEMPERATURA> model = db.TEMPERATURAs.ToList();
            return View(model);
        }
        public async Task<ActionResult> Create()
        {
            var user = await GetUser();
            TEMPERATURA model = new TEMPERATURA();
            return View(model);
        }
      
         [HttpPost]
        public async Task<ActionResult> DeleteTemperatura(TEMPERATURA model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();

            try
            {
                TEMPERATURA menu =   db.TEMPERATURAs.Find(model.ID);
                var exist = db.GJEDHAT_PARAMETRAT.Any(q => q.TemperaturaID == model.ID);
                if (exist)
                {
                    returnmodel.status = false;
                    returnmodel.Mesazhi = "Kete nuk mund ta fshini sepse ekziston tek Tabela GjedhiParametrat";
                    return Json(returnmodel, JsonRequestBehavior.DenyGet);
                }

                db.TEMPERATURAs.Remove(menu);
                await db.SaveChangesAsync();
                returnmodel.status = true;
                returnmodel.Mesazhi = "Temperatura eshte fshire me sukses";
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
            TEMPERATURA model = new TEMPERATURA();
            if (id != null)
            {
                model = db.TEMPERATURAs.Find(id.Value);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Create(TEMPERATURA model)
        {

            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();

            var exists = db.TEMPERATURAs.Any(t => t.Vlera == model.Vlera);
            if (exists) { 
                returnmodel.status = false;
                returnmodel.Mesazhi = "Nuk mund ta regjistroni kete temperature, sepse ekziston!";
                return Json(returnmodel, JsonRequestBehavior.DenyGet);
            }

            if (ModelState.IsValid)
            {

                try
                {
                    TEMPERATURA new_model = new TEMPERATURA();  

                    new_model.Vlera = model.Vlera; 
                    new_model.KrijuarNga = user.ID ;
                    new_model.Krijuar = DateTime.Now;
                    db.TEMPERATURAs.Add(new_model);
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "Temperatura u regjistrua me sukses";
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
        public async Task<ActionResult> Edit(TEMPERATURA model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();

            var exists = db.TEMPERATURAs.Any(t => t.Vlera == model.Vlera);
            if (exists)
            {
                returnmodel.status = false;
                returnmodel.Mesazhi = "Nuk mund ta regjistroni kete temperature, sepse ekziston!";
                return Json(returnmodel, JsonRequestBehavior.DenyGet);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    TEMPERATURA new_model = db.TEMPERATURAs.Find(model.ID);

                    new_model.Vlera = model.Vlera;
                    new_model.KrijuarNga = user.ID;
                    new_model.Krijuar = DateTime.Now;
                    //bone update
                    db.Entry(new_model).State = EntityState.Modified;
                    //ruaj te dhenat
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "Temperatura u editua me sukses";
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
