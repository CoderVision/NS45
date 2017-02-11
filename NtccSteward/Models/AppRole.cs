using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Models
{
    public class AppRole : IRole
    {
        public AppRole(string id, string name)
        {
            Id = id;
            Name = name;
        }
        public string Id { get; private set; }

        public string Name { get; set; }
    }
}