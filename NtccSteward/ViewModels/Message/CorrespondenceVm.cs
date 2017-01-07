using NtccSteward.Core.Interfaces.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.ViewModels.Message
{
    /// <summary>
    /// A Correspondence represents a person (church member, etc.) and all of their messages
    /// </summary>
    public class CorrespondenceVm : ICorrespondence
    {
        public CorrespondenceVm()
        {        }

        public CorrespondenceVm(ICorrespondence correspondence)
        {
            this.ID = correspondence.ID;
            this.Name = correspondence.Name;
            this.Description = correspondence.Description;
        }

        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
