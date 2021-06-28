using SMGJ.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SMGJ.Helpers;
using static SMGJ.Helpers.Enums;
using System.Threading;
using System.Threading.Tasks;


namespace SMGJ.Controllers
{
    public class GJEDHI_PARAMETRATController : BaseController
    {
        SMGJDB db = new SMGJDB();
        // GET: GJEDHI_PARAMETRAT

        public async Task<ActionResult> Index(int? id)
        {
            var user = await GetUser();
            bool result = User.IsInRole("Administrator");
            if (!result)
            {
                if (id != null)
                {
                    ViewBag.emriGjedha = db.GJEDHIs.Find(id).Emri;
                    TempData["GjedhiId"] = id;
                    var ListaRaportev = db.GJEDHAT_PARAMETRAT.Where(g => g.GjedhiID == id).ToList();
                    return View(ListaRaportev);
                }
                else
                {
                    TempData["GjedhiId"] = 0;
                    var ListaRaportev = db.GJEDHAT_PARAMETRAT.Where(g => g.KrijuarNga == user.ID).ToList();
                    return View(ListaRaportev);//nese nuk u selectu Id e gjedhit per Rolin Fermer
                }
            }
            else
            {
                if (id != null)
                {
                    ViewBag.emriGjedha = db.GJEDHIs.Find(id).Emri;
                    TempData["GjedhiId"] = id;
                    var ListaRaportev = db.GJEDHAT_PARAMETRAT.Where(g => g.GjedhiID == id).ToList();
                    return View(ListaRaportev);
                }
                else
                {
                    TempData["GjedhiId"] = 0;
                    ViewBag.GjedhiID = await loadGjedhi(null);
                    var model = db.GJEDHAT_PARAMETRAT.ToList();
                    return View(model);
                }
            }
        }

        public async Task<ActionResult> Create(int? id)
        {
            var user = await GetUser();
            GJEDHAT_PARAMETRAT model = new GJEDHAT_PARAMETRAT();
            if (id == 0 || id == null)
            {
                TempData["GjedhiId"] = 0;
                ViewBag.GjedhiID = await loadGjedhi(null);
            }
            else
            {
                TempData["GjedhiId"] = id;
            }
            
            ViewBag.GjedhiID = await loadPrindi(null);
            ViewBag.LageshtiaID = await loadLageshtia(null);
            ViewBag.TemperaturaID = await loadTemperatura(null);
            ViewBag.RrahjeteZemresID = await loadRrahjetEZemres(null);


            return View(model);
                

        }

