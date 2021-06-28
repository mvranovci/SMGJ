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
    public class KOMUNAController : BaseController
    {
        SMGJDB db = new SMGJDB();
        // GET: KOMUNA
        public async Task<ActionResult> Index()
        {
            var user = await GetUser();
            List<KOMUNA> model = db.KOMUNAs.ToList();
            return View(model);
        }
        public async Task<ActionResult> Create()
        {
            var user = await GetUser();
            KOMUNA model = new KOMUNA();
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Create(KOMUNA model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();

            var exists = db.KOMUNAs.Any(x => x.Emri.ToLower().Trim() == model.Emri.ToLower().Trim());
            if (exists)
            {
                returnmodel.status = false;
                returnmodel.Mesazhi = "Nuk mund ta regjistroni kete komune, sepse ekziston!";
                return Json(returnmodel, JsonRequestBehavior.AllowGet);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    KOMUNA new_model = new KOMUNA();

                    
                    new_model.Emri = model.Emri;
                    new_model.Kodi = model.Kodi;
                    new_model.Krijuar = DateTime.Now;
                    new_model.KrijuarNga = user.ID;

                    db.KOMUNAs.Add(new_model);
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "Komuna u regjistrua me sukses";
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

        //DeleteMenu
        [HttpPost]
        public async Task<ActionResult> DeleteKomuna(KOMUNA model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            try
            {
                KOMUNA komuna = db.KOMUNAs.Find(model.ID);
                var existsFerma = await db.FERMAs.AnyAsync(x => x.KomunaID == model.ID);
                var existsUsers = await db.USERs.AnyAsync(x => x.KomunaID == model.ID);
                if (existsFerma || existsUsers)
                {
                    returnmodel.status = false;
                    returnmodel.Mesazhi = "Nuk jeni i autorizuar per ta fshire kete komune!";
                    return Json(returnmodel, JsonRequestBehavior.DenyGet);
                }

                db.KOMUNAs.Remove(komuna);
                await db.SaveChangesAsync();
                returnmodel.status = true;
                returnmodel.Mesazhi = "Komuna eshte fshire!";
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
            KOMUNA model = new KOMUNA();
            if (id != null)
            {
                model = db.KOMUNAs.Find(id.Value);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(KOMUNA model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            var exists = db.KOMUNAs.Any(x => x.Emri.ToLower().Trim() == model.Emri.ToLower().Trim());
            if (exists)
            {
                returnmodel.status = false;
                returnmodel.Mesazhi = "Ekziston nje komune me keto te dhena!";
                return Json(returnmodel, JsonRequestBehavior.DenyGet);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    KOMUNA new_model = db.KOMUNAs.Find(model.ID);

                    new_model.Emri = model.Emri;
                    new_model.Kodi = model.Kodi;
                    new_model.Krijuar = DateTime.Now;
                    new_model.KrijuarNga = user.ID;
                    //bone update
                    db.Entry(new_model).State = EntityState.Modified;
                    //ruaj te dhenat
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "Te dhenat e komunes u edituan me sukses";
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