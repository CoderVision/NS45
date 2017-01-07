using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Modules.Church
{
    public class ChurchNotes : ModuleBase
    {
            public ChurchNotes(int id)
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
