using CoreLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.unitOfWork
{
	public interface ICategoryRepo:ISpecGenericRepo<Category>
	{
		Task DeleteAllCategories();
	}
}
