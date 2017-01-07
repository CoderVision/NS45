using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Interfaces.Enums
{
    public interface IAppEnum
    {
        int ID { get; set; }
        string Desc { get; set; }
        int AppEnumTypeID { get; set; }
        string AppEnumTypeName { get; set; }
        int SortOrder { get; set; }
    }
}
