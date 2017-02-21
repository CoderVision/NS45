using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NtccSteward.Framework;
using NtccSteward.Core.Models.Account;
using System.Web.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace NtccSteward.Controllers
{
    public class AdminController : Controller
    {
        private readonly IApiProvider _apiProvider;
        private Session _session;

        public AdminController(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        private void InitSession()
        {
            if (_session == null)
                _session = ContextHelper.GetSession(HttpContext);
        }

        public ActionResult Index()
        {
            InitSession();

            if (string.IsNullOrWhiteSpace(_session?.SessionId))
            {
                return RedirectToAction("Index", "Account");
            }
            else
                return View();
        }
    }
}
