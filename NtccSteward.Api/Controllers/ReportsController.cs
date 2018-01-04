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
    public class ReportsController : ApiController
    {
        private readonly ILogger logger;
        private readonly IReportsRepository reportsRepository = null;

        public ReportsController(IReportsRepository reportsRepository, ILogger logger)
        {
            this.reportsRepository = reportsRepository;
            this.logger = logger;
        }

        [Route("reports/{reportId}/church/{churchId}")]
        [HttpGet]
        public IHttpActionResult Get(int reportId, int churchId)
        {
            ReportTypes reportType;
            if (!Enum.TryParse(reportId.ToString(), out reportType))
                return BadRequest($"Report id '{reportId}' is not valid");

            try
            {
                var reportData = this.reportsRepository.GetReportData(reportType, churchId);

                return Ok(reportData);
            }
            catch (Exception ex)
            {
                ErrorHelper.ProcessError(this.logger, ex, nameof(Get));

                return InternalServerError();
            }
        }
    }
}
