using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrisMailler.Core.InputData
{
	public class DataSchemaIteratorValue : DataSchemaKey
	{
		public IList<DataSchemaIteratorValueMappedColumn> MappedColumns { get; set; }

		public DataSchemaIteratorValue(string name, IList<DataSchemaIteratorValueMappedColumn> mappedColumn) : base(DataSchemaKeyType.IteratedValue, name)
		{
			Contract.Requires(name != null);
			Contract.Requires(mappedColumn != null);
			Contract.Requires(mappedColumn.Count > 0);

			MappedColumns = mappedColumn;
		}
	}

	public class DataSchemaIteratorValueMappedColumn
	{
		public IDictionary<DataSchemaIterator, int> IteratorIndexes;
		public string ColumnName;

		public DataSchemaIteratorValueMappedColumn(IDictionary<DataSchemaIterator, int> iteratorIndexes, string columnName)
		{
			Contract.Requires(iteratorIndexes != null);
			Contract.Requires(iteratorIndexes.Count > 0);
			Contract.Requires(columnName != null);

			IteratorIndexes = iteratorIndexes;
			ColumnName = columnName;
		}
	}
}
