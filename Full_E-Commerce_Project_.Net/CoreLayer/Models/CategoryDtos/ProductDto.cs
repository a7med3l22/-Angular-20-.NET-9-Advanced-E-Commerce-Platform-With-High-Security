using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Models.CategoryDtos
{
	public record ProductDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal OldPrice { get; set; }
		public decimal NewPrice { get; set; }
		// every Product Has ICollection Of Photos
		public List<string> PhotosUrl { get; set; } 
		// every Product Related To One Category
		public string CategoryName { get; set; }
		public int CategoryId { get; set; } // لازم اعملها علشان هستخدمها ف الاضافه لل جيسون هحط هنا رقم الكاتيجوري

	}
}
