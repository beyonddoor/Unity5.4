using Mono.Cecil;
using Mono.Cecil.Cil;
using System;

namespace Unity.UNetWeaver
{
	internal class SyncListStructProcessor
	{
		private TypeDefinition m_TypeDef;

		private TypeReference m_ItemType;

		public SyncListStructProcessor(TypeDefinition typeDef)
		{
			Weaver.DLog(typeDef, "SyncListStructProcessor for " + typeDef.Name, new object[0]);
			this.m_TypeDef = typeDef;
		}

		public void Process()
		{
			GenericInstanceType genericInstanceType = (GenericInstanceType)this.m_TypeDef.BaseType;
			if (genericInstanceType.GenericArguments.Count == 0)
			{
				Weaver.fail = true;
				Log.Error("SyncListStructProcessor no generic args");
				return;
			}
			this.m_ItemType = Weaver.scriptDef.MainModule.Import(genericInstanceType.GenericArguments[0]);
			Weaver.DLog(this.m_TypeDef, "SyncListStructProcessor Start item:" + this.m_ItemType.FullName, new object[0]);
			Weaver.ResetRecursionCount();
			MethodReference methodReference = this.GenerateSerialization();
			if (Weaver.fail)
			{
				return;
			}
			MethodReference methodReference2 = this.GenerateDeserialization();
			if (methodReference2 == null || methodReference == null)
			{
				return;
			}
			this.GenerateReadFunc(methodReference2);
			this.GenerateWriteFunc(methodReference);
			Weaver.DLog(this.m_TypeDef, "SyncListStructProcessor Done", new object[0]);
		}

