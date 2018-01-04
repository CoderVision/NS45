using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Core.Models.Members
{
    public class Activity
    {
        public int Id { get; set; }
        public int ChurchId { get; set; }

        // Id of the person initiating the activity, e.g., "soulwinner", etc.
        public int SourceId { get; set; }

        // Id of the person that was the focus of the activity, e.g., "contact" or "guest", etc.
        public int TargetId { get; set; }

        // ActivityTypeEnumId is the action that was performed (invited, called, etc.)
        public int ActivityTypeEnumID { get; set; }

        // ActivityResponseTypeEnumId is their response (positive, negative, neutral, no reponse)
        public int ActivityResponseTypeEnumID { get; set; }

        //MemberStatusChangeType 7  Provide a reason for changing the status of a person
        public int MemberStatusChangeTypeEnumId { get; set; }

        public int MemberStatusEnumId { get; set; }

        public string Note { get; set; }

        // set in the client
        public DateTimeOffset CreatedDate { get; set; }

        public DateTimeOffset ActivityDate { get; set; }
    }
}