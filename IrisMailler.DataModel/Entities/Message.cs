using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrisMailler.DataModel.Entities
{
	public class Message : EntityBase
	{
		public override int Id { get; set; }
		public FolderNode FolderNode { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Subject { get; set; }
		public string MailFrom { get; set; }
		public string NameFrom { get; set; }
		public string ReplyTo { get; set; }
		[Column(TypeName = "ntext")]
		public string HtmlBody { get; set; }
		[Column(TypeName = "ntext")]
		public string TextBody { get; set; }
	}
}
