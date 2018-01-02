using NtccSteward.Core.Interfaces.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Models.Common.Enums
{
    public class AppEnum : IAppEnum
    {
        public int ID { get; set; }
        public string Desc { get; set; }
        public int AppEnumTypeID { get; set; }
        public string AppEnumTypeName { get; set; }
        public int OptionsEnumTypeID { get; set; }
        public int SortOrder { get; set; }
    }
}
