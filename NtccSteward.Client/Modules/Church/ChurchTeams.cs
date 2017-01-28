using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Modules.Church
{
    public class ChurchTeams : ModuleBase
    {
        public ChurchTeams(int id)
        {
            this.Id = id;

            Initialize();
        }

        private void Initialize()
        {
            this.DisplayText = "Teams";

            Teams = new List<ModuleBase>();
        }

        public List<ModuleBase> Teams { get; set; }
    }
}
