using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Models.Members
{
    public class NewMember
    {
        public int id { get; set; }

        public DateTime DateCame { get; set; }

        public bool IsGroup { get; set; }

        public bool Prayed { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Line1 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string Phone { get; set; }

        public string Phone2 { get; set; }

        public string Email { get; set; }

        public List<Sponsor> SponsorList { get; set; }

        public int CreatedByUserId { get; set; }

        public int ChurchId { get; set; }
    }
}
