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

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace NtccSteward.Api.Controllers
{
    //[RoutePrefix("api")]
    public class AccountController : ApiController
    {
        private readonly IAccountRepository _repository = null;
        private readonly ILogger _logger;

        public AccountController(IAccountRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }


        // POST api/values
        [Route("account/CreateAccountRequest")]
        [HttpPost]
        public IHttpActionResult CreateAccountRequest([FromBody] AccountRequest accountRequest)
        {
            try
            {
                if (accountRequest == null)
                    return BadRequest();

                var accountRequestId = _repository.CreateAccountRequest(accountRequest);

                if (accountRequestId > 0)
                {
                    accountRequest.RequestId = accountRequestId;

                    return Created(Request.RequestUri + "/" + accountRequestId.ToString(), accountRequest);
                }
              
                return BadRequest();
            }
            catch (Exception ex)
            {
                new ErrorHelper().ProcessError(_logger, ex, nameof(CreateAccountRequest));

                return InternalServerError();
            }
        }


        [Route("account/GetAccountRequestStatus/{accountRequestId}")]
        public IHttpActionResult GetAccountRequestStatus(int accountRequestId)
        {
            try
            {
                var status = _repository.GetAccountRequestStatus(accountRequestId);

                if (!string.IsNullOrWhiteSpace(status))
                    return Ok(status);
                else
                    return BadRequest("Invalid accountRequestId");

            }
            catch (Exception ex)
            {
                new ErrorHelper().ProcessError(_logger, ex, nameof(CreateAccountRequest));

                return InternalServerError();
            }
        }


        [Route("account/login")]
        [HttpPost]
        public IHttpActionResult Login([FromBody] Login login)
        {
            try
            {
                // Create a new session and return it
                var session = _repository.Login(login.Email, login.Password, login.ChurchId);

                if (session == null)
                    return NotFound();
                else
                    return Ok(session);
            }
            catch (Exception ex)
            {
                new ErrorHelper().ProcessError(_logger, ex, nameof(Login));

                return InternalServerError();
            }
        }
    }
}
