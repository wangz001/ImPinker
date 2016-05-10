using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ImPinker.Startup))]
namespace ImPinker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
