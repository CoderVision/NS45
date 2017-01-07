using NtccSteward.Core.Interfaces.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Models.Message
{
    public class Correspondence : ICorrespondence
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
