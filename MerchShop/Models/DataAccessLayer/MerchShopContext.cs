using MerchShop.Models.DomainModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MerchShop.Models.DataAccessLayer
{
	public class MerchShopContext : IdentityDbContext<AppUser>
	{
		public MerchShopContext(DbContextOptions<MerchShopContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Define primary keys for each model
			modelBuilder.Entity<Vendor>()
				.HasKey(v => v.VendorID);

			modelBuilder.Entity<Merch>()
				.HasKey(m => m.MerchID);

			modelBuilder.Entity<Inventory>()
				.HasKey(i => i.ItemID);

			modelBuilder.Entity<Warehouse>()
				.HasKey(w => w.LocationID);

			modelBuilder.Entity<VendorReview>()
				.HasKey(vr => new { vr.VendorID, vr.CustomerID });

			modelBuilder.Entity<Customer>()
				.HasKey(c => c.CustomerID);

			modelBuilder.Entity<MerchReview>()
				.HasKey(mr => new { mr.MerchID, mr.CustomerID });

			modelBuilder.Entity<Order>()
				.HasKey(o => o.OrderID);

			modelBuilder.Entity<OrderLines>()
				 .HasKey(ol => new { ol.OrderID, ol.ItemID });

			// Customize Identity model if needed
			modelBuilder.Entity<AppUser>().ToTable("AspNetUsers");
			modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles");
			modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles").HasKey(ur => new { ur.UserId, ur.RoleId });
			// modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims");
			modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins").HasKey(ul => new { ul.ProviderKey });
			// modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims");
			modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens").HasKey(ut => new { ut.UserId, ut.Value });

			this.SeedRoles(modelBuilder);
			this.SeedUsers(modelBuilder);
			this.SeedUserRoles(modelBuilder);
		}

		private void SeedUsers(ModelBuilder builder)
		{
			AppUser user = new AppUser()
			{
				Id = "b74ddd14-6340-4840-95c2-db12554843e5",
				UserName = "Admin",
				NormalizedUserName = "ADMIN",
				Email = "admin@gmail.com",
				NormalizedEmail = "ADMIN@GMAIL.COM",
				LockoutEnabled = false,
				PhoneNumber = "1234567890",
				FirstName = "Robert",
				LastName = "Brown",
				IsActive = true,
				TwoFactorEnabled = false
			};

			PasswordHasher<AppUser> passwordHasher = new PasswordHasher<AppUser>();
			user.PasswordHash = passwordHasher.HashPassword(user, "Admin*123");

			builder.Entity<AppUser>().HasData(user);

			Customer customer = new Customer()
			{
				CustomerID = 1,
				FirstName = "Robert",
				LastName = "Brown",
				Email = user.Email,
				UserID = user.Id
			};
			builder.Entity<Customer>().HasData(customer);
		}

		private void SeedRoles(ModelBuilder builder)
		{
			builder.Entity<IdentityRole>().HasData(
				new IdentityRole() { Id = "fab4fac1-c546-41de-aebc-a14da6895711", Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin" },
				new IdentityRole() { Id = "c7b013f0-5201-4317-abd8-c211f91b7330", Name = "User", ConcurrencyStamp = "2", NormalizedName = "User" }
				);
		}

		private void SeedUserRoles(ModelBuilder builder)
		{
			builder.Entity<IdentityUserRole<string>>().HasData(
				new IdentityUserRole<string>() { RoleId = "fab4fac1-c546-41de-aebc-a14da6895711", UserId = "b74ddd14-6340-4840-95c2-db12554843e5" }
				);
		}

		public DbSet<AppUser> AppUsers { get; set; }
		public DbSet<IdentityRole> Roles { get; set; }
		public DbSet<Vendor> Vendors { get; set; }
		public DbSet<Merch> Merch { get; set; }
		public DbSet<Inventory> Inventory { get; set; }
		public DbSet<Warehouse> Warehouses { get; set; }
		public DbSet<VendorReview> VendorReviews { get; set; }
		public DbSet<Customer> Customers { get; set; }
		public DbSet<MerchReview> MerchReviews { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderLines> OrderLines { get; set; }
	}
}
