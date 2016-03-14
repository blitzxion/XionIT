using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace XionIT.Models
{
	public class RoleViewModel
	{
		public string Id { get; set; }
		[Required(AllowEmptyStrings = false)]
		[Display(Name = "RoleName")]
		public string Name { get; set; }
	}
		
}