using CoreLayer.Models;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Data.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.unitOfWork
{
	public class GenericRepository<T>:IGenericRepo<T> where T : BaseClass<int>
	{
		// class category dont has navigitional proprty  at this time and also class photo 
		// just class product has navigitional proprty at this time
		// so i dont need to use Spec in category or photos
		protected readonly ApplicationDbContext dbContext;

		public GenericRepository(ApplicationDbContext dbContext )
		{
			this.dbContext = dbContext;
		}
		//GetAll
		public async Task<IReadOnlyList<T>> GetAllAsync()
		{
			return await dbContext.Set<T>().AsNoTracking().ToListAsync();
		}
		
		//GetById
		public async Task<T?> GetByIdAsyncWithoutTrack(int Id)
		{
			IQueryable<T> query = dbContext.Set<T>().AsNoTracking();
			return await query.FirstOrDefaultAsync(p => p.Id == Id);
		}
		public async Task<T?> GetByIdAsyncWithTrack(int Id)
		{
			return  await dbContext.Set<T>().FindAsync(Id);
			// وز تراك مفيد ف حاجة واحده 
			// لو انا عاوز اعمل ابديت لعنصر مثلا ف اجيبه وز تراك وبعدين اعدل ف اللي انا عاوز اعدل فيه عن طريق ال برودكت اللي جبته ده 
			// من هنا بقي ال EF هيبدأ يقارن القيم اللي موجوده ف ال برودكت ويغير بس القيم ال مختلفة 
			// UPDATE Products SET Name = 'New Name' WHERE Id = 2;
			// انما لو جبت ال برودكت وز نو تراك هيغير كل القيم بمعلومية ال id اللي انتا باعته بس انما مش هيقارن القديم ب الجديد ونفس البيانات اللي ف الداتا بيز هتبقي واحدة ف الاخر ب اي طريقة م الاتنين لكن وز نو تراكينج هينفذ امر تعديل كامل لكل ال كولومز ف الداتا بيز مش اللي القيمة بتاعته اتغيرت بس  
			//UPDATE Products SET Name = 'New Name',....................  WHERE Id = 2;
			// ف زي م قولت هيحصل نفس النتيجة بس الامر اللي رايح للداتا بيز هيبقي اكتر 
			// اول ممكن اجيبه مش اتراك عادي وقبل م اعمله ابديت اعمل كده dbContext.Photos.Attach(product);
		}
		//Add
		public async Task AddAsync(T entity)
		{
			await dbContext.Set<T>().AddAsync(entity);
		}
		//Update
		public  void Update(T entity)
		{
			 dbContext.Set<T>().Update(entity);
		}
		//Remove
		public void Remove(int id)
		{
			var entity = Activator.CreateInstance<T>(); 
			entity.Id = id;
			dbContext.Set<T>().Remove(entity); // هنا هيمسح بمعلومية ال ID بس 
		}
	}
}
