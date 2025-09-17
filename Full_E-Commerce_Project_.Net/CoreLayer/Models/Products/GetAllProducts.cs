using CoreLayer.Models.CategoryDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Models.Products
{
	public class GetAllProducts<T> where T : class
	{
		public int PageNumber { get; set; }
		public int PageSize { get; set; }
		public int AllProductsCount { get; set; }
		public int TotalReturnedProducts { get; set; }
		public int ReturnedPaginationProducts { get; set; }
		public IReadOnlyList<T> Products { get; set; }


	}
}
