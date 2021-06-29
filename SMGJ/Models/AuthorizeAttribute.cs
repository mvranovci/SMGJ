using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMGJ.Controllers;
using Microsoft.AspNet.Identity;
namespace SMGJ.Models
{
   
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {

        private SMGJDB db = new SMGJDB();



         protected  override bool AuthorizeCore(HttpContextBase httpContext) 
            {
               try{
                 var user = httpContext.User.Identity.GetUserId();
                  var u = db.USERs.First(q=>q.UserId==user);
                  /*httpContext.User.Identity.AuthenticationType.ToString();*/

                  var controller = httpContext.Request.RequestContext.RouteData.Values["controller"].ToString();
                  bool isValid = db.AUTORIZIMETs.Any(a => a.Controller == controller && a.RoleID == u.RoleID);

                  return isValid;
            }
            catch{
               return false;
               }
        }


        override
        protected void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //filterContext.HttpContext.Response.Redirect(filterContext.RequestContext.HttpContext.Request.UrlReferrer);
            filterContext.Result = new RedirectResult("/Home/Index");
        }

        /*  protected virtual void HandleUnauthorizedRequest(AuthorizationContext filterContext) 
          {

          }

          protected virtual HttpValidationStatus OnCacheAuthorization(HttpContextBase httpContext)
          {

              return;
          }*/

    }
}
