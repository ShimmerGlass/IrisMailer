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
    public class CommunicationsController : ApiController
    {
		private UnitOfWork unitOfWork;
		private CommunicationRepository repos;

		public CommunicationsController()
		{
			unitOfWork = new UnitOfWork();
			repos = new CommunicationRepository(unitOfWork);
		}

		public IQueryable<Communication> GetAllCommunications()
		{
			var coms = repos.GetAll();
			return coms;
		}
    }
}
