using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Core.Models.Import
{
    public class Reason {
        public int ReasonId { get; set; }
        public string Desc { get; set; }
        public int EnumId { get; set; }
    }
    public class ReasonMap
    {
        public ReasonMap()
        {
            AddReason(1, "Not Interested", 19);
            AddReason(2, "Attends another church", 20);
            AddReason(3, "Moved / Sponsor finding new address", 21);
            AddReason(4, "Moved / Sponsor has no clue", 22);
            AddReason(5, "Coming regularly", 23);
            AddReason(6, "Guest will not answer door / phone", 24);
            AddReason(7, "Sponsor works on the job with them", 25);
            AddReason(8, "Sponsor is giving them space", 26);
            AddReason(9, "Sponsor's neighbor", 27);
            AddReason(10, "OFFICE USE ONLY", 28);
            AddReason(11, "Under age limit", 29);
            AddReason(12, "Address not valid", 30);
            AddReason(13, "Halfway house", 31);
            AddReason(14, "Mentally unstable", 32);
            AddReason(15, "Homeless", 33);
            AddReason(16, "DISAPPROVED - Change Rejected", 34);
            AddReason(17, "Guest keeps making excuses", 35);
            AddReason(18, "Moved out of state", 36);
            AddReason(19, "Sponsor is unable to contact", 37);
            AddReason(20, "Too far to come", 38);
            AddReason(21, "Military Trouble", 39);
            AddReason(22, "Joined Military", 40);
            AddReason(23, "Reactivated Guest", 41);
            AddReason(24, "Deployed", 42);
            AddReason(25, "In Jail", 43);
            AddReason(26, "Deceased", 44);
            AddReason(27, "Health Problems", 45);
        }

        public List<Reason> Reasons { get; set; }

        private void AddReason(int reasonId, string desc, int enumId)
        {
            this.Reasons.Add(new Reason { ReasonId = reasonId, Desc = desc, EnumId = enumId });
        }
    }
}