using NtccSteward.Core.Interfaces.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Models.Common.CustomAttributes
{
    /// <summary>
    /// Custom Attribute Definition
    /// </summary>
    public class CustomAttributeDef : ICustomAttributeDef
    {
        public int CustomAttrDefID { get; set; }

        public int IdentityTypeEnumID { get; set; } // person or church, etc.

        public string IdentityTypeEnumDesc { get; set; }

        public string CustomAttrName { get; set; }

        public string DataType { get; set; }

        public string DefaultValue { get; set; }

        public bool IsEditable { get; set; }
    }
}
