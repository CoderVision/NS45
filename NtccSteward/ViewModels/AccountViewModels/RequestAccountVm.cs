using NtccSteward.Core.Interfaces.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.ViewModels.Account
{
    public class RequestAccountVm : IAccountRequest
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Church Name")]
        public string ChurchName { get; set; }

        [Display(Name = "Pastor Name")]
        public string PastorName { get; set; }

        [Display(Name = "Comments")]
        public string Comments { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }
        
    }
}
