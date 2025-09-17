using CoreLayer.Models.BasketDto;
using CoreLayer.Models.CategoryDtos;
using Full_E_Commerce_Project.Handle_MiddleWares;
using StackExchange.Redis;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RepositoryLayer
{
	public interface IBasketRedisRepo
	{

		Task AddBasket(string basketId, BasketDto products);
		
	 Task<ReturnedBasketDto> GetBasket(string basketId);
		Task RemoveBasket(string basketId);
	}
}
