using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrisMailler.DataModel
{
	public class UnitOfWork : IUnitOfWork
	{
		private bool isDisposed = false;
		public EntityContext Context { get; private set; }

		public UnitOfWork()
		{
			this.Context = new EntityContext();
		}

		public void Commit()
		{
			this.Context.SaveChanges();
		}

		public void Dispose()
		{
			if (!isDisposed)
				Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			isDisposed = true;
			if (disposing)
			{
				if (this.Context != null)
					this.Context.Dispose();
			}
		}
	}
}
