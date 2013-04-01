using IrisMailler.Core.Template;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace IrisMailler.Core.Output
{
	public static class SendPool
	{
		private static List<Item> items;

		public static SendingState State { get; private set; }

		public static IEnumerable<MaillingCommunication> ActiveCommunications
		{
			get
			{
				return items.ToList().Select(i => i.Com);
			}
		}

		public static IEnumerable<IModifier<TemplateMessage>> PreModifiers { get; set; }

		public static IEnumerable<IModifier<MailMessage>> PostModifiers { get; set; }

		public static event EventHandler<MaillingCommunication> SendingStarted;
		private static void OnSendingStarted(MaillingCommunication com)
		{
			if (SendingStarted != null)
				Task.Factory.StartNew(() =>
				{
					SendingStarted(null, com);
				});
		}

		public static event EventHandler<MaillingCommunication> SendingCompleted;
		private static void OnSendingCompleted(MaillingCommunication com)
		{
			if (SendingCompleted != null)
				Task.Factory.StartNew(() =>
				{
					SendingCompleted(null, com);
				});
		}

		public static event EventHandler<SendingCancelledEventArgs> SendingCancelled;
		private static void OnSendingCancelled(MaillingCommunication com, Exception ex)
		{
			if (SendingCancelled != null)
				Task.Factory.StartNew(() =>
				{
					SendingCancelled(null, new SendingCancelledEventArgs
					{
						CancelledCommunication = com,
						Reason = ex
					});
				});
		}

		public static event EventHandler<IEnumerable<MaillingCommunication>> ActiveCommunicationsChanged;
		private static void OnActiveCommunicationsChanged()
		{
			if (SendingStarted != null)
				Task.Factory.StartNew(() =>
				{
					ActiveCommunicationsChanged(null, ActiveCommunications);
				});
		}

		static SendPool()
		{
			items = new List<Item>();
			PostModifiers = new List<IModifier<MailMessage>>();
			PreModifiers = new List<IModifier<TemplateMessage>>();
		}

		public async static Task SendCommunication(MaillingCommunication communication)
		{
			Contract.Requires(communication != null);

			if (communication.State == SendingState.Sending)
				throw new Exception("Communication is already sending.");

			await Task.Factory.StartNew((a) =>
			{
				IEnumerable<IModifier<TemplateMessage>> preModifiers = communication.PreModifiers.Concat(PreModifiers);
				IEnumerable<IModifier<MailMessage>> postModifiers = communication.PostModifiers.Concat(PostModifiers);

				TemplateMessage templateMessage = TemplateMessage.ExecuteModifiers(communication.Message, preModifiers);

				CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
				var sender = new ActionBlock<MailMessage>((message) =>
				{
					bool success = true;
					try
					{
						SmtpSender.Send(message);
					}
					catch
					{
						success = false;
					}
					communication.AddSendResult(new MaillingCommunication.SendResult(DateTime.Now, success));
				}, new ExecutionDataflowBlockOptions
				{
					MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded,
					CancellationToken = cancellationTokenSource.Token,
					SingleProducerConstrained = true,
					BoundedCapacity = DataflowBlockOptions.Unbounded
				});
				DateTime lastItem = DateTime.MinValue;
				var delayer = new TransformBlock<MailMessage, MailMessage>(
					async x =>
					{
						var waitTime = lastItem + communication.SendDelay - DateTime.UtcNow;
						if (waitTime > TimeSpan.Zero)
							await Task.Delay(waitTime);

						lastItem = DateTime.UtcNow;

						return x;
					},
					new ExecutionDataflowBlockOptions
					{
						BoundedCapacity = 1,
						CancellationToken = cancellationTokenSource.Token,
						SingleProducerConstrained = true
					}
				);
				var modifier = new TransformBlock<MailMessage, MailMessage>(
					(mess) =>
					{
						return mess.ExecuteModifiers(postModifiers);
					},
					new ExecutionDataflowBlockOptions
					{
						CancellationToken = cancellationTokenSource.Token,
						BoundedCapacity = DataflowBlockOptions.Unbounded,
						SingleProducerConstrained = true,
						MaxDegreeOfParallelism = 1
					}
				);
				var mapper = new TransformBlock<dynamic, MailMessage>(
					(row) =>
					{
						object data = communication.DataSchema.MapData(row);
						return templateMessage.Gen(data);
					},
					new ExecutionDataflowBlockOptions
					{
						CancellationToken = cancellationTokenSource.Token,
						BoundedCapacity = DataflowBlockOptions.Unbounded,
						SingleProducerConstrained = true,
						MaxDegreeOfParallelism = 1
					}
				);

				mapper.LinkTo(modifier, new DataflowLinkOptions
				{
					PropagateCompletion = true
				});
				modifier.LinkTo(delayer, new DataflowLinkOptions
				{
					PropagateCompletion = true
				});
				delayer.LinkTo(sender, new DataflowLinkOptions
				{
					PropagateCompletion = true
				});

				items.Add(new Item
				{
					Com = communication,
					CancelletionSource = cancellationTokenSource
				});

			
				try
				{
					State = SendingState.Sending;
					communication.State = SendingState.Sending;
					communication.StartSendingTime = DateTime.Now;
					OnSendingStarted(communication);
					while (communication.DataSource.Read())
						mapper.Post(communication.DataSource.Row);
					mapper.Complete();
					sender.Completion.Wait();
					OnSendingCompleted(communication);
					communication.State = SendingState.Stopped;
				}
				catch (AggregateException ex)
				{
					communication.State = SendingState.Cancelled;
					OnSendingCancelled(communication, ex.InnerException);
				}
				catch (Exception ex)
				{
					communication.State = SendingState.Cancelled;
					OnSendingCancelled(communication, ex);
				}
				finally
				{
					items.RemoveAll(i => i.Com == communication);
					State = SendingState.Stopped;
				}
			}, null, TaskCreationOptions.PreferFairness);
		}

		public static void StopCommunication(MaillingCommunication communication)
		{
			Contract.Requires(communication != null);

			try
			{
				items.Single(i => i.Com == communication).CancelletionSource.Cancel();
				communication.State = SendingState.Cancelled;
				OnSendingCancelled(communication, new Exception("User cancelled the sending."));
			}
			catch
			{
				throw new Exception("Specified communication was not found in the SendPool.");
			}
		}

		public static void StopAll()
		{
			try
			{
				foreach (Item i in items)
					i.CancelletionSource.Cancel();
			}
			catch (Exception ex)
			{
				throw new Exception("Error while stopping sendpool", ex);
			}
		}

		private class Item
		{
			public MaillingCommunication Com;
			public CancellationTokenSource CancelletionSource;
		}
	}

	public class SendingCancelledEventArgs : EventArgs
	{
		public MaillingCommunication CancelledCommunication;
		public Exception Reason;
	}
}
