using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrisMailler.Core.InputData
{
	public class TestingOptions
	{
		public string EmailColumn { get; set; }
		public List<string> EmailList { get; set; }
		public List<List<Clause>> Cases { get; set; }
	}
}
