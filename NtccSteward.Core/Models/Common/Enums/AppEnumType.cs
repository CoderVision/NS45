using NtccSteward.Core.Interfaces.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Models.Common.Enums
{
    public class AppEnumType : IAppEnumType
    {
        public int ID { get; set; }
        public int Name { get; set; }
    }
}
