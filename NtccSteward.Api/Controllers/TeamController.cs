using Marvin.JsonPatch;
using NtccSteward.Api.Framework;
using NtccSteward.Core.Framework;
using NtccSteward.Core.Models.Team;
using NtccSteward.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NtccSteward.Repository.Controllers
{
    [Route("Teams")]
    [Authorize]
    public class TeamController : ApiController
    {
        private readonly ILogger _logger;
        private readonly ITeamRepository _repository = null;

        public TeamController(ITeamRepository memberRepository, ILogger logger)
        {
            _repository = memberRepository;
            _logger = logger;
        }

        [Route("church/{churchId}/teams")]
        [HttpGet]
        /// <summary>
        /// Gets a list of all Teams for the specified church.
        /// </summary>
        /// <param name="churchId"></param>
        /// <returns></returns>
        public IHttpActionResult GetChurchTeamList(int churchId)
        {
            try
            {
                if (churchId <= 0)
                    return BadRequest("Invalid churchId");

                var list = _repository.GetList(churchId);

                return Ok(list);
            }
            catch (Exception ex)
            {
                ErrorHelper.ProcessError(_logger, ex, nameof(GetChurchTeamList));

                return InternalServerError();
            }
        }


        [Route("teams/{id}/profile")]
        [HttpGet]
        public IHttpActionResult GetProfile(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid id");

                var team = _repository.GetTeam(id);

                return Ok(team);
            }
            catch (Exception ex)
            {
                ErrorHelper.ProcessError(_logger, ex, nameof(GetProfile));

                return InternalServerError();
            }
        }

        [Route("church/{churchId}/teams/metadata")]
        [HttpGet]
        public IHttpActionResult GetMetadata(int churchId)
        {
            try
            {
                var userId = TokenIdentityHelper.GetOwnerIdFromToken();

                var response = _repository.GetMetadata(churchId, userId);

                var metadata = response.Entity;

                var ret = new
                {
                    MemberList = metadata.Members.Select(m => new { Id = m.id, Desc = $"{m.FirstName} {m.LastName}".Trim() }),
                    TeamTypes = metadata.EnumTypes,
                    TeamEnums = metadata.Enums,
                    ChurchList = metadata.Churches.Select(c => new { Id = c.id, Name = c.Name }),
                };

                return Ok(ret);
            }
            catch (Exception ex)
            {
                ErrorHelper.ProcessError(_logger, ex, nameof(GetMetadata));

                return InternalServerError();
            }
        }

        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid id");

                var response = _repository.DeleteTeam(id);

                if (response.Status == Framework.RepositoryActionStatus.Deleted)
                    return StatusCode(HttpStatusCode.NoContent);
                else if (response.Status == Framework.RepositoryActionStatus.NotFound)
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


        // add new
        [HttpPost]
        public IHttpActionResult Post(TeamInfo team)
        {
            try
            {
                var result = _repository.SaveTeam(team);

                if (result.Status == Framework.RepositoryActionStatus.Created)
                {
                    return Created(Request.RequestUri + "/" + result.Entity.Id, result.Entity);
                }
                else if (result.Status == Framework.RepositoryActionStatus.Ok)
                {
                    return Ok(result.Entity);
                }
                else
                    return BadRequest();

            }
            catch (Exception ex)
            {
                ErrorHelper.ProcessError(_logger, ex, nameof(Post));

                return InternalServerError();
            }
        }

        /// <summary>
        /// Gets teammates for a specific a specific team
        /// </summary>
        /// <param name="churchId"></param>
        /// <returns></returns>
        /// 
        [Route("teams/{teamId}/teammates")]
        [HttpGet]
        public IHttpActionResult GetTeammates(int teamId)
        {
            try
            {
                if (teamId <= 0)
                    return BadRequest("Invalid teamId");

                var team = _repository.GetTeammates(teamId);

                return Ok(team);
            }
            catch (Exception ex)
            {
                ErrorHelper.ProcessError(_logger, ex, nameof(GetTeammates));

                return InternalServerError();
            }
        }

        [Route("teams/{teamId}/teammates")]
        [HttpPost]
        public IHttpActionResult PostTeammates(int teamId, Teammate teammate)
        {
            try
            {
                if (teamId <= 0)
                    return BadRequest("Invalid teamId");

                teammate.TeamId = teamId;

                var result = _repository.SaveTeammate(teammate);

                  return Ok(result.Entity);
            }
            catch (Exception ex)
            {
                ErrorHelper.ProcessError(_logger, ex, nameof(GetTeammates));

                return InternalServerError();
            }
        }


        [Route("teams/{teamId}/teammates/{memberId}")]
        [HttpDelete]
        public IHttpActionResult DeleteTeammate(int teamId, int memberId)
        {
            try
            {
                if (teamId <= 0)
                    return BadRequest("Invalid teamId");

                if (memberId <= 0)
                    return BadRequest("Invalid memberId");

                var team = _repository.DeleteTeammate(teamId, memberId);

                if (team.Status == Framework.RepositoryActionStatus.Deleted)
                    return StatusCode(HttpStatusCode.NoContent);
                else if (team.Status == Framework.RepositoryActionStatus.NotFound)
                    return NotFound();
                else
                    return BadRequest();
            }
            catch (Exception ex)
            {
                ErrorHelper.ProcessError(_logger, ex, nameof(DeleteTeammate));

                return InternalServerError();
            }
        }

        [Route("teams/{teamId}")]
        [HttpPatch]
        public IHttpActionResult Patch(int teamId, [FromBody]JsonPatchDocument<Team> doc)
        {
            if (doc == null)
                return BadRequest();

            try
            {
                var profile = _repository.GetTeam(teamId);

                if (profile == null)
                    return NotFound();

                doc.ApplyTo(profile);

                _repository.SaveTeam(profile);

                foreach (var teammate in profile.Teammates)
                    _repository.SaveTeammate(teammate);

                return Ok(profile);
            }
            catch (Exception ex)
            {
                ErrorHelper.ProcessError(_logger, ex, nameof(Patch));

                return InternalServerError();
            }
        }
    }
}
