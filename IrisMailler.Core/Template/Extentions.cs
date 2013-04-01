using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace IrisMailler.Core.Template
{
	public static class Extentions
	{
		public static MailMessage ExecuteModifiers(this MailMessage mess, IEnumerable<IModifier<MailMessage>> modifiers)
		{
			Contract.Requires(modifiers != null);
			 
			foreach (IModifier<MailMessage> mod in modifiers)
				mess = mod.Execute(mess);

			return mess;
		}
	}
}
