using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Models.Church
{
    public class Church
    {
        public int id { get; set; } // specific name convention necessray for SlickGrid

        public string Name { get; set; }

        public int Status { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public int PastorId { get; set; }

        public string Pastor { get; set; }
    }
}
