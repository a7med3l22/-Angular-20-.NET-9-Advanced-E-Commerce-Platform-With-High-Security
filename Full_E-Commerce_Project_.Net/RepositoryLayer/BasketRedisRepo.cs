using CoreLayer.Handle_Exception;
using CoreLayer.Models.BasketDto;
using CoreLayer.Models.CategoryDtos;
using Full_E_Commerce_Project.Handle_MiddleWares;
using Microsoft.AspNetCore.Http;
using RepositoryLayer.unitOfWork;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RepositoryLayer
{
	public class BasketRedisRepo: IBasketRedisRepo
	{
		private readonly IDatabase connection;
		private readonly IUnitOfWork unitOfWork;
		private readonly IHttpContextAccessor httpContext;

		public BasketRedisRepo(IConnectionMultiplexer connection, IUnitOfWork unitOfWork, IHttpContextAccessor httpContext)
		{
			this.connection = connection.GetDatabase();
			this.unitOfWork = unitOfWork;
			this.httpContext = httpContext;
		}
		// Methods For Basket
		public async Task  AddBasket(string basketId, BasketDto Basketproducts)
		{
			//Console.Beep(2200, duration: 10); // يصدر صوت بتردد 1000Hz لمدة نص ثانية

			var jsonBasketproducts = JsonSerializer.Serialize(Basketproducts);
			var isAdded = await connection.StringSetAsync(basketId, jsonBasketproducts, TimeSpan.FromDays(4)); // just to create the key if not exists
			if (!isAdded)
			{
				throw new MyException(code: 400, "Error While Adding Basket");
			}

		}
		public async Task<BasketDto> GetBasket(string basketId)
		{
			//Console.Beep(3600, duration: 40); // يصدر صوت بتردد 1000Hz لمدة نص ثانية

			var jsonBasketProducts = await connection.StringGetAsync(basketId);
			if (jsonBasketProducts.IsNullOrEmpty)
			{
				throw new MyException(404, "Not Found Any Basket With This Id");
			}
			var Basketproducts = JsonSerializer.Deserialize<BasketDto>(jsonBasketProducts!)!;
			
			 List<BasketItemDto> ReturnedBasketItems= new List<BasketItemDto>();
				var productRepo= unitOfWork.GetProductRepo();
			foreach (var item in Basketproducts.basket)
			{
				var product=await productRepo.GetByIdSpecAsync(item.id,p=>p.Photos,product=>product.Category);
				if (product == null)
				{
					throw new MyException(404, $"Not Found Any Product With This Id {item.id} In Your Basket");
				}



				
				ReturnedBasketItems.Add(new BasketItemDto
				{
					id = item.id,
					quantity = item.quantity,

					// هاجيب الباقي من الداتا بيز لأا غيرت الفكرة 

					name = item.name,
					description = item.description,
					oldPrice = item.oldPrice,
					newPrice = item.newPrice,

					photosUrl = item.photosUrl,
					categoryName = item.categoryName
				});
			}

			var returnedBasket = new BasketDto()
			{
				basket = ReturnedBasketItems
			};
				return returnedBasket;
		}
		public async Task RemoveBasket(string basketId)
		{
			//Console.Beep(1000, duration: 70); // يصدر صوت بتردد 1000Hz لمدة نص ثانية

			var isDeleted = await connection.KeyDeleteAsync(basketId);
			if (!isDeleted)
			{
				throw new MyException(400, "Not Found Any Basket With This Id To Delete It");
			}
		}

	
	}
}
