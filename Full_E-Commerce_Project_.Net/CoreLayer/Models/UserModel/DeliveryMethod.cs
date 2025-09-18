namespace CoreLayer.Models.UserModel
{
	public class DeliveryMethod
	{
		public int Id { get; set; } // Primary Key
		public string Name { get; set; } = string.Empty; // مثلا "توصيل سريع" أو "استلام من الفرع"
		public decimal Price { get; set; } // تكلفة التوصيل
		public int DeliveryTimeInDays { get; set; } // مدة التوصيل المتوقعة بالأيام
	}
}