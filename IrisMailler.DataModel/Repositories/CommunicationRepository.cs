using IrisMailler.DataModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrisMailler.DataModel.Repositories
{
	public class CommunicationRepository : RepositoryBase<Communication>
	{
		public CommunicationRepository(IUnitOfWork unitOfWork)
			: base(unitOfWork)
		{
		}

		public IEnumerable<Communication> FindByFolder(int folderId)
		{
			return dbSet.Where(x => x.FolderNode.Id == folderId);
		}
	}
}
