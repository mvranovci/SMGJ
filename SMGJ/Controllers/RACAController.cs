using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using SMGJ.Models;

namespace SMGJ.Controllers
{
    [CustomAuthorizeAttribute]
    public class RACAController : BaseController
    {
        SMGJDB db = new SMGJDB();
        // GET: RACA
        public ActionResult Index()
        {
            List<RACA> model = db.RACAs.ToList();
            return View(model);
        }
        public async Task<ActionResult> Create()
        {
            var user = await GetUser();
            RACA model = new RACA();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(RACA r)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            RACA ra = new RACA();
            if (r.ID != 0)
            {
                ra = db.RACAs.Find(r.ID);
            }
            ra.Emertimi = r.Emertimi;
            ra.Krijuar = DateTime.Now;
            ra.KrijuarNga = user.ID;

            var exist = db.RACAs.Where(e => e.Emertimi.ToLower().Trim() == r.Emertimi.ToLower().Trim()).Any();
            if (exist)
            {
                returnmodel.status = false;
                returnmodel.Mesazhi = "Këto raca ekzistojnë!";
                return Json(returnmodel, JsonRequestBehavior.DenyGet);
            }

            if (r.ID == 0)
            {
                db.RACAs.Add(ra);
            }
            else
            {
                db.Entry(ra).State = EntityState.Modified;
            }

            await db.SaveChangesAsync();
            returnmodel.status = true;
            returnmodel.Mesazhi = " Raca u regjistrua me sukses";
            return Json(returnmodel, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> DeleteRaca(RACA model)
        {
                var user = await GetUser();
                MessageJs returnmodel = new MessageJs();
                var existsGjedhi = await db.GJEDHIs.AnyAsync(x => x.RacaID == model.ID);

            if (existsGjedhi)
                {
                    returnmodel.status = false;
                    returnmodel.Mesazhi = "Raca e gjedhit nuk mund të fshihet";
                    return Json(returnmodel, JsonRequestBehavior.DenyGet);
                }
                try
                {
                    RACA raca = db.RACAs.Find(model.ID);
                    db.RACAs.Remove(raca);
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "Raca e gjedhit është fshirë me sukses";
                    return Json(returnmodel, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    returnmodel.status = false;
                    returnmodel.Mesazhi = "Ka ndodhur një gabim";
                    return Json(returnmodel, JsonRequestBehavior.DenyGet);
                }
            }


            public async Task<ActionResult> Edit(int? id)
        {
            var user = await GetUser();
            RACA model = new RACA();
            if (id != null)
            {
                model = db.RACAs.Find(id.Value);
            }
            return View(model);
        }

        [HttpPost]

        public async Task<ActionResult> Edit(RACA model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();

            var exists = db.GJEDHIs.Any(x => x.RacaID == model.ID);
            if (exists)
            {
                returnmodel.status = false;
                returnmodel.Mesazhi = "Nuk mund ta ndryshoni, sepse ekziston Gjedh qe e permban kete lloj te races";
                return Json(returnmodel, JsonRequestBehavior.DenyGet);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    RACA new_model = db.RACAs.Find(model.ID);

                    new_model.Emertimi = model.Emertimi;
                    new_model.Krijuar = DateTime.Now;
                    new_model.KrijuarNga = user.ID;
                    db.Entry(new_model).State = EntityState.Modified;
                    //ruaj te dhenat
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "Vlera u ndryshua me sukses";
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



