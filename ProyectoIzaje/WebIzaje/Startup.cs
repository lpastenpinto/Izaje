using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebIzaje.Startup))]
namespace WebIzaje
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
