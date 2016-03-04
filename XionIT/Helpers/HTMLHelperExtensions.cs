using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using XionIT.Models;

namespace XionIT
{
	public static class HMTLHelperExtensions
	{
		public static IList<string> GetUserRoles(this HtmlHelper html, string userId)
		{
			try
			{
				using (var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
				{
					return userManager.GetRoles(userId);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				return null;
			}
		}

		public static string IsSelected(this HtmlHelper html, string action = null, string controller = null, string cssClass = null)
		{

			if (string.IsNullOrEmpty(cssClass))
				cssClass = "active";

			string currentAction = (string)html.ViewContext.RouteData.Values["action"];
			string currentController = (string)html.ViewContext.RouteData.Values["controller"];

			if (string.IsNullOrEmpty(controller))
				controller = currentController;

			if (string.IsNullOrEmpty(action))
				action = currentAction;

			return controller == currentController && action == currentAction ?
				cssClass : string.Empty;
		}

		public static string PageClass(this HtmlHelper html)
		{
			string currentAction = (string)html.ViewContext.RouteData.Values["action"];
			return currentAction;
		}
	}
}