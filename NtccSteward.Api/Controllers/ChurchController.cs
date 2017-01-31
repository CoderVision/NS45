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
using System.Web.Http.Routing;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace NtccSteward.Api.Controllers
{
    //[RoutePrefix("api")]
    public class ChurchController : ApiController
    {
        private readonly ILogger _logger;
        private readonly IChurchRepository _repository = null;

        public ChurchController(IChurchRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // Get list of churches
        [HttpGet]
        public IHttpActionResult Get(int page = 1, int pageSize = 10000, bool showAll = false)
        {
            try
            {
                var list = _repository.GetList(showAll);

                var totalCount = list.Count();
                var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);

                // add pagination infor to the response header
                var urlHelper = new UrlHelper(Request);
                var prevLink = page > 1 ? urlHelper.Link("Get"
                    , new
                    {
                        page = page - 1,
                        pageSize = pageSize,
                        showAll =showAll
                    }) : "";
                var nextLink = page < totalPages ? urlHelper.Link("Get"
                    , new
                    {
                        page = page + 1,
                        pageSize = pageSize,
                        showAll = showAll
                    }) : "";
                var paginationHeader = new
                {
                    currentPage = page,
                    pageSize = pageSize,
                    totalCount = totalCount,
                    totalPages = totalPages,
                    previousPageLink = prevLink,
                    nextPageLink = nextLink
                };

                HttpContext.Current.Response.Headers.Add("X-Pagination"
                    , JsonConvert.SerializeObject(paginationHeader));

                return Ok(list
                    .Skip(pageSize * (page - 1))
                    .Take(pageSize));
            }
            catch (Exception ex)
            {
                new ErrorHelper().ProcessError(_logger, ex, nameof(Get));

                return InternalServerError();
            }
        }

        [Route("church/metadata")]
        [HttpGet]
        public IHttpActionResult GetProfileMetadata(int churchId)
        {
            return Ok(_repository.GetProfileMetadata(churchId));
        }
    }
}
