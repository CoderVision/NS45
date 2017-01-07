using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Models.Common.Parameters
{
    public class ItemByID
    {
        public ItemByID(int id, bool includeMetadata)
        {
            this.ID = id;
            this.IncludeMetadata = includeMetadata;
        }
        public int ID { get; set; }

        public bool IncludeMetadata { get; set; }
    }
}