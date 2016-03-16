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
using System.Linq.Expressions;

namespace XionIT.Controllers
{
	[CopyMessageTempDataFilter]
	public abstract class BaseController : Controller
	{
		private ApplicationDbContext _appDbContext;
		public ApplicationDbContext AppDbContext
		{
			get
			{
				return _appDbContext ?? HttpContext.GetOwinContext().Get<ApplicationDbContext>();
			}
			private set
			{
				_appDbContext = value;
			}
		}

		private ApplicationUserManager _userManager;
		public ApplicationUserManager UserManager
		{
			get
			{
				return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
			private set
			{
				_userManager = value;
			}
		}

		private ApplicationRoleManager _roleManager;
		public ApplicationRoleManager RoleManager
		{
			get
			{
				return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
			}
			private set
			{
				_roleManager = value;
			}
		}

		public BaseController()
		{

		}

		public BaseController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
		{
			UserManager = userManager;
			RoleManager = roleManager;
		}

		public BaseController(ApplicationUserManager userManager, ApplicationRoleManager roleManager, ApplicationDbContext appDbContext)
			:this(userManager, roleManager)
		{
			AppDbContext = appDbContext;
		}

		/// <summary>
		/// Sets a error to display on the next redirect/postback.  This will survive a redirect!
		/// </summary>
		/// <param name="error"></param>
		public void SetRedirectError(string error)
		{
			TempData["error"] = error;
		}
		/// <summary>
		/// Sets a warning to display on the next redirect/postback.  This will survive a redirect!
		/// </summary>
		/// <param name="warning"></param>
		public void SetRedirectWarning(string warning)
		{
			TempData["warning"] = warning;
		}
		/// <summary>
		/// Sets a alert (Action Required) to display on the next redirect/postback.  This will survive a redirect!
		/// </summary>
		/// <param name="warning"></param>
		public void SetRedirectActionRequired(string action)
		{
			TempData["ActionRequired"] = action;
		}
		/// <summary>
		/// Sets a message to display on the next redirect/postback.  This will survive a redirect!
		/// </summary>
		/// <param name="message"></param>
		public void SetRedirectInfo(string message)
		{
			TempData["info"] = message;
		}
		/// <summary>
		/// Sets a succes message to display on the next redirect/postback.  This will survive a redirect!
		/// </summary>
		/// <param name="message"></param>
		public void SetRedirectSuccess(string message)
		{
			TempData["success"] = message;
		}

		protected async Task<SelectList> GetUserSelectListAsync(Func<ApplicationUser, string> format = null)
		{
			format = format ?? (x => { return x.UserName; });
			var aUsers = new List<Tuple<string, string>>();
			await UserManager.Users.ForEachAsync(x => aUsers.Add(new Tuple<string, string>(x.Id, format(x))));
			return new SelectList(aUsers, "Item1", "Item2");
		}

		protected async Task<SelectList> GetAssetSelectListAsync(Func<Asset, string> format = null)
		{
			format = format ?? ((x) => { return string.Format(@"{0} ({1}, {2})", x.Name, x.Model, x.Serialnumber);  });
			var aAssets = new List<Tuple<int, string>>();
			await AppDbContext.Assets.ForEachAsync(x => aAssets.Add(new Tuple<int, string>(x.Id, format(x))));
			return new SelectList(aAssets, "Item1", "Item2");
		}


	}
}