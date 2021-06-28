using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SMGJ
{

    public class MvcApplication : System.Web.HttpApplication
    {
        

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            var culturename = System.Threading.Thread.CurrentThread.CurrentCulture.Name;

            CultureInfo cultureInfo = new CultureInfo(culturename);

            System.Globalization.DateTimeFormatInfo dateTimeInfo = new System.Globalization.DateTimeFormatInfo();
            dateTimeInfo.DateSeparator = "/";
            dateTimeInfo.LongDatePattern = "dd/MM/yyyy";
            dateTimeInfo.ShortDatePattern = "dd/MM/yyyy";
            dateTimeInfo.LongTimePattern = "HH:mm:ss tt";
            dateTimeInfo.ShortTimePattern = "HH:mm";
            cultureInfo.DateTimeFormat = dateTimeInfo;

            System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;
            System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;//CultureInfo.CreateSpecificCulture(cultureInfo.Name);
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            Response.Clear();

            HttpException httpException = exception as HttpException;

            if (httpException != null)
            {
                string action;

                switch (httpException.GetHttpCode())
                {
                    case 404:
                        // page not found
                        action = "Index";
                        break;
                    case 500:
                        // server error
                        action = "Index";
                        break;
                    default:
                        action = "Index";
                        break;
                }

                // clear error on server
                Server.ClearError();

                Response.Redirect(String.Format("~/Error/{0}", action));
            }
        }
        }
    }
