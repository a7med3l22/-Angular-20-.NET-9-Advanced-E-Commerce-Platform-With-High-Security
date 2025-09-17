using CoreLayer.Models;
using RepositoryLayer.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.unitOfWork
{
	public class CategoryRepo:GenericSpecRepository<Category>, ICategoryRepo
	{
		public CategoryRepo(ApplicationDbContext dbContext):base(dbContext)
		{
			
		}
		//Delete All Categories
		public async Task DeleteAllCategories()
		{
			
			var categories = await GetAllSpecIncludeAsync(c=>c.AllProductsInCategory);
			foreach (var category in categories)
			{
				foreach (var product in category.AllProductsInCategory.Select(p=>p.Id))
				{
					var productFolderPath=Path.Combine(Directory.GetCurrentDirectory(),"wwwroot","Images","Products",product.ToString());
					if (Directory.Exists(productFolderPath))
						Directory.Delete(productFolderPath, true);
				}
			}
			dbContext.Categories.RemoveRange(categories);
			await dbContext.SaveChangesAsync();
		}
		//Gold
		//ف اي حاجة مش بيحتاج غير ال ID ولكن ممكن اعمل تراكينج للكائن لو عاوز اني لما اعمله ابديت يقارن بين القيم وينفذ كويري للقيم اللي اتغيرت بس


		//dbContext.Attach(product); 
		//dbContext.Entry(product).Property(p => p.Name).IsModified = true; //يعني علشان استخدم دي لازم الكائن يبقي اتاش الاول علشان اعمله حاله مبدأيه وبعدين اغيرها زي م غيرتها هنا

		//Attach = "عرّف الكائن للـ DbContext وخلي حالته Unchanged".
		//IsModified = "قول للـ EF العمود ده بس اتغير".
		//كده هيعمل Update للـ Name بس مش كل الأعمدة.
	}
}
