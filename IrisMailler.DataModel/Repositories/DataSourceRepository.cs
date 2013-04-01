using IrisMailler.DataModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrisMailler.DataModel.Repositories
{
	public class DataSourceRepository : RepositoryBase<DataSource>
	{
		public DataSourceRepository(IUnitOfWork unitOfWork)
			: base(unitOfWork)
		{
		}

		public IEnumerable<DataSource> FindByFolder(int folderId)
		{
			return dbSet.Where(x => x.FolderNode.Id == folderId);
		}
	}
}
