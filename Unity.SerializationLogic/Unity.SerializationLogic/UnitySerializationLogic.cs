using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.CecilTools;
using Unity.CecilTools.Extensions;

namespace Unity.SerializationLogic
{
	public static class UnitySerializationLogic
	{
		[CompilerGenerated]
		private sealed class <AllFieldsFor>c__Iterator0 : IEnumerable<KeyValuePair<FieldDefinition, TypeResolver>>, IEnumerator<KeyValuePair<FieldDefinition, TypeResolver>>, IDisposable, IEnumerator, IEnumerable
		{
			internal TypeDefinition definition;

			internal TypeReference <baseType>__0;

			internal GenericInstanceType <genericBaseInstanceType>__1;

			internal TypeResolver typeResolver;

			internal IEnumerator<KeyValuePair<FieldDefinition, TypeResolver>> <$s_4>__2;

			internal KeyValuePair<FieldDefinition, TypeResolver> <kv>__3;

			internal Collection<FieldDefinition>.Enumerator <$s_5>__4;

			internal FieldDefinition <fieldDefinition>__5;

			internal int $PC;

			internal KeyValuePair<FieldDefinition, TypeResolver> $current;

			internal TypeDefinition <$>definition;

			internal TypeResolver <$>typeResolver;

			KeyValuePair<FieldDefinition, TypeResolver> IEnumerator<KeyValuePair<FieldDefinition, TypeResolver>>.Current
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
				return this.System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<Mono.Cecil.FieldDefinition,Unity.SerializationLogic.TypeResolver>>.GetEnumerator();
			}

			[DebuggerHidden]
			IEnumerator<KeyValuePair<FieldDefinition, TypeResolver>> IEnumerable<KeyValuePair<FieldDefinition, TypeResolver>>.GetEnumerator()
			{
				if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
				{
					return this;
				}
				UnitySerializationLogic.<AllFieldsFor>c__Iterator0 <AllFieldsFor>c__Iterator = new UnitySerializationLogic.<AllFieldsFor>c__Iterator0();
				<AllFieldsFor>c__Iterator.definition = this.<$>definition;
				<AllFieldsFor>c__Iterator.typeResolver = this.<$>typeResolver;
				return <AllFieldsFor>c__Iterator;
			}

			public bool MoveNext()
			{
				uint num = (uint)this.$PC;
				this.$PC = -1;
				bool flag = false;
				switch (num)
				{
				case 0u:
					this.<baseType>__0 = this.definition.BaseType;
					if (this.<baseType>__0 == null)
					{
						goto IL_11A;
					}
					this.<genericBaseInstanceType>__1 = (this.<baseType>__0 as GenericInstanceType);
					if (this.<genericBaseInstanceType>__1 != null)
					{
						this.typeResolver.Add(this.<genericBaseInstanceType>__1);
					}
					this.<$s_4>__2 = UnitySerializationLogic.AllFieldsFor(this.<baseType>__0.Resolve(), this.typeResolver).GetEnumerator();
					num = 4294967293u;
					break;
				case 1u:
					break;
				case 2u:
					Block_6:
					try
					{
						switch (num)
						{
						}
						if (this.<$s_5>__4.MoveNext())
						{
							this.<fieldDefinition>__5 = this.<$s_5>__4.Current;
							this.$current = new KeyValuePair<FieldDefinition, TypeResolver>(this.<fieldDefinition>__5, this.typeResolver);
							this.$PC = 2;
							flag = true;
							return true;
						}
					}
					finally
					{
						if (!flag)
						{
							((IDisposable)this.<$s_5>__4).Dispose();
						}
					}
					this.$PC = -1;
					return false;
				default:
					return false;
				}
				try
				{
					switch (num)
					{
					}
					if (this.<$s_4>__2.MoveNext())
					{
						this.<kv>__3 = this.<$s_4>__2.Current;
						this.$current = this.<kv>__3;
						this.$PC = 1;
						flag = true;
						return true;
					}
				}
				finally
				{
					if (!flag)
					{
						if (this.<$s_4>__2 != null)
						{
							this.<$s_4>__2.Dispose();
						}
					}
				}
				if (this.<genericBaseInstanceType>__1 != null)
				{
					this.typeResolver.Remove(this.<genericBaseInstanceType>__1);
				}
				IL_11A:
				this.<$s_5>__4 = this.definition.Fields.GetEnumerator();
				num = 4294967293u;
				goto Block_6;
			}

			[DebuggerHidden]
			public void Dispose()
			{
				uint num = (uint)this.$PC;
				this.$PC = -1;
				switch (num)
				{
				case 1u:
					try
					{
					}
					finally
					{
						if (this.<$s_4>__2 != null)
						{
							this.<$s_4>__2.Dispose();
						}
					}
					break;
				case 2u:
					try
					{
					}
					finally
					{
						((IDisposable)this.<$s_5>__4).Dispose();
					}
					break;
				}
			}

