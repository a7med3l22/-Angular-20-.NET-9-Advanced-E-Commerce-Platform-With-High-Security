using AutoMapper;
using CoreLayer.Handle_Exception;
using CoreLayer.Models;
using CoreLayer.Models.CategoryDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Data.Context;
using RepositoryLayer.unitOfWork;
using StackExchange.Redis;
using System.Linq.Expressions;

namespace Full_E_Commerce_Project.Controllers
{
	public class CategoryController : GrandController
	{
		private readonly ApplicationDbContext dbContext;
		private readonly IMapper mapper;
		private readonly IUnitOfWork unitOfWork;

		public CategoryController(ApplicationDbContext dbContext, IMapper mapper, IUnitOfWork unitOfWork)
		{
			this.dbContext = dbContext;
			this.mapper = mapper;
			this.unitOfWork = unitOfWork;
		}
		[HttpPost]
		public async Task<IActionResult> AddCategory(CategoryDto categoryDto)
		{
			// عاوز لو ال كاتيجوري فاضي يصفره
			if (!(await unitOfWork.GetRepo<Category>().GetAllAsync()).Any())
			{
				//    DBCC CHECKIDENT('YourTableName', RESEED, 0);
				await dbContext.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Categories', RESEED, 0);"); // علشان يصفر ال id لو الجدول فاضي يبدأ من 1 يعني 
			}
			var Category = mapper.Map<Category>(categoryDto);
			await unitOfWork.GetRepo<Category>().AddAsync(Category);
			await unitOfWork.saveChangesAsync();
			return Ok(new { Message = "Added Category Successfully" });
		}
		[HttpDelete("{categoryId}")]
		public async Task<IActionResult> RemoveCategory(int categoryId)
		{
			///////Remove Photos That Related To Deleted Category In Folder 
			var ProductsInCategory=await dbContext.Products.Where(p => p.CategoryId == categoryId).ToListAsync();
			var productFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
			foreach (var product in ProductsInCategory)
			{
				var ProductPath = Path.Combine(productFolderPath, product.Id.ToString());
				if (Directory.Exists(ProductPath))
				{
					Directory.Delete(ProductPath,true);
				}
			}// تمام كده مسحت كل حاجة متعلقه ب ال برودكتس المرتبط ب الكاتيجوري اللي اتمسح

			var Category = await unitOfWork.GetRepo<Category>().GetByIdAsyncWithoutTrack(categoryId);
			if (Category == null)
			{
				return BadRequest(new ControllerHandleError(400, "Not Found Any Category With This Id"));
			}
			unitOfWork.GetRepo<Category>().Remove(categoryId);
			await unitOfWork.saveChangesAsync();
			return Ok(new { Message = "Removed Category Successfully" });
		}
		[HttpPut("{categoryId}")]
		public async Task<IActionResult> UpdateCategory(int categoryId, CategoryDto categoryDto)
		{
			var Category = await unitOfWork.GetRepo<Category>().GetByIdAsyncWithTrack(categoryId);
			if (Category == null)
			{
				return BadRequest(new ControllerHandleError(400, "Not Found Any Category With This Id"));
			}
			mapper.Map(categoryDto, Category);
			unitOfWork.GetRepo<Category>().Update(Category);
			await unitOfWork.saveChangesAsync();
			return Ok(new { Message = "Updated Category Successfully" });
		}
		[HttpGet("{id}")]
		public async Task<ActionResult<GetCategoryDto>> GetCategoryById(int id)
		{
			//var Category = await unitOfWork.GetRepo<Category>().GetByIdAsyncWithoutTrack(id);
			var Category = await unitOfWork.GetSpecRepo<Category>().GetByIdSpecAsync(id, c => c.AllProductsInCategory.First().Photos.First());
			if (Category == null)
			{
				return BadRequest(new ControllerHandleError(400, "Not Found Any Category With This Id"));
			}
			var categoryDto = mapper.Map<GetCategoryDto>(Category);
			return Ok(categoryDto);
		}
		[HttpGet]
		public async Task<ActionResult<IReadOnlyList<GetCategoryDto>>> GetAllCategories()
		{
			var CategoryRepo= unitOfWork.GetCategoryRepo();
			var Categories = await CategoryRepo
				.GetAllSpecIncludeWithIncludeAsync
				(

					// c => c.AllProductsInCategory.First().Photos.First();
					// النتيجة: "AllProductsInCategory.Photos.Product"


					c => c.AllProductsInCategory.First().Photos.First()
				);
			//Delete All Categories
			//await CategoryRepo.DeleteAllCategories();       //////Warning !! This Method Just For Test The Delete All Categories Logic

			if (!Categories.Any())
			{
				return BadRequest(new ControllerHandleError(400, "Not Found Any Categories"));
			}
			var categoryDtos = mapper.Map<IReadOnlyList<GetCategoryDto>>(Categories);
			return Ok(categoryDtos);
		}

	}
}



