using SMGJ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Data.Entity;
using Microsoft.AspNet.Identity; 
using static SMGJ.Helpers.Enums; 

namespace SMGJ.Controllers
{   [CustomAuthorizeAttribute]
    public class PROFILIController : BaseController
    {
        SMGJDB db = new SMGJDB();
        MessageJs returnmodel = new MessageJs(); 
        // GET: PROFILI
        public async Task<ActionResult> Index()
        {
            var user = await GetUser();
            var model = db.USERs.Find(user.ID);
            var modeli = new List<SelectListItem>();
            var gjinia = EnumsToSelectList<Gjinia>.GetSelectList();
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
            ChangePasswordViewModel passwordmodel = new ChangePasswordViewModel();
            ViewBag.passwordmodel = passwordmodel;
            return View(model);
        }
        public async Task<ActionResult> Create()
        {
            var user = await GetUser();
            USER model = new USER();
            return View(model);
        }
        [HttpPost] 
        public async Task<ActionResult> EditPassword(ChangePasswordViewModel model)
        { 
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                returnmodel.status = true;
                returnmodel.Mesazhi = "Fjalekalimi i user-it u editua me sukses";
                return Json(returnmodel, JsonRequestBehavior.AllowGet);
            } 
            returnmodel.status = false;
            returnmodel.Mesazhi = "Ka ndodhur nje gabim";
            return Json(returnmodel, JsonRequestBehavior.AllowGet);
        }
    

        [HttpPost]
        public async Task<ActionResult> Edit(USER model)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            if (ModelState.IsValid)
            {
                try
                {
                    USER new_model = db.USERs.Find(model.ID);

                    new_model.ID = model.ID;
                    new_model.Emri = model.Emri;
                    new_model.Mbiemri = model.Mbiemri;
                    new_model.Email = model.Email;
                    new_model.NrTelefonit = model.NrTelefonit;
                    new_model.Datelindja = model.Datelindja;
                    new_model.Password = model.Password;
                    new_model.Gjinia = model.Gjinia;
                    new_model.RoleID = user.ID; 
                    ViewBag.RoleUserID = user.RoleID; 
                    //bone update
                    db.Entry(new_model).State = EntityState.Modified;
                    //ruaj te dhenat
                    await db.SaveChangesAsync();
                    returnmodel.status = true;
                    returnmodel.Mesazhi = "User u editua me sukses";
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
