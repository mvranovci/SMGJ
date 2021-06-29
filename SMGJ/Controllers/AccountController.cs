using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SMGJ.Helpers;
using SMGJ.Models;
using static SMGJ.Helpers.Enums;

namespace SMGJ.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        SMGJDB db = new SMGJDB();
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

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

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> UserRegister()
        {
            var user = await GetUser();
            RegisterViewModelUser model = new RegisterViewModelUser();
            var gjinia = EnumsToSelectList<Gjinia>.GetSelectList();
            var modeli = new List<SelectListItem>();
           
            foreach (var item in gjinia)
            {
                modeli.Add(new SelectListItem { Value = item.Value, Text = item.Text, Selected = false });
            }
            ViewBag.Gjinia = modeli;
            ViewBag.KomunaID = await loadKomuna(null);
            ViewBag.FermaID = await loadFerma(null);
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> UserRegister_Post(RegisterViewModelUser model)
        {
            //var user = await GetUser();
            MessageJs returnmodel = new MessageJs();
            if (ModelState.IsValid)
            {

                var user = new ApplicationUser { UserName = model.username, Email = model.EmailAdresa };
                var result = await UserManager.CreateAsync(user, model.UserPassword);
                if (result.Succeeded)
                {
                    try
                    {

                        var userfound = await UserManager.FindByEmailAsync(model.EmailAdresa);
                        int roli = (int)Roli.Fermer;
                        var rolefound = (new ApplicationDbContext()).Roles.FirstOrDefault(q => q.Id == roli.ToString());

                        USER new_user = new USER();
                        new_user.UserId = userfound.Id;

                        new_user.Emri = capitalize(model.Emri);// Per shkronje te madhe
                        new_user.Mbiemri = capitalize(model.Mbiemri);// Per shkronje te madhe
                        new_user.RoleID = roli;
                        new_user.KomunaID = model.KomunaID;
                        new_user.Email = model.EmailAdresa;
                        new_user.NrLeternjoftimit = model.NumriLeternjoftimit;

                        ASCIIEncoding binarypass = new ASCIIEncoding();
                        string encrypted = Encrypt(model.UserPassword);
                        byte[] passwordArray = binarypass.GetBytes(encrypted);
                        new_user.Password = passwordArray;
                        new_user.RandomNumber = RandomString(6, false);
                        db.USERs.Add(new_user);
                        await UserManager.AddToRoleAsync(userfound.Id, rolefound.Name.ToString());
                        await db.SaveChangesAsync();
                        returnmodel.status = true;
                        returnmodel.Mesazhi = "Perdoruesi u regjistrua me sukses";
                        return Json(returnmodel, JsonRequestBehavior.AllowGet);
                    }
                    catch
                    {
                        await UserManager.DeleteAsync(user);
                        returnmodel.status = false;
                        returnmodel.Mesazhi = "Ka ndodhur nje gabim";
                        return Json(returnmodel, JsonRequestBehavior.AllowGet);
                    }



                }
                else
                {

                    returnmodel.status = false;
                    returnmodel.Mesazhi = "Perdoruesi egziston";
                    return Json(returnmodel, JsonRequestBehavior.DenyGet);
                }
            }

            else
            {

                var message = string.Join(" | ", ModelState.Values
                           .SelectMany(v => v.Errors)
                           .Select(e => e.ErrorMessage));

                returnmodel.status = false;
                returnmodel.Mesazhi = "Ka ndodhur nje gabim";
                return Json(returnmodel, JsonRequestBehavior.DenyGet);
            }
        }

        private string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 1; i < size + 1; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            else
                return builder.ToString();
        }
        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage));
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await PasswordSignIn(model.Email, model.Password, model.RememberMe, false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }
        public async Task<SignInStatus> PasswordSignIn(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            var user = await UserManager.FindByNameAsync(userName);

            if (user == null)
            {
                return SignInStatus.Failure;
            }

            if (UserManager.IsLockedOutAsync(user.Id).Result)
            {
                return SignInStatus.LockedOut;
            }

            if (await UserManager.CheckPasswordAsync(user, password))
            {
                var userdb = await db.USERs.FirstOrDefaultAsync(q => q.UserId == user.Id);
                if (userdb.Aktiv == false)
                    return SignInStatus.Failure;

                var userIdentity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                if (isPersistent)
                {
                    AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, userIdentity);
                }
                else
                {
                    AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, userIdentity);
                }
                await db.SaveChangesAsync();
                var culture = CultureHelper.GetImplementedCulture("sq-AL");
                // Save culture in a cookie
                HttpCookie cookie = Request.Cookies["_culture"];
                if (cookie != null)
                    cookie.Value = culture;   // update cookie value
                else
                {
                    cookie = new HttpCookie("_culture");
                    cookie.Value = culture;
                    cookie.Expires = DateTime.Now.AddYears(1);
                }
                Response.Cookies.Add(cookie);
                return SignInStatus.Success;
            }

            await db.SaveChangesAsync();
            return SignInStatus.Failure;
        }
        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public async Task<ActionResult> Register()
        {
            var user = await GetUser();
            RegisterViewModel model = new RegisterViewModel();
            var allvalues = (new ApplicationDbContext()).Roles.OrderBy(q => q.Name).ToList().Select(q => new SelectListItem { Value = q.Id, Text = q.Name }).ToList();
            ViewBag.InstitucioniID = await loadKomuna(null);
            ViewBag.FermaID = await loadFerma(null);
            ViewBag.RoleID = allvalues;
            ViewBag.RoleUserID = user.RoleID;
            var modeli = new List<SelectListItem>();
            var gjinia = EnumsToSelectList<Gjinia>.GetSelectList();
            foreach (var item in gjinia)
            {
                modeli.Add(new SelectListItem { Value = item.Value.ToString(), Text = item.Text, Selected = false });
            }
            ViewBag.Gjinia = modeli;
            MENU modeil = new MENU();
            ViewBag.Ferma = modeli;
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Register_Post(RegisterViewModel model)
        {
            var usermodel = await GetUser();
            MessageJs returnmodel = new MessageJs();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    try
                    {
                        var userfound = await UserManager.FindByEmailAsync(model.Email);
                        int roliID = model.RoleID;
                        USER newUser = new USER();
                        newUser.UserId = userfound.Id;
                        newUser.Emri = model.Emri;
                        newUser.Mbiemri = model.Mbiemri;
                        newUser.NrLeternjoftimit = model.NumriPersonal;
                        newUser.NrTelefonit = model.Telefoni;
                        newUser.KomunaID = model.InstitucioniID;
                        newUser.FermaID = model.FermaID;
                       
                        if (model.Ditelindja != null)
                        { 
                            newUser.Datelindja = model.Ditelindja.Value;
                        }
                        else
                        {
                            newUser.Datelindja = null;
                        }
                        newUser.Email = model.Email;
                        newUser.RandomNumber = RandomString(6, false);
                        newUser.Aktiv = model.Statusi;
                        newUser.RoleID = roliID;
                        newUser.Gjinia = Convert.ToBoolean(model.Gjinia);
                        
                        db.USERs.Add(newUser);
                        await db.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        await UserManager.DeleteAsync(user);

                    }
                    returnmodel.status = true;
                    return Json(returnmodel, JsonRequestBehavior.AllowGet);
                }
                AddErrors(result);
            }

            var message = string.Join(" | ", ModelState.Values
                       .SelectMany(v => v.Errors)
                       .Select(e => e.ErrorMessage));
            var modeli = new List<SelectListItem>();
            var gjinia = EnumsToSelectList<Gjinia>.GetSelectList();
            foreach (var item in gjinia)
            {
                modeli.Add(new SelectListItem { Value = item.Value.ToString(), Text = item.Text, Selected = false });
            }
            ViewBag.Gjinia = modeli;
            ViewBag.RoleID = (new ApplicationDbContext()).Roles.OrderBy(q => q.Name).ToList().Select(q => new SelectListItem { Value = q.Id, Text = q.Name }).ToList();
            ViewBag.InstitucioniID = await loadKomuna(null);
            ViewBag.FermaID = await loadFerma(null);

            return View(model);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                MessageJs returnmodel = new MessageJs();
                var user = UserManager.FindByEmail(model.Email);
                if (user == null)
                {
                    returnmodel.status = false;
                    returnmodel.Mesazhi = "Nuk ekziston emaili i kerkuar";
                    return Json(returnmodel, JsonRequestBehavior.DenyGet);
                    // Don't reveal that the user does not exist or is not confirmed
                    // return View("ForgotPasswordConfirmation");
                }

                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

                var callbackUrl = Url.Action("ResetPassword", "Account", new { UserId = user.Id, code = code }, protocol: Request.Url.Scheme);
                //  await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
                string subjekti = "Resetimi i fjalëkalimit";
                string body = "<p>I/e nderuar,</p><p> Ju lutemi klikoni butonin më poshtë për të përfunduar resetimin  e fjalëkalimit tuaj</p><p style=\"max-width:153px; padding: 10px 15px; border-radius: 5px; background-color:blue\"><a href=\"" + callbackUrl + "\" style=\"color:white;text-decoration: none;\">NDRYSHO FJALËKALIMIN</a> </p>" +
                  "<br><p>Pasi t'a ndryshoni passwordin ky link nuk eshte me valid!</p><p>Nëse butoni më lart nuk funksionon atëherë ju lutem ta kopjoni linkun më poshtë dhe ta hapni në një dritare të re</p><p style=\"background-color:#e6e6f8;padding: 10px;\"> " + callbackUrl + "</p><br> <br><br><p><i>HR TEAM</i><p style=\" font-size:11px; \">Note: Do not reply to this email. </p><p style=\"text-align:center; font-size:11px; \"> Copyright 	&copy; HR All rights reserved </p>";
                var threadDergoEmail = new Thread(() => SendMail.DergoEmail(model.Email, subjekti, body)); threadDergoEmail.Start();
                returnmodel.status = true;
                return Json(returnmodel, JsonRequestBehavior.AllowGet);

                //For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
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
                return RedirectToAction("Login", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [AllowAnonymous]
        public ActionResult LogOff()
        {
            Session["User"] = null;
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}
