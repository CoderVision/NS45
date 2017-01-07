using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Modules.Members
{
    public class MemberContacts : ModuleBase
    {
        public MemberContacts(int id)
        {
            Initialize();

            this.Id = id;
        }

        private void Initialize()
        {
            this.DisplayText = "Contacts";

            Contacts = new List<ModuleBase>();
        }


        public List<ModuleBase> Contacts { get; set; }
    }
}
