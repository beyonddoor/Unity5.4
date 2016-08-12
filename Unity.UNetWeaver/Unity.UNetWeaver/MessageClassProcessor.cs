using Mono.Cecil;
using Mono.Cecil.Cil;
using System;

namespace Unity.UNetWeaver
{
	internal class MessageClassProcessor
	{
		private TypeDefinition m_td;

		public MessageClassProcessor(TypeDefinition td)
		{
			Weaver.DLog(td, "MessageClassProcessor for " + td.Name, new object[0]);
			this.m_td = td;
		}

		public void Process()
		{
			Weaver.DLog(this.m_td, "MessageClassProcessor Start", new object[0]);
			Weaver.ResetRecursionCount();
			this.GenerateSerialization();
			if (Weaver.fail)
			{
				return;
			}
			this.GenerateDeSerialization();
			Weaver.DLog(this.m_td, "MessageClassProcessor Done", new object[0]);
		}

		private void GenerateSerialization()
		{
			Weaver.DLog(this.m_td, "  GenerateSerialization", new object[0]);
			foreach (MethodDefinition current in this.m_td.Methods)
			{
				if (current.Name == "Serialize")
				{
					return;
				}
			}
			if (this.m_td.Fields.Count == 0)
			{
				return;
			}
			MethodDefinition methodDefinition = new MethodDefinition("Serialize", MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig, Weaver.voidType);
			methodDefinition.Parameters.Add(new ParameterDefinition("writer", ParameterAttributes.None, Weaver.scriptDef.MainModule.Import(Weaver.NetworkWriterType)));
			ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
			foreach (FieldDefinition current2 in this.m_td.Fields)
			{
				if (!current2.IsStatic && !current2.IsPrivate && !current2.IsSpecialName)
				{
					if (current2.FieldType.Resolve().HasGenericParameters)
					{
						Weaver.fail = true;
						Log.Error(string.Concat(new object[]
						{
							"GenerateSerialization for ",
							this.m_td.Name,
							" [",
							current2.FieldType,
							"/",
							current2.FieldType.FullName,
							"]. UNet [MessageBase] member cannot have generic parameters."
						}));
						return;
					}
					if (current2.FieldType.Resolve().IsInterface)
					{
						Weaver.fail = true;
						Log.Error(string.Concat(new object[]
						{
							"GenerateSerialization for ",
							this.m_td.Name,
							" [",
							current2.FieldType,
							"/",
							current2.FieldType.FullName,
							"]. UNet [MessageBase] member cannot be an interface."
						}));
						return;
					}
					MethodReference writeFunc = Weaver.GetWriteFunc(current2.FieldType);
					if (writeFunc == null)
					{
						Weaver.fail = true;
						Log.Error(string.Concat(new object[]
						{
							"GenerateSerialization for ",
							this.m_td.Name,
							" unknown type [",
							current2.FieldType,
							"/",
							current2.FieldType.FullName,
							"]. UNet [MessageBase] member variables must be basic types."
						}));
						return;
					}
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldfld, current2));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Call, writeFunc));
				}
			}
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			this.m_td.Methods.Add(methodDefinition);
		}

		private void GenerateDeSerialization()
		{
			Weaver.DLog(this.m_td, "  GenerateDeserialization", new object[0]);
			foreach (MethodDefinition current in this.m_td.Methods)
			{
				if (current.Name == "Deserialize")
				{
					return;
				}
			}
			if (this.m_td.Fields.Count == 0)
			{
				return;
			}
			MethodDefinition methodDefinition = new MethodDefinition("Deserialize", MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig, Weaver.voidType);
			methodDefinition.Parameters.Add(new ParameterDefinition("reader", ParameterAttributes.None, Weaver.scriptDef.MainModule.Import(Weaver.NetworkReaderType)));
			ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
			foreach (FieldDefinition current2 in this.m_td.Fields)
			{
				if (!current2.IsStatic && !current2.IsPrivate && !current2.IsSpecialName)
				{
					MethodReference readFunc = Weaver.GetReadFunc(current2.FieldType);
					if (readFunc == null)
					{
						Weaver.fail = true;
						Log.Error(string.Concat(new object[]
						{
							"GenerateDeSerialization for ",
							this.m_td.Name,
							" unknown type [",
							current2.FieldType,
							"]. UNet [SyncVar] member variables must be basic types."
						}));
						return;
					}
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Call, readFunc));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Stfld, current2));
				}
			}
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			this.m_td.Methods.Add(methodDefinition);
		}
	}
}
