using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrisMailler.Core.InputData
{
	public class DataSchemaKey
	{
		public string Name { get; set; }

		public DataSchemaKey Parent { get; set; }

		public IList<DataSchemaKey> Childs { get; set; }

		public DataSchemaKeyType Type { get; set; }

		public string Path
		{
			get
			{
				if (Parent != null)
					return Name + "_" + Parent.Path;
				else
					return Name;
			}
		}

		public DataSchemaKey(DataSchemaKeyType type, string name)
		{
			Contract.Requires(name != null);

			Type = type;
			Name = name;
			Childs = new List<DataSchemaKey>();
		}

		public void AddChild(DataSchemaKey key)
		{
			Contract.Requires(key != null);

			key.Parent = this;
			Childs.Add(key);
		}
	}

	public enum DataSchemaKeyType 
	{
		Value,
		IteratedValue,
		Iterator,
		Container
	}
}
