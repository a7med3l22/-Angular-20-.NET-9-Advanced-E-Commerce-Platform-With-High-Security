using CoreLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.unitOfWork
{
	public interface ISpecGenericRepo<T>: IGenericRepo<T> where T : BaseClass<int>
	{
		//Task<IReadOnlyList<T>> GetAllSpecIncludeAsync(params Expression<Func<T, object>>[] funcsSpec);
		Task<IReadOnlyList<T>> GetAllSpecIncludeWithIncludeAsync(params Expression<Func<T, object>>[] func);
		Task<T?> GetByIdSpecAsync(int Id, params Expression<Func<T, object>>[] funcs);
		
	}
}
