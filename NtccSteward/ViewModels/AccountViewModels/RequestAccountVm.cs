using NtccSteward.Core.Interfaces.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ch = NtccSteward.Core.Models.Church;

namespace NtccSteward.ViewModels.Account
{
    public class RequestAccountVm : IAccountRequest
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Church")]
        public int ChurchId { get; set; }

        [Display(Name = "Church Name")]
        public string ChurchName { get; set; }

        [Required]
        [Display(Name = "Pastor Name")]
        public string PastorName { get; set; }

        [Display(Name = "Comments")]
        public string Comments { get; set; }

        [Required]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [Display(Name = "State")]
        public string State { get; set; }

        public int RequestId { get; set; }

        public List<ch.Church> ChurchList { get; set; }
    }
}
