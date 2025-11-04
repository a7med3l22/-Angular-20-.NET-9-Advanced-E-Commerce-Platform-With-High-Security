using CoreLayer.Models.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Models.orderDto
{
	public record OrderResultDto
	{
		public int OrderId { get; set; }///
		public ICollection<OrderItem> OrderItems { get; set; } 

		public UsersDeleviredAddresse DeliveryAddress { get; set; } = null!;
		public DateTime DateTime { get;  set; }

		public string PaymentStatus { get; set; }
		public decimal SubTotal { get; set; }

		public decimal Total { get; set; } ///

		public DeliveryMethod? DeliveryMethod { get; set; }

	}
}
