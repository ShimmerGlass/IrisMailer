using EFChangeNotify;
using IrisMailler.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using IrisMailler.Core.InputData;
using IrisMailler.Core.Template;
using IrisMailler.Core;
using IrisMailler.Core.Output;
using IrisMailler.DataModel.Entities;

namespace IrisdMailler.Sender
{
	static class Manager
	{
		private static List<CommunicationWrapper> items;

		public static void Init()
		{
		/*	items = new List<CommunicationWrapper>();

			var notifer = new EntityChangeNotifier<Communication, EntityContext>(c => c.State == CommunicationState.CancelledPending || c.State == CommunicationState.SendingPending);
			
			notifer.Changed += (object sender, EntityChangeEventArgs<Communication> e) =>
			{
				foreach (Communication com in e.Results)
				{
					if (com.State == CommunicationState.SendingPending)
					{
						try
						{
							SqlServerDataSource dataSource = new SqlServerDataSource(com.DataSource.ConnectionString, com.DataSource.TableName);

							DataSchema schema = new DataSchema();
							schema.FromXml(com.DataSource.DataSchemaXml);

							TemplateMessage message = new TemplateMessage
							{
								Subject = com.Message.Subject,
								Email = com.Message.Email,
								MailFrom = com.Message.MailFrom,
								NameFrom = com.Message.NameFrom,
								ReplyTo = com.Message.ReplyTo,
								HtmlBody = com.Message.HtmlBody,
								TextBody = com.Message.TextBody
							};

							MaillingCommunication communication = new MaillingCommunication(dataSource, message, schema);
							communication.CompileTemplate();

							if (String.IsNullOrEmpty(com.ScheduleString))
								SendPool.SendCommunication(communication);
							else
							{
								communication.ScheduleString = com.ScheduleString;
								Scheduler.ScheduleCommunication(communication);
							}

							using (EntityContext context = new EntityContext())
							{
								context.Communications.First(c => c.CommunicationId == com.CommunicationId).State = CommunicationState.Sending;
								context.SaveChanges();
							}
						}
						catch (Exception ex)
						{
						}
					}
					else if (com.State == CommunicationState.CancelledPending)
					{
						try
						{
							MaillingCommunication communication = items.Single(i => i.Communication.CommunicationId == com.CommunicationId).MaillingCommunication;
							SendPool.StopCommunication(communication);

							using (EntityContext context = new EntityContext())
							{
								context.Communications.First(c => c.CommunicationId == com.CommunicationId).State = CommunicationState.Stopped;
								context.SaveChanges();
							}
						}
						catch (Exception ex)
						{
						}
					}
				}

				e.ContinueListening = true;
			};

			/*notifer.Error += (sender, e) =>
			{
				Console.WriteLine("[{0}, {1}, {2}]:\n{3}", e.Reason.Info, e.Reason.Source, e.Reason.Type, e.Sql);
			};*/
		}
	}
}
