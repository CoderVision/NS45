using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(NtccSteward.Api.Startup))]

namespace NtccSteward.Api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new System.Web.Http.HttpConfiguration();
            WebApiConfig.Register(config);
            UnityConfig.RegisterComponents(config);
            app.UseWebApi(config);
            //UnityConfig.RegisterComponents();
        }
    }
}
