using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SMGJ.Models;
namespace SMGJ.Controllers
{
    public class QumeshtiDetajetController : BaseController
    {
        SMGJDB db = new SMGJDB();
        // GET: QumeshtiDetajet
        public async Task<ActionResult> Index()
        {
            var user = await GetUser();
            List<QUMESHTI_DETAJET> model;
            if (user.ID == 2)
            {
                model = db.QUMESHTI_DETAJET.Where(qd => qd.KrijuarNga == user.ID).ToList();
            }
            else
            {
                model = db.QUMESHTI_DETAJET.ToList();
            }
            return View(model);
        }

        // GET: QumeshtiDetajet/Details/5
        public ActionResult Details(int id)
        {
            
            return View();
        }

        // GET: QumeshtiDetajet/Create
        public async Task<ActionResult> Create()
        {
            var user = await GetUser();
            if(user.RoleID == 1)
                return RedirectToAction("Index");
            return View();
        }

        // POST: QumeshtiDetajet/Create
        [HttpPost]
        public async Task<ActionResult> Create(QUMESHTI_DETAJET model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            //returnmodel.ContentType = "text/plain";
            QUMESHTI_DETAJET new_model = new QUMESHTI_DETAJET();
            if (ModelState.IsValid)
            { 
                try
                {
                    decimal totali = 0;
                    try
                    {
                        totali = db.QUMESHTIs.Where(q => q.DataProdhimit == model.DataProdhimit && q.KrijuarNga == user.ID).Sum(q => q.SasiaProdhuar);
                        
                        if (totali - model.Humbjet - model.UshqimViqave< 0)
                        {
                            returnmodel.status = false;
                            returnmodel.Mesazhi = "Totali neto nuk është valid sepse është negativ!\n (" + (totali - model.Humbjet - model.UshqimViqave).ToString()+")";
                            returnmodel.totali = totali;
                            return Json(returnmodel, JsonRequestBehavior.AllowGet);
                        }

                    }
                    catch
                    {
                        returnmodel.status = false;
                        returnmodel.Mesazhi = "Nuk ka qumësht për këtë ditë!";
                        returnmodel.totali = 0;
                        return Json(returnmodel, JsonRequestBehavior.AllowGet);
                    }

                    new_model.FermaID = db.FERMAs.Where(f => f.KrijuarNga == user.ID).Select(f => f.ID).Single();
                    var test = db.QUMESHTI_DETAJET.Where(qd => qd.DataProdhimit == model.DataProdhimit).FirstOrDefault();
                    if(test != null)
                    {
                        returnmodel.status = false;
                        returnmodel.Mesazhi = "Detajet e ditës për qumësht ekzistojnë në databazë!";
                        returnmodel.totali = 0;
                        return Json(returnmodel, JsonRequestBehavior.AllowGet);
                    }
                    new_model.DataProdhimit = model.DataProdhimit;
                    new_model.UshqimViqave = model.UshqimViqave;
                    new_model.Humbjet = model.Humbjet;
                    new_model.TotalLitra = totali - model.Humbjet - model.UshqimViqave;
                    new_model.Krijuar = DateTime.Now;
                    new_model.KrijuarNga = user.ID;

                    //new_model.KrijuarNga = user.ID;
                    //if (result)
                    //{
                    //    new_model.KrijuarNga = model.KrijuarNga;
                    //}
                    //else
                    //{
                    //    new_model.KrijuarNga = user.ID;
                    //}
                    //new_model.Krijuar = DateTime.Now;
                    db.QUMESHTI_DETAJET.Add(new_model);
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "Detajet e qumështit u regjistruan me sukses!";
                    return Json(returnmodel, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    returnmodel.status = false;
                    returnmodel.Mesazhi = "Ka ndodhur nje gabim!";
                    return Json(returnmodel, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                returnmodel.status = false;
                returnmodel.Mesazhi = "Modeli nuk eshte valid!";
                return Json(returnmodel, JsonRequestBehavior.DenyGet);
            }
        }


        //    try
        //    {
        //        // TODO: Add insert logic here
        //        var user = await GetUser();
        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        // GET: QumeshtiDetajet/Edit/5
        public async Task<ActionResult> Edit(int?id)
        {
            var user = await GetUser();
            return View();
        }

        // POST: QumeshtiDetajet/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(QUMESHTI_DETAJET model)
        {
            var user = await GetUser();
            try
            {

                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: QumeshtiDetajet/Delete/5
        //public async Task<ActionResult> Delete(QUMESHTI_DETAJET model)
        //{
        //    var user = await GetUser();
        //    return View();
        //}

        // POST: QumeshtiDetajet/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(QUMESHTI_DETAJET model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            try
            {
                QUMESHTI_DETAJET qdetajet = db.QUMESHTI_DETAJET.Find(model.ID);
                db.QUMESHTI_DETAJET.Remove(qdetajet);
                await db.SaveChangesAsync();
                returnmodel.status = true;
                returnmodel.Mesazhi = "Detajet jane fshire!";
                return Json(returnmodel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                returnmodel.status = false;
                returnmodel.Mesazhi = "Ka ndodhur nje gabim";
                return Json(returnmodel, JsonRequestBehavior.DenyGet);
            }
        }


        [HttpPost]

        public async Task<ActionResult> LlogaritTotalin(QUMESHTI_DETAJET model)
        {
            var user = await GetUser();
            
            MessageJs returnmodel = new MessageJs();
            //int totali = 0;
            //decimal totali = (from q in db.QUMESHTIs where q.DataProdhimit == model.DataProdhimit select (q.SasiaProdhuar)).Sum();
            try
            {
                
                decimal totali = db.QUMESHTIs.Where(q => q.DataProdhimit == model.DataProdhimit && q.KrijuarNga == user.ID).Sum(q => q.SasiaProdhuar);
                if (totali < 0)
                {
                    returnmodel.status = false;
                    returnmodel.Mesazhi = "Totali neto nuk është valid sepse është negativ!" + totali.ToString();
                    returnmodel.totali = totali;
                }
                returnmodel.status = true;
                returnmodel.Mesazhi = "Totali bruto për ditë e dhënë është: " + totali.ToString();
                returnmodel.totali = totali;


                return Json(returnmodel, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                returnmodel.status = false;
                returnmodel.Mesazhi = "Nuk ka qumësht për këtë ditë!";
                returnmodel.totali = 0;
                return Json(returnmodel, JsonRequestBehavior.AllowGet);
            }
            
            
            
        }
    }
}