			[DebuggerHidden]
			public void Reset()
			{
				throw new NotSupportedException();
			}
		}

		public static bool WillUnitySerialize(FieldDefinition fieldDefinition)
		{
			return UnitySerializationLogic.WillUnitySerialize(fieldDefinition, new TypeResolver(null));
		}

		public static bool WillUnitySerialize(FieldDefinition fieldDefinition, TypeResolver typeResolver)
		{
			if (fieldDefinition == null)
			{
				return false;
			}
			if (fieldDefinition.IsStatic || UnitySerializationLogic.IsConst(fieldDefinition) || fieldDefinition.IsNotSerialized || fieldDefinition.IsInitOnly)
			{
				return false;
			}
			if (UnitySerializationLogic.ShouldNotTryToResolve(fieldDefinition.FieldType))
			{
				return false;
			}
			bool flag = UnitySerializationLogic.HasSerializeFieldAttribute(fieldDefinition);
			return (fieldDefinition.IsPublic || flag || UnitySerializationLogic.ShouldHaveHadAllFieldsPublic(fieldDefinition)) && !(fieldDefinition.FullName == "UnityScript.Lang.Array") && UnitySerializationLogic.IsFieldTypeSerializable(typeResolver.Resolve(fieldDefinition.FieldType)) && !UnitySerializationLogic.IsDelegate(typeResolver.Resolve(fieldDefinition.FieldType));
		}

		private static bool IsDelegate(TypeReference typeReference)
		{
			return typeReference.IsAssignableTo("System.Delegate");
		}

		public static bool ShouldFieldBePPtrRemapped(FieldDefinition fieldDefinition)
		{
			return UnitySerializationLogic.ShouldFieldBePPtrRemapped(fieldDefinition, new TypeResolver(null));
		}

		public static bool ShouldFieldBePPtrRemapped(FieldDefinition fieldDefinition, TypeResolver typeResolver)
		{
			return UnitySerializationLogic.WillUnitySerialize(fieldDefinition, typeResolver) && UnitySerializationLogic.CanTypeContainUnityEngineObjectReference(typeResolver.Resolve(fieldDefinition.FieldType));
		}

		private static bool CanTypeContainUnityEngineObjectReference(TypeReference typeReference)
		{
			if (UnitySerializationLogic.IsUnityEngineObject(typeReference))
			{
				return true;
			}
			if (typeReference.IsEnum())
			{
				return false;
			}
			if (UnitySerializationLogic.IsSerializablePrimitive(typeReference))
			{
				return false;
			}
			if (UnitySerializationLogic.IsSupportedCollection(typeReference))
			{
				return UnitySerializationLogic.CanTypeContainUnityEngineObjectReference(CecilUtils.ElementTypeOfCollection(typeReference));
			}
			TypeDefinition typeDefinition = typeReference.Resolve();
			return typeDefinition != null && UnitySerializationLogic.HasFieldsThatCanContainUnityEngineObjectReferences(typeDefinition, new TypeResolver(typeReference as GenericInstanceType));
		}

		private static bool HasFieldsThatCanContainUnityEngineObjectReferences(TypeDefinition definition, TypeResolver typeResolver)
		{
			return (from kv in UnitySerializationLogic.AllFieldsFor(definition, typeResolver)
			where kv.Value.Resolve(kv.Key.FieldType).Resolve() != definition
			select kv).Any((KeyValuePair<FieldDefinition, TypeResolver> kv) => UnitySerializationLogic.CanFieldContainUnityEngineObjectReference(definition, kv.Key, kv.Value));
		}

		[DebuggerHidden]
		private static IEnumerable<KeyValuePair<FieldDefinition, TypeResolver>> AllFieldsFor(TypeDefinition definition, TypeResolver typeResolver)
		{
			UnitySerializationLogic.<AllFieldsFor>c__Iterator0 <AllFieldsFor>c__Iterator = new UnitySerializationLogic.<AllFieldsFor>c__Iterator0();
			<AllFieldsFor>c__Iterator.definition = definition;
			<AllFieldsFor>c__Iterator.typeResolver = typeResolver;
			<AllFieldsFor>c__Iterator.<$>definition = definition;
			<AllFieldsFor>c__Iterator.<$>typeResolver = typeResolver;
			UnitySerializationLogic.<AllFieldsFor>c__Iterator0 expr_23 = <AllFieldsFor>c__Iterator;
			expr_23.$PC = -2;
			return expr_23;
		}

		private static bool CanFieldContainUnityEngineObjectReference(TypeReference typeReference, FieldDefinition t, TypeResolver typeResolver)
		{
			return typeResolver.Resolve(t.FieldType) != typeReference && UnitySerializationLogic.WillUnitySerialize(t, typeResolver) && !UnityEngineTypePredicates.IsUnityEngineValueType(typeReference);
		}

