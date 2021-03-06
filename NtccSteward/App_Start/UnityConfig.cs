using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc5;
using NtccSteward.Framework;
using System.Configuration;
using NtccSteward.Models;

namespace NtccSteward
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

            var userStore = new AppUserStore<AppUser>();
            var userManager = new AppUserManager(userStore);
            container.RegisterInstance<AppUserManager>(userManager);

            var appRoleStore = new AppRoleStore();
            var appRoleManager = new AppRoleManager(appRoleStore);
            container.RegisterInstance<AppRoleManager>(appRoleManager);

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            

            /*
                Getting started with Unity.Mvc5
                ------------------------------ -

                Unity.Mvc5 is an update of the popular Unity.Mvc3 package, updated to target .NET 4.5, MVC5 and Unity 3.0

                To get started, just add a call to UnityConfig.RegisterComponents() in the Application_Start method of Global.asax.cs
                and the MVC framework will then use the Unity.Mvc5 DependencyResolver to resolve your components.

                e.g.

                public class MvcApplication : System.Web.HttpApplication
                        {
                            protected void Application_Start()
                            {
                                AreaRegistration.RegisterAllAreas();
                                UnityConfig.RegisterComponents();                           // <----- Add this line
                                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                                RouteConfig.RegisterRoutes(RouteTable.Routes);
                                BundleConfig.RegisterBundles(BundleTable.Bundles);
                            }
                        }

                        Add your Unity registrations in the RegisterComponents method of the UnityConfig class. All components that implement IDisposable should be
                        registered with the HierarchicalLifetimeManager to ensure that they are properly disposed at the end of the request.

                        It is not necessary to register your controllers with Unity.
        */
        }
    }
}