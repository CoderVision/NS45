using NtccSteward.Core.Interfaces.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using NtccSteward.ViewModels.Church;

namespace NtccSteward.ViewModels.Account
{
    public class LoginVm : ILogin
    {
        public LoginVm()
        {
            ChurchList = new List<ChurchVm>();

            // temp list, add from database later.
            ChurchList.Add(new ChurchVm() { ChurchId = 3, Name = "Ntcc Graham" });
        }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Church")]
        public int ChurchId { get; set; } // user must select a church to log into

        [Display(Name = "Remember Me")]
        public bool Remember { get; set; }

        public List<ChurchVm> ChurchList { get; set; }
    }
}
