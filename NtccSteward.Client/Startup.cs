using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NtccSteward.Client.Startup))]
namespace NtccSteward.Client
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
