using Mono.Cecil;
using System;

namespace Unity.CecilTools.Extensions
{
	public static class TypeReferenceExtensions
	{
		public static string SafeNamespace(this TypeReference type)
		{
			if (type.IsGenericInstance)
			{
				return ((GenericInstanceType)type).ElementType.SafeNamespace();
			}
			if (type.IsNested)
			{
				return type.DeclaringType.SafeNamespace();
			}
			return type.Namespace;
		}

		public static bool IsAssignableTo(this TypeReference typeRef, string typeName)
		{
			bool result;
			try
			{
				if (typeRef.IsGenericInstance)
				{
					result = ElementType.For(typeRef).IsAssignableTo(typeName);
				}
				else if (typeRef.FullName == typeName)
				{
					result = true;
				}
				else
				{
					result = typeRef.CheckedResolve().IsSubclassOf(typeName);
				}
			}
			catch (AssemblyResolutionException)
			{
				result = false;
			}
			return result;
		}

		public static bool IsEnum(this TypeReference type)
		{
			return type.IsValueType && !type.IsPrimitive && type.CheckedResolve().IsEnum;
		}

		public static bool IsStruct(this TypeReference type)
		{
			return type.IsValueType && !type.IsPrimitive && !type.IsEnum() && !TypeReferenceExtensions.IsSystemDecimal(type);
		}

		private static bool IsSystemDecimal(TypeReference type)
		{
			return type.FullName == "System.Decimal";
		}
	}
}
