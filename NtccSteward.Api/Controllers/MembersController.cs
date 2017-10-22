
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using NtccSteward.Core.Models.Members;
using NtccSteward.Core.Models.Common.Parameters;
using NtccSteward.Core.Framework;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using NtccSteward.Api.Framework;
using System.Web.Http.Routing;
using Newtonsoft.Json;
using System.Web.Http.Cors;
using NtccSteward.Repository.Framework;
using NtccSteward.Repository;
using Marvin.JsonPatch;

namespace NtccSteward.Api.Controllers
{
    // this can be added to the WebApiConfig, or to a class specifically.
    //  you can enter specific domains separated by a comma, or a * for all
    //[RoutePrefix("api")]
    public class MembersController : ApiController
    {
        private readonly ILogger _logger;
        private readonly IMemberRepository _repository = null;

        public MembersController(IMemberRepository memberRepository, ILogger logger)
        {
            _repository = memberRepository;
            _logger = logger;
        }

        /// <summary>
        /// Gets a list of members for the specified church
        /// </summary>
        /// <param name="churchId">Church ID</param>
        /// <param name="statusIds">EnumID separated by a dash -, e.g. (49-50)</param>
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns></returns>
        //[VersionedRoute("members/Get", 1)] // indicates Version 1 of the route.
        [HttpGet]
        public IHttpActionResult Get(int churchId, string statusIds, int page = 1, int pageSize = 10000)
        {
            try
            {
                if (churchId == 0)
                    return BadRequest();

                if (string.IsNullOrWhiteSpace(statusIds))
                    statusIds = "49-50";

                var idlist = statusIds.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                var list = _repository.GetList(churchId, Array.ConvertAll<string,int>(idlist, int.Parse));

                var totalCount = list.Count();
                var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);

                // add pagination infor to the response header
                var urlHelper = new UrlHelper(Request);
                var prevLink = page > 1 ? urlHelper.Link("Get"
                    , new {
                        churchId = churchId
                        , statusIds = statusIds
                        , page = page - 1
                        , pageSize = pageSize
                    }) : "";
                var nextLink = page < totalPages ? urlHelper.Link("Get"
                    , new
                    {
                        churchId = churchId
                        , statusIds = statusIds
                        , page = page + 1
                        , pageSize = pageSize
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
                    .Skip(pageSize * (page-1))
                    .Take(pageSize));
            }
            catch (Exception ex)
            {
                ErrorHelper.ProcessError(_logger, ex, nameof(Get));

                return InternalServerError();
            }
        }


        [Route("members/metadata")]
        [HttpGet]
        public IHttpActionResult GetProfileMetadata(int churchId)
        {
            return Ok(_repository.GetProfileMetadata(churchId));
        }


        // this worked with below
        //[Route("members/{id}/profile")]
        //could also have:  [Route("members/{id}/history")] for history, etc.
        [HttpGet]
        public IHttpActionResult Get(int id, int churchId)
        //public IHttpActionResult GetProfile([FromBody] ItemByID identity)
        {
            try
            {
                if (id <= 0)
                    return BadRequest();

                var memberProfile = _repository.Get(id, churchId);
                if (memberProfile == null)
                {
                    return NotFound();
                }

                // The method returns ObjectResult if it finds a ToDo item with a matching ID. 
                // Returning ObjectResult is equivalent to returning the CLR model, but it makes the return type IActionResult. 
                // That way, the method can return a different action result for other code paths.
                return Ok(memberProfile);
            }
            catch (Exception ex)
            {
                ErrorHelper.ProcessError(_logger, ex, nameof(Get));

                return InternalServerError();
            }
        }


        //http://localhost:62428/api/member/GetContacts
        [Route("members/GetContactList")]
        [HttpGet]
        public IHttpActionResult GetContactList()
        {
            //var list = repository.GetByStatus(status.ChurchID, status.StatusID);
            var list = new List<Member>();
            list.Add(CreateMemberTemp("Curtis", "Morgan", 1));
            list.Add(CreateMemberTemp("Mike", "Tickler", 2));
            list.Add(CreateMemberTemp("John", "Conner", 3));
            list.Add(CreateMemberTemp("Dave", "Bean", 4));
            list.Add(CreateMemberTemp("Kurt", "Anderson", 5));

            return Ok(list);
        }


        private Member CreateMemberTemp(string firstName, string lastName, int id)
        {
            var m = new Member();

            m.id = id;
            m.FirstName = firstName;
            m.LastName = lastName;
            m.Status = "Active";
            m.StatusChangeType = "Changed";

            return m;
        }


        /*
        POST http://localhost:58648/api/members/GetById

        User-Agent: Fiddler
        Host: localhost:58648
        Content-Length: 8
        Content-Type: application/json

        { ID:4 }    
        */




        // Create Member
        [HttpPost]
        public IHttpActionResult Post([FromBody] NewMember member)
        {
            try
            {
                if (member == null)
                    return BadRequest();

                var result = _repository.Add(member);

                if (result.Status == RepositoryActionStatus.Created)
                {
                    return Created(Request.RequestUri + "/" + member.id, member);
                }
                else if (result.Status == RepositoryActionStatus.NotFound)
                {
                    return NotFound();
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                ErrorHelper.ProcessError(_logger, ex, nameof(Post));

                return InternalServerError();
            }
        }

        [HttpPut]
        public IHttpActionResult Put(int id, [FromBody] MemberProfile memberProfile)
        {
            try
            {
                if (memberProfile == null)
                    return BadRequest();

                var result = _repository.SaveProfile(memberProfile);

                if (result.Status == RepositoryActionStatus.Updated)
                {
                    return Ok(memberProfile);
                }
                else if (result.Status == RepositoryActionStatus.NotFound)
                {
                    return NotFound();
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                ErrorHelper.ProcessError(_logger, ex, nameof(Put));

                return InternalServerError();
            }
        }

        /// <summary>
        /// Deletes a person or contact info
        /// </summary>
        /// <param name="id">Person.IdentityId</param>
        /// <param name="entityType">EnumTypeId=12:  56 person, 61 ContactInfo</param>
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

        [HttpPatch]
        public IHttpActionResult Patch(int id, int churchId, [FromBody]JsonPatchDocument<MemberProfile> doc)
        {
            if (doc == null)
                return BadRequest();

            try
            {
                var profile = _repository.Get(id, churchId);

                if (profile == null)
                    return NotFound();

                doc.ApplyTo(profile);

                _repository.SaveProfile(profile);

                return Ok(profile);
            }
            catch(Exception ex)
            {
                ErrorHelper.ProcessError(_logger, ex, nameof(Patch));

                return InternalServerError();
            }
        }
    }
}
