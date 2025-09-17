using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Models.CategoryDtos
{
	public record GetCategoryDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public List<ProductDto> AllProducts { get; set; }

	}
	public record CategoryDto
	{
		public string Name { get; set; }
		public string Description { get; set; }
	}
}
