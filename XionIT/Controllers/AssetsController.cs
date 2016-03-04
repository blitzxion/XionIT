using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XionIT.Controllers
{
	[Authorize(Roles = @"Admin")]
	public class AssetsController : Controller
    {
        // GET: Assets
        public ActionResult Index()
        {
            return View();
        }
    }
}