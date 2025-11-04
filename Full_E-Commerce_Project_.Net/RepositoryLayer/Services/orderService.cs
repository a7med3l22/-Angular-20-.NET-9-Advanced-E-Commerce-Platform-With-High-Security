using AutoMapper;
using CoreLayer.Models;
using CoreLayer.Models.BasketDto;
using CoreLayer.Models.orderDto;
using CoreLayer.Models.UserModel;
using Full_E_Commerce_Project.Handle_MiddleWares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RepositoryLayer.IdentityData.identityContext;
using RepositoryLayer.unitOfWork;
using Stripe;
using Stripe.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Product = CoreLayer.Models.Product;
using System.Threading.Tasks;
namespace RepositoryLayer.Services
{
	[Authorize]
	public class OrderService : IOrderService
	{
		private readonly IBasketRedisRepo basketRedisRepo;
		private readonly IMapper mapper;
		private readonly AppUserContext userContext;
		private readonly IUnitOfWork unitOfWork;
		private readonly IConfiguration configuration;

		public OrderService(IBasketRedisRepo basketRedisRepo, IMapper mapper, AppUserContext userContext, IUnitOfWork unitOfWork, IConfiguration configuration)
		{
			this.basketRedisRepo = basketRedisRepo;
			this.mapper = mapper;
			this.userContext = userContext;
			this.unitOfWork = unitOfWork;
			this.configuration = configuration;
		}

		public async Task<UsersMainAddresse> GetUsersMainAddresseByUserIdAsync(string userId)
		{
			var address = await userContext.UsersMainAddresse.FirstOrDefaultAsync(ua => ua.AppUserId == userId);
			return address!;
		}

		//Create Order //Test It
		public async Task<Order> prepareOrderAsync(string UserId, int deliveryMethodId, string basketId, UsersDeleviredAddress usersDeleviredAddress)
		{
			var deliveryMethod = userContext.deliveryMethods.FirstOrDefault(dm => dm.Id == deliveryMethodId);
			if (deliveryMethod == null)
			{
				throw new MyException(404, "Delivery Method Not Found");
			}
			//Now We Have Correct deliveryMethodId

			//عاوز بقي اجيب العناصر اللي ف الباسكيت

			var basket = await basketRedisRepo.GetBasket(basketId);
			if (basket == null || basket.basket.Count == 0)
			{
				throw new MyException(400, "Your Basket Is Empty");
			}
			// دلوقتي عاوز ابني ال orderItems

			List<int> productIds = basket.basket.Select(item => item.id).ToList();

			var products = await unitOfWork.GetSpecRepo<Product>().GetAllSpecIncludeAsync(p => productIds.Contains(p.Id));
			List<OrderItem> orderItems = new List<OrderItem>();
			foreach (var item in basket.basket)
			{
				var product = products.FirstOrDefault(p => p.Id == item.id);
				if (product == null)
				{
					throw new MyException(404, "Not Found Any Product With This Id");
				}

				if (product.NewPrice != item.newPrice)
				{
					//هحدث الباسكيت
					basket.basket.FirstOrDefault(bItem => bItem.id == item.id)!.newPrice = product.NewPrice;

					await basketRedisRepo.AddBasket(basketId, basket);

					throw new MyException(409, $"Price Has Been Changed For This Product {product.Name} From {item.newPrice} To {product.NewPrice}");
				}

				orderItems.Add(new OrderItem
				{
					ProductId = item.id,
					ProductName = item.name,
					Quantity = item.quantity,
					Price = product.NewPrice //هنا باخد السعر من الداتا بيز
											 //مش هثق ف السعر من هنا زي م قولت قبل كده
				});

			}
			//كده خلصت ال orderItems
			var user = await userContext.Users.FirstOrDefaultAsync(u => u.Id == UserId);
			var usersDeleviredAddresse = new UsersDeleviredAddresse
			{
				City = usersDeleviredAddress.City,
				Country = usersDeleviredAddress.Country,
				Street = usersDeleviredAddress.Street,
				State = usersDeleviredAddress.State,
				ZipCode = usersDeleviredAddress.ZipCode
			};
			usersDeleviredAddresse.FirstName = user!.FirstName;
			usersDeleviredAddresse.LastName = user!.LastName;
			var order = new Order
			{
				AppUserId = UserId,
				OrderItems = orderItems,
				DeliveryMethodId = deliveryMethodId,
				DeliveryPrice = deliveryMethod.Price,
				SubTotal = orderItems.Sum(item => item.Price * item.Quantity),
				DeliveryAddress = usersDeleviredAddresse
			};


			return order;
			//كده خلصت ال order

			

		}
		public async Task<OrderResultDto> CreateOrderAsync(Order order, string basketId)
		{
			if (!await userContext.Orders.AnyAsync())
			{
				//"DBCC CHECKIDENT ('Orders', RESEED, 0)"
				await userContext.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Orders', RESEED, 0)");
			}
			await userContext.Orders.AddAsync(order);
			var result = await userContext.SaveChangesAsync();
			if (result == 0)
			{
				throw new MyException(500, "An Error Occured While Creating Your Order");
			}
			//كده اتعمل الاوردر
			await basketRedisRepo.RemoveBasket(basketId);
			var orderDto = mapper.Map<OrderResultDto>(order);

			return orderDto;
		}

