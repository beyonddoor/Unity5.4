using Mono.Cecil;
using System;

namespace Unity.CecilTools.Extensions
{
	public static class TypeDefinitionExtensions
	{
		public static bool IsSubclassOf(this TypeDefinition type, string baseTypeName)
		{
			TypeReference baseType = type.BaseType;
			return baseType != null && (baseType.FullName == baseTypeName || baseType.CheckedResolve().IsSubclassOf(baseTypeName));
		}
	}
}
