using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IrisMailler.Core.InputData
{
	public class DataSchema
	{
		public DataSchemaKey Root;
		public Type ModelType { get; private set; }

		public DataSchema()
		{
			Root = new DataSchemaKey(DataSchemaKeyType.Container, "Model");
		}

		public void GenerateModel(IEnumerable<DataSourceBase.DataColumn> columns)
		{
			Contract.Requires(columns != null);
			Contract.Requires(columns.Count() > 0);

			EmitHelper emit = new EmitHelper();
			ModelType = ResolveTypeInternal(Root, emit, columns);
			try
			{
				Assembly assembly = emit.Save();
				ModelType = assembly.GetType(ModelType.Name);
			}
			catch (Exception ex)
			{
				throw new Exception("Unable to save assembly during model generation", ex);
			}
		}

		private Type ResolveTypeInternal(DataSchemaKey key, EmitHelper emit, IEnumerable<DataSourceBase.DataColumn> columns)
		{
			Dictionary<string, Type> properties = new Dictionary<string, Type>();
			foreach (DataSchemaKey k in key.Childs)
			{
				switch (k.Type)
				{
					case DataSchemaKeyType.Container:
						IEnumerable<DataSchemaKey> ikeys = k.Childs.Where(c => c.Type == DataSchemaKeyType.IteratedValue);
						List<DataSchemaIteratorValue> ivalues = new List<DataSchemaIteratorValue>();
						foreach (DataSchemaKey c in ikeys)
						{
							try
							{
								DataSchemaIteratorValue value = (DataSchemaIteratorValue)c;
								ivalues.Add(value);
							}
							catch (InvalidCastException)
							{
								throw new InvalidDataSchemaException("Invalid DataSchemaKey type for " + c.Name + ", must be DataSchemaIteratorValue.", c);
							}
						}
						if (ivalues.Select(c => c.MappedColumns.Count).Distinct().Count() > 1)
							throw new InvalidDataSchemaException("All DataSchemaIteratorValue of " + k.Name + " must have the same count of MappedColumn", k);

						properties.Add(k.Name, ResolveTypeInternal(k, emit, columns));
						break;

					case DataSchemaKeyType.Value:
						try
						{
							DataSchemaValue value = (DataSchemaValue)k;
							try
							{
								DataSourceBase.DataColumn col = columns.Where(c => c.Name == value.MappedColumn).First();
								properties.Add(k.Name, col.DataType);
							}
							catch
							{
								throw new InvalidDataSchemaException("DataColumn " + value.MappedColumn + " was not present in the provided column collection.", value);
							}
						}
						catch (InvalidCastException)
						{
							throw new InvalidDataSchemaException("Invalid DataSchemaKey type for " + k.Name + ", must be DataSchemeValue.", k);
						}

						break;

					case DataSchemaKeyType.Iterator:
						Type t = ResolveTypeInternal(k, emit, columns);
						properties.Add(k.Name, t.MakeArrayType());
						break;

					case DataSchemaKeyType.IteratedValue:
						try
						{
							DataSchemaIteratorValue value = (DataSchemaIteratorValue)k;

							foreach (DataSchemaIteratorValueMappedColumn mc in value.MappedColumns)
								if (columns.Where(c => c.Name == mc.ColumnName).Count() == 0)
									throw new InvalidDataSchemaException("DataColumn " + mc.ColumnName + " was not present in the provided column collection.", value);

							IEnumerable<Type> columnsTypes = columns
								.Where(c => value.MappedColumns.Select(mc => mc.ColumnName)
								.Contains(c.Name))
								.Select(c => c.DataType)
								.Distinct();

							if (columnsTypes.Count() > 1)
								throw new InvalidDataSchemaException("All mapped columns of " + k.Name + " must have the same datatype.", k);
							properties.Add(k.Name, columnsTypes.First());
						}
						catch (InvalidCastException)
						{
							throw new InvalidDataSchemaException("Invalid DataSchemaKey type for " + k.Name + ", must be DataSchemaIteratorValue.", k);
						}
						break;
				}
			}

			Type returnType = emit.CreateType(key.Path, properties);
			//Contract.Ensures(returnType != null);
			return returnType;
		}

		public object MapData(Dictionary<string, object> row)
		{
			Contract.Requires(row != null);
			Contract.Requires(ModelType != null);

			return MapDataInternal(row, Root, ModelType, new Dictionary<DataSchemaIterator, int>());
		}

		private object MapDataInternal(Dictionary<string, object> row, DataSchemaKey key, Type type, Dictionary<DataSchemaIterator, int> interatorsIndexes)
		{
			Contract.Requires(type != null);

			object generetedObject = Activator.CreateInstance(type);
			var p = type.GetProperties();
			foreach (DataSchemaKey childKey in key.Childs)
			{
				PropertyInfo property = type.GetProperty("_" + childKey.Name);
				object value = new object();
				switch (childKey.Type)
				{
					case DataSchemaKeyType.Container:
						value = MapDataInternal(row, childKey, property.PropertyType, interatorsIndexes);
						break;

					case DataSchemaKeyType.Value:
						DataSchemaValue v = (DataSchemaValue)childKey;
						value = row[v.MappedColumn];
						break;

					case DataSchemaKeyType.Iterator:
						DataSchemaIterator iterator = (DataSchemaIterator)childKey;
						interatorsIndexes.Add(iterator, 0);
						Array array = Array.CreateInstance(property.PropertyType.GetElementType(), iterator.MaxItemCount);

						for (int i = 0; i < iterator.MaxItemCount; i++)
						{
							interatorsIndexes[iterator] = i;
							object arrayValue = MapDataInternal(row, childKey, property.PropertyType.GetElementType(), interatorsIndexes);
							array.SetValue(arrayValue, i);
						}
						value = array;
						break;

					case DataSchemaKeyType.IteratedValue:
						DataSchemaIteratorValue va = (DataSchemaIteratorValue)childKey;
						foreach (DataSchemaIteratorValueMappedColumn mc in va.MappedColumns)
						{
							bool match = true;
							foreach (KeyValuePair<DataSchemaIterator, int> index in mc.IteratorIndexes)
							{
								if (!interatorsIndexes.Contains(index))
								{
									match = false;
									break;
								}
							}
							if (match)
							{
								value = row[mc.ColumnName];
								break;
							}
						}
						break;
				}


				property.SetValue(generetedObject, value, null);
			}
			return generetedObject;
		}
	}
}
