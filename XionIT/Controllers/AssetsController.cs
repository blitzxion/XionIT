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
        public async Task<ActionResult> Index()
        {
			var model = await AppDbContext.Assets.ToListAsync();
			return View(model);
        }


		public async Task<ActionResult> Create()
		{
			var allusers = await UserManager.Users.ToListAsync();
			ViewBag.Users = new SelectList(allusers, "Id", "Email");
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(CreateAssetViewModel model)
		{
			var allusers = await UserManager.Users.ToListAsync();
			ViewBag.Users = new SelectList(allusers, "Id", "Email");

			if (ModelState.IsValid)
			{
				// If we use "right now" more than once
				var now = DateTime.UtcNow;
				
				try
				{
					// Create the new asset based off the view model
					var newAsset = new Asset()
					{
						Name = model.Name,
						Model = model.AssetModel,
						Serialnumber = model.Serialnumber,
						AssetTag = model.AssetTag,
						Description = model.Description,
						Notes = model.Notes,
						Created = now,
					};
					
					// For each selected user, assign them to this asset (and they will be assigned this asset)
					foreach (var userId in model.SelectedUsers)
					{
						var appUser = allusers.FirstOrDefault(x => x.Id == userId);
						if (appUser != null)
							newAsset.Users.Add(appUser);
					}

					// Add the new asset to the DBContext
					AppDbContext.Assets.Add(newAsset);

					// Save those changes
					await AppDbContext.SaveChangesAsync();
				}
				catch (Exception ex)
				{
					ModelState.AddModelError(@"", ex.ToString());
					return View();
				}

				SetRedirectSuccess(@"Asset created.");
				return RedirectToAction("Index");
			}

			return View();
		}
		
		public async Task<ActionResult> Details(int id)
		{
			var asset = await AppDbContext.Assets.FirstOrDefaultAsync(x => x.Id == id);

			if (asset == null)
				return new RedirectWithErrorResult("index", "Unknown or missing asset.");

			return View(asset);
		}

		public async Task<ActionResult> Edit(int id)
		{
			var asset = await AppDbContext.Assets.FirstOrDefaultAsync(x => x.Id == id);

			if (asset == null)
				return new RedirectWithErrorResult("index", "Unknown or missing asset.");

			var model = new EditAssetViewModel(asset);

			var allusers = await UserManager.Users.ToListAsync();
			ViewBag.Users = new SelectList(allusers, "Id", "Email");

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Edit(EditAssetViewModel editModel)
		{
			var allusers = await UserManager.Users.ToListAsync();
			ViewBag.Users = new SelectList(allusers, "Id", "Email");

			try
			{
				if (ModelState.IsValid)
				{
					var asset = await AppDbContext.Assets.FirstOrDefaultAsync(x => x.Id == editModel.AssetId);
					if (asset == null)
						return new RedirectWithErrorResult("index", "Unknown or missing asset.");

					editModel.UpdateAssetAsync(asset, allusers);

					await AppDbContext.SaveChangesAsync();

					SetRedirectSuccess(@"Asset was updated.");
					return RedirectToAction(@"index");
				}
			}
			catch (Exception ex)
			{
				SetRedirectError(ex.ToString());
			}

			return View(editModel);
		}

		public async Task<ActionResult> Delete(int id)
		{
			var asset = await AppDbContext.Assets.FirstOrDefaultAsync(x => x.Id == id);

			if (asset == null)
				return new RedirectWithErrorResult("index", "Unknown or missing asset.");

			var model = new DeleteAssetViewModel(asset);

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Delete(DeleteAssetViewModel model)
		{
			var asset = await AppDbContext.Assets.FirstOrDefaultAsync(x => x.Id == model.AssetId);

			if (asset == null)
				return new RedirectWithErrorResult("index", "Unknown or missing asset.");

			if(!model.Delete)
			{
				SetRedirectInfo(@"Asset was not deleted.");
				return RedirectToAction("index");
			}


			try
			{
				AppDbContext.Assets.Remove(asset);

				await AppDbContext.SaveChangesAsync();

				SetRedirectSuccess(@"Asset deleted!");
				return RedirectToAction("index");
			}
			catch (Exception ex)
			{
				SetRedirectError(ex.ToString());
			}

			return View(model);
		}


    }
}