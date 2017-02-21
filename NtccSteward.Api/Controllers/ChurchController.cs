using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NtccSteward.Core.Framework;
using NtccSteward.Repository;
using System.Net.Http;
using System.Web.Http;
using System.Net;
using System.Web;
using NtccSteward.Api.Framework;
using System.Web.Http.Routing;
using Newtonsoft.Json;
using NtccSteward.Repository.Framework;
using NtccSteward.Core.Models.Church;

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
        public IHttpActionResult Get(int page, int pageSize, bool showAll = false)
        {
            try
            {
                if (page == 0)
                    page = 1;

                if (pageSize == 0)
                    pageSize = 1000;

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
                ErrorHelper.ProcessError(_logger, ex, nameof(Get));

                return InternalServerError();
            }
        }


        [Route("church/metadata")]
        [HttpGet]
        public IHttpActionResult GetProfileMetadata()
        {
            return Ok(_repository.GetProfileMetadata());
        }

        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest();

                var profile = _repository.Get(id);
                if (profile == null)
                {
                    return NotFound();
                }

                return Ok(profile);
            }
            catch (Exception ex)
            {
                ErrorHelper.ProcessError(_logger, ex, nameof(Get));

                return InternalServerError();
            }
        }

        public IHttpActionResult Put(int id, ChurchProfile church)
        {
            try
            {
                if (church == null)
                    return BadRequest("church required");

                if (id != church.Id)
                    return BadRequest("id and church.id must match");

                var result = _repository.SaveProfile(church);

                if (result.Status == RepositoryActionStatus.Updated)
                    return Ok(church);
                else if (result.Status == RepositoryActionStatus.NotFound)
                    return NotFound();
                else
                    return BadRequest();
            }
            catch (Exception ex)
            {
                ErrorHelper.ProcessError(_logger, ex, nameof(Delete));

                return InternalServerError();
            }
        }

        // create
        [HttpPost]
        public IHttpActionResult Post(Church church)
        {
            try
            {
                if (church == null)
                    return BadRequest("church required");

                var result = _repository.Add(church);

                if (result.Status == RepositoryActionStatus.Created)
                    return Created(Request.RequestUri + "/" + result.Entity.id.ToString(), result.Entity);

                return BadRequest();
            }
            catch (Exception ex)
            {
                ErrorHelper.ProcessError(_logger, ex, nameof(Delete));

                return InternalServerError();
            }
        }

        /// <summary>
        /// Deletes a church or contact info
        /// </summary>
        /// <param name="id">Church.IdentityId</param>
        /// <param name="entityType">EnumTypeId=12:  55 church, 61 ContactInfo</param>
        /// <returns></returns>
        [HttpDelete()]
        public IHttpActionResult Delete(int id, int entityType)
        {
            try
            {
                var result = this._repository.Delete(id, entityType);

                if (result.Status == RepositoryActionStatus.Deleted)
                {
                    return StatusCode(HttpStatusCode.NoContent);
                }
                else if (result.Status == RepositoryActionStatus.NotFound)
                {
                    return NotFound();
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                ErrorHelper.ProcessError(_logger, ex, nameof(Delete));

                return InternalServerError();
            }
        }

    }
}
