using AutoMapper;
using CoreLayer.Models.UserModel;
using DocumentFormat.OpenXml.Wordprocessing;
using Full_E_Commerce_Project.Handle_MiddleWares;
using Full_E_Commerce_Project.Mapping;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer;
using RepositoryLayer.Account;
using RepositoryLayer.Data.Context;
using RepositoryLayer.IdentityData.identityContext;
using RepositoryLayer.unitOfWork;
using StackExchange.Redis;
using System.Text;

namespace Full_E_Commerce_Project
{
	public static class MyServices
	{
		
		public static IServiceCollection OwnServices(this IServiceCollection services,IConfiguration config)
		{
			services.AddCors(CorsOptions=>
			{
				CorsOptions.AddPolicy("AllowAngular",
					CorsPolicyBuilder =>
					{
						CorsPolicyBuilder.WithOrigins("http://localhost:4200") // الدومين اللي من حقه يعمل ريكويست
              .AllowAnyHeader()                     // السماح بأي هيدر
              .AllowAnyMethod()                     // السماح بأي ميثود GET, POST, etc.
              .AllowCredentials();                  // لازم لو عايز ترسل كوكيز
						// .AllowCredentials()
						//   بتسمح لل Browser إنه يبعث Cookies أو Authorization Headers (Bearer JWT مثلًا)
						//  مع الريكوست اللي جاي من Angular.
						// يعني لو الـ Frontend بيحتاج يبعث JWT Token في Authorization Header أو Cookies للـ API → لازم تكتبها.
					}


					);
			}
				
				);
			services.AddMemoryCache();
			services.AddScoped<IBasketRedisRepo, BasketRedisRepo>();
			services.AddScoped<AccountRepo>();
			
			services.AddSingleton<IConnectionMultiplexer>(
				//public static IServiceCollection AddSingleton<TService, TImplementation>
				//( this IServiceCollection services, Func<IServiceProvider, TImplementation> implementationFactory)
				//where TService : class where TImplementation : class, TService

				//Func<IServiceProvider, TImplementation> implementationFactory
				_=>
				ConnectionMultiplexer.Connect(config["RedisConnection:ConnectionString"]!)
				);
			services.AddScoped<photoReslover>();
			services.AddHttpContextAccessor(); // بيتسجل ك سينجيلتون
			services.AddTransient<HandleErrorMiddleWare>();
			services.AddDbContext<ApplicationDbContext>(
				opt => opt.UseSqlServer(config.GetConnectionString("DefaultConnection"))
				);
			services.AddDbContext<AppUserContext>(
				opt => opt.UseSqlServer(config.GetConnectionString("AppUserConnection"))
				);
			services.AddScoped<IUnitOfWork,UnitOfWork>();
			services.AddIdentity<AppUser,IdentityRole>()
				.AddEntityFrameworkStores<AppUserContext>()
				.AddDefaultTokenProviders();


			services.AddAutoMapper(config => config.AddProfiles(new List<Profile>() { new ProductMapping() }));
			
			
			
			
			
			
			
			
			
			
			services.AddAuthentication(configureOptions =>
			{
				configureOptions.DefaultAuthenticateScheme =JwtBearerDefaults.AuthenticationScheme;
				configureOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}
				)
				.AddJwtBearer(
				configureOptions =>
				{
					configureOptions.Events = new()
					{
						//	public Func<MessageReceivedContext, Task> OnMessageReceived { get; set; } =
						//	(MessageReceivedContext context) => Task.CompletedTask;

						OnMessageReceived =
						(context) =>
						{
							//context.Request.Cookies=>Request باخد الكوكي من الريكويست اللي جايلي عن طريق اسواجر او المتصفح منه مش من عن طريق الانجولر
							context.Token= context.Request.Cookies["AuthCookie"];
							return Task.CompletedTask;
						}
					};






					//عمر ال فاليو بتبقي خلال البلوك اللي هي فيها  صح؟
					//[Gooooooooooooooooooooooold]💯💯
					//configureOptions.TokenValidationParameters ={ }
					// لو عملت يساوي ع طول معناها اني كده بعمل رفرنس جديد ف بالتالي لازم اعمل نيو لاني ب اليساوي دي غيرت الرفرنس 
					// اما لو انا عملت كده ف ال nested object initializer
					//// كده يعني //JwtBearerOptions x = new() //{ // TokenValidationParameters = { } //};
					// ف ال يساوي في حالة ال  nested object initializer معناها اني بستخدم ف نفس الرفرنس وبغير ف الفاليوز اللي جواه , مش بعمل رفرنس جديد
					configureOptions.TokenValidationParameters = new()
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						ValidIssuer = config["JWT:Issuer"],
						ValidAudience = config["JWT:Audience"],
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]!)),
						ClockSkew = TimeSpan.Zero // علشان مفيش وقت زيادة في صلاحية التوكن

					};


				}
				) 
				
				;



			return services;
		}
	}
}
