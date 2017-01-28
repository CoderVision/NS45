using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Modules
{
    public abstract class ModuleBase
    {
        public int Id { get; set; }

        public string DisplayText { get; set; }
    }
}
