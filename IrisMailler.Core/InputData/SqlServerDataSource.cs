using CodeEngine.Framework.QueryBuilder;
using CodeEngine.Framework.QueryBuilder.Enums;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrisMailler.Core.InputData
{
	public sealed class SqlServerDataSource : DataSourceBase
	{
		public string ConnectionString { get; set; }
		public string TableName { get; set; }


		private DbDataReader Reader;
		private SqlConnection SqlConnection;

		public override IReadOnlyCollection<DataColumn> Columns
		{
			get
			{
				if (columns == null)
					return GetColumns().AsReadOnly();
				else
					return columns.AsReadOnly();
			}
		}

		public SqlServerDataSource(string connectionString, string tableName)
		{
			Contract.Requires(connectionString != null);
			Contract.Requires(tableName != null);

			this.ConnectionString = connectionString;
			this.TableName = tableName;
			SqlConnection = new SqlConnection(connectionString);
			Clauses = new List<Clause>();
			Mode = DataSourceMode.Production;
		}

		public SqlServerDataSource(string connectionString, string tableName, TestingOptions testingOptions)
		{
			Contract.Requires(connectionString != null);
			Contract.Requires(tableName != null);

			this.ConnectionString = connectionString;
			this.TableName = tableName;
			SqlConnection = new SqlConnection(connectionString);
			Clauses = new List<Clause>();
			Mode = DataSourceMode.Test;
			TestingConfiguration = testingOptions;
		}

		protected override List<DataColumn> GetColumns()
		{
			List<DataColumn> output = new List<DataColumn>();
			try
			{
				SelectQueryBuilder builder = new SelectQueryBuilder();
				builder.SelectFromTable("information_schema.columns");
				builder.SelectColumns(new string[] { "column_name", "data_type" });
				builder.AddWhere("table_name", Comparison.Equals, TableName);
				builder.SetDbProviderFactory(DbProviderFactories.GetFactory("System.Data.SqlClient"));
				using (DbCommand cmd = builder.BuildCommand())
				{

					SqlConnection.Open();
					try
					{
						cmd.Connection = SqlConnection;
						DbDataReader reader = cmd.ExecuteReader();
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								Type type;
								switch (reader.GetString(1))
								{
									case "bigint": type = typeof(Int64); break;
									case "binary": type = typeof(Byte[]); break;
									case "bit": type = typeof(bool); break;
									case "date": type = typeof(DateTime); break;
									case "datetime": type = typeof(DateTime); break;
									case "datetime2": type = typeof(DateTime); break;
									case "decimal": type = typeof(decimal); break;
									case "float": type = typeof(float); break;
									case "money": type = typeof(decimal); break;
									case "nchar": type = typeof(char[]); break;
									case "ntext": type = typeof(string); break;
									case "numeric": type = typeof(decimal); break;
									case "nvarchar": type = typeof(string); break;
									case "smallint": type = typeof(Int16); break;
									case "text": type = typeof(string); break;
									case "time": type = typeof(TimeSpan); break;
									case "int": type = typeof(int); break;
									case "varchar": type = typeof(string); break;
									default: type = typeof(object); break;
								}
								output.Add(new DataColumn(
									reader.GetString(0),
									type
								));
							}
						}
						else
							throw new Exception("No column found");
					}
					catch (Exception ex)
					{
						throw ex;
					}
					finally
					{
						SqlConnection.Close();
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error while getting column names", ex);
			}

			columns = output;
			return output;
		}

		private void SetClauses(SelectQueryBuilder builder)
		{
			foreach (Clause condition in Clauses)
			{
				switch (condition.Operator)
				{
					case Clause.ComparisonOperator.BeginWith:
						if (condition.CaseSensitive)
							builder.AddWhere(condition.ColumnName, Comparison.Equals, "%" + condition.Value.ToString(), condition.Level);
						else
							builder.AddWhere(condition.ColumnName, Comparison.Like, "%" + condition.Value.ToString(), condition.Level);
						break;

					case Clause.ComparisonOperator.Contains:
						if (condition.CaseSensitive)
							builder.AddWhere(condition.ColumnName, Comparison.Equals, "%" + condition.Value.ToString() + "%", condition.Level);
						else
							builder.AddWhere(condition.ColumnName, Comparison.Like, "%" + condition.Value.ToString() + "%", condition.Level);
						break;

					case Clause.ComparisonOperator.EndWith:
						if (condition.CaseSensitive)
							builder.AddWhere(condition.ColumnName, Comparison.Equals, condition.Value.ToString() + "%", condition.Level);
						else
							builder.AddWhere(condition.ColumnName, Comparison.Like, condition.Value.ToString() + "%", condition.Level);
						break;

					case Clause.ComparisonOperator.Equals:
						if (condition.CaseSensitive)
							builder.AddWhere(condition.ColumnName, Comparison.Equals, condition.Value.ToString(), condition.Level);
						else
							builder.AddWhere(condition.ColumnName, Comparison.Like, condition.Value.ToString(), condition.Level);
						break;

					case Clause.ComparisonOperator.GreaterOrEquals:
						builder.AddWhere(condition.ColumnName, Comparison.GreaterOrEquals, condition.Value, condition.Level);
						break;

					case Clause.ComparisonOperator.GreaterThan:
						builder.AddWhere(condition.ColumnName, Comparison.GreaterThan, condition.Value, condition.Level);
						break;

					case Clause.ComparisonOperator.LowerOrEquals:
						builder.AddWhere(condition.ColumnName, Comparison.LessOrEquals, condition.Value, condition.Level);
						break;

					case Clause.ComparisonOperator.LowerThan:
						builder.AddWhere(condition.ColumnName, Comparison.LessThan, condition.Value, condition.Level);
						break;

					case Clause.ComparisonOperator.NotEquals:
						builder.AddWhere(condition.ColumnName, Comparison.NotEquals, condition.Value, condition.Level);
						break;

				}
			}
		}

		protected override void InitReader()
		{
			GetRowCount();
			if (rowCount < 1)
				throw new Exception("No data is present");

			SelectQueryBuilder builder = new SelectQueryBuilder();
			builder.SelectFromTable(TableName);
			builder.SelectAllColumns();
			if (Limit != 0)
				builder.TopClause = new TopClause(Limit);
			SetClauses(builder);

			builder.SetDbProviderFactory(DbProviderFactories.GetFactory("System.Data.SqlClient"));
			DbCommand cmd = builder.BuildCommand();
			SqlConnection.Open();
			cmd.Connection = SqlConnection;
			Reader = cmd.ExecuteReader();
			IsReaderInited = true;
		}

		protected override void FinalizeReader()
		{
			Reader.Close();
			Reader.Dispose();
			SqlConnection.Close();
			IsReaderInited = false;
		}

		public override long GetRowCount()
		{
			SelectQueryBuilder builder = new SelectQueryBuilder();
			builder.SelectFromTable(TableName);
			builder.SelectCount();
			SetClauses(builder);
			builder.SetDbProviderFactory(DbProviderFactories.GetFactory("System.Data.SqlClient"));
			DbCommand cmd = builder.BuildCommand();
			SqlConnection.Open();
			cmd.Connection = SqlConnection;
			DbDataReader reader = cmd.ExecuteReader();
			reader.Read();
			long value = (long)reader.GetInt32(0);
			reader.Close();
			reader.Dispose();
			SqlConnection.Close();
			rowCount = value;
			return value;
		}

		protected override bool ReadData()
		{
			try
			{
				if (Reader.HasRows)
				{

					if (Reader.Read())
					{
						row = new Dictionary<string, object>();
						for (int i = 0; i < Reader.FieldCount; i++)
							row.Add(Reader.GetName(i), Reader.GetValue(i));
						return true;
					}
					else
						return false;
				}
				else return false;
			}
			catch (Exception ex)
			{
				throw new Exception("Error while retreiving data", ex);
			}
		}
	}
}
