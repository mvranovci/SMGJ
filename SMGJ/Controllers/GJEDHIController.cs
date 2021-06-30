using SMGJ.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SMGJ.Models;
using static SMGJ.Helpers.Enums;

namespace SMGJ.Controllers
{
    [CustomAuthorizeAttribute]
    public class GJEDHIController : BaseController
    {
        SMGJDB db = new SMGJDB();
        MessageJs returnmodel = new MessageJs();
        public async Task<ActionResult> Index()

        {
            var user = await GetUser();
            var u = db.USERs.Find(user.ID);
            var ferma = db.FERMAs.ToList();

            var role_admin = User.IsInRole("Administrator");
            if(role_admin){
                //Nese roli admin return listen me te gjithe gjedhat e te gjitha fermave
                var lista = await db.GJEDHIs.ToListAsync();
                return View(lista);
            }
            try
            {
                var usferm = u.FermaID;

                if (usferm != null)
                {
                    var model = db.GJEDHIs.Where(q => q.FermaID == u.FermaID).ToList();
                    return View(model);
                }
                // USED TO DISPLAY FARM NAME IN INDEX
                var emriferma = db.FERMAs.Where(x => x.KrijuarNga == user.ID).Single();
                ViewBag.emriferma = emriferma.Emri;

                // lista permban listen me gjedhat e perdoruesit te kycur
                var lista = db.GJEDHIs.Where(x => x.FermaID == user.FermaID);
                return View(lista);
            }
            catch
            {
                return RedirectToAction("Index", "FERMA");

            } 
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
            ViewBag.TipiID = await loadTipi(null);
            ViewBag.Roli = user.RoleID;

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(GJEDHI model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();

            var exists = db.GJEDHIs.Any(x => x.Emri.ToLower().Trim() == model.Emri.ToLower().Trim());
            if (exists)
            {
                returnmodel.status = false;
                returnmodel.Mesazhi = "Gjedhi me kete emer ekziston!";
                return Json(returnmodel, JsonRequestBehavior.AllowGet);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    GJEDHI new_model = new GJEDHI();

                    var ferma = db.FERMAs.Find(user.FermaID);

                    new_model.Emri = model.Emri;
                    new_model.RacaID = model.RacaID;
                    new_model.PrindiID = model.PrindiID;
                    if (user.RoleID == 1)
                    {
                        new_model.FermaID = model.FermaID;
                    }
                    else
                    {
                        new_model.FermaID = ferma.ID;
                    }
                    new_model.TipiID = model.TipiID;
                    new_model.PrindiID = model.PrindiID;
                    new_model.Datelindja = model.Datelindja;
                    new_model.Gjinia = model.Gjinia;
                    new_model.Vathe = model.Vathe;
                    new_model.Pesha = model.Pesha;
                    new_model.KrijuarNga = user.ID;
                    new_model.Krijuar = DateTime.Now;
                    new_model.KrijuarNga = user.ID;
                    
                    db.GJEDHIs.Add(new_model);
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "Gjedhi u regjistrua me sukses";
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
            if (user.RoleID == (int)Roli.Fermer)
            {
                if (!hasFarm(user.ID))
                    return RedirectToAction("Create", "Ferma");

                var exists = (from gj in db.GJEDHIs where gj.ID == id select gj).FirstOrDefault();

                if (exists == null)
                    return RedirectToAction("Index", "Gjedhi");

                if (exists.FermaID != user.FermaID)
                    return RedirectToAction("Index", "Gjedhi");

            }
            if (ModelState.IsValid)
            {
                GJEDHI model = db.GJEDHIs.Find(id);
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
                ViewBag.FermaID = await loadFerma(model.FermaID);
                ViewBag.RacaID = await loadRaca(model.RacaID);
                ViewBag.PrindiID = await loadPrindi(model.PrindiID);
                ViewBag.TipiID = await loadTipi(model.TipiID);
                ViewBag.Gjinia = modeli;
                ViewBag.Roli = user.RoleID;

                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "GJEDHI");
            }
                             
        }
      
        [HttpPost]
        public async Task<ActionResult> Edit(GJEDHI model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            var gjedhi = db.GJEDHIs.Find(model.ID);

            var exists = db.GJEDHIs.Any(x => x.Emri.ToLower().Trim() == model.Emri.ToLower().Trim());
            if (exists && db.GJEDHIs.Find(model.ID).Emri.ToLower().Trim() != model.Emri.ToLower().Trim())
            {
                returnmodel.status = false;
                returnmodel.Mesazhi = "Gjedhi me kete emer ekziston!";
                return Json(returnmodel, JsonRequestBehavior.DenyGet);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    GJEDHI new_model = db.GJEDHIs.Find(model.ID);
                    var ferma = db.FERMAs.Find(user.FermaID);
                    
                  
                    new_model.Emri = model.Emri;
                    if (user.RoleID == 1)
                    {
                        new_model.FermaID = model.FermaID;
                    }
                    else
                    {
                        new_model.FermaID = ferma.ID;
                    }
                    new_model.RacaID = model.RacaID;
                    new_model.TipiID = model.TipiID;
                    if (model.PrindiID == model.ID)
                    {
                        returnmodel.status = false;
                        returnmodel.Mesazhi = "Gjedhi nuk mund te jete prind i vetvetes!";
                        return Json(returnmodel, JsonRequestBehavior.DenyGet);
                    }
                    else
                    {
                        new_model.PrindiID = model.PrindiID;
                    }
                    new_model.Datelindja = model.Datelindja;
                    new_model.Vathe = model.Vathe;
                    new_model.Pesha = model.Pesha;
                    new_model.Gjinia = model.Gjinia;
                    //new_model.Krijuar = DateTime.Now;
                    //new_model.KrijuarNga = user.ID;
                    //bone update
                    db.Entry(new_model).State = EntityState.Modified;
                    //ruaj te dhenat
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "Te dhenat e gjedhit u ndryshuan me sukses";
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
                returnmodel.status = false;
                returnmodel.Mesazhi = "Modeli nuk eshte valid";
                return Json(returnmodel, JsonRequestBehavior.DenyGet);
            }
        }
        [HttpPost]
        public async Task<ActionResult> Delete(GJEDHI model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            try
            {
                GJEDHI gjedhi = db.GJEDHIs.Find(model.ID);
                var exists = await db.GJEDHIs.AnyAsync(x => x.PrindiID == model.ID);
                if (exists)
                {
                    returnmodel.status = false;
                    returnmodel.Mesazhi = "Kete gjedh nuk mund ta fshini, sepse eshte prind!";
                    return Json(returnmodel, JsonRequestBehavior.DenyGet);
                }

                db.GJEDHIs.Remove(gjedhi);
                await db.SaveChangesAsync();
                returnmodel.status = true;
                returnmodel.Mesazhi = "Gjedhi eshte fshire!";
                return Json(returnmodel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                returnmodel.status = false;
                returnmodel.Mesazhi = "Ka ndodhur nje gabim";
                return Json(returnmodel, JsonRequestBehavior.DenyGet);
            }
        }

    }


}
