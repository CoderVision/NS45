using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Interfaces.Messages
{
    public interface ICorrespondence
    {
        int ID { get; set; }

        string Name { get; set; }

        string Description { get; set; }
    }
}
