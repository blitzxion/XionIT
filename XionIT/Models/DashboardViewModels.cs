using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XionIT.Models
{
	public class DashboardItemModel
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime Created { get; set; }
		public DateTime Deleted { get; set; }
	}

	public class DashboardTotalsModel
	{
		public int Total { get; set; }
		public int ThisWeek { get; set; }
		public int ThisMonth { get; set; }
	}

	public class DashboardViewModel
	{
		public IList<DashboardItemModel> RecentPeople { get; set; }
		public IList<DashboardItemModel> RecentAssets { get; set; }
		public DashboardTotalsModel TotalPeople { get; set; }
		public DashboardTotalsModel TotalAssets { get; set; }

		public DashboardViewModel()
		{
			RecentPeople = new List<DashboardItemModel>();
			RecentAssets = new List<DashboardItemModel>();
			TotalPeople = new DashboardTotalsModel();
			TotalAssets = new DashboardTotalsModel();
		}

	}
}