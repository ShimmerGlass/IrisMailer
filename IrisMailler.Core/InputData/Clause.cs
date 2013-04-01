using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrisMailler.Core.InputData
{
	public class Clause
	{
		public string ColumnName { get; set; }
		public ComparisonOperator Operator { get; set; }
		public object Value { get; set; }
		public bool CaseSensitive { get; set; }
		public int Level { get; set; }

		public enum ComparisonOperator
		{
			Equals,
			NotEquals,
			BeginWith,
			EndWith,
			Contains,
			GreaterThan,
			LowerThan,
			GreaterOrEquals,
			LowerOrEquals
		};

		public Clause(string ColumnName, ComparisonOperator Operator, object Value, bool CaseSensitive, int Level)
		{
			Contract.Requires(ColumnName != null);

			this.ColumnName = ColumnName;
			this.Operator = Operator;
			this.Value = Value;
			this.CaseSensitive = CaseSensitive;
			this.Level = Level;
		}

		public Clause(string ColumnName, ComparisonOperator Operator, object Value, int Level)
		{
			Contract.Requires(ColumnName != null);

			this.ColumnName = ColumnName;
			this.Operator = Operator;
			this.Value = Value;
			this.CaseSensitive = true;
			this.Level = Level;
		}

		public Clause(string ColumnName, ComparisonOperator Operator, object Value, bool CaseSensitive)
		{
			Contract.Requires(ColumnName != null);

			this.ColumnName = ColumnName;
			this.Operator = Operator;
			this.Value = Value;
			this.CaseSensitive = CaseSensitive;
			this.Level = 1;
		}

		public Clause(string ColumnName, ComparisonOperator Operator, object Value)
		{
			Contract.Requires(ColumnName != null);

			this.ColumnName = ColumnName;
			this.Operator = Operator;
			this.Value = Value;
			this.CaseSensitive = true;
			this.Level = 1;
		} 
	}
}
