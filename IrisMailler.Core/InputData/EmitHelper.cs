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
	public class EmitHelper
	{
		private AssemblyBuilder assemblyBuilder;
		private ModuleBuilder module;
		private string fileName;

		public EmitHelper()
		{
			string guid = Guid.NewGuid().ToString();
			this.fileName = guid + ".dll";
			AssemblyName assemblyName = new AssemblyName(guid);
			assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Save);
			module = assemblyBuilder.DefineDynamicModule(assemblyBuilder.GetName().Name, fileName, false);
		}

		public Type CreateType(string typeName, Dictionary<string, Type> properties)
		{
			Contract.Requires(typeName != null);
			Contract.Requires(properties != null);
			Contract.Requires(!String.IsNullOrWhiteSpace(typeName));
			Contract.Requires(properties.Count > 0);

			typeName = typeName + "_" + Guid.NewGuid().ToString().Replace("-", "");

			TypeBuilder typeBuilder = module.DefineType(
					typeName,
					TypeAttributes.Public |
					TypeAttributes.Class,
					typeof(object)
				);

			foreach (KeyValuePair<string, Type> p in properties)
			{
				FieldBuilder field = typeBuilder.DefineField(p.Key, p.Value, FieldAttributes.Public);

				PropertyBuilder property = typeBuilder.DefineProperty(
					"_" + p.Key,
					PropertyAttributes.None,
					p.Value,
					new Type[] { p.Value }
				);

				MethodAttributes GetSetAttr = MethodAttributes.Public;

				MethodBuilder currGetPropMthdBldr = typeBuilder.DefineMethod(
					"get_value",
					GetSetAttr,
					p.Value,
					Type.EmptyTypes
				);

				ILGenerator currGetIL = currGetPropMthdBldr.GetILGenerator();
				currGetIL.Emit(OpCodes.Ldarg_0);
				currGetIL.Emit(OpCodes.Ldfld, field);
				currGetIL.Emit(OpCodes.Ret);

				MethodBuilder currSetPropMthdBldr = typeBuilder.DefineMethod(
					"set_value",
					GetSetAttr,
					null,
					new Type[] { p.Value }
				);

				ILGenerator currSetIL = currSetPropMthdBldr.GetILGenerator();
				currSetIL.Emit(OpCodes.Ldarg_0);
				currSetIL.Emit(OpCodes.Ldarg_1);
				currSetIL.Emit(OpCodes.Stfld, field);
				currSetIL.Emit(OpCodes.Ret);

				property.SetGetMethod(currGetPropMthdBldr);
				property.SetSetMethod(currSetPropMthdBldr);
			}
			return typeBuilder.CreateType();
		}

		public Assembly Save()
		{
			assemblyBuilder.Save(fileName);
			return Assembly.LoadFrom(fileName);
		}
	}
}
