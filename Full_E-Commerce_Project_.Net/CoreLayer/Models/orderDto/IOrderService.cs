using CoreLayer.Models.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Models.orderDto
{
	public interface IOrderService
	{
		Task<Order> prepareOrderAsync(string UserId, int deliveryMethodId, string basketId, UsersDeleviredAddress usersDeleviredAddress);
		Task<OrderResultDto> CreateOrderAsync(Order order, string basketId);
		Task<OrderResultDto> GetOrderByIdAsync(int orderId);
		Task<IReadOnlyList<OrderResultDto>> GetAllOrdersForUserAsync(string appUserId);
		Task DeleteOrderByIdAsync(int orderId);
		Task<IReadOnlyList<DeliveryMethod>> GetAllDeliveryMethodsAsync();
		Task<DeliveryMethod> GetDeliveryById(int id);
		Task<UsersMainAddresse> GetUsersMainAddresseByUserIdAsync(string userId);
		Task SaveNewMainAddress(string userId, SetUsersMainAddresse usersMainAddresse);
		Task<Order> CreateOrUpdatePaymentIntentAsync(string UserId, int deliveryMethodId, string basketId, UsersDeleviredAddress usersDeleviredAddress);
		Task HandleWebhookAsync(string json, string sigHeader);
	}
}
