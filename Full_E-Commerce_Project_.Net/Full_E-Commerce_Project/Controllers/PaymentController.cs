using CoreLayer.Models.orderDto;
using CoreLayer.Models.UserModel;
using Full_E_Commerce_Project.Handle_MiddleWares;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;

namespace Full_E_Commerce_Project.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PaymentController : ControllerBase
	{
		private readonly IOrderService orderService;

		public PaymentController(IOrderService orderService)
		{
			this.orderService = orderService;
		}

		// 🔹 إنشاء أو تحديث عملية الدفع
		[HttpPost("CreateOrUpdatePayment")]
		public async Task<ActionResult> CreateOrUpdatePayment([FromQuery]int deliveryMethodId, [FromQuery]string basketId, [FromBody]UsersDeleviredAddress usersDeleviredAddress)
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var order = await orderService.CreateOrUpdatePaymentIntentAsync(userId!, deliveryMethodId, basketId, usersDeleviredAddress);
			return Ok(new { clientSecret = order.ClientSecret, paymentIntentId=order.PaymentIntentId });
		}

		// 🔹 Webhook من Stripe
		[HttpPost("webhook")]
		public async Task<IActionResult> Webhook()
		{
			using var reader = new StreamReader(HttpContext.Request.Body);
			var json = await reader.ReadToEndAsync();
			var sig = Request.Headers["Stripe-Signature"];

			try
			{
				await orderService.HandleWebhookAsync(json, sig!);
				return Ok();
			}
			catch (MyException ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
