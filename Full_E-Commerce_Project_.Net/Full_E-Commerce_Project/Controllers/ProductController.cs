using AutoMapper;
using CoreLayer.Handle_Exception;
using CoreLayer.Models;
using CoreLayer.Models.CategoryDtos;
using CoreLayer.Models.Dtos;
using CoreLayer.Models.Products;
using Full_E_Commerce_Project.Handle_MiddleWares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.unitOfWork;

namespace Full_E_Commerce_Project.Controllers
{
	[Authorize]
	public class ProductController : GrandController
	{
		private readonly IMapper mapper;
		private readonly IUnitOfWork unitOfWork;

		public ProductController(IMapper mapper,IUnitOfWork unitOfWork)
		{
			this.mapper = mapper;
			this.unitOfWork = unitOfWork;
		}
		//addProduct-UpdateProduct-RemoveProduct
		[HttpPost]
		public async Task<IActionResult> AddProduct(AddProductDto addProductDto)
		{
			var productRepo = unitOfWork.GetProductRepo();
			await productRepo.AddProduct(addProductDto);
			return Ok(new { Message = "Added Product Successfully!!" });
		}
		[HttpPut("{productId}")]
		public async Task<IActionResult> UpdateProduct(int productId, UpdateProductDto UpdateProductDto)
		{
			var productRepo = unitOfWork.GetProductRepo();
			await productRepo.UpdateProduct(productId, UpdateProductDto);
			if (productRepo.isDeletedFromDB)
			{
				return Ok(new
				{
					Message = "Updated product successfully!",
					Warning = "Some images were deleted from DB because they don't exist locally."
				});
			}
			else
			{
				return Ok(new { Message = "Updated Product Successfully!!" });
			}
		}


		[HttpDelete("{productId}")]
		public async Task<IActionResult> DeleteProduct(int productId)
		{
			var productRepo = unitOfWork.GetProductRepo();
			await productRepo.DeleteProduct(productId);
			return Ok(new { Message = "Deleted Product Successfully!!" });
		}
		// getAllProducts-getProductById
		[HttpGet("AllProducts")]
		public async Task<ActionResult<GetAllProducts<ProductDto>>> GetAllProducts([FromQuery]ProductParams productParams)
		{
			//Console.Beep(1000, duration: 500); // يصدر صوت بتردد 1000Hz لمدة نص ثانية
			var productRepo = unitOfWork.GetProductRepo();

			//productRepo.Get_Photos_Name_Hash_InLocal(); //For Getting 
			var ProductsQuery = productRepo.AllProductsWithSearchAndSortByQuery(productParams.CategoryId, productParams.SortBy, productParams.Search, product => product.Photos, product => product.Category);
			var products = await productRepo.Get(ProductsQuery,productParams.PageNumber, productParams.PageSize);
			var ProductsDto=mapper.Map<IReadOnlyList<ProductDto>>(products);
			var AllProductsCount = await productRepo.GetCountAsync(); // For All Products Count
			var TotalReturnedProducts =await productRepo.RetrunedProductsCountAsync(ProductsQuery); // For Total Returned Products Count
			var ReturnedPaginationProducts = ProductsDto.Count; // For Total Returned Products Count

			return Ok(new GetAllProducts<ProductDto> {
				AllProductsCount= AllProductsCount,
				TotalReturnedProducts= TotalReturnedProducts,
				ReturnedPaginationProducts= ReturnedPaginationProducts,
				Products = ProductsDto,
				PageNumber= productParams.PageNumber,
				PageSize= productParams.PageSize


			});
		}
		[HttpGet("{productId}")]
		public async Task<IActionResult> GetProductById(int productId)
		{
			//Console.Beep(3000, duration: 20); // يصدر صوت بتردد 1000Hz لمدة نص ثانية

			var productRepo = unitOfWork.GetProductRepo();
			var product = await productRepo.GetByIdSpecAsync(productId,product => product.Photos, product => product.Category);
		
			var ProductDto=mapper.Map<ProductDto>(product);

			return Ok(ProductDto);
		}
	}
}
