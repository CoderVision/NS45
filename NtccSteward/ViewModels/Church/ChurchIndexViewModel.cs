using NtccSteward.Core.Models.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cm = NtccSteward.Core.Models.Church;

namespace NtccSteward.ViewModels.Church
{
    public class ChurchIndexViewModel
    {
        public List<cm.Church> ChurchList { get; set; }

        public List<AppEnum> MetaList { get; set; }
    }
}