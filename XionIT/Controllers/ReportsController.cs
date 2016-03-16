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

    }
}