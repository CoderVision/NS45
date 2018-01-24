using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Core.Models.Report
{
    public class ActiveGuestListReportData : IReportData
    {
        public ReportTypes ReportType { get; set; } = ReportTypes.ActiveGuestList;

        public int MemberId { get; set; }
        public string MemberName { get; set; }

        public int SponsorId { get; set; }
        public string SponsorName { get; set; }

        public int TeamId { get; set; }

        public string TeamName { get; set; }

        public string MemberAddress { get; set; }

        public string MemberEmail { get; set; }

        public string MemberPhone { get; set; }

        public int StatusId { get; set; }

        public string Status { get; set; }

        public DateTimeOffset? LastActivityDate { get; set; }

        public string Comments { get; set; }

        public int StatusChangeTypeId { get; set; }

        public string StatusChangeType { get; set; }
    }
}