		private static bool IsConst(FieldDefinition fieldDefinition)
		{
			return fieldDefinition.IsLiteral && !fieldDefinition.IsInitOnly;
		}

		public static bool HasSerializeFieldAttribute(FieldDefinition field)
		{
			foreach (TypeReference current in UnitySerializationLogic.FieldAttributes(field))
			{
				if (UnityEngineTypePredicates.IsSerializeFieldAttribute(current))
				{
					return true;
				}
			}
			return false;
		}

		private static IEnumerable<TypeReference> FieldAttributes(FieldDefinition field)
		{
			return from _ in field.CustomAttributes
			select _.AttributeType;
		}

		public static bool ShouldNotTryToResolve(TypeReference typeReference)
		{
			if (typeReference.Scope.Name == "Windows")
			{
				return true;
			}
			if (typeReference.Scope.Name == "mscorlib")
			{
				TypeDefinition typeDefinition = typeReference.Resolve();
				return typeDefinition == null;
			}
			try
			{
				typeReference.Resolve();
			}
			catch
			{
				return true;
			}
			return false;
		}

		private static bool IsFieldTypeSerializable(TypeReference typeReference)
		{
			return UnitySerializationLogic.IsTypeSerializable(typeReference) || UnitySerializationLogic.IsSupportedCollection(typeReference);
		}

		private static bool IsTypeSerializable(TypeReference typeReference)
		{
			return !typeReference.IsAssignableTo("UnityScript.Lang.Array") && (UnitySerializationLogic.IsSerializablePrimitive(typeReference) || typeReference.IsEnum() || UnitySerializationLogic.IsUnityEngineObject(typeReference) || UnityEngineTypePredicates.IsSerializableUnityStruct(typeReference) || UnitySerializationLogic.ShouldImplementIDeserializable(typeReference));
		}

		private static bool IsSerializablePrimitive(TypeReference typeReference)
		{
			switch (typeReference.MetadataType)
			{
			case MetadataType.Boolean:
			case MetadataType.Char:
			case MetadataType.SByte:
			case MetadataType.Byte:
			case MetadataType.Int16:
			case MetadataType.UInt16:
			case MetadataType.Int32:
			case MetadataType.UInt32:
			case MetadataType.Int64:
			case MetadataType.UInt64:
			case MetadataType.Single:
			case MetadataType.Double:
			case MetadataType.String:
				return true;
			default:
				return false;
			}
		}

		public static bool IsSupportedCollection(TypeReference typeReference)
		{
			return (typeReference is ArrayType || CecilUtils.IsGenericList(typeReference)) && (!typeReference.IsArray || ((ArrayType)typeReference).Rank <= 1) && UnitySerializationLogic.IsTypeSerializable(CecilUtils.ElementTypeOfCollection(typeReference));
		}

		private static bool ShouldHaveHadAllFieldsPublic(FieldDefinition field)
		{
			return UnityEngineTypePredicates.IsUnityEngineValueType(field.DeclaringType);
		}

		private static bool IsUnityEngineObject(TypeReference typeReference)
		{
			return UnityEngineTypePredicates.IsUnityEngineObject(typeReference);
		}

		public static bool IsNonSerialized(TypeReference typeDeclaration)
		{
			return typeDeclaration == null || typeDeclaration.IsEnum() || typeDeclaration.HasGenericParameters || typeDeclaration.MetadataType == MetadataType.Object || typeDeclaration.FullName.StartsWith("System.") || typeDeclaration.IsArray || typeDeclaration.FullName == "UnityEngine.MonoBehaviour" || typeDeclaration.FullName == "UnityEngine.ScriptableObject";
		}

		public static bool ShouldImplementIDeserializable(TypeReference typeDeclaration)
		{
			if (UnitySerializationLogic.IsNonSerialized(typeDeclaration))
			{
				return false;
			}
			bool result;
			try
			{
				bool arg_7E_0;
				if (!UnityEngineTypePredicates.IsMonoBehaviour(typeDeclaration) && !UnityEngineTypePredicates.IsScriptableObject(typeDeclaration))
				{
					if (typeDeclaration.CheckedResolve().IsSerializable && !typeDeclaration.CheckedResolve().IsAbstract)
					{
						if (!typeDeclaration.CheckedResolve().CustomAttributes.Any((CustomAttribute a) => a.AttributeType.FullName.Contains("System.Runtime.CompilerServices.CompilerGenerated")))
						{
							goto IL_7D;
						}
					}
					arg_7E_0 = UnityEngineTypePredicates.ShouldHaveHadSerializableAttribute(typeDeclaration);
					goto IL_7E;
				}
				IL_7D:
				arg_7E_0 = true;
				IL_7E:
				result = arg_7E_0;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}
	}
}
