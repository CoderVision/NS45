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
using NtccSteward.Api.Framework;

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
                new ErrorHelper().ProcessError(_logger, ex, nameof(GetList));

                return InternalServerError();
            }
        }
    }
}
