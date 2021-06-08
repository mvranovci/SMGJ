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
    }
}
