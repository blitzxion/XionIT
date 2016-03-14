using XionIT.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace XionIT.Controllers
{
	[Authorize(Roles = @"Admin")]
	public class PeopleController : BaseController
	{
		public PeopleController()
		{
		}

		public PeopleController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
			: base(userManager, roleManager)
		{
		}

		//
		// GET: /People/
		public async Task<ActionResult> Index()
		{
			var model = new List<IndexPeopleViewModel>();
			var users = await UserManager.Users.ToListAsync();

			foreach (var user in users)
			{
				model.Add(new IndexPeopleViewModel()
				{
					Id = user.Id,
					Name = user.UserName,
					Email = user.Email,
					Created = user.Created,
					NumberOfAssets = user.Assets.Count(),
					Roles = string.Join(", ", await UserManager.GetRolesAsync(user.Id))
				});
			}


			return View(model);
		}

		//
		// GET: /People/Details/5
		public async Task<ActionResult> Details(string id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var user = await UserManager.FindByIdAsync(id);

			ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);

			return View(user);
		}

		//
		// GET: /People/Create
		public async Task<ActionResult> Create()
		{
			//Get the list of Roles
			ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
			return View();
		}

		//
		// POST: /People/Create
		[HttpPost]
		public async Task<ActionResult> Create(NewPersonViewModel userViewModel) //, params string[] selectedRoles)
		{
			if (ModelState.IsValid)
			{
				var now = DateTime.UtcNow;
				var user = new ApplicationUser { UserName = userViewModel.Email, Email = userViewModel.Email, Created = now, Updated = now };

				// Admins only get passwords to login
				IdentityResult adminresult = null;

				if (userViewModel.Roles.Contains(@"Admin", StringComparer.InvariantCultureIgnoreCase))
					adminresult = await UserManager.CreateAsync(user, userViewModel.Password);
				else
					adminresult = await UserManager.CreateAsync(user); // Not Admin, doesn't need a password.

				//Add User to the selected Roles 
				if (adminresult.Succeeded)
				{
					if (userViewModel.Roles != null)
					{
						var result = await UserManager.AddToRolesAsync(user.Id, userViewModel.Roles);
						if (!result.Succeeded)
						{
							ModelState.AddModelError("", result.Errors.First());
							ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
							return View();
						}
					}
				}
				else
				{
					ModelState.AddModelError("", adminresult.Errors.First());
					ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
					return View();

				}

				SetRedirectSuccess(@"User created.");
				return RedirectToAction("Index");
			}
			ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
			return View();
		}

		//
		// GET: /People/Edit/1
		public async Task<ActionResult> Edit(string id)
		{
			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			var user = await UserManager.FindByIdAsync(id);
			if (user == null)
				return HttpNotFound();

			var userRoles = await UserManager.GetRolesAsync(user.Id);
			ViewBag.Roles = new SelectList(RoleManager.Roles, "Name", "Name");

			ViewBag.AllAssets = await GetAssetSelectListAsync();

			var model = new EditUserViewModel()
			{
				Id = user.Id,
				Email = user.Email,
				SelectedRole = userRoles.FirstOrDefault(),
				AssignedAssetIds = user.Assets.Select(x => x.Id).ToList(),
			};

			return View(model);
		}

		//
		// POST: /People/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Edit(EditUserViewModel editUser)
		{
			ViewBag.Roles = new SelectList(RoleManager.Roles, "Name", "Name");
			ViewBag.AllAssets = await GetAssetSelectListAsync();

			if (ModelState.IsValid)
			{
				try
				{
					var user = await UserManager.FindByIdAsync(editUser.Id);
					if (user == null)
						return HttpNotFound();

					// Roles.
					var userRoles = await UserManager.GetRolesAsync(user.Id);
					var editUserRoles = new string[] { };

					if (!string.IsNullOrEmpty(editUser.SelectedRole))
						editUserRoles = new string[] { editUser.SelectedRole };

					// Roles. Add to new roles
					var result = await UserManager.AddToRolesAsync(user.Id, editUserRoles.Except(userRoles).ToArray());
					if (!result.Succeeded)
					{
						ModelState.AddModelError("", result.Errors.First());
						return View(editUser);
					}

					// Roles. Remove from roles
					result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(editUserRoles).ToArray());
					if (!result.Succeeded)
					{
						ModelState.AddModelError("", result.Errors.First());
						return View(editUser);
					}

					// Assets.
					var selectedAssets = new List<Asset>();
					if (editUser.AssignedAssetIds != null && editUser.AssignedAssetIds.Any())
						selectedAssets = AppDbContext.Assets.Where(x => editUser.AssignedAssetIds.Contains(x.Id)).ToList();

					// Assets. clear/add new assets
					user.Assets.Clear();
					selectedAssets.ForEach(x => user.Assets.Add(x));

					// Passwords. Only if they provided one
					if (!string.IsNullOrEmpty(editUser.NewPassword)
						&& !string.IsNullOrEmpty(editUser.ConfirmPassword))
					{
						// obviously must match
						if (!editUser.NewPassword.Equals(editUser.ConfirmPassword))
						{
							ModelState.AddModelError("", @"New password doesn't match confirmation password.");
							return View(editUser);
						}

						// If the user has an existing password, lets change it (and verify their current one)
						if (await UserManager.HasPasswordAsync(user.Id))
						{
							result = await UserManager.ChangePasswordAsync(user.Id, editUser.CurrentPassword, editUser.NewPassword);
							if (!result.Succeeded)
							{
								ModelState.AddModelError("", result.Errors.First());
								return View(editUser);
							}
						}
						// If the user doesn't have a password, lets just add it
						else
						{
							result = await UserManager.AddPasswordAsync(user.Id, editUser.NewPassword);
							if (!result.Succeeded)
							{
								ModelState.AddModelError("", result.Errors.First());
								return View(editUser);
							}
						}
					}

					await AppDbContext.SaveChangesAsync();

					SetRedirectSuccess(@"Updated Successfully!");
					return RedirectToAction("Index");
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", ex.ToString());
					return View(editUser);
				}
			}
			ModelState.AddModelError("", "Something failed.");
			return View(editUser);
		}

		//
		// GET: /People/Delete/5
		public async Task<ActionResult> Delete(string id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var user = await UserManager.FindByIdAsync(id);
			if (user == null)
			{
				return HttpNotFound();
			}

			var model = new DeletePeopleViewModel() {
				Id = user.Id,
				Username = user.UserName,
				NumAssets = user.Assets.Count(),
				Delete = false
			};

			return View(model);
		}

		//
		// POST: /People/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteConfirmed(DeletePeopleViewModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					if (model.Id == null)
						return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

					if (!model.Delete)
					{
						SetRedirectInfo(@"Person was not deleted.");
						return RedirectToAction("index");
					}

					var user = await UserManager.FindByIdAsync(model.Id);
					if (user == null)
						return HttpNotFound();

					var result = await UserManager.DeleteAsync(user);
					if (!result.Succeeded)
					{
						ModelState.AddModelError("", result.Errors.First());
						return View(model);
					}

					SetRedirectSuccess(@"Person deleted!");
					return RedirectToAction("Index");
				}
				catch (Exception ex)
				{
					ModelState.AddModelError(@"", ex.ToString());
					return View(model);
				}
			}
			return View(model);
		}


		async Task<SelectList> GetAssetSelectListAsync()
		{
			var aAssets = new List<Tuple<int, string>>();
			await AppDbContext.Assets.ForEachAsync(x =>
			{
				aAssets.Add(new Tuple<int, string>(x.Id, string.Format(@"{0} ({1}, {2})", x.Name, x.Model, x.Serialnumber)));
			});
			return new SelectList(aAssets, "Item1", "Item2");
		}

	}
}