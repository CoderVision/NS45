using NtccSteward.Core.Interfaces.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Models.Common.CustomAttributes
{
    public class CustomAttribute : ICustomAttribute
    {
        public int IdentityID { get; set; }

        public int CustomAttrDefID { get; set; }

        public string Value { get; set; }

        public string Name { get; set; }

        public string DataType { get; set; }

        public int AttrTypeEnumID { get; set; }

        public string AttrTypeEnumDesc { get; set; }

        public bool IsEditable { get; set; }
    }
}
