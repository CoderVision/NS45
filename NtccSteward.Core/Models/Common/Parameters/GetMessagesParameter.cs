using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Models.Common.Parameters
{
    public class GetMessagesParameter
    {
        public GetMessagesParameter()
        {        }

        public GetMessagesParameter(int correspondenceID, int messageTypeEnumID, int maxReturnRows)
        {
            this.CorrespondenceID = correspondenceID;
            this.MessageTypeEnumID = messageTypeEnumID;
            MaxReturnRows = maxReturnRows;
        }

        public int CorrespondenceID { get; set; }  // this is a problem.  Should not be Member ID, but correspondence id or identity id 

        public int MessageTypeEnumID { get; set; }

        public int MaxReturnRows { get; set; }
    }
}
