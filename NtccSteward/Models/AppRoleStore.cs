using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace NtccSteward.Models
{
    public class AppRoleStore : IRoleStore<AppRole>
    {
        private List<AppRole> _roles;
        public AppRoleStore()
        {
            // To-Do:  Remove this and move get it from the database.
            _roles = new List<AppRole>();
            _roles.Add(new Models.AppRole("2", RoleTypes.Admin));
            _roles.Add(new Models.AppRole("3", RoleTypes.User));

        }
        public Task CreateAsync(AppRole role)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(AppRole role)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<AppRole> FindByIdAsync(string roleId)
        {
            return new TaskFactory().StartNew(() => { return _roles.First(r => r.Id == roleId); });
        }

        public Task<AppRole> FindByNameAsync(string roleName)
        {
            return new TaskFactory().StartNew(() => { return _roles.First(r => r.Name == roleName); });
        }

        public Task UpdateAsync(AppRole role)
        {
            throw new NotImplementedException();
        }
    }
}