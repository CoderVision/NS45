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

    public class AppUserManager : UserManager<AppUser>, IAppUserManager //, IUserRoleStore<AppUser>
    {
        public AppUserManager(IUserStore<AppUser> store) : base(store)
        {
            
        }

        public override Task<IList<string>> GetRolesAsync(string userId)
        {
            return base.GetRolesAsync(userId);
        }

        [Dependency]
        public IApiProvider ApiProvider { get; set; }

        public override Task<AppUser> FindAsync(string userName, string password)
        {
            //return base.FindAsync(userName, password);
            var task = new Task<AppUser>(() => 
            {
                var session = ApiProvider.PostItemAsync<Login>("account/Login", new Login() { Email=userName, Password=password  });

                return new AppUser("1","test");

                //if (!string.IsNullOrWhiteSpace(session))
                //{
                //    //var appUser = ApiProvider.DeserializeJson<>
                //}
                //else
                //{

                //}

                //if (string.IsNullOrWhiteSpace(session)
                //    || session.IndexOf("error", StringComparison.CurrentCultureIgnoreCase) > -1)
                //{
                //    TempData["loginError"] = "Login attempt failed, please try again.";
                //}
                //else
                //{
                //    HttpContext.Session["Session"] = session;

                //    //return RedirectToAction("Index", "Member", new { statusIds = "49-50", page = 1, pageSize = 1000, showAll = false });
                   // return RedirectToAction("Index", "Church");
                //}

            });
            task.Start();
            return task;
        }

    //    public Task AddToRoleAsync(AppUser user, string roleName)
    //    {
    //        //throw new NotImplementedException();
    //        return new TaskFactory().StartNew(() => { return; });
            
    //    }

    //    public Task RemoveFromRoleAsync(AppUser user, string roleName)
    //    {
    //        return new TaskFactory().StartNew(() => { return; });
    //    }

    //    public Task<IList<string>> GetRolesAsync(AppUser user)
    //    {
    //        return new TaskFactory().StartNew(() => { return (IList<string>)new List<string>() { "Admin" }; });
    //    }

    //    public Task<bool> IsInRoleAsync(AppUser user, string roleName)
    //    {
    //        return new TaskFactory().StartNew(() => { return true; });
    //    }

    //    Task IUserStore<AppUser, string>.CreateAsync(AppUser user)
    //    {
    //        return new TaskFactory().StartNew(() => { return; });
    //    }

    //    Task IUserStore<AppUser, string>.UpdateAsync(AppUser user)
    //    {
    //        return new TaskFactory().StartNew(() => { return; });
    //    }

    //    Task IUserStore<AppUser, string>.DeleteAsync(AppUser user)
    //    {
    //        return new TaskFactory().StartNew(() => { return; });
    //    }
    }
}