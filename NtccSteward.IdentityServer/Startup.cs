using IdentityServer3.Core.Configuration;
using NtccSteward.IdentityServer.Config;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace NtccSteward.IdentityServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/identity", idSvrApp =>
            {
                var idServerServiceFactory = new IdentityServerServiceFactory()
                    .UseInMemoryClients(Clients.Get())
                    .UseInMemoryUsers(Users.Get())
                    .UseInMemoryScopes(Scopes.Get());

                var options = new IdentityServerOptions
                {
                    Factory = idServerServiceFactory,
                    SiteName = "NtccSteward Security Token Service",
                    IssuerUri = ConfigurationManager.AppSettings["NtccStewardIssuerUri"],  // does not have to be an existing uri
                    PublicOrigin = ConfigurationManager.AppSettings["NtccStewardStsOrigin"],
                    SigningCertificate = LoadCertificate()
                };

                idSvrApp.UseIdentityServer(options);
            });
        }

        private X509Certificate2 LoadCertificate()
        {
            return new X509Certificate2(
                $@"{AppDomain.CurrentDomain.BaseDirectory}\certificates\localhost.pfx", "localhost");
        }
    }
}