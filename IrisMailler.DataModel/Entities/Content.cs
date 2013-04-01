using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrisMailler.DataModel.Entities
{
	public class Content : EntityBase
	{
		public override int Id { get; set; }
		public string Name { get; set; }
		public string ContentString { get; set; }
		public int FolderNodeId { get; set; }
		public virtual FolderNode FolderNode { get; set; }
	}
}
