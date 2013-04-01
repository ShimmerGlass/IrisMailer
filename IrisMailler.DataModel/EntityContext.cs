using IrisMailler.DataModel.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace IrisMailler.DataModel
{
	public class EntityContext : DbContext
	{
		public DbSet<Campaign> Campaigns { get; set; }
		public DbSet<DataSource> DataSources { get; set; }
		public DbSet<Communication> Communications { get; set; }
		public DbSet<Message> Messages { get; set; }
		public DbSet<FolderNode> FolderNodes { get; set; }
		public DbSet<Content> Contents { get; set; }

		public EntityContext()
			: base(IrisMailler.DataModel.Config.ConnectionString)
		{
			base.Configuration.ProxyCreationEnabled = false;
		}
	}
}
