using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.WebPages.Html;
// using System.Web.Mvc;

namespace XionIT.Models
{
	public class NewPersonViewModel
	{
		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required]
		public string[] Roles { get; set; }

		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
	}

	public class IndexPeopleViewModel
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Roles { get; set; }
		public int NumberOfAssets { get; set; }

		public DateTime Created { get; set; }
	}

	public class EditUserViewModel
	{
		[Required]
		public string Id { get; set; }

		public string Email { get; set; }

		public string SelectedRole { get; set; }

		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Current Password")]
		public string CurrentPassword { get; set; }

		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "New Password")]
		public string NewPassword { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		public ICollection<int> AssignedAssetIds { get; set; }
	}

	public class DeletePeopleViewModel
	{
		public string Id { get; set; }

		public string Username { get; set; }

		public int NumAssets { get; set; }

		public bool Delete { get; set; }
		
	}

}