
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using SMGJ.Models;
using System.Data.Entity;

namespace SMGJ.Controllers
{

    public class HomeController : BaseController
    {
        public async Task<ActionResult> Index()
        {
            SMGJDB db = new SMGJDB();
            var user = await GetUser();
            ViewBag.nrviqa = db.GJEDHIs.Where(x => x.TipiID == 4 && x.FermaID == user.FermaID).Count();
            ViewBag.nrlopa = db.GJEDHIs.Where(x => x.TipiID == 6 && x.FermaID == user.FermaID).Count();
            ViewBag.nrbulli = db.GJEDHIs.Where(x => x.TipiID == 7 && x.FermaID == user.FermaID).Count();
            ViewBag.nrmushtjerra = db.GJEDHIs.Where(x => x.TipiID == 11 && x.FermaID == user.FermaID).Count();
            ViewBag.nrMale = db.GJEDHIs.Where(x => x.Gjinia == false && x.FermaID == user.FermaID).Count();
            ViewBag.nrFemale = db.GJEDHIs.Where(x => x.Gjinia == true && x.FermaID == user.FermaID).Count();

            GJEDHI gjedhi = new GJEDHI();

            var nrGjedhave = db.FERMAs.Where(x => x.ID == gjedhi.FermaID).Count();
            ViewBag.numriGjedhave = nrGjedhave;


            // (from a in db.USERs where a.RoleID == (int)Roli.Administrator select a)
            // var totali = (from qd in db.QUMESHTI_DETAJET where qd.DataProdhimit. == DateTime.Today select qd.TotalLitra).FirstOrDefault();
            var totali = (from qd in db.QUMESHTI_DETAJET
                          join u in db.USERs on qd.FermaID equals u.FermaID
                          where DbFunctions.TruncateTime(qd.DataProdhimit) == DbFunctions.TruncateTime(DateTime.Now)
                          && qd.FermaID == user.FermaID
                          select qd.TotalLitra).FirstOrDefault();


            var gjedhiMePSH = (from N in db.NJOFTIMETs
                               join gjp in db.GJEDHAT_PARAMETRAT on N.Gjedhi_ParametriID equals gjp.ID
                               join Gj in db.GJEDHIs on gjp.GjedhiID equals Gj.ID
                               join F in db.FERMAs on Gj.FermaID equals F.ID
                               where F.ID == user.FermaID
                               select new { Gj.Emri, N.Data }).ToList().OrderByDescending(x => x.Data).FirstOrDefault();

            if (gjedhiMePSH == null)
            {
                ViewBag.GjedhiMePSH = null;
            }
            else
            {

                var gjedhiEmri = gjedhiMePSH.Emri;
                ViewBag.GjedhiMePSH = gjedhiEmri;
            }

            //var litratPerDite = db.QUMESHTI_DETAJET.Where(q => q.DataProdhimit == model.DataProdhimit).Sum(q => q.TotalLitra);
            //     decimal totali = db.QUMESHTIs.Where(q => q.DataProdhimit == model.DataProdhimit).Sum(q => q.SasiaProdhuar);

            ViewBag.Litrat = totali;

            var sasia = (from q in db.QUMESHTIs
                         join gj in db.GJEDHIs on q.GjedhiID equals gj.ID
                         where user.FermaID == gj.FermaID
                         group q by new { a = q.GjedhiID } into g
                         select new gjedhi_qumshti
                         {
                             ID = g.Key.a,
                             sasia = g.Sum(q => q.SasiaProdhuar),
                             emri = g.FirstOrDefault().GJEDHI.Emri,
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
