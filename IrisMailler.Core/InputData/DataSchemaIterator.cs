using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrisMailler.Core.InputData
{
	public class DataSchemaIterator : DataSchemaKey
	{
		public int MaxItemCount { get; set; }

		public DataSchemaIterator(string name, int maxItemCount) : base(DataSchemaKeyType.Iterator, name)
		{
			Contract.Requires(name != null);
			Contract.Requires(maxItemCount > 0);

			MaxItemCount = maxItemCount;
		}
	}
}
