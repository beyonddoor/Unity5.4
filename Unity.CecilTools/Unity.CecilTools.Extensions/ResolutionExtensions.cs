using Mono.Cecil;
using System;

namespace Unity.CecilTools.Extensions
{
	public static class ResolutionExtensions
	{
		public static TypeDefinition CheckedResolve(this TypeReference type)
		{
			return ResolutionExtensions.Resolve<TypeReference, TypeDefinition>(type, (TypeReference reference) => reference.Resolve());
		}

		public static MethodDefinition CheckedResolve(this MethodReference method)
		{
			return ResolutionExtensions.Resolve<MethodReference, MethodDefinition>(method, (MethodReference reference) => reference.Resolve());
		}

		private static TDefinition Resolve<TReference, TDefinition>(TReference reference, Func<TReference, TDefinition> resolve) where TReference : MemberReference where TDefinition : class, IMemberDefinition
		{
			if (reference.Module == null)
			{
				throw new ResolutionException(reference);
			}
			TDefinition tDefinition = resolve(reference);
			if (tDefinition == null)
			{
				throw new ResolutionException(reference);
			}
			return tDefinition;
		}
	}
}
