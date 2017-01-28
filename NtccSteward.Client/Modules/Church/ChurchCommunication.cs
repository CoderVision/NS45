using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Modules.Church
{
    public class ChurchCommunication : ModuleBase
    {
        public ChurchCommunication(int id)
        {
            this.Id = id;

            Initialize();
        }


        private void Initialize()
        {
            this.DisplayText = "Communication";

            Protocols = new List<ModuleBase>();
        }


        public List<ModuleBase> Protocols { get; set; }
    }
}
