using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrisMailler.Core.Output
{
	public static class Scheduler
	{
		private static IScheduler QuartzSheduler;
		private static List<Item> items;

		public static List<MaillingCommunication> ActiveCommunications
		{
			get
			{
				List<MaillingCommunication> coms = new List<MaillingCommunication>();
				foreach (object c in QuartzSheduler.GetCurrentlyExecutingJobs().Select(j => j.JobDetail.JobDataMap["Communication"]))
					coms.Add((MaillingCommunication)c);
				return coms;
			}
		}

		public static List<MaillingCommunication> ScheduledCommunications
		{
			get
			{
				var details = (from groupName in QuartzSheduler.GetJobGroupNames()
							   from jobKey in QuartzSheduler.GetJobKeys(
									Quartz.Impl.Matchers.GroupMatcher<Quartz.JobKey>
									.GroupEquals(groupName))
							   select QuartzSheduler.GetJobDetail(jobKey)
				);

				List<MaillingCommunication> coms = new List<MaillingCommunication>();
				foreach (IJobDetail c in details)
					coms.Add((MaillingCommunication)c.JobDataMap["Communication"]);
				return coms;
			}
		}

		static Scheduler() {
			ISchedulerFactory schedFact = new StdSchedulerFactory();
			QuartzSheduler = schedFact.GetScheduler();
			QuartzSheduler.Start();
			items = new List<Item>();
		}

		public static DateTimeOffset ScheduleCommunication(MaillingCommunication com)
		{
			Contract.Requires(com != null);
			
			ITrigger trigger = TriggerBuilder.Create()
				.WithCronSchedule(com.ScheduleString)
				.Build();

			IJobDetail detail = new JobDetailImpl("senderJob", typeof(SenderJob));
			detail.JobDataMap.Add("Communication", com);

			items.Add(new Item
			{
				Com = com,
				Key = trigger.Key
			});

			return QuartzSheduler.ScheduleJob(detail, trigger);
		}

		public static void UnscheduleCommunication(MaillingCommunication com) {
			Contract.Requires(com != null);

			try {
				Item i = items.Single(it => it.Com == com);
				QuartzSheduler.UnscheduleJob(i.Key);
				items.RemoveAll(it => it.Com == com);
			}
			catch (Exception ex) {
				throw new Exception("Error while unscheduling communication", ex);
			}
		}

		private class Item {
			public MaillingCommunication Com;
			public TriggerKey Key;
		}
	}
}
