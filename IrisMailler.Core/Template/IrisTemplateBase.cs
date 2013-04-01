using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IrisMailler.Core.Template
{
	public partial class IrisTemplateBase<T> : TemplateBase<T>
	{
		public string ToUpperCase(string s)
		{
			return s.ToUpper();
		}

		public string ToLowerCase(string s)
		{
			return s.ToLower();
		}

		public string UrlEncode(string s)
		{
			return HttpUtility.UrlEncode(s);
		}

		public string UrlDecode(string s)
		{
			return HttpUtility.UrlDecode(s);
		}

		public string HtmlEncode(string s)
		{
			return HttpUtility.HtmlEncode(s);
		}

		public string HtmlDecode(string s)
		{
			return HttpUtility.HtmlDecode(s);
		}
	}
}
