using CoreLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Data.Context
{
	public class ApplicationDbContext : DbContext
	{

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
			// اللي فاهم بيريح وبيستريح ف افهم صح ثم افهم صح ثم افهم صح 
			//var x = new DbContextOptions();// مش هعرف اعمل نسخة منه لانه ابستراكت كلاس و بالتالي ال DI مش هيعرف يحقنه لانه مش هيعرف يعمل نسخة منه علشان كده استخدم الجينيرك 
			//var y = new DbContextOptions<ApplicationDbContext>();//هعرف اعمل نسخة منه عادي لانه كلاس عادي و ب التالي ال DI هيعرف يحقنه عادي لانه هيعرف يعمل نسخة منه  
		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			//base.OnModelCreating(modelBuilder); // مش هستخدمها لانها مفيش جواها اي حاجة ف مش محتاجها 
			modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(),type=>type.Namespace== "RepositoryLayer.Data.Migrations");
		}
		public DbSet<Product> Products { get; set; }
		public DbSet<Photo> Photos { get; set; }
		public DbSet<Category> Categories { get; set; }



	}
}
