using CoreLayer.Handle_Exception;
using CoreLayer.Models.BasketDto;
using CoreLayer.Models.CategoryDtos;
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
	}
}
