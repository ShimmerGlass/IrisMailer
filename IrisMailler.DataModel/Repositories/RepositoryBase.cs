using IrisMailler.DataModel.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrisMailler.DataModel.Repositories
{
	public abstract class RepositoryBase<T> : IRepository<T>
		where T : EntityBase
	{
		protected EntityContext context;
		protected DbSet<T> dbSet;

		public RepositoryBase(IUnitOfWork unitOfWork)
		{
			context = unitOfWork.Context;
			dbSet = context.Set<T>();
		}

		public virtual IQueryable<T> GetAll()
		{
			IQueryable<T> query = context.Set<T>();
			return query;
		}

		public IQueryable<T> FindBy(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
		{
			IQueryable<T> query = dbSet.Where(predicate);
			return query;
		}

		public T FindById(int id)
		{
			T query = dbSet.FirstOrDefault(x => x.Id == id);
			return query;
		}

		public virtual void Add(T entity)
		{
			dbSet.Add(entity);
		}

		public virtual void Delete(T entity)
		{
			dbSet.Remove(entity);
		}

		public virtual void Edit(T entity)
		{
			context.Entry(entity).State = EntityState.Modified;
		}
	}
}
