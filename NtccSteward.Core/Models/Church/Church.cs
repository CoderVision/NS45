using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Models.Church
{
    public class Church
    {
        public int id { get; set; } // specific name convention necessray for SlickGrid

        public string ChurchName { get; set; }

        public string Status { get; set; }

        public string StatusChangeType { get; set; }
    }
}