        [HttpPost]
        public async Task<ActionResult> Create(GJEDHAT_PARAMETRAT model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            if (ModelState.IsValid)
            {
                try
                {
                    GJEDHAT_PARAMETRAT new_model = new GJEDHAT_PARAMETRAT();

                    new_model.GjedhiID = model.GjedhiID;
                    new_model.LageshtiaID = model.LageshtiaID;
                    new_model.TemperaturaID = model.TemperaturaID;
                    new_model.RrahjeteZemresID = model.RrahjeteZemresID;
                    new_model.DataMatjes = model.DataMatjes;
                    new_model.Krijuar = DateTime.Now;
                    new_model.KrijuarNga = user.ID;

                    db.GJEDHAT_PARAMETRAT.Add(new_model);
                    await db.SaveChangesAsync();


                    if (new_model.LageshtiaID > 60 || new_model.LageshtiaID < 40)
                    {
                        NJOFTIMET re_model = new NJOFTIMET();
                        re_model.Gjedhi_ParametriID = new_model.ID;
                        re_model.Data = DateTime.Now;
                        re_model.Mesazhi = "Lageshtia e ajrit eshte jashte normales";
                        db.NJOFTIMETs.Add(re_model);
                        await db.SaveChangesAsync();
                        NJOFTIMET_USER njofto = new NJOFTIMET_USER();
                        njofto.NjoftimetID = re_model.ID;
                        
                        var user1 = (from a in db.USERs where a.RoleID == (int)Roli.Administrator select a).FirstOrDefault();
                        njofto.PrejID = user1.ID;
                        njofto.TekID = user.ID;
                        db.NJOFTIMET_USER.Add(njofto);
                        await db.SaveChangesAsync();
                        string subjekti = "Anomali ne shendetin e gjedhit";
                        string body = "<p>I/e nderuar,</p><p> Lageshtia e ajrit eshte jashte normales per gjedhin me ID:" + new_model.GjedhiID + "  </p>" +
                            "Lageshtia e ajrit eshte   " + new_model.LageshtiaID;

                        var threadDergoEmail = new Thread(() => SendMail.DergoEmail(user1.Email, subjekti, body)); threadDergoEmail.Start();

                    }
                    if (new_model.RrahjeteZemresID > 9 || new_model.RrahjeteZemresID < 6)
                    {
                        NJOFTIMET re_model = new NJOFTIMET();
                        re_model.Gjedhi_ParametriID = new_model.ID;
                        re_model.Data = DateTime.Now;
                        re_model.Mesazhi = "Rrahjet e zemres jane jashte normales";
                        db.NJOFTIMETs.Add(re_model);
                        await db.SaveChangesAsync();
                        NJOFTIMET_USER njofto = new NJOFTIMET_USER();
                        njofto.NjoftimetID = re_model.ID;
                        var vlera = (from a in db.RRAHJET_ZEMRES where a.ID == model.RrahjeteZemresID select a.Vlera).FirstOrDefault();
                        

                        var user1 = (from a in db.USERs where a.RoleID == (int)Roli.Administrator select a).FirstOrDefault();
                        njofto.PrejID = user1.ID;
                        njofto.TekID = user.ID;
                        db.NJOFTIMET_USER.Add(njofto);
                        await db.SaveChangesAsync();
                        string subjekti = "Anomali ne shendetin e gjedhit";
                        string body = "<p>I/e nderuar,</p> <p> Rrahjet e zemres jane jashte normales per gjedhin me ID:" + new_model.GjedhiID + "  </p>" +
                            "Rrahjet e zemres jane "  + vlera;

                        var threadDergoEmail = new Thread(() => SendMail.DergoEmail(user1.Email, subjekti, body)); threadDergoEmail.Start();
                    }
                    if (new_model.TemperaturaID > 5 || new_model.TemperaturaID < 3)
                    {
                        NJOFTIMET re_model = new NJOFTIMET();
                        re_model.Gjedhi_ParametriID = new_model.ID;
                        re_model.Data = DateTime.Now;
                        re_model.Mesazhi = "Temperatura e gjedhit eshte jashte normales";
                        db.NJOFTIMETs.Add(re_model);
                        NJOFTIMET_USER njofto = new NJOFTIMET_USER();
                        njofto.NjoftimetID = re_model.ID;
                        var vlera1 = (from a in db.TEMPERATURAs where a.ID == model.TemperaturaID select a.Vlera).FirstOrDefault();
                        var user1 = (from a in db.USERs where a.RoleID == (int)Roli.Administrator select a).FirstOrDefault();
                        njofto.PrejID = user1.ID;
                        njofto.TekID = user.ID;
                        db.NJOFTIMET_USER.Add(njofto);
                        await db.SaveChangesAsync();
                        string subjekti = "Anomali ne shendetin e gjedhit";
                        string body = "<p>I/e nderuar,</p><p> Temperatura eshte jashte normales per gjedhin me ID:" + new_model.GjedhiID + "  </p>" +
                            "Temperatura eshte" + vlera1;

                        var threadDergoEmail = new Thread(() => SendMail.DergoEmail(user1.Email, subjekti, body)); threadDergoEmail.Start();
                    }

                    await db.SaveChangesAsync();
                    
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "Parametrat per gjedhin u regjistruan me sukses";
                    TempData["GjedhiId"] = model.GjedhiID;
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
            GJEDHAT_PARAMETRAT model = new GJEDHAT_PARAMETRAT();
            if (id != null)
            {
                model = db.GJEDHAT_PARAMETRAT.Find(id.Value);
                ViewBag.GjedhiID = await loadGjedhi(null);
                ViewBag.LageshtiaID = await loadLageshtia(null);
                ViewBag.TemperaturaID = await loadTemperatura(null);
                ViewBag.RrahjeteZemresID = await loadRrahjetEZemres(null);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(GJEDHAT_PARAMETRAT model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            if (ModelState.IsValid)
            {
                try
                {
                    GJEDHAT_PARAMETRAT new_model = db.GJEDHAT_PARAMETRAT.Find(model.ID);

                    new_model.GjedhiID = model.GjedhiID;
                    new_model.LageshtiaID = model.LageshtiaID;
                    new_model.TemperaturaID = model.TemperaturaID;
                    new_model.RrahjeteZemresID = model.RrahjeteZemresID;
                    new_model.DataMatjes = model.DataMatjes;
                    new_model.Krijuar = DateTime.Now;
                    new_model.KrijuarNga = user.ID;
                    //bone update
                    db.Entry(new_model).State = EntityState.Modified;
                    //ruaj te dhenat
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "Parametrat e gjedhit u edituan me sukses";
                    return Json(returnmodel, JsonRequestBehavior.AllowGet);
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

        public async Task<ActionResult> DeleteParametrat(GJEDHAT_PARAMETRAT model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            try
            {
                GJEDHAT_PARAMETRAT gjedhiparametrat = db.GJEDHAT_PARAMETRAT.Find(model.ID);
                /*  var u = db.USERs.Find(ferma.KrijuarNga);
                  u.FermaID = null;
                  db.Entry(u).State = EntityState.Modified;*/
                db.GJEDHAT_PARAMETRAT.Remove(gjedhiparametrat);
                await db.SaveChangesAsync();
                returnmodel.status = true;
                returnmodel.Mesazhi = "Parametrat e gjedhit jane fshire me sukses!";
                return Json(returnmodel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                returnmodel.status = false;
                returnmodel.Mesazhi = "Ka ndodhur nje gabim!";
                return Json(returnmodel, JsonRequestBehavior.DenyGet);
            }
        }
    }


}
