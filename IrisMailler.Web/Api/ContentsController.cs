using IrisMailler.DataModel;
using IrisMailler.DataModel.Entities;
using IrisMailler.DataModel.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IrisMailler.Web.Api
{
    public class ContentsController : ApiController
    {
		private UnitOfWork unitOfWork;

		public ContentsController()
		{
			unitOfWork = new UnitOfWork();
		}

		[HttpGet]
		public Tuple<IEnumerable<FolderNode>, IEnumerable<Content>> Folder(int? id)
		{
			ContentRepository contentRepos = new ContentRepository(unitOfWork);
			FolderNodeRepository folderRepos = new FolderNodeRepository(unitOfWork);

			return new Tuple<IEnumerable<FolderNode>, IEnumerable<Content>>(folderRepos.FindByFolder(id), contentRepos.FindByFolder(id));
		}

		[HttpGet]
		public Tuple<IEnumerable<FolderNode>, IEnumerable<Content>> Folder()
		{
			return Folder(null);
		}
    }
}
