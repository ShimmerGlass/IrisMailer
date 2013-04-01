using IrisMailler.DataModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrisMailler.DataModel.Repositories
{
	public class FolderNodeRepository : RepositoryBase<FolderNode>
	{
		public FolderNodeRepository(IUnitOfWork unitOfWork)
			: base(unitOfWork)
		{
		}

		public IEnumerable<FolderNode> FindByFolder(int? folderId)
		{
			return dbSet.Where(x => x.ParentNode.Id == folderId).ToList();
		}

		public IEnumerable<FolderNode> FindByRoot(FolderNodeType type)
		{
			return dbSet.Where(x => x.ParentNode == null && x.Type == type).ToList();
		}
	}
}
