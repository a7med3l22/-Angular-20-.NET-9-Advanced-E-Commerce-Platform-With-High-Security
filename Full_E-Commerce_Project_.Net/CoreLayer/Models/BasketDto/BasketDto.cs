using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Models.BasketDto
{
	public record BasketDto
	{
		public List<BasketItemDto> basket { get; set; }
	}
	public record BasketItemDto
	{
		//انا مش محتاج احفظ اي حاجة ف الباسكت غير ال Id وال Quantity والباقي حاجات هاجيبها من الداتا بيز
		public int id { get; set; }
		public string name { get; set; }
		public string description { get; set; }
		public decimal oldPrice { get; set; }
		public decimal newPrice { get; set; }
		public List<string> photosUrl { get; set; }
		public string categoryName { get; set; }   // هغير المنطق علشان تبقي العملية اسرع وانا وضحت ف ال باسكت ريبو التفاصيل 
		public int quantity { get; set; } 
	}
	/// <summary>
	/// ///////////////
	/// </summary>
	/// 
	//public record ReturnedBasketDto
	//{
	//	public List<ReturnedBasketItemDto> basket { get; set; }
	//}
	//public record ReturnedBasketItemDto
	//{
	//	//انا مش محتاج احفظ اي حاجة ف الباسكت غير ال Id وال Quantity والباقي حاجات هاجيبها من الداتا بيز
	//	public int id { get; set; }
	//	public string name { get; set; }
	//	public string description { get; set; }
	//	public decimal oldPrice { get; set; }
	//	public decimal newPrice { get; set; }
	//	public List<string> photosUrl { get; set; }
	//	public string categoryName { get; set; }
	//	public int quantity { get; set; }
	//}
}
