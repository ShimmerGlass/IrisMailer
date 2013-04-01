using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Diagnostics.Contracts;

namespace IrisMailler.Core.Output
{
	public class SmtpSender
	{
		[ThreadStatic]
		private static SmtpClient client;

		public static void Send(MailMessage message)
		{
			Contract.Requires(message != null);

			if (client == null)
				client = new SmtpClient("127.0.0.1");

			try
			{
				client.Send(message);
			}
			finally
			{
				message.Dispose();
			}
		}
	}
}
