
using CoreLayer.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading;

namespace Full_E_Commerce_Project.Handle_MiddleWares
{
	public class HandleErrorMiddleWare : IMiddleware
	{
		private readonly IWebHostEnvironment environment;
		private readonly IMemoryCache memoryCache;
		private readonly TimeSpan _rateLimitWindow = TimeSpan.FromSeconds(30);
		private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks
		= new ConcurrentDictionary<string, SemaphoreSlim>();

		public HandleErrorMiddleWare(IWebHostEnvironment environment, IMemoryCache memoryCache)
		{
			this.environment = environment;
			this.memoryCache = memoryCache;
		}
		public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			try
			{
				//if (!IsRequestAllowed(context)) // وقفتها علشان بتعطلني وقت الديفيلوب 
				//{
				//	throw new MyException(429, "Too Many Requests. Please try again After 30 Second!!.");
				//}
				ApplySecurityHeaders(context);
				await next(context);
			}
			catch (Exception ex)
			{

				DeveloperThrowEx developerThrowEx;



				if (ex is MyException myEx)
				{
					developerThrowEx = new() { code = myEx.code, Message = myEx.message, DeveloperMessage = environment.IsDevelopment() ? myEx.develoberMessage : null, DeveloperMessages = environment.IsDevelopment() ? myEx.develoberMessages : null };
				}
				else
				{

					myEx = new MyException(500);
					developerThrowEx = new() { code = myEx.code, Message = myEx.message, DeveloperMessage = environment.IsDevelopment() ? (ex.StackTrace ?? ex.Message) : null };
				}

				/////////////////////////////
				var json = JsonSerializer.Serialize(developerThrowEx, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
				context.Response.ContentType = "application/json";
				context.Response.StatusCode = developerThrowEx.code;
				await context.Response.WriteAsync(json);

			}
		}



		private bool IsRequestAllowed(HttpContext context)
		{
			var ip = context.Connection.RemoteIpAddress?.ToString();
			var cacheKey = $"Rate:{ip}";
			var dateNow = DateTime.Now;
			var semaphore = _locks.GetOrAdd(cacheKey, _ => new SemaphoreSlim(1, 1));
			//إنما بالـ lambda _ => new SemaphoreSlim(1, 1) → مش هيعمل new إلا لو المفتاح مش موجود فعلًا.
			/*
		 عملت ل كل كاش نيو سيمفور سليم عشان لو في اكتر من ريكويست جايين في نفس اللحظة من نفس الاي بي يبقي كل واحد فيهم هيستني دوره عشان يدخل عشان م يحصلش مشكله في الكاونت زي ما هشرح تحت
			ومعملتش نيو سيمفور سليم برا عشان لو عملت كده كل الريكويستات هتستني في صف واحد عشان تدخل وده هيخلي الريكويستات تاخد وقت طويل اوي عشان تخلص
			 */
			semaphore.Wait();
			try
			{
				var (timeStamp, count) = memoryCache.GetOrCreate(cacheKey,
					entry =>
					{
						entry.AbsoluteExpirationRelativeToNow = _rateLimitWindow;
						return (dateNow, 1);
					}
					);
				if (dateNow - timeStamp < _rateLimitWindow)
				{
					if (count >= 8)
					{
						return false;
					}
					memoryCache.Set(cacheKey, (dateNow, count + 1), absoluteExpirationRelativeToNow: _rateLimitWindow);
				}
				else
				{
					memoryCache.Set(cacheKey, (dateNow, 1), absoluteExpirationRelativeToNow: _rateLimitWindow);
				}
				return true; // شارحها تحت  خالص

			}

			finally
			{

				semaphore.Release();
			}

		}
		private void ApplySecurityHeaders(HttpContext context)
		{
			// ✅ يمنع تخمين نوع الملفات (MIME sniffing)
			context.Response.Headers["X-Content-Type-Options"] = "nosniff";

			// ✅ يمنع وضع موقعك في iframe (حماية من Clickjacking)
			context.Response.Headers["X-Frame-Options"] = "DENY";

			// ✅ سياسة حماية قوية للمحتوى (Content Security Policy)
			// تقدر تخصص الـ script-src و img-src حسب احتياجك
			context.Response.Headers["Content-Security-Policy"] =
				"default-src 'self'; script-src 'self'; object-src 'none'; frame-ancestors 'none'";

			// ✅ سياسة الـ Referrer
			context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

			// ✅ HSTS → بيجبر المتصفح يستخدم HTTPS (لو موقعك https)
			context.Response.Headers["Strict-Transport-Security"] =
				"max-age=31536000; includeSubDomains; preload";

			// ⚠️ قديم ومش مدعوم في أغلب المتصفحات، ممكن تشيله
			// context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
		}

	}
}
	/*
تمام 👌 ركز معايا يا أحمد:

المثال 👇

عندك متغير في الكاش:

count = 5;


جه طلبين في نفس اللحظة تقريبًا. كل واحد عمل:

count = count + 1;

اللي المفروض يحصل لو كل طلب دخل لوحده:

الطلب الأول يقرأ count = 5 → يزوّد 1 → يخزّن 6.

الطلب التاني يقرأ count = 6 (القيمة الجديدة) → يزوّد 1 → يخزّن 7.

النتيجة الطبيعية: count = 7. ✅

اللي بيحصل فعليًا من غير Lock:

الطلب الأول والتاني قروا نفس القيمة القديمة في نفس اللحظة (5).

الطلب الأول يزوّد 1 → يخزّن 6.

الطلب التاني كمان يزوّد 1 → يخزّن برضه 6.
(لأنه كان شايف القيمة القديمة = 5 قبل ما الأول يكتب الجديد).

👀 النتيجة النهائية: count = 6 بدل ما تبقى 7.
	 */












