using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Modules.Members
{
    public class MemberModule : ModuleBase
    {
        public MemberModule(ModuleBase initialModule)
        {
            Initialize(initialModule);
        }

        private void Initialize(ModuleBase initialModule)
        {
            Modules = new List<string>();
            Modules.Add("Personal Info");
            Modules.Add("Activity");  // all soulwinning activity
            Modules.Add("Awards");
            Modules.Add("Messages");  // sent sms, email, etc.
            //Modules.Add("Notes");
            Modules.Add("History");

            if (initialModule is MemberProfile)
                this.DisplayText = $"{((MemberProfile)initialModule).FullName}";
            else
                this.DisplayText = $"Member Profile";

            InitialModule = initialModule;
        }

        public List<string> Modules { get; set; }

        public ModuleBase InitialModule { get; set; }
    }
}

