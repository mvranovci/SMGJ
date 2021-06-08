using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
 
using System.Web.Mvc; 
using SMGJ.Models;

namespace SMGJ.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        private SMGJDB db = new SMGJDB();
        public async Task<GetUser> GetUser()
        {
            USER user = new USER();
            GetUser getu = new GetUser();
            if (Session["User"] == null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    try
                    {
                       //ar userfound = await UserManager.FindByEmailAsync(User.Identity.Name);
                        
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                } 
            }
                return null;
        }
         
    }
}