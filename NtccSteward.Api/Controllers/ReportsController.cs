using NtccSteward.Api.Framework;
using NtccSteward.Core.Models.Report;
using NtccSteward.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NtccSteward.Api.Controllers
{
    [Authorize]
    public class ReportsController : ApiController
    {
        private readonly ILogger logger;
        private readonly IReportsRepository repository = null;

        public ReportsController(IReportsRepository repository, ILogger logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        [Route("reports/{reportId}")]
        [HttpGet]
        public IHttpActionResult Get(int reportId)
        {
            ReportTypes reportType;
            if (!Enum.TryParse(reportId.ToString(), out reportType))
                return BadRequest($"Report id '{reportId}' is not valid");

            try
            {
                var list = this.Request.GetQueryNameValuePairs().ToList();

                var reportData = this.repository.GetReportData(reportType, list);

                return Ok(reportData);
            }
            catch (Exception ex)
            {
                ErrorHelper.ProcessError(this.logger, ex, nameof(Get));

                return InternalServerError();
            }
        }


        [Route("reports/metadata/{churchId}")]
        [HttpGet]
        public IHttpActionResult GetMetadata(int churchId)
        {
            var metadata = this.repository.GetMetadata(churchId);

            var ret = new
            {
                StatusList = metadata.Enums.Where(i => i.AppEnumTypeName == "MemberStatus").ToArray(),
                MemberList = metadata.Members.Select(m => new { Id = m.id, Name=m.FullName, TeamId= m.TeamId }),
                TeamList = metadata.Teams.Select(t => new { Id = t.Id, Name = t.Name }),
                ChurchList = metadata.Churches.Select(c => new { Id = c.id, Name = c.Name })
            };

            return Ok(ret);
        }
    }
}
