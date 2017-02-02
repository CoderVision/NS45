using NtccSteward.Core.Models.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cm = NtccSteward.Core.Models.Members;

namespace NtccSteward.ViewModels.Member
{
    public class MemberIndexViewModel
    {
        public List<cm.Member> MemberList { get; set; }

        public List<AppEnum> MetaList { get; set; }

        public string Title { get; set; }
    }
}