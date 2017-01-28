using NtccSteward.Core.Interfaces.Messages;
using NtccSteward.Core.Models.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.ViewModels.Message
{

    // maybe this should be something like "Contact", and it should have a "Type" (Group or individual)
    /// <summary>
    /// A Group represents a group of persons (church member, etc.).
    /// </summary>
    public class GroupVm : IRecipientGroup
    {
        public GroupVm()
        {

        }

        public GroupVm(IRecipientGroup recipientGroup)
        {
            this.ID = recipientGroup.ID;
            this.Name = recipientGroup.Name;
            this.Description = recipientGroup.Description;
            this.ChurchId = recipientGroup.ChurchId;
        }

        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int ChurchId { get; set; }
    }
}
