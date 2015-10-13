using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(aakashPawar.Startup))]
namespace aakashPawar
{
    public partial class Startup
    {

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
