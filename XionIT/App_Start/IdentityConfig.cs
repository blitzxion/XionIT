using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using XionIT.Models;

namespace XionIT
{
	public class EmailService : IIdentityMessageService
	{
		public Task SendAsync(IdentityMessage message)
		{
			// Plug in your email service here to send an email.
			return Task.FromResult(0);
		}
	}

	public class SmsService : IIdentityMessageService
	{
		public Task SendAsync(IdentityMessage message)
		{
			// Plug in your SMS service here to send a text message.
			return Task.FromResult(0);
		}
	}

	// Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
	public class ApplicationUserManager : UserManager<ApplicationUser>
	{
		public ApplicationUserManager(IUserStore<ApplicationUser> store)
			: base(store)
		{
		}

		public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
		{
			var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
			// Configure validation logic for usernames
			manager.UserValidator = new UserValidator<ApplicationUser>(manager)
			{
				AllowOnlyAlphanumericUserNames = false,
				RequireUniqueEmail = true
			};

			// Configure validation logic for passwords
			manager.PasswordValidator = new PasswordValidator
			{
				RequiredLength = 6,
				RequireNonLetterOrDigit = true,
				RequireDigit = true,
				RequireLowercase = true,
				RequireUppercase = true,
			};

			// Configure user lockout defaults
			manager.UserLockoutEnabledByDefault = true;
			manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
			manager.MaxFailedAccessAttemptsBeforeLockout = 5;

			// Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
			// You can write your own provider and plug it in here.
			manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
			{
				MessageFormat = "Your security code is {0}"
			});
			manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
			{
				Subject = "Security Code",
				BodyFormat = "Your security code is {0}"
			});
			manager.EmailService = new EmailService();
			manager.SmsService = new SmsService();
			var dataProtectionProvider = options.DataProtectionProvider;
			if (dataProtectionProvider != null)
			{
				manager.UserTokenProvider =
					new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
			}
			return manager;
		}
	}

	// This is useful if you do not want to tear down the database each time you run the application.
	// public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
	// This example shows you how to create a new database if the Model changes
	public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext> // DropCreateDatabaseAlways<ApplicationDbContext>
	{
		protected override void Seed(ApplicationDbContext context)
		{
			InitializeIdentityForEF(context);
			base.Seed(context);
		}

		//Create User=admin@admin.com with password=Admin@123456 in the Admin role        
		public static void InitializeIdentityForEF(ApplicationDbContext db)
		{
			var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
			var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
			const string name = "admin@admin.com";
			const string password = "Admin@123456";
			const string roleName = "Admin";

			//Create Role Admin if it does not exist
			var role = roleManager.FindByName(roleName);
			if (role == null)
			{
				role = new IdentityRole(roleName);
				var roleresult = roleManager.Create(role);
			}

			//Create Role User if it does not exist
			if (roleManager.FindByName(@"User") == null)
				roleManager.Create(new IdentityRole(@"User"));

			// Create the user if it doesn't already exist
			var user = userManager.FindByName(name);
			if (user == null)
			{
				var now = DateTime.UtcNow;
				user = new ApplicationUser { UserName = name, Email = name, Created = now, Updated = now };
				var result = userManager.Create(user, password);
				result = userManager.SetLockoutEnabled(user.Id, false);
			}

			// Add user admin to Role Admin if not already added
			var rolesForUser = userManager.GetRoles(user.Id);
			if (!rolesForUser.Contains(role.Name))
			{
				var result = userManager.AddToRole(user.Id, role.Name);
			}

			InitializeSeedAssets(db, 100);
			InitializeSeedUsers(db, userManager, 35);

		}

		static void InitializeSeedUsers(ApplicationDbContext context, ApplicationUserManager userManager, int numUsers, string userRole = @"User")
		{
			var allAssets = context.Assets.ToList();

			Random rnd = new Random();
			for (int i = 0; i < numUsers; i++)
			{
				var now = DateTimeExtensions.RandomDate(DateTime.UtcNow.AddDays(-20), DateTime.UtcNow, rnd);
				var email = StringExtenstions.RandomEmail(rnd);

				var user = new ApplicationUser()
				{
					UserName = email,
					Email = email,
					Created = now,
					Updated = now
				};

				foreach (var asset in allAssets.PickRandom(0, 10, rnd))
					user.Assets.Add(asset);

				// No passwords, for now
				var result = userManager.Create(user);
				userManager.AddToRole(user.Id, userRole);

			}

		}

		static void InitializeSeedAssets(ApplicationDbContext context, int numAssets)
		{
			string[] names = new string[] { "TV", "Laptop", "Computer", "Phone", "Monitor", "Projector", "Tablet", "Other" };
			string[] models = new string[] { "Samsung", "Sony", "Panasonic", "Dell", "HP", "Compaq", "Apple", "Foxconn", "Microsoft", "IBM", "Toshiba", "Intel", "AMD", "LG", "Verizon", "Motorola", "Canon", "Cisco", "TI", "Quadcomm" };

			Random rnd = new Random();
			for (int i = 0; i < numAssets; i++)
			{
				var asset = new Asset()
				{
					Name = names.PickRandom(),
					Model = models.PickRandom(),
					Serialnumber = StringExtenstions.RandomString(8, rnd),
					AssetTag = StringExtenstions.RandomString(5, rnd),
					Notes = LoremIpsum.Generate(5, 10, 1, 2, 1),
					Description = LoremIpsum.Generate(5, 10, 2, 3, 2),
					Created = DateTimeExtensions.RandomDate(DateTime.UtcNow.AddDays(-20), DateTime.UtcNow, rnd)
				};

				context.Assets.Add(asset);
			}

			context.SaveChanges();

		}

	}

	// Configure the application sign-in manager which is used in this application.
	public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
	{
		public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
			: base(userManager, authenticationManager)
		{
		}

		public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
		{
			return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
		}

		public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
		{
			return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
		}
	}

	// Configure the RoleManager used in the application. RoleManager is defined in the ASP.NET Identity core assembly
	public class ApplicationRoleManager : RoleManager<IdentityRole>
	{
		public ApplicationRoleManager(IRoleStore<IdentityRole, string> roleStore)
			: base(roleStore)
		{
		}

		public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
		{
			return new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));
		}
	}
}
