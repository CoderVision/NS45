using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using IdentityServer3.AccessTokenValidation;
using System.Configuration;
using System.IdentityModel.Tokens;

[assembly: OwinStartup(typeof(NtccSteward.Api.Startup))]

namespace NtccSteward.Api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            // left off with #3 "Authorizing Access to the API", "Client Credentials Flow"
            //https://app.pluralsight.com/player?course=oauth2-openid-connect-angular-aspdotnet&author=kevin-dockx&name=oauth2-openid-connect-angular-aspdotnet-m03&clip=0&mode=live
            app.UseIdentityServerBearerTokenAuthentication(
                new IdentityServerBearerTokenAuthenticationOptions
                {
                    Authority = ConfigurationManager.AppSettings["NtccStewardSts"],
                    RequiredScopes = new[] { "ApplicationAccess" }
                });

            var config = new System.Web.Http.HttpConfiguration();
            WebApiConfig.Register(config);
            UnityConfig.RegisterComponents(config);
            app.UseWebApi(config);
            //UnityConfig.RegisterComponents();
        }
    }
}
