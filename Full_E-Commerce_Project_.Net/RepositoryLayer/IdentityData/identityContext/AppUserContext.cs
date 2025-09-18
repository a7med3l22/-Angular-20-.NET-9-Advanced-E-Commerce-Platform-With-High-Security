using CoreLayer.Models.UserModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.IdentityData.identityContext
{
	public class AppUserContext : IdentityDbContext<AppUser>
	{
		public AppUserContext(DbContextOptions options) : base(options)
		{
		}

		public DbSet<UsersMainAddresse> UsersMainAddresse { get; set; }
		public DbSet<AppUser> AppUsers { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderItem> OrderItems { get; set; }
		public DbSet<DeliveryMethod> deliveryMethods { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.ApplyConfigurationsFromAssembly(typeof(AppUserContext).Assembly,type=>type.Namespace== "RepositoryLayer.appUserConfiguration.Configurations");
		}
	}
}
