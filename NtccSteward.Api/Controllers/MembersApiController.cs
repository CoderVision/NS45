
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using NtccSteward.Api.Repository;
using NtccSteward.Core.Models.Members;
using NtccSteward.Core.Models.Common.Parameters;
using NtccSteward.Core.Framework;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using NtccSteward.Api.Framework;
using System.Web.Http.Routing;
using Newtonsoft.Json;

namespace NtccSteward.Api.Controllers
{
    [RoutePrefix("api/member")]
    public class MembersApiController : ApiController
    {
        private readonly ILogger _logger;
        private readonly IMemberRepository _repository = null;

        //public MembersApiController(IAppConfigProvider appConfigProvider)
        public MembersApiController(IMemberRepository memberRepository, ILogger logger)
        {
            _repository = memberRepository;
            _logger = logger;
        }


        [Route("GetList")]
        [HttpGet]
        public IHttpActionResult GetList(int churchId, int statusId = 1, int page = 1, int pageSize = 10000)
        //public IHttpActionResult GetByStatus([FromBody] MembersByStatus status)
        {
            try
            {
                if (churchId == 0 || statusId == 0)
                    return BadRequest();

                var list = _repository.GetByStatus(churchId, statusId);

                if (list.Count == 0)
                {
                    return NotFound();
                }
                else
                {
                    var totalCount = list.Count();
                    var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);

                    // add pagination infor to the response header
                    var urlHelper = new UrlHelper(Request);
                    var prevLink = page > 1 ? urlHelper.Link("GetList"
                        , new {
                            churchId = churchId
                            , statusId = statusId
                            , page = page - 1
                            , pageSize = pageSize
                        }) : "";
                    var nextLink = page < totalPages ? urlHelper.Link("GetList"
                        , new
                        {
                            churchId = churchId
                            , statusId = statusId
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
            }
            catch (Exception ex)
            {
                new ErrorHelper(_logger).ProcessError(ex, nameof(GetList));

                return InternalServerError();
            }
        }


        //http://localhost:62428/api/member/GetContacts
        [Route("GetContactList")]
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

        // this worked with below
        [Route("GetProfile")]
        [HttpGet]
        public IHttpActionResult GetProfile(int id, bool includeMetadata = true)
        //public IHttpActionResult GetProfile([FromBody] ItemByID identity)
        {
            try
            {
                if (id <= 0)
                    return BadRequest();

                var memberProfile = _repository.GetProfile(id, includeMetadata);
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
                new ErrorHelper(_logger).ProcessError(ex, nameof(GetProfile));

                return InternalServerError();
            }
        }


        [Route("CreateMember")]
        [HttpPost]
        public IHttpActionResult CreateMember([FromBody] NewMember member)
        {
            try
            {
                if (member == null)
                    return BadRequest();

                _repository.Add(member);

                return Created(Request.RequestUri + "/" + member.id, member);
            }
            catch (Exception ex)
            {
                new ErrorHelper(_logger).ProcessError(ex, nameof(CreateMember));

                return InternalServerError();
            }
        }

        [Route("SaveMemberProfile")]
        [HttpPut]
        public IHttpActionResult SaveMemberProfile([FromBody] MemberProfile memberProfile)
        {
            try
            {
                if (memberProfile == null)
                    return BadRequest();

                _repository.SaveProfile(memberProfile);

                return Ok(memberProfile);
            }
            catch (Exception ex)
            {
                new ErrorHelper(_logger).ProcessError(ex, nameof(SaveMemberProfile));

                return InternalServerError();
            }
        }

        //// DELETE api/values/5
        [HttpDelete()]
        public IHttpActionResult DeleteMember(int id)
        {
            try
            {
                var success = this._repository.Delete(id);

                if (success)
                {
                    return StatusCode(HttpStatusCode.NoContent);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                new ErrorHelper(_logger).ProcessError(ex, nameof(DeleteMember));

                return InternalServerError();
            }
        }
    }
}
