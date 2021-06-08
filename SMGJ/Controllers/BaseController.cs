using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using SMGJ.Helpers;
using SMGJ.Models;

namespace SMGJ.Controllers
{
    public class BaseController : Controller
    {
        private ApplicationUserManager _userManager;
        // GET: Base
        private SMGJDB db = new SMGJDB();
        private SMGJEntities db1 = new SMGJEntities();
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string cultureName = null; 
            var user = (GetUser)Session["User"];

            try
            { 
                System.Web.HttpCookie cultureCookie = Request.Cookies["_culture"];
                if (Session["kultura"] != null)
                {
                    cultureName = Session["kultura"].ToString();
                }
                else
                {
                    if (cultureCookie != null)
                        cultureName = cultureCookie.Value;
                   
                } 

                // Validate culture name
                cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe

                CultureInfo culture = new System.Globalization.CultureInfo(cultureName);
                System.Globalization.DateTimeFormatInfo dateTimeInfo = new System.Globalization.DateTimeFormatInfo();
                System.Globalization.NumberFormatInfo numberFormatInfo = new System.Globalization.NumberFormatInfo();

                dateTimeInfo.DateSeparator = "/";
                dateTimeInfo.LongDatePattern = "dd/MM/yyyy";
                dateTimeInfo.ShortDatePattern = "dd/MM/yyyy";
                dateTimeInfo.LongTimePattern = "HH:mm:ss tt";
                dateTimeInfo.ShortTimePattern = "HH:mm";
                culture.DateTimeFormat = dateTimeInfo;
                numberFormatInfo.NumberDecimalSeparator = ".";
                culture.NumberFormat = numberFormatInfo;

                // Modify current thread's cultures            
                Thread.CurrentThread.CurrentCulture = culture;
                //Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName); ;
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

                if (Session["aID"] != null)
                {
                    ViewBag.aID = Session["aID"].ToString();
                }
            }
            catch
            {


            }

            return base.BeginExecuteCore(callback, state);
        }
        public async Task<GetUser> GetUser()
        {
            USER user = new USER();
            GetUser usertotal = new GetUser();
            if (Session["User"] == null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    try
                    {
                        var userfound = await UserManager.FindByEmailAsync(User.Identity.Name);
                        if (userfound == null)
                            userfound = await UserManager.FindByNameAsync(User.Identity.Name);

                        user = db.USERs.Single(q => q.UserId == userfound.Id);
                        usertotal.ID = user.ID;
                        usertotal.UserID = user.UserId;
                        usertotal.Emri = user.Emri;
                        usertotal.Mbiemri = user.Mbiemri;
                        usertotal.NumriLeternjoftimit = user.NrLeternjoftimit;
                        Session["User"] = usertotal;
                    }
                    catch (Exception)
                    {

                        usertotal = null;
                    }
                }
            }
            else
            {
                usertotal = (GetUser)Session["User"];
            }
            return usertotal;
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

    }
}