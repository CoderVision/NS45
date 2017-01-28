using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Modules.Church
{
    public class ChurchModule : ModuleBase
    {
        public ChurchModule(int id)
        {
            this.Id = id;

            Initialize();
        }

        private void Initialize()
        {
            Modules = new List<string>();
            Modules.Add("Church Info");
            //Modules.Add("Attributes");  // all soulwinning activity
            Modules.Add("Communication");  // all soulwinning activity
            //Modules.Add("Notes");
            //Modules.Add("History");
            //Modules.Add("Teams");

            var profile = new ChurchProfile(this.Id);
            //profile.Load();

            this.DisplayText = $"Church Profile for {profile.ChurchName}";

            InitialModule = profile;
        }

        public List<string> Modules { get; set; }

        public ModuleBase InitialModule { get; set; }
    }
}
