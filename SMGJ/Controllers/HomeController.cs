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
        SMGJDB db = new SMGJDB();

            var user = await GetUser();
            var sasia = (from q in db.QUMESHTIs
                         join gj in db.GJEDHIs on q.GjedhiID equals gj.ID
                         where user.FermaID == gj.FermaID
                         group q by new { a = q.GjedhiID } into g
                         select new gjedhi_qumshti
                         {
                             ID = g.Key.a,
                             sasia = g.Sum(q => q.SasiaProdhuar),
                             emri=g.FirstOrDefault().GJEDHI.Emri,
                             vathi = g.FirstOrDefault().GJEDHI.Vathe
                         }).AsEnumerable();
            

            return View(sasia);
            
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
