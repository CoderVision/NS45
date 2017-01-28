using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc5;
using System.Configuration;
using NtccSteward.Framework;

namespace NtccSteward.Client
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            var webApiUri = ConfigurationManager.AppSettings["webApiUri"].ToString();
            container.RegisterInstance<IApiProvider>(new ApiProvider(webApiUri));

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}