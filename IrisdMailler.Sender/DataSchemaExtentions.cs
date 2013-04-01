using IrisMailler.Core.InputData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IrisdMailler.Sender
{
	public static class DataSchemaExtentions
	{
		public static void FromXml(this DataSchema shema, string xmlString)
		{
			XDocument doc = XDocument.Parse(xmlString);
			ParseNodeInternal(doc.Root, shema.Root);
		}

		private static void ParseNodeInternal(XElement element, DataSchemaKey key)
		{
			foreach (XElement el in element.Elements())
			{
				string name = el.Attribute("name").Value;
				switch (el.Name.LocalName)
				{
					case "value" :
						string mappedColumn = el.Value;
						DataSchemaValue v = new DataSchemaValue(name, mappedColumn);
						ParseNodeInternal(el, v);
						key.AddChild(v);
					break;
					
					case "container" :
						DataSchemaKey c = new DataSchemaKey(DataSchemaKeyType.Container, name);
						ParseNodeInternal(el, c);
						key.AddChild(c);
					break;

					case "iterator" :
						int max = Int32.Parse(el.Attribute("maxItemCount").Value);
						DataSchemaIterator i = new DataSchemaIterator(name, max);
						ParseNodeInternal(el, i);
						key.AddChild(i);
					break;

					case "iteratorValue" :
						Dictionary<DataSchemaIterator,int> iterators = new Dictionary<DataSchemaIterator,int>();
						FindIterators(key, iterators);
						iterators.Reverse();
						List<DataSchemaIteratorValueMappedColumn> columns = new List<DataSchemaIteratorValueMappedColumn>();
						string tmpl = el.Value;
						GenCoulumns(tmpl, iterators, columns, 0);
						DataSchemaIteratorValue iv = new DataSchemaIteratorValue(name, columns);
						ParseNodeInternal(el, iv);
						key.AddChild(iv);
					break;
				}
			}			
		}

		private static void FindIterators(DataSchemaKey k, Dictionary<DataSchemaIterator,int> i)
		{
			if (k.Parent != null)
			{
				if (k.Parent.Type == DataSchemaKeyType.Iterator)
					i.Add((DataSchemaIterator)k.Parent, 0);
				FindIterators(k.Parent, i);
			}
		}

		private static void GenCoulumns(string tmpl, Dictionary<DataSchemaIterator,int> iterators, List<DataSchemaIteratorValueMappedColumn> columns, int index)
		{
			KeyValuePair<DataSchemaIterator, int> it = iterators.ElementAt(index);
			for (var i = 0; i < it.Key.MaxItemCount; i++)
			{
				iterators[it.Key] = i;
				int[] vals = iterators.Select(kvp => kvp.Value).ToArray();
				columns.Add(new DataSchemaIteratorValueMappedColumn(iterators, String.Format(tmpl, vals)));
				if (index < iterators.Count - 1)
					GenCoulumns(tmpl, iterators, columns, index + 1);
			}
		}
				
	}
}
