using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NtccSteward.IdSvr.Startup))]
namespace NtccSteward.IdSvr
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
