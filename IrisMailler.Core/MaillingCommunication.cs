using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IrisMailler.Core.InputData;
using IrisMailler.Core.Output;
using IrisMailler.Core.Template;
using System.Threading;
using System.Collections.Concurrent;
using System.Net.Mail;

namespace IrisMailler.Core
{
	public class MaillingCommunication
	{
		#region Properties

		private DataSourceBase dataSource;
		public DataSourceBase DataSource
		{
			get
			{
				return dataSource;
			}
			set
			{
				if (State != SendingState.Sending)
					dataSource = value;
				else
					throw new Exception("Unable to modify the DataSource while the communication is sending.");
			}
		}

		private DataSchema dataSchema;
		public DataSchema DataSchema
		{
			get
			{
				return dataSchema;
			}
			set
			{
				if (State != SendingState.Sending)
					dataSchema = value;
				else
					throw new Exception("Unable to modify the DataSchema while the communication is sending.");
			}
		}

		private TemplateMessage message;
		public TemplateMessage Message
		{
			get
			{
				return message;
			}
			set
			{
				if (State != SendingState.Sending)
					message = value;
				else
					throw new Exception("Unable to modify the MessageTemplate while the communication is sending.");
			}
		}

		public string ScheduleString { get; set; }

		public IEnumerable<IModifier<MailMessage>> PostModifiers;

		public IEnumerable<IModifier<TemplateMessage>> PreModifiers;

		public TimeSpan SendDelay
		{
			get
			{
				if (MaxSendSpeed != 0)
					return TimeSpan.FromHours(1 / (double)MaxSendSpeed);
				else
					return TimeSpan.Zero;
			}
		}

		public long MaxSendSpeed { get; set; }

		public long CurrentSpeed
		{
			get
			{
				return (long)sendResults.Where(s =>
					s.Date > DateTime.Now.Subtract(TimeSpan.FromSeconds(1))
				).ToList().Count * 3600;
			}
		}

		private SendingState state;
		public SendingState State {
			get {
				return state;
			}
			set 
			{
				OnStateChanged(value);
				state = value;

				if (state == SendingState.Sending)
				{
					sentItemCount = 0;
					failedItemCount = 0;
					sendResults = new ConcurrentBag<SendResult>();
				}

			} 
		}

		private ConcurrentBag<SendResult> sendResults;
		public IReadOnlyCollection<SendResult> SendResults
		{
			get
			{
				return sendResults.ToList().AsReadOnly();
			}
		}

		public DateTime StartSendingTime { get; set; }

		public DateTime EndSendingTime { get; set; }

		public TimeSpan ElapsedSendingTime {
			get {
				if (State == SendingState.Sending)
					return DateTime.Now.Subtract(StartSendingTime);
				else
					return EndSendingTime.Subtract(StartSendingTime);
			}
		}

		private long sentItemCount;
		public long SentItemCount
		{
			get
			{
				return sentItemCount;
			}
		}

		private long failedItemCount;
		public long FailedItemCount
		{
			get
			{
				return failedItemCount;
			}
		}

		public long TotalItemCount
		{
			get
			{
				return sentItemCount + failedItemCount;
			}
		}

		public int SendProgress {
			get
			{
				if (dataSource.RowCount != 0)
					return (int)Math.Round((float)TotalItemCount / (float)dataSource.RowCount * 100);
				else
					return 0;
			}
		}

		#endregion

		#region Events

		public event EventHandler<SendingState> SendStateChanged;
		private void OnStateChanged(SendingState newState)
		{
			if (SendStateChanged != null)
				Task.Factory.StartNew(() =>
				{
					SendStateChanged(this, newState);
				});
		}

		#endregion

		#region Methods

		public MaillingCommunication(DataSourceBase dataSource, TemplateMessage message, DataSchema schema)
		{
			State = SendingState.Stopped;
			this.dataSource = dataSource;
			this.message = message;
			dataSchema = schema;
			sendResults = new ConcurrentBag<SendResult>();
			PostModifiers = new List<IModifier<MailMessage>>();
			PreModifiers = new List<IModifier<TemplateMessage>>();
		}

		public MaillingCommunication(DataSourceBase dataSource, TemplateMessage message, long maxSpeed, DataSchema schema)
		{
			if (maxSpeed < 0)
				throw new ArgumentOutOfRangeException("maxSpeed", maxSpeed, "Maximum speed must be positive");

			State = SendingState.Stopped;
			this.dataSource = dataSource;
			this.message = message;
			dataSchema = schema;
			MaxSendSpeed = maxSpeed;
			sendResults = new ConcurrentBag<SendResult>();
			PostModifiers = new List<IModifier<MailMessage>>();
			PreModifiers = new List<IModifier<TemplateMessage>>();
		}

		public void CompileTemplate() 
		{
			dataSchema.GenerateModel(dataSource.Columns.ToList());
			message.Compile(dataSchema.ModelType);
		}

		public void AddSendResult(SendResult result)
		{
			sendResults.Add(result);
			if (result.Success) Interlocked.Increment(ref sentItemCount);
			else Interlocked.Increment(ref failedItemCount);
		}

		public class SendResult
		{
			public DateTime Date;
			public bool Success;

			public SendResult(DateTime date, bool success = true)
			{
				Date = date;
				Success = success;
			}
		}

		#endregion
	}
}
