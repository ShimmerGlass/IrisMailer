using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrisMailler.DataModel.Entities
{
	public class FolderNode : EntityBase
	{
		public override int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public FolderNodeType Type { get; set; }
		public virtual FolderNode ParentNode { get; set; }
		public virtual IEnumerable<FolderNode> ChildNodes { get; set; }
		public string Path { get; set; }
	}

	public enum FolderNodeType
	{
		Campaign,
		Communication,
		DataSource,
		Message
	}
}
