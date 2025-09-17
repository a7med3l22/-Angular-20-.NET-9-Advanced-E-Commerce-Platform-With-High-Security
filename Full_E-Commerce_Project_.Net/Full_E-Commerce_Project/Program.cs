using Full_E_Commerce_Project.Handle_MiddleWares;

namespace Full_E_Commerce_Project
{
    public class Program
    {
		// Gold 
		//
		// فيه عندنا ال رووت كونتينر وال اسكوب كونتينيرز ..
		//
		// لو طلبت خدمة سينجيلتون من ال رووت:
		//   - بيعمل نسخة واحدة طول عمر التطبيق.
		//   - بيتعملها ديسبوز لما التطبيق ينتهي عن طريق ال رووت أو لو عملتها ديسبوز ب ايدي.
		//
		// لو طلبت خدمة سينجيلتون من ال اسكوب:
		//   - بيتم انشاء نسخه من الخدمة دي ف الرووت لو مفيش نسخة منها ف ال رووت.
		//   - بيستخدم النسخه اللي ف الرووت دي.
		//   - بيتعملها ديسبوز لما التطبيق ينتهي طبعا.
		//   - لو طلبتها من ال DI Injection بردو نفس الحوار.
		//
		// لو طلبت خدمة اسكوب من ال رووت (وده غلط طبعًا):
		//   - بيديني نسخه من الخدمة دي.
		//   - النسخة دي مش بيتم حفظها ف ال رووت كونتينر.
		//   - يعني بيديني نسخه جديدة ف كل مرة اطلب منه خدمة اسكوب من ال رووت.
		//   - مش بيحصلها ديسبوز الا لو عملتها ديسبوز يدوي.
		//   - لو معملتش ديسبوز يدوي → الـ GC هو اللي بيتولي مهمة تنضيفها.
		//   - وده غلط اني اعمل كده.
		//
		// لو طلبت خدمة Scoped عن طريق Scoped:
		//   - بيعمل نسخة من الخدمة لو مفيش نسخه منها ف ال اسكوب كونتينر ده.
		//   - النسخه دي بتفضل عايشه طول عمر الاسكوب.
		//   - بيحصلها ديسبوز لما الاسكوب يخلص.
		//
		// لو طلبت خدمة Transient من ال رووت:
		//   - بيعمل نسخه جديدة ف كل مرة اطلب فيها الخدمة.
		//   - بيتعملها dispose لو عملت ديسبوز يدوي.
		//   - لو معملتش كده → الـ GC هينضفها.
		//
		// لو طلبت خدمة Transient جوه Scoped:
		//   - بيديني نسخة جديدة ف كل مرة اطلب فيها الخدمة.
		//   - بيحصلها ديسبوز لما الاسكوب يخلص.
		//   - وطبعا ممكن اعمل ديسبوز يدوي.
		//   - لازم الخدمة تكون بتعمل Implement للـ IDisposable علشان يحصلها ديسبوز.
		//
		// ملحوظة:
		//   - لو الخدمة Transient دي محقونه جوه خدمة Singleton أو Scoped → بيتعملها ديسبوز مع الخدمة اللي هي محقونه فيها.
		//   - لو جيه من Scoped بردو بيتعملها ديسبوز لما استخدم using مع ال Scoped.
		//   - غير كده لازم اعمل ديسبوز يدوي.
		//
		// ملحوظة مهمة:
		//   - مينفعش اني احقن خدمة Scoped جوه خدمة Singleton.
		//   - لاني كده بخلي الخدمة الـ Singleton تعتمد ع الخدمة الـ Scoped.
		//   - وده غلط لأن حاجة بتعيش أكتر (Singleton) مينفعش تعتمد على حاجة بتعيش أقل منها (Scoped).

		public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            
            builder.Services.AddEndpointsApiExplorer(); 
            builder.Services.AddSwaggerGen();
            builder.Services.OwnServices(builder.Configuration);

			var app = builder.Build();

			// Configure the HTTP request pipeline.

			app.UseHttpsRedirection();

			// CORS أولًا
			app.UseCors("AllowAngular");

			// أي middleware خاص بالأخطاء
			app.UseMiddleware<HandleErrorMiddleWare>();

			// Authentication قبل Authorization
			app.UseAuthentication();
			app.UseAuthorization();

			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.MapControllers();

			// أي setup إضافي، زي الميجريشن
			await app.MyOwnApp();

			app.Run();

		}
	}
}
