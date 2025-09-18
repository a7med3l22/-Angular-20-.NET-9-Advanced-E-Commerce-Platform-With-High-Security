namespace CoreLayer.Models.UserModel
{
	public class OrderItem:BaseClass<int>
	{
//		تحفظ في الـ OrderItem:

//ProductId(لو المنتج لسه موجود).

//ProductName(copy وقت الشراء).

//Price(copy وقت الشراء).

//Quantity.

// // //
//لو المنتج اتمسح → الأوردر ما يتأثرش لأن الاسم والسعر محفوظين snapshot.

//لو المنتج لسه موجود → تقدر تعمل تحليلات (مبيعات per ProductId، تقارير...).

//Best Practice في أغلب أنظمة الـ E-Commerce(Amazon, Talabat, Jumia…) بيعملوا كده.

	
		public decimal Price { get; set; }

		public int? ProductId { get; set; }
		//"أنا باخد Snapshot: الاسم والسعر إجباريين، إنما ProductId مجرد Reference، لو المنتج لسه موجود يبقى حلو، لو اتمسح مش فارق معايا."
		public string ProductName { get; set; }
		public int Quentity { get; set; }
	}
}