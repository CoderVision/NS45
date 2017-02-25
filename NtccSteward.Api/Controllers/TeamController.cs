using NtccSteward.Api.Framework;
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
    public class TeamController : ApiController
    {
        private readonly ILogger _logger;
        private readonly ITeamRepository _repository = null;

        public TeamController(ITeamRepository memberRepository, ILogger logger)
        {
            _repository = memberRepository;
            _logger = logger;
        }

        [Route("church/{churchId}/team")]
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


        public IHttpActionResult Get(int id)
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
                ErrorHelper.ProcessError(_logger, ex, nameof(Get));

                return InternalServerError();
            }
        }

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
                ErrorHelper.ProcessError(_logger, ex, nameof(Get));

                return InternalServerError();
            }
        }


        // add new
        public IHttpActionResult Post(TeamInfo team)
        {
            try
            {
                var result = _repository.CreateTeam(team);

                if (result.Status == Framework.RepositoryActionStatus.Created)
                {

                    return Created(Request.RequestUri + "/" + result.Entity.Id, result.Entity);
                }
                else
                    return BadRequest();

            }
            catch (Exception ex)
            {
                ErrorHelper.ProcessError(_logger, ex, nameof(Get));

                return InternalServerError();
            }
        }

        /// <summary>
        /// Gets teammates for a specific a specific team
        /// </summary>
        /// <param name="churchId"></param>
        /// <returns></returns>
        /// 
        [Route("team/{teamId}/teammates")]
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


        [Route("team/{teamId}/teammates/{teammateId}")]
        public IHttpActionResult DeleteTeammate(int teamId, int teammateId)
        {
            try
            {
                if (teamId <= 0)
                    return BadRequest("Invalid teamId");

                if (teammateId <= 0)
                    return BadRequest("Invalid teammateId");

                var team = _repository.DeleteTeammate(teamId, teammateId);

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
    }
}
