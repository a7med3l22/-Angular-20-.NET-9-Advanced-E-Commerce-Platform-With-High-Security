using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RepositoryLayer.IdentityData.identityContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Services
{
	public class ClearUnconfirmedUsersService : BackgroundService
	{
		private readonly IServiceProvider serviceProvider;

		public ClearUnconfirmedUsersService(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested) //وبعد م ينفذ اللي بعده هيروح يعمل اتشك هنا هيلاقي ان ال stoppingToken متعملوش كانسل ف يعيد تاني وهكذا 
			{ 
			var DateTimeNow = DateTime.UtcNow;
			var startDayDateForDelete = DateTime.UtcNow.Date;
			var TimeForDeleteAt3Am = startDayDateForDelete.AddHours(3);

			if (DateTimeNow >= TimeForDeleteAt3Am)
			{
				TimeForDeleteAt3Am = TimeForDeleteAt3Am.AddDays(1);
			}
			//وبكده حددنا الوقت اللي هيمسح فيه ب الظبط 
			// عاوز احددله هيمسح بعد قد اي من دلوقتي 
			var RemainingTimeToDelete = TimeForDeleteAt3Am - DateTimeNow;
			// امسح بقي 
			await Task.Delay(RemainingTimeToDelete, stoppingToken); // هيوقف تنفيذ اللي بعده لحد م الوقت ييجي  


			using var scope = serviceProvider.CreateScope();
			var AppUserContext = scope.ServiceProvider.GetRequiredService<AppUserContext>()!;

			//			---100% من 100% 🔥 Gold
			//			لو جبت من ال اسكوب هنا خدمة اسكوب بيحصلها ديسبوز لما الاسكوب ينتهي يعني لما يوصل ل نهاية البلوك ..ولو جبت منه خدمة سينجيلتون مش بيحصلها ديسبوز لان عمرها اكبر من الاسكوب
			//ولو جبت منه خدمة ترانزيت بيحصله ديسبوز لان عمره اقل من ال اسكوب

			//ده لو طبعا بيعملوا امبليمنت ل idisposable
			//---
			//ف اللي بيحصل ان لما يحصل ديسبوز لل اسكوب ف بعدها بيعمل ديسبوز للخدمات اللي اخدتها منه تلقائيا..ف بيشوف لو الخدمة متسجلة اسكوب زية ف بيعملها ديسبوز ولو متسجلة ترانزيت بيتعملها ديسبوز بردو لان عمرها اقل من ال اسكوب ولو سينجيلتون مش بيتعملها ديسبوز لان عمرها اكبر من ال اسكوب ..ف ال سينجيلتون بيتعملها ديسبوز عند نهاية التطبيق و طبعا لو هما بيعملوا امبليمنت ل idisposable زي م قولت قبل كده
			//---
			//اي حاجة بتعمل كونيكشن خارجي ف الكونيكشن ده مش بيحصل الا لو روحت كلمت الحاجة الخارجيه دي ..يعني مثلا ال dbcontect مش بيفتح كونيكشن مع الداتا بيز الا لما يروحلها يعني ينفذ كويري جواها

			//---
			//هام جدا لو حقنت خدمة متسجلة ك ترانزيت جوه خدمة متسجلة ك
			//1 -
			//اسكوب زي ال كنترولر نفسه ف بيحصلها ديسبوز تلقائي بعد م يحصل ديسبوز لل اسكوب لان عمرها اقل منه
			//--
			//2 -
			//ترانزيت ف بيحصلها ديسبوز لما يحصل ديسبوز لل ترانزيت اللي اتحقنت فيها لان عمرهم واحد
			//--
			//3 -
			//سينجيلتون بيحصلها ديسبوز لما يحصل ديسبوز لل سينجيلتون لان عمرها اقل من ال سينجيلتون
			//-- -
			//ومينفعش اني احقن خدمة عمرها اصغر من الخدمة اللي هي محقونه فيها
			//--
			//يعني مينفعش احقن خدمة اسكوب جوه خدمة سينجيلتون لاني كده بخلي الخدمة ال سينجيلتون تعتمد ع الاسكوب!
			//-- -
			//ولكن لو مضطر اني اعمل كده
			//ف اعمل فريابول بيشاور علي رفرنس تايب من نوع IServiceScopeFactory
			//ف ب التالي عن طريق الفريابول ده اكريت اسكوب واحفظه ف فريابول والفريابول ده اعمل قبليه يوزنج ف بالتالي لو جبت اي خدمة من ال اسكوب اللي عملته ,ف لما يحصل ديسبوز لل اسكوب اللي كريته هيحصل تلقائي ديسبوز لل خدمات اللي جبتها منه بعد م يحصلها ديسبوز طبعا ده لو الخدمات اللي انا جبتها منه متسجله ك اسكوب زيه او متسجله ك ترانزيت
			//-----

			await AppUserContext.AppUsers.Where(u => (u.CreationDate.AddDays(30) < DateTimeOffset.UtcNow) && (!u.EmailConfirmed)).ExecuteDeleteAsync();
		}
	}
	}
}
