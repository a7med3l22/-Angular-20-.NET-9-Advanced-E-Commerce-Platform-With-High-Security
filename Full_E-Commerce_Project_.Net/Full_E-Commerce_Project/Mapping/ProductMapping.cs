using AutoMapper;
using Azure.Core;
using CoreLayer.Models;
using CoreLayer.Models.CategoryDtos;
using CoreLayer.Models.Dtos;
using CoreLayer.Models.orderDto;
using CoreLayer.Models.UserModel;
using RepositoryLayer.Data.Context;
using System;
using System.Diagnostics.Metrics;

namespace Full_E_Commerce_Project.Mapping
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

	public class ProductMapping:Profile
	{
		public ProductMapping()
		{

			//order=>OrderResultDto
			CreateMap<Order, OrderResultDto>()
				.ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.TotalPrice))
				.ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus.ToString()))

				;


			CreateMap<SetUsersMainAddresse, UsersMainAddresse>();



			CreateMap<Product, ProductDto>()
			//.ForMember(dest => dest.PhotosName, opt => opt.MapFrom(src => src.Photos.Select(p =>p.ImageName).ToList()))
			.ForMember(dest => dest.PhotosUrl, opt => opt.MapFrom<photoReslover>())
			.ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))

			;




				CreateMap<AddProductDto, Product>()
					.ForMember(dest => dest.Name, opt => opt.MapFrom(src => string.Concat(src.Name.Split(Path.GetInvalidFileNameChars()))))
					.ForMember(dest => dest.Photos, opt => opt.Ignore());

				// علشان لو بعت اسم فيه رموز يشيلها 	

				// عاوزة يعمل اجنور لو القيمة ب نال 
				CreateMap<UpdateProductDto, Product>()

					.ForMember(dest => dest.Photos, opt => opt.Ignore())
			// عاوز لو السورس ب نال يتجاهل 
			//    void Condition(Func<TSource, TDestination, TMember, TMember, bool> condition);

			.ForAllMembers(opt => opt.Condition((src, dest, srcMember) =>
			{
				if (srcMember == null)
					return false;

				if (srcMember.GetType().IsValueType)
					return !srcMember.Equals(Activator.CreateInstance(srcMember.GetType()));

				return true;
			}));
				;

				CreateMap<GetCategoryDto, Category>().ReverseMap()
				.ForMember(dest => dest.AllProducts, opt => opt.MapFrom(src => src.AllProductsInCategory))

				;
			CreateMap<CategoryDto, Category>();
		}
		
		

		}
	}

	public class photoReslover : IValueResolver<Product, ProductDto, List<string>> // لازم اسجله ف ال DI علشان يعرف يعمل نسخه منه وهو بيعمله ماب فروم !   وسجلته ك اسكوب لان ال اhttp ... اسكوب  
	{
		private readonly IHttpContextAccessor httpContext;

		public photoReslover(IHttpContextAccessor httpContext)
		{
			this.httpContext = httpContext;
		}

		public List<string> Resolve(Product source, ProductDto destination, List<string> destMember, ResolutionContext context)
		{
			var RequestUrl = httpContext.HttpContext?.Request;
			var HostUrl = $"{RequestUrl?.Scheme}://{RequestUrl?.Host}";
			return source.Photos.Select(p => $"{HostUrl}/{p.ImageName.Replace("\\", "/")}").ToList();
		}
	}

