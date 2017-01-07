using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NtccSteward.Startup))]
namespace NtccSteward
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
