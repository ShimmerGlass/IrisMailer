using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrisMailler.DataModel.Entities
{
	public class Campaign : EntityBase
	{
		public override int Id { get; set; }
		public FolderNode FolderNode { get; set; }
		[Required]
		public string Name { get; set; }
		public string Description { get; set; }
	}
}
