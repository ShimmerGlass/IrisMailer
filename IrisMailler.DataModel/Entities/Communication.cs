using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrisMailler.DataModel.Entities
{
	public class Communication : EntityBase
	{
		public override int Id { get; set; }
		public FolderNode FolderNode { get; set; }
		public virtual Campaign Campaign { get; set; }
		public virtual DataSource DataSource { get; set; }
		public Message Message { get; set; }
		[ConcurrencyCheck]
		public CommunicationState State { get; set; }
		public int SpeedLimit { get; set; }
		public string ScheduleString { get; set; }
	}

	public enum CommunicationState
	{
		SendingPending,
		Sending,
		CancelledPending,
		Stopped
	}
}
