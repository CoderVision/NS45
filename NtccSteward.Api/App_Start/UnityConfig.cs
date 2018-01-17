using Microsoft.Practices.Unity;
using NtccSteward.Api.Controllers;
using NtccSteward.Repository;
using System.Configuration;
using System.Web.Http;
using Unity.WebApi;

namespace NtccSteward.Api
{
    // In Web API install Unity.WebAPI
    //   It is different than Unit.MVC5
    public static class UnityConfig
    {
        public static void RegisterComponents(System.Web.Http.HttpConfiguration config)
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            // Dependency Injection trouble-shooting.
            //http://stackoverflow.com/questions/24254189/make-sure-that-the-controller-has-a-parameterless-public-constructor-error

            // register repositories
            var defaultConnectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
            container.RegisterInstance<ICommonRepository>(new CommonRepository(defaultConnectionString));
            container.RegisterInstance<ITeamRepository>(new TeamRepository(defaultConnectionString));
            container.RegisterInstance<IChurchRepository>(new ChurchRepository(defaultConnectionString));
            container.RegisterInstance<ILogger>(new LoggerRepository(defaultConnectionString));
            container.RegisterInstance<IMemberRepository>(new MemberRepository(defaultConnectionString));
            container.RegisterInstance<IMessageRepository>(new MessageRepository(defaultConnectionString));
            container.RegisterInstance<IReportsRepository>(new ReportsRepository(defaultConnectionString));

            // register controllers
            container.RegisterType<AccountController>();
            container.RegisterType<ChurchController>();
            container.RegisterType<MembersController>();
            //container.RegisterType<MessageApiController>();

            //GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
            config.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}