//private bool IsRequestAllowed(HttpContext context)
//{
//	var ip = context.Connection.RemoteIpAddress?.ToString();
//	var cacheKey = $"Rate:{ip}";
//	var dateNow = DateTime.Now;

//	var (timeStamp, count) = _memoryCache.GetOrCreate(cacheKey, entry =>
//	{
//		entry.AbsoluteExpirationRelativeToNow = _rateLimitWindow;
//		return (dateNow, 0); // أول محاولة جديدة
//	});

//	if (dateNow - timeStamp < _rateLimitWindow)
//	{
//		if (count >= 8) // حد الطلبات
//		{
//			return false;
//		}

//		_memoryCache.Set(cacheKey, (dateNow, count + 1),

//		 absoluteExpirationRelativeToNow: _rateLimitWindow);
//	}
//	else
//	{
//		// reset window
//		_memoryCache.Set(cacheKey, (dateNow, 0),
//			absoluteExpirationRelativeToNow: _rateLimitWindow);
//	}

//	return true;
//}
//// (Gold)
//// _memoryCache.Set(cacheKey, (timeStamp, count + 1)
//// _memoryCache.Set(cacheKey, (dateNow, count + 1)
//// DateNow => DateInCache => CountOfRequest
//// 5:35=>5:30=>1
//// 5:40=>5:30=>2
//// 5:60=>5:30=>3
//// 6:05=>6:05=>0
//// --------------
//// 5:35=>5:30=>1
//// 5:40=>5:35=>2
//// 5:60=>5:40=>3
//// 6:35=>6:35=>0
//// في الاولي لو ضغطت خمس ريكويستات ورا بعض ف في كل ريكويست بيشوف الفرق بين الوقت دلوقتي وبين أول ريكويست عملته .
//// وفي التانية لو ضغطت خمس ريكويستات ورا بعض ف في كل ريكويست بيشوف الفرق بين الوقت دلوقتي وبين اخر ريكويست عملته.
//// ف ب التالي التانيه بتسمح بعدد ريكويستات اقل م الاولي لان الكاونت فيها بيزيد اكتر لان المده م بين اول ريكويست ورابع ريكويست اكتر من المده اللي م بين ثالث ريكوست ورابع ريكويست.
//// وبالتالي كل م المده تبقي اقل الكاونت هيزيد وكل م الكاونت بيزيد كل م هو مش هيبقل ريكويستات تاني ف بالتالي التاني بتسمع بعدد اقل م الاولي.
