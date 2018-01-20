using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.IdentityServer.Models
{
    public class Login
    {
        //
        // Summary:
        //     The URL to POST credentials to for local logins. Will be null if local login
        //     is disabled. IdentityServer3.Core.ViewModels.LoginCredentials for the model for
        //     the submitted data.
        public string LoginUrl { get; set; }

        //
        // Summary:
        //     Indicates if "remember me" has been disabled and should not be displayed to the
        //     user.
        public bool AllowRememberMe { get; set; }
        //
        // Summary:
        //     The value to populate the "remember me" field.
        public bool RememberMe { get; set; }
        //
        // Summary:
        //     The value to populate the username field.
        public string Username { get; set; }
        //
        // Summary:
        //     The display name of the client.
        public string ClientName { get; set; }
        //
        // Summary:
        //     The URL for more information about the client.
        public string ClientUrl { get; set; }
        //
        // Summary:
        //     The URL for the client's logo image.
        public string ClientLogoUrl { get; set; }
    }
}