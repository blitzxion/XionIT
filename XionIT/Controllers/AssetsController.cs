using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using XionIT.Models;
using System.Data.Entity;

namespace XionIT.Controllers
{
	[Authorize(Roles = @"Admin")]
	public class AssetsController : BaseController
    {
        // GET: Assets
        public ActionResult Index()
        {
            return View();
        }


		public async Task<ActionResult> Create()
		{
			var allusers = await UserManager.Users.ToListAsync();
			ViewBag.Users = new SelectList(allusers, "Id", "Email");
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> Create(NewAssetViewModel model)
		{
			var allusers = await UserManager.Users.ToListAsync();
			ViewBag.Users = new SelectList(allusers, "Id", "Email");

			if (ModelState.IsValid)
			{
				var now = DateTime.UtcNow;

				// Double check the user information and make sure that user exists
				if(!allusers.Any(x => x.Id.Equals(model.AssignedUserId)))
				{
					ModelState.AddModelError("", @"That user doesn't exist. Please select a different user.");
					return View();
				}

				//TODO: Fill the DBModel with the information from the ViewModel
				//TODO: Save to the DB this information

				return RedirectToAction("Index");
			}

			return View();
		}
		


    }
}