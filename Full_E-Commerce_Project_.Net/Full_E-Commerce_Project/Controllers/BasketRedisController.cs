using CoreLayer.Handle_Exception;
using CoreLayer.Models.BasketDto;
using CoreLayer.Models.CategoryDtos;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer;
using StackExchange.Redis;
using System.Text.Json;

namespace Full_E_Commerce_Project.Controllers
{

	public class BasketRedisController : GrandController
	{
		private readonly IBasketRedisRepo basketRedisRepo;

		public BasketRedisController(IBasketRedisRepo basketRedisRepo)
		{
			this.basketRedisRepo = basketRedisRepo;
		}
		//Add Redis Basket
		[HttpPost]
		public async Task<IActionResult> AddBasket(string basketId, BasketDto basketProducts)
		{
			await basketRedisRepo.AddBasket(basketId, basketProducts);
			var returnedProducts=await basketRedisRepo.GetBasket(basketId);
			return Ok(returnedProducts);
		}
		[HttpGet]
		public async Task<ActionResult<BasketDto>> GetBasket(string basketId)
		{
			var returnedProducts = await basketRedisRepo.GetBasket(basketId);
			// أي Object هترجعه من الـ Controller → هيتحول لـ JSON camelCase تلقائيًا.

			return Ok(returnedProducts);
		}
		[HttpDelete]
		public async Task<IActionResult> RemoveBasket(string basketId)
		{
			await basketRedisRepo.RemoveBasket(basketId);
			return Ok(new { Message = "Deleted Basket Successfully!!" });
		}
		//ToDo Comments
		//[Goooooooold]

		//الهدف هو عدم تعديل السعر تلقائيًا، وضمان إعلام المستخدم بأي فرق عند تأكيد عمليه الدفع.

		// بص انتا تحفظ الاسعار ف الريدس عادي ب السعر اللي  عمل بيه ولكن بعد م يملا بينات الفيزا علشان يدفع ويضغط سبمت ف ال بوتوم دي ف الميثود بتاعته اعمل اتشك ع الاسعار اللي ف ال داتا بيز مع الاسعار ال ف الريدس لو كله تمام هيدفع لو فيه فرق اسعار يطلعله رسالة تنبية ان الاسعار اتغيرت ويرجعه ل صفحة ال باسكت تاني ب الاسعار الجديده وبكده اول م يعمل سبمت للدفع هبقي متأكد ميه ف الميه ان الاسعار محدثه وبعلم المستخدم وبكده هبقي حليت المشكلة 


//		ي حبيبي انا اصلا مش هحفظ الاوردر ف ال داتا بيز الا بعد تأكيد عملية الدفع !!
//يعني اول لما يدفع عند عملية الدفع يعمل اتشك وبعد م يتم الدفع بنجاح هحفظ الاوردر وسعره الاجمالي من نفس ال رفرنس اللي عمل اتشك بيه عند تأكيد عملية الدفع يعني
//_____
//هو راح يعمل سبمت ل بيانات الفيزا علشان يدفع هبتدي احفظ ال توتال ف فريابول التوتال اللي جبته م الاسعار اللي ف الداتا بيز وهبدأ اقارنها ب التوتال اللي ف الريدس تمام ولو هما تمام هيدفع وبعد نجاح عمليه الدفع هحفظ ال توتال ب نفس الرفرنس بتاع ال فريابول اللي كنت حاسب بيه الاسعار ف الداتا بيز عند عملية الدفع وبكده السعر اللي دفع بيه هو السعر اللي حفظته ف الاوردر وبكده ابقي ضمنت كل حاجة بحيث ان لو السعر اتغير ف اللحظة دي هبقي حفظت الاوردر بتاعه ب السعر اللي هو دفع بيه

	
	// نفذ 
	
	}
}
