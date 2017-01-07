using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Modules.Members
{
    public class MemberAwards : ModuleBase
    {
        public MemberAwards(int id)
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
