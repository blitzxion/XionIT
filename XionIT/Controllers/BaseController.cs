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


	}
}