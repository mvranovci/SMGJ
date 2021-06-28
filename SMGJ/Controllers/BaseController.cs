using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity.Owin;
using SMGJ.Helpers;
using SMGJ.Models;

namespace SMGJ.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        private ApplicationUserManager _userManager;
        // GET: Base
        private SMGJDB db = new SMGJDB();
        private SMGJEntities db1 = new SMGJEntities();

        static readonly string PasswordHash = "P@@Sw0rdA()ŠP@21";
        static readonly string SaltKey = "S@LT&KEYOK_NOT";
        static readonly string VIKey = "@1B2c3D4e5F6g7H8f";
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
                        usertotal.RoleID = user.RoleID;
                        usertotal.FermaID = user.FermaID;
                        Session["User"] = usertotal;
                        Session["RoliID"] = usertotal.RoleID;
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
                Session["Roli"] = usertotal.RoleID;
            }
            return usertotal;
        }
        
        public async Task<SelectList> loadKomuna(int? selected)
        { 
            var allvalues = await  db.KOMUNAs.ToListAsync();

            if (selected.HasValue)
                return new SelectList(allvalues, "ID", "Emri", selected.Value);
            else
                return new SelectList(allvalues, "ID", "Emri");
        }
        public async Task<SelectList> loadGjedhi1(int? selected)
        {
            var user = await GetUser();
            
            List<GJEDHI> allvalues =  db.GJEDHIs.ToList();
            List<GJEDHI> values;
            if (!User.IsInRole("Administrator"))
            {
                values =  db.GJEDHIs.Where(q => q.KrijuarNga == user.ID).ToList();
            }
            else
            {
                values = allvalues;
            }
            if (selected.HasValue)
                return new SelectList(values, "ID", "Emri", selected.Value);
            else
                return new SelectList(values, "ID", "Emri");
        }
        public async Task<SelectList> loadYndyra(int? selected)
        {
            var allvalues = await db.YNDYRAs.ToListAsync();

            if (selected.HasValue)
                return new SelectList(allvalues, "ID", "Emertimi", selected.Value);
            else
                return new SelectList(allvalues, "ID", "Emertimi");
        }
        public async Task<SelectList> loadKontaminimi(int? selected)
        {
            var allvalues = await db.KONTAMINIMIs.ToListAsync();

            if (selected.HasValue)
                return new SelectList(allvalues, "ID", "Vlera", selected.Value);
            else
                return new SelectList(allvalues, "ID", "Vlera");
        }
        public async Task<SelectList> loadUser(int? selected)
        {
            var allvalues = await db.USERs.ToListAsync();

            if (selected.HasValue)
                return new SelectList(allvalues, "ID", "Emri", selected.Value);
            else
                return new SelectList(allvalues, "ID", "Emri");
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

        public static string Encrypt(string plainText)
        {
            try
            {
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

                byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
                var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
                var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

                byte[] cipherTextBytes;

                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                        cryptoStream.FlushFinalBlock();
                        cipherTextBytes = memoryStream.ToArray();
                        cryptoStream.Close();
                    }
                    memoryStream.Close();
                }
                return Convert.ToBase64String(cipherTextBytes);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<SelectList> loadMenu(int? selected)
        {
            var allvalues = await db.MENUs.Where(q=>q.Aktiv==true).ToListAsync();
            if (selected.HasValue)
                return new SelectList(allvalues, "ID", "Emertimi", selected.Value);
            else
                return new SelectList(allvalues, "ID", "Emertimi");
        }

        public static string Decrypt(string encryptedText)
        {
            try
            {
                byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
                byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
                var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

                var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
                var memoryStream = new MemoryStream(cipherTextBytes);
                var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                byte[] plainTextBytes = new byte[cipherTextBytes.Length];

                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                memoryStream.Close();
                cryptoStream.Close();
                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
            }
            catch (Exception)
            {
                return null;
            }
        }
        //-------------------------- GJEDHI LOAD METHODS ---------------------------------------------
        public async Task<SelectList> loadFerma(int? selected)
        {
            var allvalues = await db.FERMAs.ToListAsync();

            if (selected.HasValue)
                return new SelectList(allvalues, "ID", "Emri", selected.Value);
            else
                return new SelectList(allvalues, "ID", "Emri");
        }
        public async Task<SelectList> loadRaca(int? selected)
        {
            var allvalues = await db.RACAs.ToListAsync();

            if (selected.HasValue)
                return new SelectList(allvalues, "ID", "Emertimi", selected.Value);
            else
                return new SelectList(allvalues, "ID", "Emertimi");
        }
        public async Task<SelectList> loadPrindi(int? selected)
        {
            var user = await GetUser();
            //lista qe permban vetem gjedhat e perdoruesit te kycur
            var lista = db.GJEDHIs.Where(x => x.KrijuarNga == user.ID).ToList();
            if (selected.HasValue)
                return new SelectList(lista, "ID", "Emri", selected.Value);
            else
                return new SelectList(lista, "ID", "Emri");
        }
        public async Task<SelectList> loadTipi(int? selected)
        {
            var allvalues = await db.TIPIs.ToListAsync();

            if (selected.HasValue)
                return new SelectList(allvalues, "ID", "Emertimi", selected.Value);
            else
                return new SelectList(allvalues, "ID", "Emertimi");
        }
        public async Task<SelectList> loadGjedhi(int? selected)
        {
            var allvalues = await db.GJEDHIs.ToListAsync();

            if (selected.HasValue)
                return new SelectList(allvalues, "ID", "Emri", selected.Value);
            else
                return new SelectList(allvalues, "ID", "Emri");
        }

        public async Task<SelectList> loadLageshtia(int? selected)
        {
            var allvalues = await db.LAGESHTIA_AJRIT.ToListAsync();

            if (selected.HasValue)
                return new SelectList(allvalues, "ID", "Vlera", selected.Value);
            else
                return new SelectList(allvalues, "ID", "Vlera");
        }

        public async Task<SelectList> loadTemperatura(int? selected)
        {
            var allvalues = await db.TEMPERATURAs.ToListAsync();

            if (selected.HasValue)
                return new SelectList(allvalues, "ID", "Vlera", selected.Value);
            else
                return new SelectList(allvalues, "ID", "Vlera");
        }

        public async Task<SelectList> loadRrahjetEZemres(int? selected)
        {
            var allvalues = await db.RRAHJET_ZEMRES.ToListAsync();

            if (selected.HasValue)
                return new SelectList(allvalues, "ID", "Vlera", selected.Value);
            else
                return new SelectList(allvalues, "ID", "Vlera");
        }
        // DOES USER HAVE A FARM
        public Boolean hasFarm(int? userID)
        {
            var useri = db.USERs.Find(userID);
            var hasfarm = useri.FermaID != null;

            if (hasfarm)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
        public class NoDirectAccessAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                if (filterContext.HttpContext.Request.UrlReferrer == null ||
         filterContext.HttpContext.Request.Url.Host != filterContext.HttpContext.Request.UrlReferrer.Host)
                {
                    filterContext.Result = new RedirectToRouteResult(new
                                              RouteValueDictionary(new { controller = "Home", action = "Logout", area = "Main" }));
                }
            }
        }



    }
}
