using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using IrisMailler.Core.Template;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics.Contracts;
using RazorEngine;
using RazorEngine.Configuration;

namespace IrisMailler.Core.Template
{
	public class TemplateMessage
	{
		public string MailFrom { get; set; }

		public string NameFrom { get; set; }

		public string ReplyTo { get; set; }

		public string Subject { get; set; }

		public string HtmlBody { get; set; }

		public string TextBody { get; set; }

		public string Email { get; set; }

		private TemplateService templateService;

		public void Compile(Type model)
		{
			Contract.Requires(model != null);

			if (String.IsNullOrWhiteSpace(Subject))
				throw new ArgumentNullException("Subject");
			if (String.IsNullOrWhiteSpace(Email))
				throw new ArgumentNullException("Email");
			if (String.IsNullOrWhiteSpace(MailFrom))
				throw new ArgumentNullException("MailFrom");
			if (String.IsNullOrWhiteSpace(NameFrom))
				throw new ArgumentNullException("NameFrom");

			templateService = new TemplateService(new TemplateServiceConfiguration
			{
				BaseTemplateType = typeof(IrisTemplateBase<>)
			});
			
			templateService.Compile(Subject, model, "Subject");
			templateService.Compile(Email, model, "Email");
			templateService.Compile(MailFrom, model, "MailFrom");
			templateService.Compile(NameFrom, model, "NameFrom");
			templateService.Compile(HtmlBody, model, "HtmlBody");

			if (ReplyTo != null)
				templateService.Compile(ReplyTo, model, "ReplyTo");
			if (TextBody != null)
				templateService.Compile(TextBody, model, "TxtBody");
		}

		public MailMessage Gen(dynamic row)
		{
			try
			{
				MailMessage message = new MailMessage();
				message.From = new MailAddress(
					templateService.Run("MailFrom", row),
					templateService.Run("NameFrom", row)
				);
				message.To.Add(templateService.Run("Email", row));
				if (templateService.HasTemplate("ReplyTo"))
					message.ReplyToList.Add(new MailAddress(
						templateService.Run("ReplyTo", row)
					));
				message.Subject = templateService.Run("Subject", row);
				message.Body = templateService.Run("HtmlBody", row);
				if (templateService.HasTemplate("TxtBody"))
					message.AlternateViews.Add(
						AlternateView.CreateAlternateViewFromString(templateService.Run("TxtBody", row))
					);
				message.IsBodyHtml = true;

				return message;
			}
			catch (Exception ex)
			{
				throw new Exception("Error while generating MailMessage", ex);
			}
		}

		public static TemplateMessage ExecuteModifiers(TemplateMessage tm, IEnumerable<IModifier<TemplateMessage>> modifiers)
		{
			foreach (IModifier<TemplateMessage> mod in modifiers)
				tm = mod.Execute(tm);
			return tm;
		}
	}
}
