using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Models.Common.Parameters
{
    public class GetRecipientGroupsParameter
    {
        public GetRecipientGroupsParameter()
        {        }

        public GetRecipientGroupsParameter(int churchID, int messageTypeEnumID, int maxReturnRows)
        {
            ChurchID = churchID;
            MessageTypeEnumID = messageTypeEnumID;
            MaxReturnRows = maxReturnRows;
        }

        public int ChurchID { get; set; }
        public int MessageTypeEnumID { get; set; }
        public int MaxReturnRows { get; set; }
    }
}
