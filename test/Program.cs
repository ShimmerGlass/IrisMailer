using IrisMailler.Core;
using IrisMailler.Core.InputData;
using IrisMailler.Core.Output;
using IrisMailler.Core.Template;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;

namespace test
{
	class Program
	{
		static void Main(string[] args)
		{
			SendPool.SendingStarted += SendPool_SendingStarted;
			SendPool.SendingCompleted += SendPool_SendingCompleted;
			SendPool.SendingCancelled += SendPool_SendingCancelled;

			for (int i = 0; i < 10; i++)
			{ 
				SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
				builder.DataSource = "(local)";
				builder.IntegratedSecurity = true;
				builder.InitialCatalog = "IrisData";

				SqlServerDataSource dataSource = new SqlServerDataSource(builder.ConnectionString, "mailing");
				dataSource.Limit = 5000;

				DataSchema schema = new DataSchema();
				var names = new DataSchemaIterator("Names", 2);
				names.AddChild(new DataSchemaIteratorValue("Name", new List<DataSchemaIteratorValueMappedColumn>()
				{
					new DataSchemaIteratorValueMappedColumn(new Dictionary<DataSchemaIterator, int>()
					{
						{names, 0}
					}, "Name"),
					new DataSchemaIteratorValueMappedColumn(new Dictionary<DataSchemaIterator, int>()
					{
						{names, 1}
					}, "FirstName")
				}));

				schema.Root.AddChild(names);
				schema.Root.AddChild(new DataSchemaValue("id", "id"));

				TemplateMessage message = new TemplateMessage
				{
					Subject = "Salut @Model.id",
					Email = "fze@fze.fe",
					MailFrom = "tg@tg.com",
					NameFrom = "Thib",
					HtmlBody = "Hey ça va ?"
				};

				MaillingCommunication com = new MaillingCommunication(dataSource, message, schema);


				com.CompileTemplate();

				
				SendPool.SendCommunication(com);
			}

			while (true)
			{
				long s = 0;
				foreach (MaillingCommunication c in SendPool.ActiveCommunications)
					s += c.CurrentSpeed;
				Console.WriteLine("Speed : " + s);
				Thread.Sleep(1000);
			}

			Console.ReadLine();
		}

		static void SendPool_SendingCancelled(object sender, SendingCancelledEventArgs e)
		{
			Console.WriteLine("Sending cancelled, " + e.Reason.Message);
		}

		static void SendPool_SendingCompleted(object sender, MaillingCommunication e)
		{
			Console.WriteLine("The sending has completed");
		}

		static void SendPool_SendingStarted(object sender, MaillingCommunication e)
		{
			Console.WriteLine("SendPool has started sending a communication !");
			
		}
	}
}
