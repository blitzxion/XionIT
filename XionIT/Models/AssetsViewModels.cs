using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace XionIT.Models
{
	public class NewAssetViewModel
	{
		[Required]
		public string Name { get; set; }

		public string Model { get; set; }

		public string Serialnumber { get; set; }

		public string AssetTag { get; set; }

		public string Description { get; set; }

		public string Notes { get; set; }

		public string AssignedUserId { get; set; }

	}
}