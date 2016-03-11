using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace XionIT.Models
{
	// You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
	public class ApplicationUser : IdentityUser
	{
		public DateTime Created { get; set; }

		public DateTime Updated { get; set; }

		public virtual ICollection<Asset> Assets { get; set; }

		public ApplicationUser()
			: base()
		{
			Assets = new HashSet<Asset>();
		}

		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
		{
			// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
			var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
			// Add custom user claims here
			return userIdentity;
		}
	}

	public class Asset
	{
		[Key]
		public int Id { get; set; }

		public string Name { get; set; }

		public string Model { get; set; }

		public string Serialnumber { get; set; }

		public string AssetTag { get; set; }

		public string Description { get; set; }

		public string Notes { get; set; }

		public DateTime Created { get; set; }

		public virtual ICollection<ApplicationUser> Users { get; set; }

		public Asset()
		{
			Users = new HashSet<ApplicationUser>();
		}

	}

	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext()
			: base("DefaultConnection", throwIfV1Schema: false)
		{
		}

		static ApplicationDbContext()
		{
			// Set the database intializer which is run once during application start
			// This seeds the database with admin user credentials and admin role
			Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
		}

		public static ApplicationDbContext Create()
		{
			return new ApplicationDbContext();
		}

		public DbSet<Asset> Assets { get; set; }

	}
}