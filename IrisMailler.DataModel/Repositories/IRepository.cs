using IrisMailler.DataModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IrisMailler.DataModel.Repositories
{
	public interface IRepository<T> where T : EntityBase
	{
		IQueryable<T> GetAll();

		IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);

		T FindById(int id);

		void Add(T entity);

		void Delete(T entity);

		void Edit(T entity);
	}
}
