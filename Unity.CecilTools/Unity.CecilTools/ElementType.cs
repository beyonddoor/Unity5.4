using Mono.Cecil;
using System;

namespace Unity.CecilTools
{
	public static class ElementType
	{
		public static TypeReference For(TypeReference byRefType)
		{
			TypeSpecification typeSpecification = byRefType as TypeSpecification;
			if (typeSpecification != null)
			{
				return typeSpecification.ElementType;
			}
			throw new ArgumentException(string.Format("TypeReference isn't a TypeSpecification {0} ", byRefType));
		}
	}
}
