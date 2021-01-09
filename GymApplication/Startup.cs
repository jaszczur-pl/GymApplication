using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using System;
using System.Web.Http.Cors;

[assembly: OwinStartup(typeof(GymApplication.Startup))]
namespace GymApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            ConfigureAuth(app);
        }
    }
}