using SMGJ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace SMGJ.Controllers
{
    public class PROFILIController : BaseController
    {
        SMGJDB db = new SMGJDB();
        // GET: PROFILI
        public async Task<ActionResult> Index()
        {
            var user = await GetUser();
             
            return View();
        }
        public async Task<ActionResult> Create()
        {
            var user = await GetUser();
            
            return View();
        }
    }
}