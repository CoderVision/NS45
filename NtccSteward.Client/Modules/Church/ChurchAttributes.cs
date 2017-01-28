using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Modules.Church
{
    public class ChurchAttributes : ModuleBase
    {
        public ChurchAttributes(int id)
        {
            this.Id = id;

            Initialize();
        }

        private void Initialize()
        {
            this.DisplayText = "Attributes";

            Attributes = new List<ModuleBase>();
        }


        public List<ModuleBase> Attributes { get; set; }
    }
}
