using CoreLayer.Models.orderDto;
using CoreLayer.Models.UserModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Services;
using System.Security.Claims;

namespace Full_E_Commerce_Project.Controllers
{
	[Authorize]
	public class OrderController : GrandController
	{
		private readonly IOrderService orderService;

		public OrderController(IOrderService orderService)
		{
			this.orderService = orderService;
		}
		//Create Order
		//[HttpPost("CreateOrder")]
		//public async Task<ActionResult<OrderResultDto>> CreateOrder(UsersDeleviredAddresse deleviredAddresse,int deliveryMethodId,string basketId)
		//{
		//	var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		//	var OrderResultDto = await orderService.CreateOrderAsync(userId!, deliveryMethodId, basketId, deleviredAddresse);
		//	return Ok(OrderResultDto);
		//}
		[HttpGet("GetOrderByIdAsync")]
		public async Task<ActionResult<OrderResultDto>> GetOrderByIdAsync(int orderId)
		{
			var order= await orderService.GetOrderByIdAsync(orderId);
			return Ok(order);
		}
		[HttpGet("GetAllOrdersForUserAsync")]
		public async Task<ActionResult<IReadOnlyList<OrderResultDto>>> GetAllOrdersForUserAsync()
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var orders= await orderService.GetAllOrdersForUserAsync(userId!);
			return Ok(orders);
		}
		[HttpDelete]
		public async Task<ActionResult> DeleteOrderByIdAsync(int orderId)
		{
			await orderService.DeleteOrderByIdAsync(orderId);
			return Ok();
		}
		[HttpGet("GetAllDeliveryMethodsAsync")]
		public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetAllDeliveryMethodsAsync()
		{
			var deliveryMethods= await orderService.GetAllDeliveryMethodsAsync();
			return Ok(deliveryMethods);
		}
		[HttpGet("GetDeliveryById")]
		public async Task<ActionResult<DeliveryMethod>> GetDeliveryById(int id)
		{
			var deliveryMethods = await orderService.GetDeliveryById(id);
			return Ok(deliveryMethods);
		}
		//Get aDDRESS bY uSER iD
		[HttpGet("AddressByUserId")]
		public async Task<ActionResult<UsersMainAddresse>> GetAddressByUserIdAsync()
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var address= await orderService.GetUsersMainAddresseByUserIdAsync(userId!);
			return Ok(address);
		}
		[HttpPost("SaveNewMainAddress")]
		public async Task<ActionResult<UsersMainAddresse>> SaveNewMainAddress(SetUsersMainAddresse usersMainAddresse)
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			await orderService.SaveNewMainAddress(userId!,usersMainAddresse);
			return Ok(usersMainAddresse);
		}

		[HttpPost("saveAsDeliveryAddress")]
		public async Task<ActionResult<UsersMainAddresse>> SaveAsDeliveryAddress(SetUsersMainAddresse usersMainAddresse)
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			await orderService.SaveNewMainAddress(userId!, usersMainAddresse);
			return Ok(usersMainAddresse);
		}
		
	}
}
