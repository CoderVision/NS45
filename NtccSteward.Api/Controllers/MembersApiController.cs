
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


        [Route("GetByStatus")]
        [HttpGet]
        public IHttpActionResult GetByStatus(int churchId, int statusId)
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
                    return Ok(list);
            }
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid().ToString();
                var errorMsg = $"An error occurred while MembersApiController.GetByStatus in the database.  [ErrorId: {errorId}] ";

                var errorMessage = ex.Message;
                _logger.LogInfo(LogLevel.Error, "Error MembersApiController.GetByStatus", errorMsg + ".\r\n\r\n" + ex.Message, 0);

                HttpContext.Current.Response.Headers.Add("ErrorId", errorId);

                return InternalServerError();
            }

        }

        //http://localhost:62428/api/member/GetContacts
        [Route("GetContacts")]
        [HttpGet]
        public IHttpActionResult GetContacts()
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


        // consider options:
        //string url = Url.RouteUrl("GetByIdRoute", new { Id = member.Id }, Request.Scheme, Request.Host.ToUriComponent());
        //HttpContext.Response.StatusCode = 201;  // created
        //HttpContext.Response.Headers["Location"] = url;

        ////[HttpPost("GetByStatus")]
        //////public IEnumerable<Member>
        ////public JsonResult GetByStatus([FromBody] MembersByStatus status) // this worked with above
        ////{
        ////    var member = new Member();
        ////    member.FirstName = "Gary";
        ////    member.LastName = "Lima";
        ////    member.Id = 1;
        ////    member.Status = "Active";
        ////    var list = new List<Member>();
        ////    list.Add(member);

        ////    return Json(list);
        ////}


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
                var errorId = Guid.NewGuid().ToString();
                var errorMsg = $"An error occurred while MembersApiController.GetProfile in the database.  [ErrorId: {errorId}] ";

                var errorMessage = ex.Message;
                _logger.LogInfo(LogLevel.Error, "Error MembersApiController.GetProfile", errorMsg + ".\r\n\r\n" + ex.Message, 0);

                HttpContext.Current.Response.Headers.Add("ErrorId", errorId);

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
                // log exception, return internal server error

                var errorId = Guid.NewGuid().ToString();
                var errorMsg = $"An error occurred while MembersApiController.CreateMember in the database.  [ErrorId: {errorId}] ";

                var errorMessage = ex.Message;
                _logger.LogInfo(LogLevel.Error, "Error MembersApiController.CreateMember", errorMsg + ".\r\n\r\n" + ex.Message, 0);

                HttpContext.Current.Response.Headers.Add("ErrorId", errorId);

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
                // log exception, return internal server error

                var errorId = Guid.NewGuid().ToString();
                var errorMsg = $"An error occurred while MembersApiController.SaveMemberProfile in the database.  [ErrorId: {errorId}] ";

                var errorMessage = ex.Message;
                _logger.LogInfo(LogLevel.Error, "Error MembersApiController.SaveMemberProfile", errorMsg + ".\r\n\r\n" + ex.Message, 0);

                HttpContext.Current.Response.Headers.Add("ErrorId", errorId);

                return InternalServerError();
            }
        }

        //// DELETE api/values/5
        //[HttpPost("{id}")]
        //public IActionResult DeleteMember(int id)
        //{
        //    var success = this.repository.TryDelete(id);

        //    if (success)
        //    {
        //        return Ok();
        //    }
        //    else
        //    {
        //        return new ContentResult() { Content = "Unable to delete member" };
        //    }
        //}
    }
}
