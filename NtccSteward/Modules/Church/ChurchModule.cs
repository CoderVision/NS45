using NtccSteward.Core.Interfaces.Church;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Modules.Church
{
    public class ChurchModule : ModuleBase
    {
        public ChurchModule(ModuleBase initialModule)
        {
            Initialize(initialModule);
        }

        private void Initialize(ModuleBase initialModule)
        {
            Modules = new List<string>();
            Modules.Add("Church Info");
            //Modules.Add("Attributes");  // all soulwinning activity
            //Modules.Add("Communication");  // all soulwinning activity
            //Modules.Add("Notes");
            //Modules.Add("History");
            //Modules.Add("Teams");

            if (initialModule is ChurchProfile)
                this.DisplayText = $"{((ChurchProfile)initialModule).Name}";
            else
                this.DisplayText = $"Church Profile";

            InitialModule = initialModule;
        }

        public List<string> Modules { get; set; }

        public ModuleBase InitialModule { get; set; }
    }
}
