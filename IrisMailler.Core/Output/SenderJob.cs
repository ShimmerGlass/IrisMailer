using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrisMailler.Core.Output
{
	public class SenderJob : IJob
	{
		public SenderJob() { }

		public void Execute(IJobExecutionContext context)
		{
			object com = context.JobDetail.JobDataMap["Communication"];
			MaillingCommunication c = (MaillingCommunication)com;
			try 
			{
				SendPool.SendCommunication(c).Wait();
			}
			catch (Exception ex)
			{
				throw new JobExecutionException(ex, false);
			}
		}
	} 
}
