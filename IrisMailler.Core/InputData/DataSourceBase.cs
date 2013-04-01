using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace IrisMailler.Core.InputData
{
	public abstract class DataSourceBase
	{
		protected List<DataColumn> columns;
		public abstract IReadOnlyCollection<DataColumn> Columns
		{
			get;
		}

		public List<Clause> Clauses
		{
			get;
			set;
		}

		private int limit;
		public int Limit
		{
			get
			{
				return limit;
			}
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException("Limit", value, "Limit must be greater than zero.");
				limit = value;
			}
		}

		public TestingOptions TestingConfiguration { get; set; }

		private int testCasesCursor;
		private int testMailingListCursor;

		protected Dictionary<string, object> row;
		public Dictionary<string, object> Row
		{
			get
			{
				return row;
			}
		}

		protected long rowCount;
		public long RowCount
		{
			get
			{
				if (Mode == DataSourceMode.Production)
					return rowCount;
				else
				{
					return TestingConfiguration.Cases.Count * TestingConfiguration.EmailList.Count;
				}
			}
		}

		public bool IsReaderInited { get; protected set; }

		public DataSourceMode Mode { get; set; }

		public enum DataSourceMode
		{
			Production,
			Test
		}

		public struct DataColumn {
			public string Name;
			public Type DataType;

			public DataColumn(string name, Type dataType) {
				Contract.Requires(name != null);
				Contract.Requires(dataType != null);

				Name = name;
				DataType =  dataType;
			}
		}

		protected abstract void InitReader();

		protected abstract void FinalizeReader();

		protected abstract bool ReadData();

		public abstract long GetRowCount();

		protected abstract List<DataColumn> GetColumns();
		

		public bool Read()
		{
			if (Mode == DataSourceMode.Test)
			{
				if (TestingConfiguration.Cases.Count == testCasesCursor)
					return false;
				Clauses = TestingConfiguration.Cases[testCasesCursor];
				Limit = TestingConfiguration.EmailList.Count;
			}

			if (!IsReaderInited)
				InitReader();

			bool hasData;
			if ((Mode == DataSourceMode.Test && testMailingListCursor == 0) || Mode == DataSourceMode.Production)
				hasData = ReadData();
			else
				hasData = true;

			if (hasData)
			{
				if (Mode == DataSourceMode.Test)
				{
					Row[TestingConfiguration.EmailColumn] = TestingConfiguration.EmailList[testMailingListCursor];

					if (testMailingListCursor + 1 < TestingConfiguration.EmailList.Count)
						testMailingListCursor++;
					else
					{
						testMailingListCursor = 0;
						testCasesCursor++;
						FinalizeReader();
					}
				}

				return true;
			}
			else
			{
				FinalizeReader();
				return false;
			}
		}

		
	}
}
