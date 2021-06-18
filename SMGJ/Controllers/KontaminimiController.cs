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
    public class KONTAMINIMIController : BaseController
    {
        SMGJDB db = new SMGJDB();


        // GET: Kontaminimi
        public async Task<ActionResult> Index()
        {

            
            var user = await GetUser();
            //IEnumerable<KONTAMINIMI> kontaminimi = db.KONTAMINIMIs.AsEnumerable();

            var kontaminimi = (from k in db.KONTAMINIMIs select k).AsEnumerable();
            //var kontaminimi = db.KONTAMINIMIs.ToList();
            return View(kontaminimi);
        }

        public async Task<ActionResult> Create()
        {
            //var x = Url.Content();
            var user = await GetUser();
            KONTAMINIMI model = new KONTAMINIMI();
            var niveliList = EnumsToSelectList<Nivelet_e_kontaminimit>.GetSelectList();
            var lista = new List<SelectListItem>();
            foreach (var item in niveliList)
                lista.Add(new SelectListItem { Value = item.Value, Text = item.Text, Selected = false });
            ViewBag.Nivelet = lista;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(KONTAMINIMI model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            if (ModelState.IsValid)
            {
                try
                {
                    KONTAMINIMI new_model = new KONTAMINIMI();
                    var test = from O_K in db.KONTAMINIMIs
                               where O_K.Niveli == model.Niveli && O_K.Vlera == model.Vlera
                               select new { O_K.ID };
                    if(test.Count() != 0)
                    {
                        returnmodel.Mesazhi = "Këto vlera të kontaminimit ekzistojnë në databazë!";
                        return Json(returnmodel, JsonRequestBehavior.DenyGet);
                    }

                    
                    new_model.KrijuarNga = user.ID;
                    new_model.Krijuar = DateTime.Now;
                    new_model.Vlera = model.Vlera;
                    new_model.Niveli = model.Niveli;

                    db.KONTAMINIMIs.Add(new_model);
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "Kontaminimi regjistrua me sukses";
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
        public async Task<ActionResult> DeleteKont(KONTAMINIMI model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            try
            {
                KONTAMINIMI kontaminimi = db.KONTAMINIMIs.Find(model.ID);
                //var test = from M in db.MENUs
                //           join MR in db.MENU_ROLI on M.ID equals MR.MenuID
                //           select new { ID = M.ID, Emertimi = M.Emertimi, MR_ID = MR.MenuID };

                var test = from K in db.KONTAMINIMIs
                           join Q in db.QUMESHTIs on K.ID equals Q.KontaminimiID
                           select new { ID = K.ID};
                var z = (from t in test select t.ID).ToList();

                if (z.Contains(model.ID))
                {
                    //Nuk lejohet 
                    returnmodel.Mesazhi = "Ky kontamin nuk mund të fshihet sepse ekzistojnë qumështa me këtë kontaminim!";
                    returnmodel.status = false;
                    return Json(returnmodel, JsonRequestBehavior.DenyGet);

                }
                else
                {
                    //Lejohet
                    db.KONTAMINIMIs.Remove(kontaminimi);
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    return Json(returnmodel, JsonRequestBehavior.AllowGet);
                }

                
             

             

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
            KONTAMINIMI model = new KONTAMINIMI();
            //var user = await GetUser();
            //KONTAMINIMI model = new KONTAMINIMI();

            var niveliList = EnumsToSelectList<Nivelet_e_kontaminimit>.GetSelectList();
            var lista = new List<SelectListItem>();
            foreach (var item in niveliList)
                lista.Add(new SelectListItem { Value = item.Value, Text = item.Text, Selected = false });
            ViewBag.Nivelet = lista;
            if (id != null)
            {
                //int x = Int32.Parse(Decrypt(id.Value.ToString()));
                model = db.KONTAMINIMIs.Find(id.Value);
            }
            return View(model);
        }


        public async Task<ActionResult> Edit1(string id)
        {
            var user = await GetUser();
            KONTAMINIMI model = new KONTAMINIMI();



            //var user = await GetUser();
            //KONTAMINIMI model = new KONTAMINIMI();

            var niveliList = EnumsToSelectList<Nivelet_e_kontaminimit>.GetSelectList();
            var lista = new List<SelectListItem>();
            foreach (var item in niveliList)
                lista.Add(new SelectListItem { Value = item.Value, Text = item.Text, Selected = false });
            ViewBag.Nivelet = lista;
            if (id != null)
            {
                int x = Int32.Parse(Decrypt(id));
                model = db.KONTAMINIMIs.Find(x);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(KONTAMINIMI model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            if (ModelState.IsValid)
            {
                try
                {
                    KONTAMINIMI new_model = db.KONTAMINIMIs.Find(model.ID);

                    /*
                     *Ekziston nje qumesht me kete kontaminim keshtu qe nuk mund te ndryshohet 
                     * */
                    var test1 = from K in db.KONTAMINIMIs
                                join Q in db.QUMESHTIs on K.ID equals Q.KontaminimiID
                                select new { ID = K.ID };
                    var z = (from t in test1 select t.ID).ToList();
                    if (z.Contains(model.ID))
                    {
                        returnmodel.Mesazhi = "Ky kontamin nuk mund të ndryshohet sepse ekzistojnë qumështa me këtë kontaminim!";
                        returnmodel.status = false;
                        return Json(returnmodel, JsonRequestBehavior.DenyGet);
                    }


                    var test = from O_K in db.KONTAMINIMIs
                               where O_K.Niveli == model.Niveli && O_K.Vlera == model.Vlera
                               select new { O_K.ID };
                    /*
                     *Ekziston nje kontaminim me keto vlera keshtu qe nuk mund te ndryshohet 
                     * */
                    if (test.Count() != 0)
                    {
                        returnmodel.Mesazhi = "Këto vlera të kontaminimit ekzistojnë në databazë!";
                        return Json(returnmodel, JsonRequestBehavior.DenyGet);
                    }

                    
                    //new_model.Controller = model.Controller;
                    //new_model.Action_Metoda = model.Action_Metoda;
                    //new_model.Aktiv = model.Aktiv;
                    //new_model.Emertimi = model.Emertimi;
                    //new_model.Renditja = model.Renditja;
                    //bone update

                    new_model.KrijuarNga = user.ID;
                    new_model.Krijuar = DateTime.Now;
                    new_model.Vlera = model.Vlera;
                    new_model.Niveli = model.Niveli;
                    db.Entry(new_model).State = EntityState.Modified;
                    //ruaj te dhenat
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "Menu-ja u editua me sukses";
                    return Json(returnmodel, JsonRequestBehavior.AllowGet);
                    //return Json(returnmodel, JsonRequestBehavior.AllowGet);
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

    }// End of class
}// End of namespace 