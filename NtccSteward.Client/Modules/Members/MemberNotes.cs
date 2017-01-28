using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Modules.Members
{
    public class MemberNotes : ModuleBase
    {
        public MemberNotes(int id)
        {
            this.Id = id;

            Initialize();
        }

        private void Initialize()
        {
            this.DisplayText = "Notes";

            Notes = new List<ModuleBase>();
        }


        public List<ModuleBase> Notes { get; set; }
    }
}
