using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XionIT.Models;

namespace XionIT.Controllers
{
	[Authorize(Roles = @"Admin")]
	public class DashboardController : BaseController
    {


        // GET: Dashboard
        public ActionResult Index()
        {
			var model = new DashboardViewModel();

			var now = DateTime.Now;
			var startOfTheWeek = now.StartOfWeek(DayOfWeek.Sunday);
			var startOfTheMonth = now.FirstDayOfMonth();

			var people = UserManager.Users;
			model.TotalPeople.Total = people.Count();
			model.TotalPeople.ThisWeek = people.Where(x => x.Created >= startOfTheWeek).Count();
			model.TotalPeople.ThisMonth = people.Where(x => x.Created >= startOfTheMonth).Count();
			people.OrderByDescending(x => x.Created).Take(10).ToList().ForEach(x => model.RecentPeople.Add(new DashboardItemModel() { Name = x.UserName, Description = @"Created", Created = x.Created ?? DateTime.MinValue }));

			return View(model);
        }
    }
}