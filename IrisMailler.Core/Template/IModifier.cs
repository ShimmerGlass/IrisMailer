using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace IrisMailler.Core.Template
{
	public interface IModifier<T>
	{
		T Execute(T input);
	}
}
