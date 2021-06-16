using SMGJ.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static SMGJ.Helpers.Enums;

namespace SMGJ.Controllers
{
    public class USERController : BaseController
    {
        // GET: USER
        SMGJDB db = new SMGJDB();
        public async Task<ActionResult> Index()
        {
            var user = await GetUser();
            RegisterViewModel model = new RegisterViewModel();
            List<USER> users = new List<USER>();
            users = await db.USERs.ToListAsync();


            var userList = UserManager.Users.ToList();
            var allRoles = (new ApplicationDbContext()).Roles.OrderBy(q => q.Name).ToList().Select(q => new SelectListItem { Value = q.Id, Text = q.Name }).ToList();

            IEnumerable<Perdoruesit> resoursesfiltered = (from a in users
                                                          join b in userList on a.UserId equals b.Id
                                                          join c in allRoles on a.RoleID equals int.Parse(c.Value)
                                                          join i in db.KOMUNAs on a.KomunaID equals i.ID
                                                          select new Perdoruesit
                                                          {
                                                              AktivNeInstitucion = a.Aktiv.HasValue? a.Aktiv.Value : false,
                                                              Email = a.Email,
                                                              Perdoruesi = a.Emri + " " + a.Mbiemri,
                                                              Institucioni = i.Emri, 
                                                              User = b.UserName,
                                                              RoliKryesor = c.Text,
                                                              UserID = a.UserId,
                                                              ID = a.ID,
                                                              InstitucioniID = a.KomunaID, 
                                                              RoliKryesorID = c.Value
                                                          }).AsEnumerable();

            
            ViewBag.user = user;
            ViewBag.RoliPerdoruesit = user.RoleID;
            return View(resoursesfiltered);
        }
        public async Task<ActionResult> PasswordReset(int id)
        {
            USER user = await db.USERs.FindAsync(id);
            ChangePasswordViewModel model = new ChangePasswordViewModel();
            model.ID = user.ID;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PasswordReset_POST([Bind(Include = "ID,NewPassword,ConfirmPassword")] ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userID = await db.USERs.FindAsync(model.ID);
                    var user = await UserManager.FindByIdAsync(userID.UserId);
                    var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    var result = await UserManager.ResetPasswordAsync(user.Id, token, model.NewPassword);
                    if (result.Succeeded)
                    { 
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    
                }
            } 
            return RedirectToAction("Index");

        }
        public async Task<ActionResult> Edit(int? id)
        {
            var userlogged = await GetUser();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USER user = await db.USERs.FindAsync(id.Value);
            if (user == null)
            {
                return HttpNotFound();
            }
            Editimi m = new Editimi(); 
            m.Email = user.Email;
            if(user.Datelindja.HasValue)
            
            {
                m.Ditelindja = user.Datelindja; 
            }
            m.Emri = user.Emri;
            m.Mbiemri = user.Mbiemri;
            m.NumriPersonal = user.NrLeternjoftimit;
            m.Telefoni = user.NrTelefonit;
            ViewBag.check = user.Aktiv == true ? "checked" : "";
            var userauth = await UserManager.FindByIdAsync(user.UserId);
            m.UserName = userauth.UserName;
            m.ID = user.ID; 
            var allRoles = (new ApplicationDbContext()).Roles.OrderBy(q => q.Name).ToList(); 
            ViewBag.RoleID = new SelectList(allRoles, "Id", "Name", selectedValue: user.RoleID); 
            ViewBag.InstitucioniID = await loadKomuna(user.KomunaID);
            var gjinia = EnumsToSelectList<Gjinia>.GetSelectList(); 
            var modeli = new List<SelectListItem>();
            foreach (var item in gjinia)
            { 
                if(user.Gjinia != null)
                {
                    int vlera = int.Parse(item.Value); 
                    if (Convert.ToBoolean(vlera) == user.Gjinia)
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
            ViewBag.RoleUserID = user.RoleID;
            ViewBag.Gjinia = modeli;
            return View(m);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Editimi model)
        {
            MessageJs returnmodel = new MessageJs();
            if (ModelState.IsValid)
            {
                try
                {
                    USER useri = await db.USERs.FindAsync(model.ID);
                    string roliVjeterID = useri.RoleID.ToString();
                    var user = await UserManager.FindByIdAsync(useri.UserId);
                    if (user.UserName.ToLower().Trim() != model.UserName.ToLower().Trim())
                    {
                        user.UserName = model.UserName;
                        await UserManager.UpdateAsync(user);
                    }
                     
                    //modifikimi tek USER
                    useri.Emri = model.Emri;
                    useri.Mbiemri = model.Mbiemri;
                    useri.Datelindja = model.Ditelindja;
                    useri.NrLeternjoftimit = model.NumriPersonal; 
                    useri.NrTelefonit = model.Telefoni; 
                    useri.RoleID = model.RoleID;
                    useri.Email = model.Email;
                    useri.KomunaID = model.InstitucioniID; 
                    useri.Aktiv = model.Statusi;
                    useri.Gjinia = Convert.ToBoolean(model.Gjinia);
                    db.Entry(useri).State = EntityState.Modified; 
                    await db.SaveChangesAsync();
                    if (roliVjeterID.ToString() != model.RoleID.ToString())
                    {
                        var rolefound = (new ApplicationDbContext()).Roles.FirstOrDefault(q => q.Id == roliVjeterID);
                        await UserManager.RemoveFromRoleAsync(useri.UserId, rolefound.Name.ToString());
                        //vendosja e rolit te ri
                        string roliRiID = model.RoleID.ToString();
                        var newrolefound = (new ApplicationDbContext()).Roles.FirstOrDefault(q => q.Id == roliRiID);
                        await UserManager.AddToRoleAsync(useri.UserId, newrolefound.Name.ToString());
                        await db.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                { 
                }
                returnmodel.status = true;
                return Json(returnmodel, JsonRequestBehavior.AllowGet);
            }

            var userlogged = await GetUser();
            var allRoles = (new ApplicationDbContext()).Roles.OrderBy(q => q.Name).ToList();
            ViewBag.InstitucioniID = await loadKomuna(model.InstitucioniID); 
            ViewBag.RoleID = new SelectList(allRoles, "Id", "Name", selectedValue: model.RoleID); 
            return View(model);
        }
    }
}