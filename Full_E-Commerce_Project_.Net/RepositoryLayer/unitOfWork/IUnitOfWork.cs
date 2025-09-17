using CoreLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.unitOfWork
{
	public interface IUnitOfWork
	{
		IGenericRepo<T> GetRepo<T>() where T : BaseClass<int>;
		ISpecGenericRepo<T> GetSpecRepo<T>() where T : BaseClass<int>;
		IProductRepo GetProductRepo();
		ICategoryRepo GetCategoryRepo();
		Task<int> saveChangesAsync();
	}
}
