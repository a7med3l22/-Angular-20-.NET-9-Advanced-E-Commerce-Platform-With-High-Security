using Full_E_Commerce_Project.Handle_MiddleWares;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Data.Context;
using RepositoryLayer.Data.Seeding;
using RepositoryLayer.IdentityData.identityContext;
using System.Runtime.CompilerServices;

namespace Full_E_Commerce_Project
{
	/*
		Local variables بيعيشوا مؤقت لحد نهاية الميثود.

		Instance fields بيعيشوا طول ما الـ Object نفسه لسه موجود.

		Static fields هما اللي بيعيشوا طول عمر التطبيق.
	 //يعني لو المتغير اللي محفوظ فيه القيمة استاتيك هيبقي القيمة هتفضل جواه طول عمر التطبيق 
	 */
	public static class MyApp
	{
		public static async Task<IApplicationBuilder> MyOwnApp(this WebApplication app)
		{
			app.UseStaticFiles();
		
			// عاوز يعمل ابديت لل داتا بيز تلقائي 
			using var scope=app.Services.CreateScope();
			var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			var dbAppContext = scope.ServiceProvider.GetRequiredService<AppUserContext>();
			////add Seeding
			await AddSeeding.Seeding(dbContext); // مش عاوز اعمل سيدنج للداتا لانها هتدي ايرور علشان زودت الهاش ومينفعش اضيفها ب نال لل داتا بيز لاني مش عاملها نلابول ف ال بروبيرتي
			await AddSeeding.SeedingDeliveryMethods(dbAppContext);




			await dbContext.Database.MigrateAsync();
			await dbAppContext.Database.MigrateAsync();

			await dbAppContext.AppUsers.Where(u => (u.CreationDate.AddDays(30) < DateTimeOffset.UtcNow) && (!u.EmailConfirmed)).ExecuteDeleteAsync();

			return app;
		}
	}
}