		//Get Order By Id
		public async Task<OrderResultDto> GetOrderByIdAsync(int orderId)
		{
			//var order= await unitOfWork.GetSpecRepo<Order>().GetByIdSpecAsync(orderId, o => o.OrderItems, o => o.DeliveryMethod);
			var order = await userContext.Orders.Include(o => o.OrderItems).Include(o => o.DeliveryMethod).FirstOrDefaultAsync(o => o.Id == orderId);
			if (order == null)
			{
				throw new MyException(404, "Not Found Any Order With This Id");
			}
			var orderResult = mapper.Map<OrderResultDto>(order);
			return orderResult;
		}
		//Get All Orders For User
		public async Task<IReadOnlyList<OrderResultDto>> GetAllOrdersForUserAsync(string appUserId)
		{
			//var orders= await unitOfWork.GetSpecRepo<Order>().GetAllSpecIncludeAsync(o => o.AppUserId == appUserId, o => o.OrderItems, o => o.DeliveryMethod);

			var orders = await userContext.Orders.Where(o => o.AppUserId == appUserId).Include(o => o.OrderItems).Include(o => o.DeliveryMethod).ToListAsync();
			var ordersResult = mapper.Map<IReadOnlyList<OrderResultDto>>(orders);
			return ordersResult;
		}
		//delete Order By Id
		public async Task DeleteOrderByIdAsync(int orderId)
		{
			//var order= await unitOfWork.GetRepo<Order>().GetByIdAsyncWithoutTrack(orderId);
			var order = userContext.Orders.FirstOrDefault(o => o.Id == orderId);
			if (order == null)
			{
				throw new MyException(404, "Not Found Any Order With This Id");
			}
			//unitOfWork.GetRepo<Order>().Remove(orderId);
			userContext.Orders.Remove(order);
			var result = await userContext.SaveChangesAsync();
			if (result == 0)
			{
				throw new MyException(500, "An Error Occured While Deleting Your Order");
			}
		}
		//Get All Delivery Methods
		public async Task<IReadOnlyList<DeliveryMethod>> GetAllDeliveryMethodsAsync()
		{
			//var deliveryMethods= await unitOfWork.GetRepo<DeliveryMethod>().GetAllAsync();
			var deliveryMethods = await userContext.deliveryMethods.ToListAsync();
			return deliveryMethods;
		}
		public async Task<DeliveryMethod> GetDeliveryById(int id)
		{
			var deliveryMethod = await userContext.deliveryMethods.FirstOrDefaultAsync(d => d.Id == id);
			return deliveryMethod!;

		}

		//Save New Main Address
		public async Task SaveNewMainAddress(string userId, SetUsersMainAddresse usersMainAddresse)
		{
			var existingAddress = await userContext.UsersMainAddresse.FirstOrDefaultAsync(ua => ua.AppUserId == userId);

			existingAddress = mapper.Map(usersMainAddresse, existingAddress);
			userContext.UsersMainAddresse.Update(existingAddress!);
			var result = await userContext.SaveChangesAsync();
			if (result == 0)
			{
				throw new MyException(500, "An Error Occured While Saving Your Main Address");
			}

		}
		public async Task<Order> CreateOrUpdatePaymentIntentAsync(string UserId, int deliveryMethodId, string basketId, UsersDeleviredAddress usersDeleviredAddress)
		{
			

			StripeConfiguration.ApiKey = configuration["Stripe:Secret"];
			var order = await prepareOrderAsync(UserId, deliveryMethodId, basketId, usersDeleviredAddress);
			var amount = (long)(order.TotalPrice * 100); // Stripe بالسنت

			var paymentIntentService = new PaymentIntentService();
			PaymentIntent paymentIntent;

			if (!string.IsNullOrEmpty(order.PaymentIntentId))
			{
				paymentIntent = await paymentIntentService.UpdateAsync(order.PaymentIntentId, new PaymentIntentUpdateOptions
				{
					Amount = amount
				});
			}
			else
			{
				paymentIntent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
				{
					Amount = amount,
					Currency = "usd",
					PaymentMethodTypes = new List<string> { "card" },
					Metadata = new Dictionary<string, string>
			{
				{ "order_id", order.Id.ToString() },
				{ "user_id", UserId }
			}
				});

				order.PaymentIntentId = paymentIntent.Id;
				order.ClientSecret = paymentIntent.ClientSecret;
			}
			await CreateOrderAsync( order,  basketId);
			//await userContext.Orders.AddAsync(order);
			//await userContext.SaveChangesAsync();
			return order;
		}

		public async Task HandleWebhookAsync(string json, string sigHeader)
		{
			var webhookSecret = configuration["Stripe:WebhookSecret"];
			Event stripeEvent;

			try
			{
				stripeEvent = EventUtility.ConstructEvent(json, sigHeader, webhookSecret);
			}
			catch (Exception ex)
			{
				throw new MyException(400, $"Webhook error: {ex.Message}");
			}

			if (stripeEvent.Type == "payment_intent.succeeded")
			{
				var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
				if (paymentIntent == null) return;

				var order = await userContext.Orders.FirstOrDefaultAsync(o => o.PaymentIntentId == paymentIntent.Id);
				if (order != null)
				{
					
					order.PaymentStatus = PaymentStatus.Success;

					 userContext.Orders.Update(entity: order);
					await userContext.SaveChangesAsync();
				}
			}
			else if (stripeEvent.Type == "payment_intent.payment_failed")
			{
				var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
				if (paymentIntent == null) return;

				var order = await userContext.Orders.FirstOrDefaultAsync(o => o.PaymentIntentId == paymentIntent.Id);
				if (order != null)
				{
					order.PaymentStatus = PaymentStatus.Failed;
					userContext.Orders.Update(entity: order);
					await userContext.SaveChangesAsync();
				}
			}
		}

	}
}
