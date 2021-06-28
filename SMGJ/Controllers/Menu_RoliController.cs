using SMGJ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static SMGJ.Helpers.Enums;

namespace SMGJ.Controllers
{   [CustomAuthorizeAttribute]
    public class Menu_RoliController : BaseController
    {
        SMGJDB db = new SMGJDB();
        // GET: Menu_Roli
        // GET: MenuRoli
        public async Task<ActionResult> Index()
        {
            var user = await GetUser();
            ViewBag.MenuID = await loadMenu(null);

            var modeli =(new ApplicationDbContext()).Roles.OrderBy(q=>q.Name).ToList().Select(q=> new SelectListItem{ Value = q.Id, Text=q.Name}).ToList();
            ViewBag.Roli = modeli;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateMenueRoli(menu_roli obj)
        {
            var user = await GetUser();
            MessageJs js = new MessageJs();
            var allmenuRoli = db.MENU_ROLI.ToList();
            try
            {
                foreach (var item in obj.menu)
                {
                    if (allmenuRoli.Any(m => m.RoliID == obj.RoleID && m.MenuID == item.ID))
                    {
                        js.status = false;
                        js.Mesazhi = "Ky rol ekziston";
                        continue;
                    }
                    else
                    {

                        MENU_ROLI mr = new MENU_ROLI();
                        mr.MenuID = item.ID;
                        mr.RoliID = obj.RoleID;
                        mr.KrijuarNga = user.ID;
                        mr.Krijuar = DateTime.Now;
                        db.MENU_ROLI.Add(mr);
                    }
                }
                await db.SaveChangesAsync();
            }
            catch
            {
                js.status = false;
                js.Mesazhi = "Ka ndodhur nje gabim";
                return Json(js, JsonRequestBehavior.AllowGet);
            }
            js.status = true;
            /*ViewData["MMM"] = db.MENU_ROLI.ToList();*/
            return Json(js, JsonRequestBehavior.AllowGet);
            /*var jsoni = new JavaScriptSerializer().Serialize(obj);
            Console.WriteLine(jsoni);*/
        }

        public async Task<ActionResult> Delete(MENU_ROLI menuID)
        {
            var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            try
            {
                MENU_ROLI menu_roli = db.MENU_ROLI.Find(menuID.ID);
                db.MENU_ROLI.Remove(menu_roli);
                await db.SaveChangesAsync();
                returnmodel.status = true;
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
        public async Task<ActionResult> MenueList(int? roleID)
        {
            var user = await GetUser();

            if (roleID != null)
            {
                //ViewData["user"] = db.USERs.ToList();
                var roles = db.MENU_ROLI.Where(q => q.RoliID == roleID).ToList();
                return PartialView("MenueList", roles);

            }
            return PartialView("MenueList", db.MENU_ROLI.ToList());

        }

    }
}
