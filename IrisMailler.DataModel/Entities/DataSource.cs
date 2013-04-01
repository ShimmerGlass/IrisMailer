using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrisMailler.DataModel.Entities
{
	public class DataSource : EntityBase
	{
		public override int Id { get; set; }
		public FolderNode FolderNode { get; set; }
		public string ConnectionString { get; set; }
		public string TableName { get; set; }
		public string DataSchemaXml { get; set; }		
	}
}
