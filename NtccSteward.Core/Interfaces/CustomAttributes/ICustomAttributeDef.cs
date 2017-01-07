using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Interfaces.CustomAttributes
{
    public interface ICustomAttributeDef
    {
        int CustomAttrDefID { get; set; }

        int IdentityTypeEnumID { get; set; } // person or church, etc.

        string IdentityTypeEnumDesc { get; set; }

        string CustomAttrName { get; set; }

        string DataType { get; set; }

        string DefaultValue { get; set; }

        bool IsEditable { get; set; }
    }
}
