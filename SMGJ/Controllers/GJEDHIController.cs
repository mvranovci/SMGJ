using SMGJ.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static SMGJ.Helpers.Enums;

namespace SMGJ.Controllers
{
    public class GJEDHIController : BaseController
    {
        // GET: GJEDHI
        SMGJDB db = new SMGJDB();
        // GET: KOMUNA
        public async Task<ActionResult> Index()
        
        {
            var user = await GetUser();           
            var u = db.USERs.Find(user.ID);           
            var ferma = db.FERMAs.ToList();
            try { 
                            var usferm =u.FermaID;

                if (usferm != null)
                { 
                  //  var model = db.GJEDHIs.Where(q => q.KrijuarNga == user.ID).ToList();
                    var model = db.GJEDHIs.Where(q => q.FermaID == u.FermaID).ToList();
                    return View(model);
                }
               
            }
            catch {
                return RedirectToAction("Index", "FERMA");

            }
             return RedirectToAction("Index", "FERMA"); ;            
        }
        public async Task<ActionResult> Create()
        {
            var user = await GetUser();

            var model = new List<SelectListItem>();
            var gjinia = EnumsToSelectList<Gjinia>.GetSelectList();
            foreach (var item in gjinia)
            {
                model.Add(new SelectListItem { Value = item.Value, Text = item.Text, Selected = false });
            }
            ViewBag.Gjinia = model;
            ViewBag.FermaID = await loadFerma(null);
            ViewBag.TipiID = await loadTipi(null);
            ViewBag.RacaID = await loadRaca(null);
            ViewBag.PrindiID = await loadPrindi(null);

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(GJEDHI model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            if (ModelState.IsValid)
            {
                try
                {
                    GJEDHI new_model = new GJEDHI();
                    
                    
                    new_model.Emri = model.Emri;
                    new_model.FermaID = model.FermaID;
                    new_model.RacaID = model.RacaID;
                    new_model.PrindiID = model.PrindiID;
                    new_model.TipiID = model.TipiID;
                    new_model.Datelindja = model.Datelindja;
                    new_model.Gjinia = model.Gjinia;
                    new_model.Vathe = model.Vathe;
                    new_model.Pesha = model.Pesha;
                    new_model.KrijuarNga = user.ID;
                    new_model.Krijuar = DateTime.Now;

                    db.GJEDHIs.Add(new_model);
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
                    Console.WriteLine(returnmodel.status);
                    returnmodel.Mesazhi = "Modeli nuk eshte valid";
                    return Json(returnmodel, JsonRequestBehavior.DenyGet);
                }
            }


        public async Task<ActionResult> Edit(int? id)
        {
            var user = await GetUser();
            GJEDHI model = new GJEDHI();
            ViewBag.FermaID = await loadFerma(null);
            ViewBag.RacaID = await loadRaca(null);
            ViewBag.PrindiID = await loadPrindi(null);
            ViewBag.TipiID = await loadTipi(null);
            if (id != null)
            {
                model = db.GJEDHIs.Find(id.Value);
            }
            var gjinia = EnumsToSelectList<Gjinia>.GetSelectList();
            var modeli = new List<SelectListItem>();
            foreach (var item in gjinia)
            {
                if (model.Gjinia != null)
                {
                    int vlera = int.Parse(item.Value);
                    if (Convert.ToBoolean(vlera) == model.Gjinia)
                    {
                        modeli.Add(new SelectListItem { Value = item.Value, Text = item.Text, Selected = true });
                    }
                    else
                    {
                        modeli.Add(new SelectListItem { Value = item.Value, Text = item.Text, Selected = false });
                    }
                }
                else
                {
                    modeli.Add(new SelectListItem { Value = item.Value, Text = item.Text, Selected = false });
                }
            }
            ViewBag.Gjinia = modeli;
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(GJEDHI model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            var exists = db.GJEDHIs.Any(x => x.Emri == model.Emri);
            if (exists)
            {
                returnmodel.status = false;
                returnmodel.Mesazhi = "Nuk mund ta ndryshoni kete gjedh sepse ekziston!";
                return Json(returnmodel, JsonRequestBehavior.DenyGet);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    GJEDHI new_model = db.GJEDHIs.Find(model.ID);
                    new_model.Emri = model.Emri;
                    new_model.FermaID = model.FermaID;
                    new_model.RacaID = model.RacaID;
                    new_model.TipiID = model.TipiID;
                    //new_model.PrindiID = model.PrindiID;
                    new_model.Datelindja = model.Datelindja;
                    new_model.Vathe = model.Vathe;
                    new_model.Pesha = model.Pesha;
                    new_model.Gjinia = model.Gjinia;
                    new_model.Krijuar = DateTime.Now;
                    new_model.KrijuarNga = user.ID;
                    //bone update
                    db.Entry(new_model).State = EntityState.Modified;
                    //ruaj te dhenat
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "Gjedhi u ndryshua me sukses";
                    return Json(returnmodel, JsonRequestBehavior.AllowGet);
                    //return Json(returnmodel, JsonRequestBehavior.AllowGet);
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
                returnmodel.status = false;
                returnmodel.Mesazhi = "Modeli nuk eshte valid";
                return Json(returnmodel, JsonRequestBehavior.DenyGet);
            }
        }

    }


}