using SMGJ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SMGJ.Controllers
{
    public class NJOFTIMETController : BaseController
    {
        // GET: NJOFTIMET
        public async Task<ActionResult> Index()
        {
            SMGJDB db = new SMGJDB();
            var user = await GetUser();
            var mesazhet = (from nu in db.NJOFTIMET_USER
                            join u in db.USERs on nu.PrejID equals u.ID
                            join n in db.NJOFTIMETs on nu.NjoftimetID equals n.ID
                            where nu.TekID == user.ID
                            select new Njoftimet_e_reja { Emri = u.Emri, Mesazhi = n.Mesazhi, Koha = n.Data.Value, GjedhiId = n.Gjedhi_ParametriID.Value }).AsEnumerable();

            
            return View(mesazhet);
        }

        // GET: NJOFTIMET/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: NJOFTIMET/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NJOFTIMET/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: NJOFTIMET/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: NJOFTIMET/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: NJOFTIMET/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: NJOFTIMET/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
