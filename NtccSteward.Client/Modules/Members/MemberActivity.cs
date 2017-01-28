using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Modules.Members
{
    public class MemberActivity : ModuleBase
    {
        public MemberActivity(int id)
        {
            this.Id = id;

            Initialize();
        }

        private void Initialize()
        {
            this.DisplayText = "Activity";

            Activity = new List<ModuleBase>();
        }


        public List<ModuleBase> Activity { get; set; }
    }
}
