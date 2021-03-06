﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NtccSteward.Core.Models;
using NtccSteward.Repository;
using NtccSteward.Core.Framework;
using System.Text;
using NtccSteward.Api.Framework;
using System.Net;
using NtccSteward.Core.Models.Account;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using Marvin.JsonPatch;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace NtccSteward.Api.Controllers
{
    //[RoutePrefix("api")]
    public class AccountController : ApiController
    {
        private readonly IAccountRepository repository = null;
        private readonly ILogger logger;
        private readonly IChurchRepository churchRepository;

        public AccountController(IAccountRepository repository, IChurchRepository churchRepository, ILogger logger)
        {
            this.repository = repository;
            this.logger = logger;
            this.churchRepository = churchRepository;
        }


        [Route("account/GetUsers/{active}")]
        [HttpGet]
        public IHttpActionResult GetAccountRequests(bool active)
        {
            try
            {
                var userId = TokenIdentityHelper.GetOwnerIdFromToken();

                var acctRequests = this.repository.GetAccountRequests();
                var roles = this.repository.GetRoles();
                var users = this.repository.GetUserProfiles(active);
                var churches = this.churchRepository.GetList(false, userId);

                var usersInfo = new
                {
                    config = new { churches = churches, roles = roles },
                    acctRequests = acctRequests,
                    users = users,
                };

                return Ok(usersInfo);
            }
            catch (Exception ex)
            {
                ErrorHelper.ProcessError(this.logger, ex, nameof(GetAccountRequests));

                return InternalServerError(ex);
            }
        }

        [Route("account")]
        [HttpPost]
        public IHttpActionResult PostUser(UserProfile userProfile)
        {
            if (userProfile == null)
                return BadRequest();

            try
            {
                var profile = this.repository.SaveUserProfile(userProfile);

                if (profile == null)
                    return NotFound();

                return Ok(profile);
            }
            catch (Exception ex)
            {
                ErrorHelper.ProcessError(this.logger, ex, nameof(PostUser));

                return InternalServerError();
            }
        }

        [Route("account/processAccountRequest")]
        [HttpPost]
        public IHttpActionResult ProcessAccountRequest(AccountRequest accountRequest)
        {
            try
            {
                // example of how to get the user's id
                var userId = TokenIdentityHelper.GetOwnerIdFromToken();

                accountRequest.ReviewerUserId = userId;

                var identityId = this.repository.ProcessAccountRequest(accountRequest);

                var user = this.repository.GetUserProfile(identityId);

                // return the new user
                return Ok(user);
            }
            catch (Exception ex)
            {
                ErrorHelper.ProcessError(this.logger, ex, nameof(GetAccountRequests));

                return InternalServerError(ex);
            }
        }

        [Route("account/{userId}")]
        [HttpPatch]
        public IHttpActionResult Patch(int userId, [FromBody]JsonPatchDocument<UserProfile> doc)
        {
            if (doc == null)
                return BadRequest();

            try
            {
                var profile = this.repository.GetUserProfile(userId);

                if (profile == null)
                    return NotFound();

                doc.ApplyTo(profile);

                this.repository.SaveUserProfile(profile);

                return Ok(profile);
            }
            catch (Exception ex)
            {
                ErrorHelper.ProcessError(this.logger, ex, nameof(Patch));

                return InternalServerError();
            }
        }

        [Authorize]
        [Route("account")]
        [HttpGet]
        public IHttpActionResult GetUser()
        {
            try
            {
                var userId = TokenIdentityHelper.GetOwnerIdFromToken();

                var userProfile = this.repository.GetUserProfile(userId);

                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                ErrorHelper.ProcessError(this.logger, ex, nameof(GetUser));

                return InternalServerError(ex);
            }
        }
    }
}
