﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NtccSteward.Framework;
using NtccSteward.Core.Models.Account;
using Microsoft.AspNetCore.Http;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace NtccSteward.Controllers
{
    public class HelpController : Controller
    {
        private readonly IApiProvider _apiProvider;
        private Session _session;

        public HelpController(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        private void Initialize()
        {
            if (_session == null)
            {
                var sessionJson = HttpContext.Session.GetString("Session");
                _session = _apiProvider.DeserializeJson<Session>(sessionJson);
            }
        }

        public ActionResult Index()
        {
            Initialize();

            if (string.IsNullOrWhiteSpace(_session?.SessionId))
            {
                return RedirectToAction("Index", "Account");
            }
            else
                return View();
        }
    }
}
