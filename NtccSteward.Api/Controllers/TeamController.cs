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

        /// <summary>
        /// Gets a list of all Teams for the specified church.
        /// </summary>
        /// <param name="churchId"></param>
        /// <returns></returns>
        public IHttpActionResult Get(int churchId)
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
                    return BadRequest("Invalid churchId");

                var team = _repository.GetTeammates(teamId);

                return Ok(team);
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }
    }
}
