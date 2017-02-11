using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using NtccSteward.Framework;
using NtccSteward.Core.Models.Account;

namespace NtccSteward.Models
{
    public interface IAppUserManager
    {
        Task<AppUser> FindAsync(string userName, string password);
    }

    public class AppUserManager : UserManager<AppUser>, IAppUserManager
    {
        public AppUserManager(IUserStore<AppUser> store) : base(store)
        {
            
        }


        [Dependency]
        public IApiProvider ApiProvider { get; set; }


        public override Task<AppUser> FindAsync(string userName, string password)
        {
            //return base.FindAsync(userName, password);
            var task = new Task<AppUser>(() => 
            {
                return new AppUser("","");
            });
            task.Start();
            return task;
        }
   }
}