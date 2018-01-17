using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services.Default;
using NtccSteward.IdentityServer.Config;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using Microsoft.Owin.StaticFiles;

namespace NtccSteward.IdentityServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    RequestPath = new PathString("/core/content"),
            //    FileSystem = new PhysicalFileSystem("Content")
            //});
            //DefaultViewServiceConfiguration
            // style override:
            // https://github.com/IdentityServer/IdentityServer3/issues/715

            app.Map("/identity", idSvrApp =>
            {
                var corsPolicyService = new DefaultCorsPolicyService()
                {
                    AllowAll = true
                };

                var idServerServiceFactory = new IdentityServerServiceFactory()
                    .UseInMemoryClients(Clients.Get())
                    .UseInMemoryUsers(Users.Get())
                    .UseInMemoryScopes(Scopes.Get());

                idServerServiceFactory.CorsPolicyService = new
                    Registration<IdentityServer3.Core.Services.ICorsPolicyService>(corsPolicyService);

                var options = new IdentityServerOptions
                {
                    Factory = idServerServiceFactory,
                    SiteName = "NtccSteward Security Token Service",
                    IssuerUri = ConfigurationManager.AppSettings["NtccStewardIssuerUri"],  // does not have to be an existing uri
                    PublicOrigin = ConfigurationManager.AppSettings["NtccStewardStsOrigin"],
                    SigningCertificate = LoadCertificate(),  // certificate that is used for encrpting token, not ssl
                    AuthenticationOptions = new AuthenticationOptions()
                    {
                        EnablePostSignOutAutoRedirect = true
                    }
                };

                idSvrApp.UseIdentityServer(options);
            });
        }

        private X509Certificate2 LoadCertificate()
        {
            return new X509Certificate2(
                $@"{AppDomain.CurrentDomain.BaseDirectory}\certificates\localhost.pfx", "localhost");
            //        return new X509Certificate2(
            //string.Format(@"{0}\certificates\idsrv3test.pfx",
            //AppDomain.CurrentDomain.BaseDirectory), "idsrv3test");
        }
    }
}