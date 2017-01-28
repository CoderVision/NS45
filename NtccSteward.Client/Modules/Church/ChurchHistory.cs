using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Modules.Church
{
    public class ChurchHistory : ModuleBase
    {
        public ChurchHistory(int id)
        {
            this.Id = id;

            Initialize();
        }

        private void Initialize()
        {
            this.DisplayText = "History";

            History = new List<ModuleBase>();
        }


        public List<ModuleBase> History { get; set; }
    }
}
