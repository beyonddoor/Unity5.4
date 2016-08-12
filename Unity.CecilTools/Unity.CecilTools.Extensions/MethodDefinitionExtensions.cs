using Mono.Cecil;
using System;

namespace Unity.CecilTools.Extensions
{
	internal static class MethodDefinitionExtensions
	{
		public static bool SameAs(this MethodDefinition self, MethodDefinition other)
		{
			return self.FullName == other.FullName;
		}

		public static string PropertyName(this MethodDefinition self)
		{
			return self.Name.Substring(4);
		}

		public static bool IsConversionOperator(this MethodDefinition method)
		{
			return method.IsSpecialName && (method.Name == "op_Implicit" || method.Name == "op_Explicit");
		}

		public static bool IsSimpleSetter(this MethodDefinition original)
		{
			return original.IsSetter && original.Parameters.Count == 1;
		}

		public static bool IsSimpleGetter(this MethodDefinition original)
		{
			return original.IsGetter && original.Parameters.Count == 0;
		}

		public static bool IsSimplePropertyAccessor(this MethodDefinition method)
		{
			return method.IsSimpleGetter() || method.IsSimpleSetter();
		}

		public static bool IsDefaultConstructor(MethodDefinition m)
		{
			return m.IsConstructor && !m.IsStatic && m.Parameters.Count == 0;
		}
	}
}
