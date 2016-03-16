using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using XionIT.Models;

namespace XionIT.Controllers
{
	public class SiteMapController : BaseController
	{
		// GET: SiteMap
		public ActionResult Index()
		{
			var sitemapItems = new List<SitemapItem>();
			var map = GetSiteMap();

			foreach(var x in map) {
				sitemapItems.Add(new SitemapItem(Url.QualifiedAction(x.Action, x.Controller)));
				foreach (var child in x.Children) // Single level, i don't have crazy pages.
					sitemapItems.Add(new SitemapItem(Url.QualifiedAction(child.Action, child.Controller)));
			}

			return new SitemapResult(sitemapItems);
		}

		public ActionResult Human()
		{
			var map = GetSiteMap();

			return View(map);
		}

		IList<MapItem> GetSiteMap()
		{
			var map = new List<MapItem>() {
				MapItem.Create(@"index", @"home", "Home Page").AddChild("about").AddChild("contact"),
				MapItem.Create(@"index", "dashboard", "Dashboard", true),
				MapItem.Create(@"index", "people", "People", true).AddChild("details", hasParams:true).AddChild("create").AddChild("edit", hasParams:true).AddChild("delete", hasParams:true),
				MapItem.Create(@"index", "assets", "Assets", true).AddChild("details", hasParams:true).AddChild("create").AddChild("edit", hasParams:true).AddChild("delete", hasParams:true),
				MapItem.Create(@"index", "reports", "Reports", true).AddChild("asset", hasParams:true).AddChild("person", hasParams:true),
				MapItem.Create(@"index", "account", "Account Managment").AddChild("login").AddChild("register"),
				MapItem.Create(@"index", "sitemap", "Site Map").AddChild("human")
			};
			return map;
		}


	}

}