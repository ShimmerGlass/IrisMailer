using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrisMailler.Core.InputData
{
	[Serializable]
	public class InvalidDataSchemaException : Exception
	{
		public DataSchemaKey Key;

		public InvalidDataSchemaException(string message, DataSchemaKey key) : base(message)
		{
			Key = key;
		}
	}
}
