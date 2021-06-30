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
    public class QumeshtiController : BaseController
    {
        SMGJDB db = new SMGJDB();
        // GET: Qumeshti
        public async Task<ActionResult> Index()
        
        {
            var user = await GetUser();

            ViewBag.GjedhiID = await loadGjedhi1(null);
            ViewBag.YndyraID = await loadYndyra(null);
            ViewBag.KontaminimiID = await loadKontaminimi(null);
            ViewBag.UserID = user.ID;

            var admin = this.User.IsInRole("Administrator");
            if (!admin)
            {
                List<QUMESHTI> model = db.QUMESHTIs.Where(t => t.KrijuarNga == user.ID).ToList();

                return View(model);
            }
            else
            {
                List<QUMESHTI> model = db.QUMESHTIs.ToList();
                return View(model);

            }

        }
        public async Task<ActionResult> Create()
        {
            var user = await GetUser();
            QUMESHTI model = new QUMESHTI();
            ViewBag.GjedhiID = await loadGjedhi1(null);
            ViewBag.YndyraID = await loadYndyra(null);
            ViewBag.KontaminimiID = await loadKontaminimi(null);
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteQumeshti(QUMESHTI model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();

            try
            {
                QUMESHTI menu = db.QUMESHTIs.Find(model.ID);
           
                db.QUMESHTIs.Remove(menu);
                await db.SaveChangesAsync();
                returnmodel.status = true;
                returnmodel.Mesazhi = "Qumeshti eshte fshire me sukses";
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

            if (id == null)
                return HttpNotFound();

            var exists = (from q in db.QUMESHTIs where q.ID == id select q).FirstOrDefault();

            if (exists == null)
                return RedirectToAction("Index", "Qumeshti");


            if (User.IsInRole("Fermer"))
            {
               
                var qumeshti_detajet = (from q in db.QUMESHTIs
                                        join gj in db.GJEDHIs on q.GjedhiID equals gj.ID
                                        join f in db.FERMAs on gj.FermaID equals f.ID
                                        where q.ID == exists.ID
                                        select f.ID).FirstOrDefault();

                if (qumeshti_detajet !=  user.FermaID)
                    return RedirectToAction("Index", "Qumeshti");
            }

            ViewBag.UserID = await loadUser(null);
            QUMESHTI model = db.QUMESHTIs.Find(id);            
            ViewBag.GjedhiID = await loadGjedhi1(model.GjedhiID);
            ViewBag.YndyraID = await loadYndyra(model.YndyraID);
            ViewBag.KontaminimiID = await loadKontaminimi(model.KontaminimiID);
       
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Create(QUMESHTI model)
        {

            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();


            if (ModelState.IsValid)
            {

                try
                {
                    QUMESHTI new_model = new QUMESHTI();

                    new_model.GjedhiID = model.GjedhiID;
                    new_model.YndyraID = model.YndyraID;
                    new_model.KontaminimiID = model.KontaminimiID;
                    new_model.SasiaProdhuar = model.SasiaProdhuar;
                    new_model.DataProdhimit = model.DataProdhimit;
                    new_model.DataSkadimit = model.DataSkadimit;

                    new_model.KrijuarNga = user.ID;
                    new_model.Krijuar = DateTime.Now;
                    db.QUMESHTIs.Add(new_model);
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "Qumeshti u regjistrua me sukses";
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
        public async Task<ActionResult> Edit(QUMESHTI model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            
            if (ModelState.IsValid)
            {
                try
                {
                    QUMESHTI new_model = db.QUMESHTIs.Find(model.ID);

                    new_model.GjedhiID = model.GjedhiID;
                    new_model.YndyraID = model.YndyraID;
                    new_model.KontaminimiID = model.KontaminimiID;
                    new_model.SasiaProdhuar = model.SasiaProdhuar;
                    new_model.DataProdhimit = model.DataProdhimit;
                    new_model.DataSkadimit = model.DataSkadimit;

                    new_model.Krijuar = DateTime.Now;
                    //bone update
                    //ViewBag.GjedhiID = await loadGjedhi1(model.GjedhiID);
                    //ViewBag.YndyraID = await loadYndyra(model.YndyraID);
                    //ViewBag.KontaminimiID = await loadKontaminimi(model.KontaminimiID);
                    db.Entry(new_model).State = EntityState.Modified;
                    //ruaj te dhenat
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "Te dhenat e qumeshtit u ndryshuan me sukses";
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
