
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Framework
{
    public interface IAppConfigProvider
    {
        string ConnectionString { get; set; }
        string LoginConnectionString { get; set; }
        string Pepper { get; set; }
    }

    public class AppConfigProvider : IAppConfigProvider
    {
        public AppConfigProvider(string cnString, string loginCnString, string pepper)
        {
            ConnectionString = cnString;
            LoginConnectionString = loginCnString;
            Pepper = pepper;
        }

        public string ConnectionString { get; set; }

        public string LoginConnectionString { get; set; }

        public string Pepper { get; set; }
    }
}

