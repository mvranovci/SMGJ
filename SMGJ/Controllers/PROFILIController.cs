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
using System.Web.Security;
using System.IO;
using System.Text;
using Microsoft.AspNet.Identity.Owin;
using static SMGJ.Controllers.ManageController;

namespace SMGJ.Controllers
{
    public class PROFILIController : BaseController
    {
        SMGJDB db = new SMGJDB();
        MessageJs returnmodel = new MessageJs();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        // GET: PROFILI
        public async Task<ActionResult> Index()
        {
            var user = await GetUser();
            var model = db.USERs.Find(user.ID);
           
            return View(model);
        }
        public async Task<ActionResult> Create()
        {
            var user = await GetUser();
            USER model = new USER();
            return View(model);
        }
        public async Task<ActionResult> Edit()
        {
            var user = await GetUser();
            var model = db.USERs.Find(user.ID);
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
            return PartialView(model);
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

        [AllowAnonymous]
        public ActionResult ResetPassword(string code, string UserId)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = UserManager.FindById(model.UserId);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                //return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Profili");
            }
            AddErrors(result);
            return View();
        }
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        private void AddErrors(IdentityResult result)
        {
            throw new NotImplementedException();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ChangePassword(ProfiliViewModel model)
        //{
        //    var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.psw.OldPassword, model.psw.NewPassword);
        //    if (result.Succeeded)
        //    {
        //        SMGJDB db = new SMGJDB();
        //        var usertoupdate = db.USERs.Find(model.psw.ID);
        //        string password = model.psw.NewPassword;
        //        ASCIIEncoding BinaryPassword = new ASCIIEncoding();
        //        string encrypted = Encrypt(password);
        //        byte[] passwordArray = BinaryPassword.GetBytes(encrypted);
        //        usertoupdate.Password = passwordArray;
        //        db.Entry(usertoupdate).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //        if (user != null)
        //        {
        //            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //        }
        //        ViewBag.Active = "active";
        //        return RedirectToAction("Profili", new { id = model.psw.NewPassword, Message = ManageMessageId.ChangePasswordSuccess });
        //    }
        //    AddErrors(result);
        //    return RedirectToAction("Profili", new { id = model.psw.OldPassword, Message = ManageMessageId.IncorrectPassword });
        //}
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }


    }
}