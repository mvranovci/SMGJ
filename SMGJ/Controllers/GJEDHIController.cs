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
    public class GjedhiController : BaseController
    {
        SMGJDB db = new SMGJDB();
        // GET: Gjedhi
        public async Task<ActionResult> Index()
        {
            var user = await GetUser();
            
            //Pyesim se a ka ky perdorues ferme
            if (!hasFarm(user.ID))
                return RedirectToAction("Create", "Ferma");

            //var ferma = (from f in db.FERMAs where f.KrijuarNga == user.ID select f);
            //var gjedhi = (from k in db.GJEDHIs select k).AsEnumerable();
            //var emriferma = db.FERMAs.Where(x => x.KrijuarNga == user.ID).SingleOrDefault();
            //ViewBag.emriferma = emriferma.Emri;

            var gjedhi = (from k in db.GJEDHIs join f in db.FERMAs on k.FermaID equals f.ID where f.KrijuarNga == user.ID select k).AsEnumerable();
            return View(gjedhi);

        }


        [HttpPost]
        public async Task<ActionResult> Delete(GJEDHI model)
        {
            var user = await GetUser();
            if (hasFarm(user.ID))
            {
                MessageJs returnmodel = new MessageJs();
                try
                {
                    GJEDHI gjedhi = db.GJEDHIs.Find(model.ID);
                    //var test = from M in db.MENUs
                    //           join MR in db.MENU_ROLI on M.ID equals MR.MenuID
                    //           select new { ID = M.ID, Emertimi = M.Emertimi, MR_ID = MR.MenuID };

                    var kushti_parametrat = from Gj in db.GJEDHIs
                                            join P in db.GJEDHAT_PARAMETRAT on Gj.ID equals P.GjedhiID
                                            select new { ID = Gj.ID };
                    var z = (from k in kushti_parametrat select k.ID).ToList();

                    if (z.Contains(model.ID))
                    {
                        //Nuk lejohet 
                        returnmodel.Mesazhi = "Ky gjedh nuk mund të fshihet sepse ekzistojnë parametra për këtë gjedh!";
                        returnmodel.status = false;
                        return Json(returnmodel, JsonRequestBehavior.DenyGet);

                    }
                    else
                    {
                        var kushti_qumesht = from Gj in db.GJEDHIs
                                             join Q in db.QUMESHTIs on Gj.ID equals Q.GjedhiID
                                             select new { ID = Gj.ID };
                        var z1 = (from k in kushti_qumesht select k.ID).ToList();
                        if (z1.Contains(model.ID))
                        {
                            //Nuk lejohet 
                            returnmodel.Mesazhi = "Ky gjedh nuk mund të fshihet sepse ekzistojnë qumështa për këtë gjedh!";
                            returnmodel.status = false;
                            return Json(returnmodel, JsonRequestBehavior.DenyGet);
                        }
                        else
                        {
                            //Lejohet
                            db.GJEDHIs.Remove(gjedhi);
                            await db.SaveChangesAsync();
                            returnmodel.status = true;
                            return Json(returnmodel, JsonRequestBehavior.AllowGet);
                        }

                    }
                }
                catch (Exception)
                {
                    returnmodel.status = false;
                    returnmodel.Mesazhi = "Ka ndodhur nje gabim";
                    return Json(returnmodel, JsonRequestBehavior.DenyGet);
                }
            }
            else
            {
                return RedirectToAction("Create", "Ferma");
            }
        }


        public async Task<ActionResult> Create()
        {
            var user = await GetUser();
            //Pyesim se a ka ky perdorues ferme
            if (!hasFarm(user.ID))
                return RedirectToAction("Create", "Ferma");


            var gjinia = EnumsToSelectList<Gjinia>.GetSelectList();
            var model = new List<SelectListItem>();
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
            //Pyesim se a ka ky perdorues ferme
            if (!hasFarm(user.ID))
                return RedirectToAction("Create", "Ferma");

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


    }//Fundi i klases
}//Fundi i namespace