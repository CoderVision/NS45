using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Modules.Members
{
    public class MemberMessages : ModuleBase
    {
        public MemberMessages(int id)
        {
            Initialize();

            this.Id = id;
        }

        private void Initialize()
        {
            this.DisplayText = "Awards";
        }
    }
}
