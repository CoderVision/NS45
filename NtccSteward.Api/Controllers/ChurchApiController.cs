using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NtccSteward.Core.Framework;
using NtccSteward.Api.Repository;
using System.Net.Http;
using System.Web.Http;
using System.Net;
using System.Web;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace NtccSteward.Api.Controllers
{
    [RoutePrefix("api/church")]
    public class ChurchApiController : ApiController
    {
        private readonly ILogger _logger;
        private readonly IChurchRepository _repository = null;

        public ChurchApiController(IChurchRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // Changed from Post to Get
        [Route("GetList")]
        [HttpGet]
        public IHttpActionResult GetList()
        {
            try
            {
                var list = _repository.GetList();

                return Ok(list);
            }
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid().ToString();
                var errorMsg = $"An error occurred while creating the account request in the database.  [ErrorId: {errorId}] ";

                var errorMessage = ex.Message;
                _logger.LogInfo(LogLevel.Error, "Error Creating Account Request", errorMsg + ".\r\n\r\n" + ex.Message, 0);

                HttpContext.Current.Response.Headers.Add("ErrorId", errorId);

                return InternalServerError();
            }
        }
    }
}
