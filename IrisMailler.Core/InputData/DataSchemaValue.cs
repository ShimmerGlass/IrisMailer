using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IrisMailler.Core.InputData
{

	public class DataSchemaValue : DataSchemaKey
	{
		public string MappedColumn { get; set; }

		public DataSchemaValue(string name, string mappedColumn) : base(DataSchemaKeyType.Value, name)
		{
			Contract.Requires(name != null);
			Contract.Requires(mappedColumn != null);

			MappedColumn = mappedColumn;
		}
	}
}