		private void GenerateReadFunc(MethodReference readItemFunc)
		{
			string text = "_ReadStruct" + this.m_TypeDef.Name + "_";
			if (this.m_TypeDef.DeclaringType != null)
			{
				text += this.m_TypeDef.DeclaringType.Name;
			}
			else
			{
				text += "None";
			}
			MethodDefinition methodDefinition = new MethodDefinition(text, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static | MethodAttributes.HideBySig, Weaver.voidType);
			methodDefinition.Parameters.Add(new ParameterDefinition("reader", ParameterAttributes.None, Weaver.scriptDef.MainModule.Import(Weaver.NetworkReaderType)));
			methodDefinition.Parameters.Add(new ParameterDefinition("instance", ParameterAttributes.None, this.m_TypeDef));
			methodDefinition.Body.Variables.Add(new VariableDefinition("v0", Weaver.uint16Type));
			methodDefinition.Body.Variables.Add(new VariableDefinition("v1", Weaver.uint16Type));
			methodDefinition.Body.InitLocals = true;
			ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.NetworkReadUInt16));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
			MethodReference method = Helpers.MakeHostInstanceGeneric(Weaver.SyncListClear, new TypeReference[]
			{
				this.m_ItemType
			});
			iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, method));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_1));
			Instruction instruction = iLProcessor.Create(OpCodes.Nop);
			iLProcessor.Append(iLProcessor.Create(OpCodes.Br, instruction));
			Instruction instruction2 = iLProcessor.Create(OpCodes.Nop);
			iLProcessor.Append(instruction2);
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, readItemFunc));
			MethodReference self = Weaver.ResolveMethod(Weaver.SyncListStructType, "AddInternal");
			MethodReference method2 = Helpers.MakeHostInstanceGeneric(self, new TypeReference[]
			{
				this.m_ItemType
			});
			iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, method2));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_1));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4_1));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Add));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Conv_U2));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_1));
			iLProcessor.Append(instruction);
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_1));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Blt, instruction2));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			Weaver.RegisterReadByReferenceFunc(this.m_TypeDef.FullName, methodDefinition);
		}

		private void GenerateWriteFunc(MethodReference writeItemFunc)
		{
			string text = "_WriteStruct" + this.m_TypeDef.GetElementType().Name + "_";
			if (this.m_TypeDef.DeclaringType != null)
			{
				text += this.m_TypeDef.DeclaringType.Name;
			}
			else
			{
				text += "None";
			}
			MethodDefinition methodDefinition = new MethodDefinition(text, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static | MethodAttributes.HideBySig, Weaver.voidType);
			methodDefinition.Parameters.Add(new ParameterDefinition("writer", ParameterAttributes.None, Weaver.scriptDef.MainModule.Import(Weaver.NetworkWriterType)));
			methodDefinition.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, Weaver.scriptDef.MainModule.Import(this.m_TypeDef)));
			methodDefinition.Body.Variables.Add(new VariableDefinition("v0", Weaver.uint16Type));
			methodDefinition.Body.Variables.Add(new VariableDefinition("v1", Weaver.uint16Type));
			methodDefinition.Body.InitLocals = true;
			ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
			MethodReference self = Weaver.ResolveMethod(Weaver.SyncListStructType, "get_Count");
			MethodReference method = Helpers.MakeHostInstanceGeneric(self, new TypeReference[]
			{
				this.m_ItemType
			});
			iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, method));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.NetworkWriteUInt16));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_1));
			Instruction instruction = iLProcessor.Create(OpCodes.Nop);
			iLProcessor.Append(iLProcessor.Create(OpCodes.Br, instruction));
			Instruction instruction2 = iLProcessor.Create(OpCodes.Nop);
			iLProcessor.Append(instruction2);
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_1));
			MethodReference self2 = Weaver.ResolveMethod(Weaver.SyncListStructType, "GetItem");
			MethodReference method2 = Helpers.MakeHostInstanceGeneric(self2, new TypeReference[]
			{
				this.m_ItemType
			});
			iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, method2));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, writeItemFunc));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_1));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4_1));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Add));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Conv_U2));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_1));
			iLProcessor.Append(instruction);
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_1));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Blt, instruction2));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			Weaver.RegisterWriteFunc(this.m_TypeDef.FullName, methodDefinition);
		}

		private MethodReference GenerateSerialization()
		{
			Weaver.DLog(this.m_TypeDef, "  GenerateSerialization", new object[0]);
			foreach (MethodDefinition current in this.m_TypeDef.Methods)
			{
				if (current.Name == "SerializeItem")
				{
					MethodReference result = current;
					return result;
				}
			}
			MethodDefinition methodDefinition = new MethodDefinition("SerializeItem", MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig, Weaver.voidType);
			methodDefinition.Parameters.Add(new ParameterDefinition("writer", ParameterAttributes.None, Weaver.scriptDef.MainModule.Import(Weaver.NetworkWriterType)));
			methodDefinition.Parameters.Add(new ParameterDefinition("item", ParameterAttributes.None, this.m_ItemType));
			ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
			foreach (FieldDefinition current2 in this.m_ItemType.Resolve().Fields)
			{
				if (!current2.IsStatic && !current2.IsPrivate && !current2.IsSpecialName)
				{
					FieldReference fieldReference = Weaver.scriptDef.MainModule.Import(current2);
					TypeDefinition typeDefinition = fieldReference.FieldType.Resolve();
					if (typeDefinition.HasGenericParameters)
					{
						Weaver.fail = true;
						Log.Error(string.Concat(new object[]
						{
							"GenerateSerialization for ",
							this.m_TypeDef.Name,
							" [",
							typeDefinition,
							"/",
							typeDefinition.FullName,
							"]. UNet [MessageBase] member cannot have generic parameters."
						}));
						MethodReference result = null;
						return result;
					}
					if (typeDefinition.IsInterface)
					{
						Weaver.fail = true;
						Log.Error(string.Concat(new object[]
						{
							"GenerateSerialization for ",
							this.m_TypeDef.Name,
							" [",
							typeDefinition,
							"/",
							typeDefinition.FullName,
							"]. UNet [MessageBase] member cannot be an interface."
						}));
						MethodReference result = null;
						return result;
					}
					MethodReference writeFunc = Weaver.GetWriteFunc(current2.FieldType);
					if (writeFunc == null)
					{
						Weaver.fail = true;
						Log.Error(string.Concat(new object[]
						{
							"GenerateSerialization for ",
							this.m_TypeDef.Name,
							" unknown type [",
							typeDefinition,
							"/",
							typeDefinition.FullName,
							"]. UNet [MessageBase] member variables must be basic types."
						}));
						MethodReference result = null;
						return result;
					}
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_2));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldfld, fieldReference));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Call, writeFunc));
				}
			}
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			this.m_TypeDef.Methods.Add(methodDefinition);
			return methodDefinition;
		}

		private MethodReference GenerateDeserialization()
		{
			Weaver.DLog(this.m_TypeDef, "  GenerateDeserialization", new object[0]);
			foreach (MethodDefinition current in this.m_TypeDef.Methods)
			{
				if (current.Name == "DeserializeItem")
				{
					MethodReference result = current;
					return result;
				}
			}
			MethodDefinition methodDefinition = new MethodDefinition("DeserializeItem", MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig, this.m_ItemType);
			methodDefinition.Parameters.Add(new ParameterDefinition("reader", ParameterAttributes.None, Weaver.scriptDef.MainModule.Import(Weaver.NetworkReaderType)));
			ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
			iLProcessor.Body.InitLocals = true;
			iLProcessor.Body.Variables.Add(new VariableDefinition("result", this.m_ItemType));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloca, 0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Initobj, this.m_ItemType));
			foreach (FieldDefinition current2 in this.m_ItemType.Resolve().Fields)
			{
				if (!current2.IsStatic && !current2.IsPrivate && !current2.IsSpecialName)
				{
					FieldReference fieldReference = Weaver.scriptDef.MainModule.Import(current2);
					TypeDefinition typeDefinition = fieldReference.FieldType.Resolve();
					MethodReference readFunc = Weaver.GetReadFunc(current2.FieldType);
					if (readFunc == null)
					{
						Weaver.fail = true;
						Log.Error(string.Concat(new object[]
						{
							"GenerateDeserialization for ",
							this.m_TypeDef.Name,
							" unknown type [",
							typeDefinition,
							"]. UNet [SyncVar] member variables must be basic types."
						}));
						MethodReference result = null;
						return result;
					}
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloca, 0));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Call, readFunc));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Stfld, fieldReference));
				}
			}
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			this.m_TypeDef.Methods.Add(methodDefinition);
			return methodDefinition;
		}
	}
}
