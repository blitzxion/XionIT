using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using XionIT.Models;

namespace XionIT.Controllers
{
    public class ReportsController : BaseController
    {
        // GET: Reports
        public async Task<ActionResult> Index()
        {
			// Users
			ViewBag.Users = await GetUserSelectListAsync();
			ViewBag.Assets = await GetAssetSelectListAsync();

            return View();
        }

		[HttpGet]
		public ActionResult Asset(int? selectedAsset)
		{
			if (selectedAsset == null || selectedAsset < 0)
			{
				SetRedirectError("Invalid Asset provided.");
				return RedirectToAction("index");
			}

			int assetId = selectedAsset.Value;
			var asset = AppDbContext.Assets.FirstOrDefault(x => x.Id == assetId);

			if (asset == null)
			{
				SetRedirectError("Invalid Asset provided.");
				return RedirectToAction("index");
			}

			return View(asset);
		}

		[HttpGet]
		public ActionResult Person(string selectedPerson)
		{
			if (string.IsNullOrEmpty(selectedPerson))
			{
				SetRedirectError("Invalid Person provided.");
				return RedirectToAction("index");
			}

			var user = UserManager.Users.FirstOrDefault(x => x.Id == selectedPerson);

			if (user == null)
			{
				SetRedirectError("Invalid Person provided.");
				return RedirectToAction("index");
			}

			return View(user);
		}


		public ActionResult PeopleWithMostAssets()
		{
			var people = UserManager.Users.OrderByDescending(x => x.Assets.Count()).Take(5).ToList();
			return View(people);
		}

		public ActionResult PeopleWithoutAssets()
		{
			var people = UserManager.Users.Where(x => !x.Assets.Any()).ToList();
			return View(people);
		}

		public ActionResult MostAssignedAssets()
		{
			var assets = AppDbContext.Assets.OrderByDescending(x => x.Users.Count()).Take(5).ToList();
			return View(assets);
		}

		public ActionResult AssetsNotAssigned()
		{
			var assets = AppDbContext.Assets.Where(x => !x.Users.Any()).ToList();
			return View(assets);
		}

	}
}