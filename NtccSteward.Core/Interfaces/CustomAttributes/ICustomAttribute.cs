using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Interfaces.CustomAttributes
{
    public interface ICustomAttribute
    {
        int IdentityID { get; set; }

        int CustomAttrDefID { get; set; }

        string Value { get; set; }

        string Name { get; set; }

        string DataType { get; set; }

        int AttrTypeEnumID { get; set; }

        string AttrTypeEnumDesc { get; set; }

        bool IsEditable { get; set; }

    }
}
