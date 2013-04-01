using IrisMailler.DataModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrisMailler.DataModel.Repositories
{
	public class MessageRepository : RepositoryBase<Message>
	{
		public MessageRepository(IUnitOfWork unitOfWork)
			: base(unitOfWork)
		{
		}

		public IEnumerable<Message> FindByFolder(int folderId)
		{
			return dbSet.Where(x => x.FolderNode.Id == folderId);
		}
	}
}
