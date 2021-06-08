using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SMGJ.Startup))]
namespace SMGJ
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
