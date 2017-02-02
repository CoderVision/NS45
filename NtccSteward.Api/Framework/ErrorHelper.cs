using NtccSteward.Repository;
using NtccSteward.Core.Framework;
using NtccSteward.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Api.Framework
{
    public class ErrorHelper
    {
        public ErrorHelper()
        { }

        public void ProcessError(ILogger _logger, Exception ex, string methodName)
        {
            var errorMsg = $"An error occurred in {methodName}";

            var errorMessage = ex.Message;
            var id = _logger.LogInfo(LogLevel.Error, "Error MembersApiController.SaveMemberProfile", errorMsg + ".\r\n\r\n" + ex.Message, 0);

            HttpContext.Current.Response.Headers.Add("ErrorId", id.ToString());
        }
    }
}