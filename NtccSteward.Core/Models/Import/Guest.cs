using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Core.Models.Import
{
    public class Guest
    {
        public int GuestId { get; set; }
        public bool FollowUp { get; set; }
        public int AssocId { get; set; }  // Associate Pastor
        public bool LetterMailed { get; set; }  // was the letter mailed?
        public bool IsNew { get; set; }  // Is First Time visitor
        public string CurrentStatus { get; set; } // A (Active), I (Inactive), F (Faithful)
        public DateTime? DateCameToChurch { get; set; }
        public int SponsorId { get; set; }  // Soulwinner Id
        public bool MultipleGuestsFirstVisit { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool NeedsPastorFollowUp { get; set; } // is this "needs a pastor to visit", or "the pastor follow-ed up", or "pastor did has pastoral visit", etc.
        public string AddressLine1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Phone2 { get; set; }
        public bool Prayed { get; set; } // did this person pray for salvation?
        public string Note { get; set; }
        public DateTime? DateChanged { get; set; }
        public int ReasonForChange { get; set; }
        public string OldStatus { get; set; }
        public string NewStatus { get; set; }
        public bool Changed { get; set; }
        public bool PendingBaptism { get; set; }
        public bool HasBeenBaptized { get; set; }
        public bool IsLayPastor { get; set; }  // are they a laypastor?
        public string Email { get; set; }
        public int LetterTranslation { get; set; }  // 1 = English, 2 = Spanish

        public int IdentityId { get; set; }
    }
}