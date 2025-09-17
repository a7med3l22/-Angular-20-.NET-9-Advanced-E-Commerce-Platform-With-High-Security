using CoreLayer.Models;
using RepositoryLayer.Data.Context;
using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;
using AutoMapper;
namespace RepositoryLayer.unitOfWork
{
	/// <summary>
	/// To Be Continue..
	/// </summary>
	public class UnitOfWork : IUnitOfWork
	{
		public UnitOfWork(ApplicationDbContext dbContext, IMapper mapper)
		{
			this.dbContext = dbContext;
			this.mapper = mapper;
		}

		private readonly ConcurrentDictionary<Type, object> _repo = new();
		private readonly ApplicationDbContext dbContext;
		private readonly IMapper mapper;

		public IGenericRepo<T> GetRepo<T>() where T : BaseClass<int>
		{
			return (IGenericRepo<T>)_repo.GetOrAdd(typeof(T), _ => new GenericRepository<T>(dbContext));
		}
		public ISpecGenericRepo<T> GetSpecRepo<T>() where T : BaseClass<int>
		{
			return (ISpecGenericRepo<T>)_repo.GetOrAdd(typeof(T), _ => new GenericSpecRepository<T>(dbContext));
		}
		public IProductRepo GetProductRepo()
		{
			return (IProductRepo)_repo.GetOrAdd(typeof(ProductRepo), _ => new ProductRepo(mapper,dbContext));
		}
		public ICategoryRepo GetCategoryRepo()
		{
			return (ICategoryRepo)_repo.GetOrAdd(typeof(CategoryRepo), _ => new CategoryRepo( dbContext));
		}
		public async Task<int> saveChangesAsync()
		{
			return await dbContext.SaveChangesAsync();
		}
	
	}
}
/*
 
 
 ممكن تخلي الـ UnitOfWork يطبق IDisposable علشان تعمل Dispose للـ DbContext لما تخلص.
 شات جي بي تي قالي كده وده ردي عليه 
م ي حبيبي انا مش بعمل نيو من ال DbContext  علان اعملها ديسبوز ب ايدي , 
هي بتجيلي من ال DI والمسؤول عن ال Dispose بتاعها هو ال DI Container !
ف قام قالي كده 
💯 بالظبط 👌
إنت كده فاهم الصح يا أحمد.
 */
//بص انا اسمي احمد علاء محمد واحمد ده كلاس 
// ف احمد مثلا انا ممكن ارجعه ك علاء او ك محمد لانه وارث منهم 
// ولو احمد ده رجعته ك علاء او ك محمد ممكن اعمله كاست وارجعه ل احمد او اي( كلاس او انترفيس) بيورث منها احمد  
// دي نفس طريقة ال كاست ف ال سي شارب 