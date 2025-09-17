using CoreLayer.Models;
using CoreLayer.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.unitOfWork
{
	public interface IProductRepo : ISpecGenericRepo<Product>
	{
		public bool isDeletedFromDB { get; set; }
		Task AddProduct(AddProductDto addProductDto);
		Task UpdateProduct(int productId, UpdateProductDto UpdateProduct);
		Task DeleteProduct(int productId);
		Task<int> GetCountAsync();
		void Get_Photos_Name_Hash_InLocal();
		IQueryable<Product> AllProductsWithSearchAndSortByQuery(int? CategoryId, string? SortBy, string? Search, params Expression<Func<Product, object>>[] funcIncludes);
		Task<int> RetrunedProductsCountAsync(IQueryable<Product> query);
		Task<IReadOnlyList<Product>> Get(IQueryable<Product> query, int PageNumber, int PageSize);
	}
}
