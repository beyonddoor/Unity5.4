using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.CecilTools.Extensions;

namespace Unity.CecilTools
{
	public static class CecilUtils
	{
		[CompilerGenerated]
		private sealed class <TypeAndBaseTypesOf>c__Iterator0 : IEnumerable<TypeDefinition>, IEnumerator<TypeDefinition>, IEnumerable, IEnumerator, IDisposable
		{
			internal TypeReference typeReference;

			internal TypeDefinition <typeDefinition>__0;

			internal int $PC;

			internal TypeDefinition $current;

			internal TypeReference <$>typeReference;

			TypeDefinition IEnumerator<TypeDefinition>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.$current;
				}
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.$current;
				}
			}

			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<Mono.Cecil.TypeDefinition>.GetEnumerator();
			}

			[DebuggerHidden]
			IEnumerator<TypeDefinition> IEnumerable<TypeDefinition>.GetEnumerator()
			{
				if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
				{
					return this;
				}
				CecilUtils.<TypeAndBaseTypesOf>c__Iterator0 <TypeAndBaseTypesOf>c__Iterator = new CecilUtils.<TypeAndBaseTypesOf>c__Iterator0();
				<TypeAndBaseTypesOf>c__Iterator.typeReference = this.<$>typeReference;
				return <TypeAndBaseTypesOf>c__Iterator;
			}

			public bool MoveNext()
			{
				uint num = (uint)this.$PC;
				this.$PC = -1;
				switch (num)
				{
				case 0u:
					break;
				case 1u:
					this.typeReference = this.<typeDefinition>__0.BaseType;
					break;
				default:
					return false;
				}
				if (this.typeReference != null)
				{
					this.<typeDefinition>__0 = this.typeReference.CheckedResolve();
					this.$current = this.<typeDefinition>__0;
					this.$PC = 1;
					return true;
				}
				this.$PC = -1;
				return false;
			}

			[DebuggerHidden]
			public void Dispose()
			{
				this.$PC = -1;
			}

			[DebuggerHidden]
			public void Reset()
			{
				throw new NotSupportedException();
			}
		}

		public static MethodDefinition FindInTypeExplicitImplementationFor(MethodDefinition interfaceMethod, TypeDefinition typeDefinition)
		{
			return typeDefinition.Methods.SingleOrDefault((MethodDefinition m) => m.Overrides.Any((MethodReference o) => o.CheckedResolve().SameAs(interfaceMethod)));
		}

		public static IEnumerable<TypeDefinition> AllInterfacesImplementedBy(TypeDefinition typeDefinition)
		{
			return (from i in CecilUtils.TypeAndBaseTypesOf(typeDefinition).SelectMany((TypeDefinition t) => t.Interfaces)
			select i.CheckedResolve()).Distinct<TypeDefinition>();
		}

		[DebuggerHidden]
		public static IEnumerable<TypeDefinition> TypeAndBaseTypesOf(TypeReference typeReference)
		{
			CecilUtils.<TypeAndBaseTypesOf>c__Iterator0 <TypeAndBaseTypesOf>c__Iterator = new CecilUtils.<TypeAndBaseTypesOf>c__Iterator0();
			<TypeAndBaseTypesOf>c__Iterator.typeReference = typeReference;
			<TypeAndBaseTypesOf>c__Iterator.<$>typeReference = typeReference;
			CecilUtils.<TypeAndBaseTypesOf>c__Iterator0 expr_15 = <TypeAndBaseTypesOf>c__Iterator;
			expr_15.$PC = -2;
			return expr_15;
		}

		public static IEnumerable<TypeDefinition> BaseTypesOf(TypeReference typeReference)
		{
			return CecilUtils.TypeAndBaseTypesOf(typeReference).Skip(1);
		}

		public static bool IsGenericList(TypeReference type)
		{
			return type.Name == "List`1" && type.SafeNamespace() == "System.Collections.Generic";
		}

		public static TypeReference ElementTypeOfCollection(TypeReference type)
		{
			ArrayType arrayType = type as ArrayType;
			if (arrayType != null)
			{
				return arrayType.ElementType;
			}
			if (CecilUtils.IsGenericList(type))
			{
				return ((GenericInstanceType)type).GenericArguments.Single<TypeReference>();
			}
			throw new ArgumentException();
		}
	}
}
