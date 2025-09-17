using CoreLayer.Models;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Data.Context;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace RepositoryLayer.unitOfWork
{
	public class GenericSpecRepository<T> : GenericRepository<T>,ISpecGenericRepo<T> where T : BaseClass<int>
	{
		public GenericSpecRepository(ApplicationDbContext dbContext) : base(dbContext)
		{

		}

		public async Task<IReadOnlyList<T>> GetAllSpecIncludeAsync(params Expression<Func<T, object>>[] funcsSpec)
		{
			IQueryable<T> query = dbContext.Set<T>().AsNoTracking();
			query=funcsSpec.Aggregate(query, (current, func) => current.Include(func));
			return await query.ToListAsync();
		}
		public async Task<IReadOnlyList<T>> GetAllSpecIncludeWithIncludeAsync(params Expression<Func<T, object>>[] func)
		{

			return await GetAllSpecIncludeWithIncludec(func).ToListAsync();
		}
		private protected  IQueryable<T> GetAllSpecIncludeWithIncludec(params Expression<Func<T, object>>[] func)
		{

			IQueryable<T> query = dbContext.Set<T>().AsNoTracking();

			//
			foreach (var funcItem in func)
			{
				// c => c.AllProductsInCategory.First().Photos.First());
				// النتيجة: "AllProductsInCategory.Photos"
				var expressionValue = funcItem.Body;
				var includePath = GetPath(expressionValue);
				query = query.Include(includePath);
			}

			return query;
		}

		public async Task<T?> GetByIdSpecAsync(int Id, params Expression<Func<T, object>>[] funcs)
		{
			IQueryable<T> query = dbContext.Set<T>().AsNoTracking();
			//query = funcs.Aggregate(query, (value, func) => value.Include(func));
			
			foreach (var funcItem in funcs)
			{
				// c => c.AllProductsInCategory.First().Photos.First());
				// النتيجة: "AllProductsInCategory.Photos"
				var expressionValue = funcItem.Body;
				var includePath = GetPath(expressionValue);
				query = query.Include(includePath);
			}


			return await query.FirstOrDefaultAsync(p => p.Id == Id);
		}
		private string GetPath(Expression expression) //فهمك صح بنسبة 100% 👏 [Gold]
		{
			//GetPath(c.AllProductsInCategory.First().Photos.First()) =>1  هيرجع ال ريتيرن بتاع ال GetPath(c.AllProductsInCategory.First().Photos) => "AllProductsInCategory.Photos"         




			if (expression is MemberExpression memberExpression)
			{

				//parentPath=GetPath(c.AllProductsInCategory.First());  =>3   هترجع ال ريتيرن بتاع GetPath(c.AllProductsInCategory)    =>"AllProductsInCategory" 
				//parentPath=GetPath(c); =>5 => هترجع "" 

				var parentPath = GetPath(memberExpression.Expression);// 

				

				return string.IsNullOrEmpty(parentPath)
					? memberExpression.Member.Name
					: $"{parentPath}.{memberExpression.Member.Name}";


			}
			else if (expression is MethodCallExpression methodCallExpression
					 && (methodCallExpression.Method.Name == "First"
						 || methodCallExpression.Method.Name == "FirstOrDefault"))
			{
				// لو فيه استدعاء First() أو FirstOrDefault()

				//GetPath(c.AllProductsInCategory.First().Photos) =>2                     =>هترجع   =>"AllProductsInCategory.Photos"  
				//GetPath(c.AllProductsInCategory)                =>4                     =>هترجع   =>"AllProductsInCategory"   
				return GetPath(methodCallExpression.Arguments[0]);
			}
			else if (expression is UnaryExpression unaryExpression && unaryExpression.NodeType == ExpressionType.Convert)
			{
				// لو فيه عملية تحويل (Convert)
				return GetPath(unaryExpression.Operand);
			}
			else if (expression is ParameterExpression)
			{
				// ده معناه إننا وصلنا لجذر ال Expression (c => ...)


				return string.Empty;
			}
			else
			{

				throw new InvalidOperationException("Unsupported expression type");
			}

		}

	//""
	//

		
	}
}
