using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Modules.Members
{
     public class MemberHistory : ModuleBase
    {
        public MemberHistory(int id)
        {
            this.Id = id;

            Initialize();
        }

        private void Initialize()
        {
            this.DisplayText = "Activity";

            History = new List<ModuleBase>();
        }

        public List<ModuleBase> History { get; set; }
    }
}
