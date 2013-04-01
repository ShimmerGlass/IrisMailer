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
    public class CampaignsController : ApiController
    {
		private UnitOfWork unitOfWork;

		public CampaignsController()
		{
			unitOfWork = new UnitOfWork();
		}

		[HttpGet]
		public Tuple<IEnumerable<FolderNode>, IEnumerable<Campaign>> Folder(int id)
		{
			CampaignRepository campaignRepos = new CampaignRepository(unitOfWork);
			FolderNodeRepository folderRepos = new FolderNodeRepository(unitOfWork);

			return new Tuple<IEnumerable<FolderNode>, IEnumerable<Campaign>>(folderRepos.FindByFolder(id), campaignRepos.FindByFolder(id));
		}

		[HttpGet]
		public Tuple<IEnumerable<FolderNode>, IEnumerable<Campaign>> Folder()
		{
			CampaignRepository campaignRepos = new CampaignRepository(unitOfWork);
			FolderNodeRepository folderRepos = new FolderNodeRepository(unitOfWork);

			return new Tuple<IEnumerable<FolderNode>, IEnumerable<Campaign>>(folderRepos.FindByRoot(FolderNodeType.Campaign), new List<Campaign>());
		}

		[HttpGet]
		public Campaign Get(int id)
		{
			CampaignRepository campaignRepos = new CampaignRepository(unitOfWork);
			return campaignRepos.FindById(id);
		}
    }
}
