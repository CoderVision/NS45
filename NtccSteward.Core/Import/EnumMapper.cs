using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Core.Import
{
    public class EnumMap
    {
        public int ReasonId { get; set; }  //  Access Database Reason For Status Change ReasonId
        public int EnumId { get; set; }  // SQL Database Enum
        public string Description { get; set; }
    }
    public class EnumMapper
    {
        public EnumMapper()
        {
            Map = new List<EnumMap>();
            Map.Add(new EnumMap { ReasonId = 1, EnumId= 19, Description = "Not Interested" });
            Map.Add(new EnumMap { ReasonId = 2, EnumId = 20, Description = "Attends another church" });
            Map.Add(new EnumMap { ReasonId = 3, EnumId = 21, Description = "Moved/Sponsor finding new address" });
            Map.Add(new EnumMap { ReasonId = 4, EnumId = 22, Description = "Moved/Sponsor has no clue" });
            Map.Add(new EnumMap { ReasonId = 5, EnumId = 23, Description = "Coming regularly" });
            Map.Add(new EnumMap { ReasonId = 6, EnumId = 24, Description = "Guest will not answer door/phone" });
            Map.Add(new EnumMap { ReasonId = 7, EnumId = 25, Description = "Sponsor works on the job with them" });
            Map.Add(new EnumMap { ReasonId = 8, EnumId = 26, Description = "Sponsor is giving them space" });
            Map.Add(new EnumMap { ReasonId = 9, EnumId = 27, Description = "Sponsor's neighbor" });
            Map.Add(new EnumMap { ReasonId = 10, EnumId = 28, Description = "OFFICE USE ONLY" });
            Map.Add(new EnumMap { ReasonId = 11, EnumId = 29, Description = "Under age limit" });
            Map.Add(new EnumMap { ReasonId = 12, EnumId = 30, Description = "Address not valid" });
            Map.Add(new EnumMap { ReasonId = 13, EnumId = 31, Description = "Halfway house" });
            Map.Add(new EnumMap { ReasonId = 14, EnumId = 32, Description = "Mentally unstable" });
            Map.Add(new EnumMap { ReasonId = 15, EnumId = 33, Description = "Homeless" });
            Map.Add(new EnumMap { ReasonId = 16, EnumId = 34, Description = "DISAPPROVED - Change Rejected" });
            Map.Add(new EnumMap { ReasonId = 17, EnumId = 35, Description = "Guest keeps making excuses" });
            Map.Add(new EnumMap { ReasonId = 18, EnumId = 36, Description = "Moved out of state" });
            Map.Add(new EnumMap { ReasonId = 19, EnumId = 37, Description = "Sponsor is unable to contact" });
            Map.Add(new EnumMap { ReasonId = 20, EnumId = 38, Description = "Too far to come" });
            Map.Add(new EnumMap { ReasonId = 21, EnumId = 39, Description = "Military Trouble" });
            Map.Add(new EnumMap { ReasonId = 22, EnumId = 40, Description = "Joined Military" });
            Map.Add(new EnumMap { ReasonId = 23, EnumId = 41, Description = "Reactivated Guest" });
            Map.Add(new EnumMap { ReasonId = 24, EnumId = 42, Description = "Deployed" });
            Map.Add(new EnumMap { ReasonId = 25, EnumId = 43, Description = "In Jail" });
            Map.Add(new EnumMap { ReasonId = 26, EnumId = 44, Description = "Deceased" });
            Map.Add(new EnumMap { ReasonId = 27, EnumId = 45, Description = "Health Problems" });
        }
        public List<EnumMap> Map { get; set; }
    }
}