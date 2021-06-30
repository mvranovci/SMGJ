using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using SMGJ.Models;

namespace SMGJ.Controllers
{
    
    public class HomeController : BaseController
    {
        public async Task<ActionResult> Index()
        {
            var user = await GetUser();
            return View();
        }

        public async Task<ActionResult> GetUserHome()
        {
            var user = await GetUser();

            RegisterViewModelUser modeli = new RegisterViewModelUser();
            modeli.ID = user.ID;
            modeli.Emri = user.Emri;
            modeli.Mbiemri = user.Mbiemri;
            return View(modeli);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
