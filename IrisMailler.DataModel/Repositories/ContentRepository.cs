using IrisMailler.DataModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrisMailler.DataModel.Repositories
{
	public class ContentRepository : RepositoryBase<Content>
	{
		public ContentRepository(IUnitOfWork unitOfWork)
			: base(unitOfWork)
		{
		}

		public IEnumerable<Content> FindByFolder(int? folderId)
		{
			return dbSet.Where(x => x.FolderNode.Id == folderId);
		}
	}
}
