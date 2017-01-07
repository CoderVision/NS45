using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Framework
{
    public class PostResponse
    {
        public PostResponse(string redirectUrl, string errorMessage, bool success)
        {
            RedirectUrl = redirectUrl;
            ErrorMessage = errorMessage;
            Success = success;
        }

        public string RedirectUrl { get; set; }

        public string ErrorMessage { get; set; }

        public bool Success { get; set; }
    }
}
