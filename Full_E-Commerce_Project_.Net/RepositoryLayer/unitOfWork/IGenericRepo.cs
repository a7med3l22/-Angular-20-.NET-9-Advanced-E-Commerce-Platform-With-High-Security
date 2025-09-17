using CoreLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.unitOfWork
{
	public interface IGenericRepo<T> where T : BaseClass<int>
	{
		Task<IReadOnlyList<T>> GetAllAsync();
		//Task<IReadOnlyList<T>> GetAllSpecAsync(params Expression<Func<T, object>>[] funcsSpec);
		Task<T?> GetByIdAsyncWithoutTrack(int Id);
		//Task<T?> GetByIdSpecAsync(int Id, params Expression<Func<T, object>>[] funcs);
		Task<T?> GetByIdAsyncWithTrack (int Id);
		Task AddAsync(T entity);
		void Update(T entity);
		void Remove(int id);
	}
}
