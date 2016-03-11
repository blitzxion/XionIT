using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace XionIT.Models
{
	public class CreateAssetViewModel
	{
		[Required]
		public string Name { get; set; }

		public string AssetModel { get; set; }

		public string Serialnumber { get; set; }

		public string AssetTag { get; set; }

		public string Description { get; set; }

		public string Notes { get; set; }

		public ICollection<string> SelectedUsers { get; set; }

		public CreateAssetViewModel()
		{
			SelectedUsers = new HashSet<string>();
		}

		public CreateAssetViewModel(Asset asset)
			:this()
		{

			Name = asset.Name;
			AssetModel = asset.Model;
			Serialnumber = asset.Serialnumber;
			AssetTag = asset.AssetTag;
			Description = asset.Description;
			Notes = asset.Notes;
			SelectedUsers = asset.Users.Select(x => x.Id).ToList();
		}
	}

	public class EditAssetViewModel : CreateAssetViewModel
	{
		[Required]
		public int AssetId { get; set; }

		public EditAssetViewModel()
			:base()
		{

		}

		public EditAssetViewModel(Asset asset)
			:base(asset)
		{
			AssetId = asset.Id;
		}

		public void UpdateAssetAsync(Asset asset, ICollection<ApplicationUser> users)
		{
			if (asset == null || users == null) return;

			asset.Name = Name;
			asset.Model = AssetModel;
			asset.Serialnumber = Serialnumber;
			asset.AssetTag = AssetTag;
			asset.Description = Description;
			asset.Notes = Notes;

			asset.Users.Clear();
			foreach (var userId in SelectedUsers)
			{
				var appUser = users.FirstOrDefault(x => x.Id == userId);
				if (appUser != null)
					asset.Users.Add(appUser);
			}

		}

	}

	public class DeleteAssetViewModel : EditAssetViewModel
	{
		public bool Delete { get; set; }

		public DeleteAssetViewModel()
			:base()
		{

		}

		public DeleteAssetViewModel(Asset asset)
			:base(asset)
		{

		}

	}

}