using NtccSteward.Core.Models.Common.Enums;
using NtccSteward.Core.Models.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Core.Models.Report
{
    public class ReportMetadata
    {
        public ReportMetadata()
        {
            Enums = new List<AppEnum>();
            Members = new List<Member>();
            Churches = new List<Church.Church>();
            Teams = new List<Team.Team>();
        }

        public List<AppEnum> Enums { get; set; }

        public List<Member> Members { get; set; }

        public List<Church.Church> Churches { get; set; }

        public List<Team.Team> Teams { get; set; }

    }
}