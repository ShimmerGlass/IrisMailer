using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrisMailler.DataModel
{
	public interface IUnitOfWork : IDisposable
	{
		EntityContext Context { get; }

		void Commit();
	}
}
