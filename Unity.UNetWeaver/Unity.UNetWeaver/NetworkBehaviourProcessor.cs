using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Unity.UNetWeaver
{
	internal class NetworkBehaviourProcessor
	{
		private const int k_SyncVarLimit = 32;

		private const string k_CmdPrefix = "InvokeCmd";

		private const string k_RpcPrefix = "InvokeRpc";

		private const string k_TargetRpcPrefix = "InvokeTargetRpc";

		private List<FieldDefinition> m_SyncVars = new List<FieldDefinition>();

		private List<FieldDefinition> m_SyncLists = new List<FieldDefinition>();

		private List<FieldDefinition> m_SyncVarNetIds = new List<FieldDefinition>();

		private List<MethodDefinition> m_Cmds = new List<MethodDefinition>();

		private List<MethodDefinition> m_Rpcs = new List<MethodDefinition>();

		private List<MethodDefinition> m_TargetRpcs = new List<MethodDefinition>();

		private List<EventDefinition> m_Events = new List<EventDefinition>();

		private List<FieldDefinition> m_SyncListStaticFields = new List<FieldDefinition>();

		private List<MethodDefinition> m_CmdInvocationFuncs = new List<MethodDefinition>();

		private List<MethodDefinition> m_SyncListInvocationFuncs = new List<MethodDefinition>();

		private List<MethodDefinition> m_RpcInvocationFuncs = new List<MethodDefinition>();

		private List<MethodDefinition> m_TargetRpcInvocationFuncs = new List<MethodDefinition>();

		private List<MethodDefinition> m_EventInvocationFuncs = new List<MethodDefinition>();

		private List<MethodDefinition> m_CmdCallFuncs = new List<MethodDefinition>();

		private List<MethodDefinition> m_RpcCallFuncs = new List<MethodDefinition>();

		private List<MethodDefinition> m_TargetRpcCallFuncs = new List<MethodDefinition>();

		private int m_QosChannel;

		private TypeDefinition m_td;

		private int m_NetIdFieldCounter;

		public NetworkBehaviourProcessor(TypeDefinition td)
		{
			Weaver.DLog(td, "NetworkBehaviourProcessor", new object[0]);
			this.m_td = td;
		}

		public void Process()
		{
			if (this.m_td.HasGenericParameters)
			{
				Weaver.fail = true;
				Log.Error("NetworkBehaviour " + this.m_td.Name + " cannot have generic parameters");
				return;
			}
			Weaver.DLog(this.m_td, "Process Start", new object[0]);
			this.ProcessVersion();
			this.ProcessSyncVars();
			Weaver.ResetRecursionCount();
			this.ProcessMethods();
			this.ProcessEvents();
			if (Weaver.fail)
			{
				return;
			}
			this.GenerateNetworkSettings();
			this.GenerateConstants();
			Weaver.ResetRecursionCount();
			this.GenerateSerialization();
			if (Weaver.fail)
			{
				return;
			}
			this.GenerateDeSerialization();
			this.GeneratePreStartClient();
			Weaver.DLog(this.m_td, "Process Done", new object[0]);
		}

		private static void WriteClientActiveCheck(ILProcessor worker, string mdName, Instruction label, string errString)
		{
			worker.Append(worker.Create(OpCodes.Call, Weaver.NetworkClientGetActive));
			worker.Append(worker.Create(OpCodes.Brtrue, label));
			worker.Append(worker.Create(OpCodes.Ldstr, errString + " " + mdName + " called on server."));
			worker.Append(worker.Create(OpCodes.Call, Weaver.logErrorReference));
			worker.Append(worker.Create(OpCodes.Ret));
			worker.Append(label);
		}

		private static void WriteServerActiveCheck(ILProcessor worker, string mdName, Instruction label, string errString)
		{
			worker.Append(worker.Create(OpCodes.Call, Weaver.NetworkServerGetActive));
			worker.Append(worker.Create(OpCodes.Brtrue, label));
			worker.Append(worker.Create(OpCodes.Ldstr, errString + " " + mdName + " called on client."));
			worker.Append(worker.Create(OpCodes.Call, Weaver.logErrorReference));
			worker.Append(worker.Create(OpCodes.Ret));
			worker.Append(label);
		}

		private static void WriteSetupLocals(ILProcessor worker)
		{
			worker.Body.InitLocals = true;
			worker.Body.Variables.Add(new VariableDefinition("V_0", Weaver.scriptDef.MainModule.Import(Weaver.NetworkWriterType)));
		}

		private static void WriteCreateWriter(ILProcessor worker)
		{
			worker.Append(worker.Create(OpCodes.Newobj, Weaver.NetworkWriterCtor));
			worker.Append(worker.Create(OpCodes.Stloc_0));
			worker.Append(worker.Create(OpCodes.Ldloc_0));
		}

		private static void WriteMessageSize(ILProcessor worker)
		{
			worker.Append(worker.Create(OpCodes.Ldc_I4_0));
			worker.Append(worker.Create(OpCodes.Callvirt, Weaver.NetworkWriterWriteInt16));
		}

		private static void WriteMessageId(ILProcessor worker, int msgId)
		{
			worker.Append(worker.Create(OpCodes.Ldloc_0));
			worker.Append(worker.Create(OpCodes.Ldc_I4, msgId));
			worker.Append(worker.Create(OpCodes.Conv_U2));
			worker.Append(worker.Create(OpCodes.Callvirt, Weaver.NetworkWriterWriteInt16));
		}

		private static bool WriteArguments(ILProcessor worker, MethodDefinition md, string errString, bool skipFirst)
		{
			short num = 1;
			foreach (ParameterDefinition current in md.Parameters)
			{
				if (num == 1 && skipFirst)
				{
					num += 1;
				}
				else
				{
					MethodReference writeFunc = Weaver.GetWriteFunc(current.ParameterType);
					if (writeFunc == null)
					{
						Log.Error(string.Concat(new object[]
						{
							"WriteArguments for ",
							md.Name,
							" type ",
							current.ParameterType,
							" not supported"
						}));
						Weaver.fail = true;
						return false;
					}
					worker.Append(worker.Create(OpCodes.Ldloc_0));
					worker.Append(worker.Create(OpCodes.Ldarg, (int)num));
					worker.Append(worker.Create(OpCodes.Call, writeFunc));
					num += 1;
				}
			}
			return true;
		}

		private void ProcessVersion()
		{
			foreach (MethodDefinition current in this.m_td.Methods)
			{
				if (current.Name == "UNetVersion")
				{
					return;
				}
			}
			MethodDefinition methodDefinition = new MethodDefinition("UNetVersion", MethodAttributes.Private, Weaver.voidType);
			ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			this.m_td.Methods.Add(methodDefinition);
		}

		private void GenerateConstants()
		{
			if (this.m_Cmds.Count == 0 && this.m_Rpcs.Count == 0 && this.m_TargetRpcs.Count == 0 && this.m_Events.Count == 0 && this.m_SyncLists.Count == 0)
			{
				return;
			}
			Weaver.DLog(this.m_td, "  GenerateConstants ", new object[0]);
			MethodDefinition methodDefinition = null;
			bool flag = false;
			foreach (MethodDefinition current in this.m_td.Methods)
			{
				if (current.Name == ".cctor")
				{
					methodDefinition = current;
					flag = true;
				}
			}
			if (methodDefinition != null)
			{
				if (methodDefinition.Body.Instructions.Count != 0)
				{
					Instruction instruction = methodDefinition.Body.Instructions[methodDefinition.Body.Instructions.Count - 1];
					if (!(instruction.OpCode == OpCodes.Ret))
					{
						Log.Error("No cctor for " + this.m_td.Name);
						Weaver.fail = true;
						return;
					}
					methodDefinition.Body.Instructions.RemoveAt(methodDefinition.Body.Instructions.Count - 1);
				}
			}
			else
			{
				methodDefinition = new MethodDefinition(".cctor", MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, Weaver.voidType);
			}
			ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
			int num = 0;
			foreach (MethodDefinition current2 in this.m_Cmds)
			{
				FieldReference field = Weaver.ResolveField(this.m_td, "kCmd" + current2.Name);
				int hashCode = NetworkBehaviourProcessor.GetHashCode(this.m_td.Name + ":Cmd:" + current2.Name);
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4, hashCode));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Stsfld, field));
				this.GenerateCommandDelegate(iLProcessor, Weaver.registerCommandDelegateReference, this.m_CmdInvocationFuncs[num], field);
				num++;
			}
			int num2 = 0;
			foreach (MethodDefinition current3 in this.m_Rpcs)
			{
				FieldReference field2 = Weaver.ResolveField(this.m_td, "kRpc" + current3.Name);
				int hashCode2 = NetworkBehaviourProcessor.GetHashCode(this.m_td.Name + ":Rpc:" + current3.Name);
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4, hashCode2));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Stsfld, field2));
				this.GenerateCommandDelegate(iLProcessor, Weaver.registerRpcDelegateReference, this.m_RpcInvocationFuncs[num2], field2);
				num2++;
			}
			int num3 = 0;
			foreach (MethodDefinition current4 in this.m_TargetRpcs)
			{
				FieldReference field3 = Weaver.ResolveField(this.m_td, "kTargetRpc" + current4.Name);
				int hashCode3 = NetworkBehaviourProcessor.GetHashCode(this.m_td.Name + ":TargetRpc:" + current4.Name);
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4, hashCode3));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Stsfld, field3));
				this.GenerateCommandDelegate(iLProcessor, Weaver.registerRpcDelegateReference, this.m_TargetRpcInvocationFuncs[num3], field3);
				num3++;
			}
			int num4 = 0;
			foreach (EventDefinition current5 in this.m_Events)
			{
				FieldReference field4 = Weaver.ResolveField(this.m_td, "kEvent" + current5.Name);
				int hashCode4 = NetworkBehaviourProcessor.GetHashCode(this.m_td.Name + ":Event:" + current5.Name);
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4, hashCode4));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Stsfld, field4));
				this.GenerateCommandDelegate(iLProcessor, Weaver.registerEventDelegateReference, this.m_EventInvocationFuncs[num4], field4);
				num4++;
			}
			int num5 = 0;
			foreach (FieldDefinition current6 in this.m_SyncLists)
			{
				FieldReference field5 = Weaver.ResolveField(this.m_td, "kList" + current6.Name);
				int hashCode5 = NetworkBehaviourProcessor.GetHashCode(this.m_td.Name + ":List:" + current6.Name);
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4, hashCode5));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Stsfld, field5));
				this.GenerateCommandDelegate(iLProcessor, Weaver.registerSyncListDelegateReference, this.m_SyncListInvocationFuncs[num5], field5);
				num5++;
			}
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldstr, this.m_td.Name));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4, this.m_QosChannel));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.RegisterBehaviourReference));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			if (!flag)
			{
				this.m_td.Methods.Add(methodDefinition);
			}
			this.m_td.Attributes = (this.m_td.Attributes & ~TypeAttributes.BeforeFieldInit);
			if (this.m_SyncLists.Count == 0)
			{
				return;
			}
			MethodDefinition methodDefinition2 = null;
			bool flag2 = false;
			foreach (MethodDefinition current7 in this.m_td.Methods)
			{
				if (current7.Name == "Awake")
				{
					methodDefinition2 = current7;
					flag2 = true;
				}
			}
			if (methodDefinition2 != null)
			{
				if (methodDefinition2.Body.Instructions.Count != 0)
				{
					Instruction instruction2 = methodDefinition2.Body.Instructions[methodDefinition2.Body.Instructions.Count - 1];
					if (!(instruction2.OpCode == OpCodes.Ret))
					{
						Log.Error("No ctor for " + this.m_td.Name);
						Weaver.fail = true;
						return;
					}
					methodDefinition2.Body.Instructions.RemoveAt(methodDefinition2.Body.Instructions.Count - 1);
				}
			}
			else
			{
				methodDefinition2 = new MethodDefinition("Awake", MethodAttributes.Private, Weaver.voidType);
			}
			ILProcessor iLProcessor2 = methodDefinition2.Body.GetILProcessor();
			int num6 = 0;
			foreach (FieldDefinition current8 in this.m_SyncLists)
			{
				this.GenerateSyncListInitializer(iLProcessor2, current8, num6);
				num6++;
			}
			iLProcessor2.Append(iLProcessor2.Create(OpCodes.Ret));
			if (!flag2)
			{
				this.m_td.Methods.Add(methodDefinition2);
			}
		}

		private void GenerateCommandDelegate(ILProcessor awakeWorker, MethodReference registerMethod, MethodDefinition func, FieldReference field)
		{
			awakeWorker.Append(awakeWorker.Create(OpCodes.Ldtoken, this.m_td));
			awakeWorker.Append(awakeWorker.Create(OpCodes.Call, Weaver.getTypeFromHandleReference));
			awakeWorker.Append(awakeWorker.Create(OpCodes.Ldsfld, field));
			awakeWorker.Append(awakeWorker.Create(OpCodes.Ldnull));
			awakeWorker.Append(awakeWorker.Create(OpCodes.Ldftn, func));
			awakeWorker.Append(awakeWorker.Create(OpCodes.Newobj, Weaver.CmdDelegateConstructor));
			awakeWorker.Append(awakeWorker.Create(OpCodes.Call, registerMethod));
		}

		private void GenerateSyncListInitializer(ILProcessor awakeWorker, FieldReference fd, int index)
		{
			awakeWorker.Append(awakeWorker.Create(OpCodes.Ldarg_0));
			awakeWorker.Append(awakeWorker.Create(OpCodes.Ldfld, fd));
			awakeWorker.Append(awakeWorker.Create(OpCodes.Ldarg_0));
			awakeWorker.Append(awakeWorker.Create(OpCodes.Ldsfld, this.m_SyncListStaticFields[index]));
			GenericInstanceType genericInstanceType = (GenericInstanceType)fd.FieldType.Resolve().BaseType;
			genericInstanceType = (GenericInstanceType)Weaver.scriptDef.MainModule.Import(genericInstanceType);
			TypeReference typeReference = genericInstanceType.GenericArguments[0];
			MethodReference method = Helpers.MakeHostInstanceGeneric(Weaver.SyncListInitBehaviourReference, new TypeReference[]
			{
				typeReference
			});
			awakeWorker.Append(awakeWorker.Create(OpCodes.Callvirt, method));
			Weaver.scriptDef.MainModule.Import(method);
		}

		private void GenerateSerialization()
		{
			Weaver.DLog(this.m_td, "  GenerateSerialization", new object[0]);
			foreach (MethodDefinition current in this.m_td.Methods)
			{
				if (current.Name == "OnSerialize")
				{
					return;
				}
			}
			MethodDefinition methodDefinition = new MethodDefinition("OnSerialize", MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig, Weaver.boolType);
			methodDefinition.Parameters.Add(new ParameterDefinition("writer", ParameterAttributes.None, Weaver.scriptDef.MainModule.Import(Weaver.NetworkWriterType)));
			methodDefinition.Parameters.Add(new ParameterDefinition("forceAll", ParameterAttributes.None, Weaver.boolType));
			ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
			methodDefinition.Body.InitLocals = true;
			VariableDefinition item = new VariableDefinition(Weaver.boolType);
			methodDefinition.Body.Variables.Add(item);
			bool flag = false;
			if (this.m_td.BaseType.FullName != Weaver.NetworkBehaviourType.FullName)
			{
				MethodReference methodReference = Weaver.ResolveMethod(this.m_td.BaseType, "OnSerialize");
				if (methodReference != null)
				{
					VariableDefinition item2 = new VariableDefinition(Weaver.boolType);
					methodDefinition.Body.Variables.Add(item2);
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_2));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Call, methodReference));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_1));
					flag = true;
				}
			}
			if (this.m_SyncVars.Count == 0)
			{
				if (flag)
				{
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_1));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Or));
				}
				else
				{
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
				}
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
				this.m_td.Methods.Add(methodDefinition);
				return;
			}
			Instruction instruction = iLProcessor.Create(OpCodes.Nop);
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_2));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Brfalse, instruction));
			foreach (FieldDefinition current2 in this.m_SyncVars)
			{
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldfld, current2));
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
						"]. UNet [SyncVar] member variables must be basic types."
					}));
					return;
				}
				iLProcessor.Append(iLProcessor.Create(OpCodes.Call, writeFunc));
			}
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4_1));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			iLProcessor.Append(instruction);
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_0));
			int num = Weaver.GetSyncVarStart(this.m_td.BaseType.FullName);
			foreach (FieldDefinition current3 in this.m_SyncVars)
			{
				Instruction instruction2 = iLProcessor.Create(OpCodes.Nop);
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.NetworkBehaviourDirtyBitsReference));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4, 1 << num));
				iLProcessor.Append(iLProcessor.Create(OpCodes.And));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Brfalse, instruction2));
				NetworkBehaviourProcessor.WriteDirtyCheck(iLProcessor, true);
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldfld, current3));
				MethodReference writeFunc2 = Weaver.GetWriteFunc(current3.FieldType);
				if (writeFunc2 == null)
				{
					Log.Error(string.Concat(new object[]
					{
						"GenerateSerialization for ",
						this.m_td.Name,
						" unknown type [",
						current3.FieldType,
						"]. UNet [SyncVar] member variables must be basic types."
					}));
					Weaver.fail = true;
					return;
				}
				iLProcessor.Append(iLProcessor.Create(OpCodes.Call, writeFunc2));
				iLProcessor.Append(instruction2);
				num++;
			}
			NetworkBehaviourProcessor.WriteDirtyCheck(iLProcessor, false);
			if (Weaver.generateLogErrors)
			{
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldstr, "Injected Serialize " + this.m_td.Name));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.logErrorReference));
			}
			if (flag)
			{
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_1));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Or));
			}
			else
			{
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
			}
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			this.m_td.Methods.Add(methodDefinition);
		}

		private static void WriteDirtyCheck(ILProcessor serWorker, bool reset)
		{
			Instruction instruction = serWorker.Create(OpCodes.Nop);
			serWorker.Append(serWorker.Create(OpCodes.Ldloc_0));
			serWorker.Append(serWorker.Create(OpCodes.Brtrue, instruction));
			serWorker.Append(serWorker.Create(OpCodes.Ldarg_1));
			serWorker.Append(serWorker.Create(OpCodes.Ldarg_0));
			serWorker.Append(serWorker.Create(OpCodes.Call, Weaver.NetworkBehaviourDirtyBitsReference));
			serWorker.Append(serWorker.Create(OpCodes.Callvirt, Weaver.NetworkWriterWritePacked32));
			if (reset)
			{
				serWorker.Append(serWorker.Create(OpCodes.Ldc_I4_1));
				serWorker.Append(serWorker.Create(OpCodes.Stloc_0));
			}
			serWorker.Append(instruction);
		}

		private static int GetChannelId(FieldDefinition field)
		{
			int result = 0;
			foreach (CustomAttribute current in field.CustomAttributes)
			{
				if (current.AttributeType.FullName == Weaver.SyncVarType.FullName)
				{
					foreach (CustomAttributeNamedArgument current2 in current.Fields)
					{
						if (current2.Name == "channel")
						{
							result = (int)current2.Argument.Value;
							break;
						}
					}
				}
			}
			return result;
		}

		private bool CheckForHookFunction(FieldDefinition syncVar, out MethodDefinition foundMethod)
		{
			foundMethod = null;
			foreach (CustomAttribute current in syncVar.CustomAttributes)
			{
				if (current.AttributeType.FullName == Weaver.SyncVarType.FullName)
				{
					foreach (CustomAttributeNamedArgument current2 in current.Fields)
					{
						if (current2.Name == "hook")
						{
							string text = current2.Argument.Value as string;
							bool result;
							foreach (MethodDefinition current3 in this.m_td.Methods)
							{
								if (current3.Name == text)
								{
									if (current3.Parameters.Count != 1)
									{
										Log.Error("SyncVar Hook function " + text + " must have one argument " + this.m_td.Name);
										Weaver.fail = true;
										result = false;
										return result;
									}
									if (current3.Parameters[0].ParameterType != syncVar.FieldType)
									{
										Log.Error("SyncVar Hook function " + text + " has wrong type signature for " + this.m_td.Name);
										Weaver.fail = true;
										result = false;
										return result;
									}
									foundMethod = current3;
									result = true;
									return result;
								}
							}
							Log.Error("SyncVar Hook function " + text + " not found for " + this.m_td.Name);
							Weaver.fail = true;
							result = false;
							return result;
						}
					}
				}
			}
			return true;
		}

		private void GenerateNetworkChannelSetting(int channel)
		{
			MethodDefinition methodDefinition = new MethodDefinition("GetNetworkChannel", MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig, Weaver.int32Type);
			ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4, channel));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			this.m_td.Methods.Add(methodDefinition);
		}

		private void GenerateNetworkIntervalSetting(float interval)
		{
			MethodDefinition methodDefinition = new MethodDefinition("GetNetworkSendInterval", MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig, Weaver.singleType);
			ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_R4, interval));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			this.m_td.Methods.Add(methodDefinition);
		}

		private void GenerateNetworkSettings()
		{
			foreach (CustomAttribute current in this.m_td.CustomAttributes)
			{
				if (current.AttributeType.FullName == Weaver.NetworkSettingsType.FullName)
				{
					foreach (CustomAttributeNamedArgument current2 in current.Fields)
					{
						if (current2.Name == "channel" && (int)current2.Argument.Value != 0)
						{
							this.m_QosChannel = (int)current2.Argument.Value;
							this.GenerateNetworkChannelSetting(this.m_QosChannel);
						}
						if (current2.Name == "sendInterval" && (float)current2.Argument.Value != 0.1f)
						{
							this.GenerateNetworkIntervalSetting((float)current2.Argument.Value);
						}
					}
				}
			}
		}

		private void GeneratePreStartClient()
		{
			this.m_NetIdFieldCounter = 0;
			MethodDefinition methodDefinition = null;
			ILProcessor iLProcessor = null;
			foreach (MethodDefinition current in this.m_td.Methods)
			{
				if (current.Name == "PreStartClient")
				{
					return;
				}
			}
			foreach (FieldDefinition current2 in this.m_SyncVars)
			{
				if (current2.FieldType.FullName == Weaver.gameObjectType.FullName)
				{
					if (methodDefinition == null)
					{
						methodDefinition = new MethodDefinition("PreStartClient", MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig, Weaver.voidType);
						iLProcessor = methodDefinition.Body.GetILProcessor();
					}
					FieldDefinition field = this.m_SyncVarNetIds[this.m_NetIdFieldCounter];
					this.m_NetIdFieldCounter++;
					Instruction instruction = iLProcessor.Create(OpCodes.Nop);
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldflda, field));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.NetworkInstanceIsEmpty));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Brtrue, instruction));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldfld, field));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.FindLocalObjectReference));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Stfld, current2));
					iLProcessor.Append(instruction);
				}
			}
			if (methodDefinition != null)
			{
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
				this.m_td.Methods.Add(methodDefinition);
			}
		}

		private void GenerateDeSerialization()
		{
			Weaver.DLog(this.m_td, "  GenerateDeSerialization", new object[0]);
			this.m_NetIdFieldCounter = 0;
			foreach (MethodDefinition current in this.m_td.Methods)
			{
				if (current.Name == "OnDeserialize")
				{
					return;
				}
			}
			MethodDefinition methodDefinition = new MethodDefinition("OnDeserialize", MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig, Weaver.voidType);
			methodDefinition.Parameters.Add(new ParameterDefinition("reader", ParameterAttributes.None, Weaver.scriptDef.MainModule.Import(Weaver.NetworkReaderType)));
			methodDefinition.Parameters.Add(new ParameterDefinition("initialState", ParameterAttributes.None, Weaver.boolType));
			ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
			if (this.m_td.BaseType.FullName != Weaver.NetworkBehaviourType.FullName)
			{
				MethodReference methodReference = Weaver.ResolveMethod(this.m_td.BaseType, "OnDeserialize");
				if (methodReference != null)
				{
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_2));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Call, methodReference));
				}
			}
			if (this.m_SyncVars.Count == 0)
			{
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
				this.m_td.Methods.Add(methodDefinition);
				return;
			}
			Instruction instruction = iLProcessor.Create(OpCodes.Nop);
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_2));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Brfalse, instruction));
			foreach (FieldDefinition current2 in this.m_SyncVars)
			{
				MethodReference readByReferenceFunc = Weaver.GetReadByReferenceFunc(current2.FieldType);
				if (readByReferenceFunc != null)
				{
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldfld, current2));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Call, readByReferenceFunc));
				}
				else
				{
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
					if (current2.FieldType.FullName == Weaver.gameObjectType.FullName)
					{
						FieldDefinition field = this.m_SyncVarNetIds[this.m_NetIdFieldCounter];
						this.m_NetIdFieldCounter++;
						iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.NetworkReaderReadNetworkInstanceId));
						iLProcessor.Append(iLProcessor.Create(OpCodes.Stfld, field));
					}
					else
					{
						MethodReference readFunc = Weaver.GetReadFunc(current2.FieldType);
						if (readFunc == null)
						{
							Log.Error(string.Concat(new object[]
							{
								"GenerateDeSerialization for ",
								this.m_td.Name,
								" unknown type [",
								current2.FieldType,
								"]. UNet [SyncVar] member variables must be basic types."
							}));
							Weaver.fail = true;
							return;
						}
						iLProcessor.Append(iLProcessor.Create(OpCodes.Call, readFunc));
						iLProcessor.Append(iLProcessor.Create(OpCodes.Stfld, current2));
					}
				}
			}
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			iLProcessor.Append(instruction);
			methodDefinition.Body.InitLocals = true;
			VariableDefinition item = new VariableDefinition(Weaver.int32Type);
			methodDefinition.Body.Variables.Add(item);
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.NetworkReaderReadPacked32));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_0));
			int num = Weaver.GetSyncVarStart(this.m_td.BaseType.FullName);
			foreach (FieldDefinition current3 in this.m_SyncVars)
			{
				Instruction instruction2 = iLProcessor.Create(OpCodes.Nop);
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4, 1 << num));
				iLProcessor.Append(iLProcessor.Create(OpCodes.And));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Brfalse, instruction2));
				MethodReference readByReferenceFunc2 = Weaver.GetReadByReferenceFunc(current3.FieldType);
				if (readByReferenceFunc2 != null)
				{
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldfld, current3));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Call, readByReferenceFunc2));
				}
				else
				{
					MethodReference readFunc2 = Weaver.GetReadFunc(current3.FieldType);
					if (readFunc2 == null)
					{
						Log.Error(string.Concat(new object[]
						{
							"GenerateDeSerialization for ",
							this.m_td.Name,
							" unknown type [",
							current3.FieldType,
							"]. UNet [SyncVar] member variables must be basic types."
						}));
						Weaver.fail = true;
						return;
					}
					MethodDefinition methodDefinition2;
					if (!this.CheckForHookFunction(current3, out methodDefinition2))
					{
						return;
					}
					if (methodDefinition2 == null)
					{
						iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
						iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
						iLProcessor.Append(iLProcessor.Create(OpCodes.Call, readFunc2));
						iLProcessor.Append(iLProcessor.Create(OpCodes.Stfld, current3));
					}
					else
					{
						iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
						iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
						iLProcessor.Append(iLProcessor.Create(OpCodes.Call, readFunc2));
						iLProcessor.Append(iLProcessor.Create(OpCodes.Call, methodDefinition2));
					}
				}
				iLProcessor.Append(instruction2);
				num++;
			}
			if (Weaver.generateLogErrors)
			{
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldstr, "Injected Deserialize " + this.m_td.Name));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.logErrorReference));
			}
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			this.m_td.Methods.Add(methodDefinition);
		}

		private bool ProcessNetworkReaderParameters(MethodDefinition md, ILProcessor worker, bool skipFirst)
		{
			int num = 0;
			foreach (ParameterDefinition current in md.Parameters)
			{
				if (num++ != 0 || !skipFirst)
				{
					MethodReference readFunc = Weaver.GetReadFunc(current.ParameterType);
					if (readFunc == null)
					{
						Log.Error(string.Concat(new object[]
						{
							"ProcessNetworkReaderParameters for ",
							this.m_td.Name,
							":",
							md.Name,
							" type ",
							current.ParameterType,
							" not supported"
						}));
						Weaver.fail = true;
						return false;
					}
					worker.Append(worker.Create(OpCodes.Ldarg_1));
					worker.Append(worker.Create(OpCodes.Call, readFunc));
					if (current.ParameterType.FullName == Weaver.singleType.FullName)
					{
						worker.Append(worker.Create(OpCodes.Conv_R4));
					}
					else if (current.ParameterType.FullName == Weaver.doubleType.FullName)
					{
						worker.Append(worker.Create(OpCodes.Conv_R8));
					}
				}
			}
			return true;
		}

		private MethodDefinition ProcessCommandInvoke(MethodDefinition md)
		{
			MethodDefinition methodDefinition = new MethodDefinition("InvokeCmd" + md.Name, MethodAttributes.Family | MethodAttributes.Static | MethodAttributes.HideBySig, Weaver.voidType);
			ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
			Instruction label = iLProcessor.Create(OpCodes.Nop);
			NetworkBehaviourProcessor.WriteServerActiveCheck(iLProcessor, md.Name, label, "Command");
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Castclass, this.m_td));
			if (!this.ProcessNetworkReaderParameters(md, iLProcessor, false))
			{
				return null;
			}
			iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, md));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			NetworkBehaviourProcessor.AddInvokeParameters(methodDefinition.Parameters);
			return methodDefinition;
		}

		private static void AddInvokeParameters(ICollection<ParameterDefinition> collection)
		{
			collection.Add(new ParameterDefinition("obj", ParameterAttributes.None, Weaver.NetworkBehaviourType2));
			collection.Add(new ParameterDefinition("reader", ParameterAttributes.None, Weaver.scriptDef.MainModule.Import(Weaver.NetworkReaderType)));
		}

		private MethodDefinition ProcessCommandCall(MethodDefinition md, CustomAttribute ca)
		{
			MethodDefinition methodDefinition = new MethodDefinition("Call" + md.Name, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig, Weaver.voidType);
			foreach (ParameterDefinition current in md.Parameters)
			{
				methodDefinition.Parameters.Add(new ParameterDefinition(current.Name, ParameterAttributes.None, current.ParameterType));
			}
			ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
			Instruction label = iLProcessor.Create(OpCodes.Nop);
			NetworkBehaviourProcessor.WriteSetupLocals(iLProcessor);
			if (Weaver.generateLogErrors)
			{
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldstr, "Call Command function " + md.Name));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.logErrorReference));
			}
			NetworkBehaviourProcessor.WriteClientActiveCheck(iLProcessor, md.Name, label, "Command function");
			Instruction instruction = iLProcessor.Create(OpCodes.Nop);
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.UBehaviourIsServer));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Brfalse, instruction));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
			for (int i = 0; i < md.Parameters.Count; i++)
			{
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg, i + 1));
			}
			iLProcessor.Append(iLProcessor.Create(OpCodes.Call, md));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			iLProcessor.Append(instruction);
			NetworkBehaviourProcessor.WriteCreateWriter(iLProcessor);
			NetworkBehaviourProcessor.WriteMessageSize(iLProcessor);
			NetworkBehaviourProcessor.WriteMessageId(iLProcessor, 5);
			FieldDefinition fieldDefinition = new FieldDefinition("kCmd" + md.Name, FieldAttributes.Private | FieldAttributes.Static, Weaver.int32Type);
			this.m_td.Fields.Add(fieldDefinition);
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldsfld, fieldDefinition));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.NetworkWriterWritePacked32));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.getComponentReference));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.getUNetIdReference));
			MethodReference writeFunc = Weaver.GetWriteFunc(Weaver.NetworkInstanceIdType);
			iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, writeFunc));
			if (!NetworkBehaviourProcessor.WriteArguments(iLProcessor, md, "Command", false))
			{
				return null;
			}
			int value = 0;
			foreach (CustomAttributeNamedArgument current2 in ca.Fields)
			{
				if (current2.Name == "channel")
				{
					value = (int)current2.Argument.Value;
				}
			}
			string text = md.Name;
			int num = text.IndexOf("InvokeCmd");
			if (num > -1)
			{
				text = text.Substring("InvokeCmd".Length);
			}
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4, value));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldstr, text));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.sendCommandInternal));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			return methodDefinition;
		}

		private MethodDefinition ProcessTargetRpcInvoke(MethodDefinition md)
		{
			MethodDefinition methodDefinition = new MethodDefinition("InvokeRpc" + md.Name, MethodAttributes.Family | MethodAttributes.Static | MethodAttributes.HideBySig, Weaver.voidType);
			ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
			Instruction label = iLProcessor.Create(OpCodes.Nop);
			NetworkBehaviourProcessor.WriteClientActiveCheck(iLProcessor, md.Name, label, "TargetRPC");
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Castclass, this.m_td));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.ReadyConnectionReference));
			if (!this.ProcessNetworkReaderParameters(md, iLProcessor, true))
			{
				return null;
			}
			iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, md));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			NetworkBehaviourProcessor.AddInvokeParameters(methodDefinition.Parameters);
			return methodDefinition;
		}

		private MethodDefinition ProcessRpcInvoke(MethodDefinition md)
		{
			MethodDefinition methodDefinition = new MethodDefinition("InvokeRpc" + md.Name, MethodAttributes.Family | MethodAttributes.Static | MethodAttributes.HideBySig, Weaver.voidType);
			ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
			Instruction label = iLProcessor.Create(OpCodes.Nop);
			NetworkBehaviourProcessor.WriteClientActiveCheck(iLProcessor, md.Name, label, "RPC");
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Castclass, this.m_td));
			if (!this.ProcessNetworkReaderParameters(md, iLProcessor, false))
			{
				return null;
			}
			iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, md));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			NetworkBehaviourProcessor.AddInvokeParameters(methodDefinition.Parameters);
			return methodDefinition;
		}

		private MethodDefinition ProcessTargetRpcCall(MethodDefinition md, CustomAttribute ca)
		{
			MethodDefinition methodDefinition = new MethodDefinition("Call" + md.Name, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig, Weaver.voidType);
			foreach (ParameterDefinition current in md.Parameters)
			{
				methodDefinition.Parameters.Add(new ParameterDefinition(current.Name, ParameterAttributes.None, current.ParameterType));
			}
			ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
			Instruction label = iLProcessor.Create(OpCodes.Nop);
			NetworkBehaviourProcessor.WriteSetupLocals(iLProcessor);
			NetworkBehaviourProcessor.WriteServerActiveCheck(iLProcessor, md.Name, label, "TargetRPC Function");
			NetworkBehaviourProcessor.WriteCreateWriter(iLProcessor);
			NetworkBehaviourProcessor.WriteMessageSize(iLProcessor);
			NetworkBehaviourProcessor.WriteMessageId(iLProcessor, 2);
			FieldDefinition fieldDefinition = new FieldDefinition("kTargetRpc" + md.Name, FieldAttributes.Private | FieldAttributes.Static, Weaver.int32Type);
			this.m_td.Fields.Add(fieldDefinition);
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldsfld, fieldDefinition));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.NetworkWriterWritePacked32));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.getComponentReference));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.getUNetIdReference));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.NetworkWriterWriteNetworkInstanceId));
			if (!NetworkBehaviourProcessor.WriteArguments(iLProcessor, md, "TargetRPC", true))
			{
				return null;
			}
			int value = 0;
			foreach (CustomAttributeNamedArgument current2 in ca.Fields)
			{
				if (current2.Name == "channel")
				{
					value = (int)current2.Argument.Value;
				}
			}
			string text = md.Name;
			int num = text.IndexOf("InvokeTargetRpc");
			if (num > -1)
			{
				text = text.Substring("InvokeTargetRpc".Length);
			}
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4, value));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldstr, text));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.sendTargetRpcInternal));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			return methodDefinition;
		}

		private MethodDefinition ProcessRpcCall(MethodDefinition md, CustomAttribute ca)
		{
			MethodDefinition methodDefinition = new MethodDefinition("Call" + md.Name, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig, Weaver.voidType);
			foreach (ParameterDefinition current in md.Parameters)
			{
				methodDefinition.Parameters.Add(new ParameterDefinition(current.Name, ParameterAttributes.None, current.ParameterType));
			}
			ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
			Instruction label = iLProcessor.Create(OpCodes.Nop);
			NetworkBehaviourProcessor.WriteSetupLocals(iLProcessor);
			NetworkBehaviourProcessor.WriteServerActiveCheck(iLProcessor, md.Name, label, "RPC Function");
			NetworkBehaviourProcessor.WriteCreateWriter(iLProcessor);
			NetworkBehaviourProcessor.WriteMessageSize(iLProcessor);
			NetworkBehaviourProcessor.WriteMessageId(iLProcessor, 2);
			FieldDefinition fieldDefinition = new FieldDefinition("kRpc" + md.Name, FieldAttributes.Private | FieldAttributes.Static, Weaver.int32Type);
			this.m_td.Fields.Add(fieldDefinition);
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldsfld, fieldDefinition));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.NetworkWriterWritePacked32));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.getComponentReference));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.getUNetIdReference));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.NetworkWriterWriteNetworkInstanceId));
			if (!NetworkBehaviourProcessor.WriteArguments(iLProcessor, md, "RPC", false))
			{
				return null;
			}
			int value = 0;
			foreach (CustomAttributeNamedArgument current2 in ca.Fields)
			{
				if (current2.Name == "channel")
				{
					value = (int)current2.Argument.Value;
				}
			}
			string text = md.Name;
			int num = text.IndexOf("InvokeRpc");
			if (num > -1)
			{
				text = text.Substring("InvokeRpc".Length);
			}
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4, value));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldstr, text));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.sendRpcInternal));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			return methodDefinition;
		}

		private bool ProcessMethodsValidateFunction(MethodReference md, CustomAttribute ca, string actionType)
		{
			if (md.ReturnType.FullName == Weaver.IEnumeratorType.FullName)
			{
				Log.Error(string.Concat(new string[]
				{
					actionType,
					" function [",
					this.m_td.FullName,
					":",
					md.Name,
					"] cannot be a coroutine"
				}));
				Weaver.fail = true;
				return false;
			}
			if (md.ReturnType.FullName != Weaver.voidType.FullName)
			{
				Log.Error(string.Concat(new string[]
				{
					actionType,
					" function [",
					this.m_td.FullName,
					":",
					md.Name,
					"] must have a void return type."
				}));
				Weaver.fail = true;
				return false;
			}
			if (md.HasGenericParameters)
			{
				Log.Error(string.Concat(new string[]
				{
					actionType,
					" [",
					this.m_td.FullName,
					":",
					md.Name,
					"] cannot have generic parameters"
				}));
				Weaver.fail = true;
				return false;
			}
			return true;
		}

		private bool ProcessMethodsValidateParameters(MethodReference md, CustomAttribute ca, string actionType)
		{
			for (int i = 0; i < md.Parameters.Count; i++)
			{
				ParameterDefinition parameterDefinition = md.Parameters[i];
				if (parameterDefinition.IsOut)
				{
					Log.Error(string.Concat(new string[]
					{
						actionType,
						" function [",
						this.m_td.FullName,
						":",
						md.Name,
						"] cannot have out parameters"
					}));
					Weaver.fail = true;
					return false;
				}
				if (parameterDefinition.IsOptional)
				{
					Log.Error(string.Concat(new string[]
					{
						actionType,
						"function [",
						this.m_td.FullName,
						":",
						md.Name,
						"] cannot have optional parameters"
					}));
					Weaver.fail = true;
					return false;
				}
				if (parameterDefinition.ParameterType.Resolve().IsAbstract)
				{
					Log.Error(string.Concat(new string[]
					{
						actionType,
						" function [",
						this.m_td.FullName,
						":",
						md.Name,
						"] cannot have abstract parameters"
					}));
					Weaver.fail = true;
					return false;
				}
				if (parameterDefinition.ParameterType.IsByReference)
				{
					Log.Error(string.Concat(new string[]
					{
						actionType,
						" function [",
						this.m_td.FullName,
						":",
						md.Name,
						"] cannot have ref parameters"
					}));
					Weaver.fail = true;
					return false;
				}
				if (parameterDefinition.ParameterType.FullName == Weaver.NetworkConnectionType.FullName && (!(ca.AttributeType.FullName == Weaver.TargetRpcType.FullName) || i != 0))
				{
					Log.Error(string.Concat(new string[]
					{
						actionType,
						" [",
						this.m_td.FullName,
						":",
						md.Name,
						"] cannot use a NetworkConnection as a parameter. To access a player object's connection on the server use connectionToClient"
					}));
					Log.Error("Name: " + ca.AttributeType.FullName + " parameter: " + md.Parameters[0].ParameterType.FullName);
					Weaver.fail = true;
					return false;
				}
				if (Weaver.IsDerivedFrom(parameterDefinition.ParameterType.Resolve(), Weaver.ComponentType) && parameterDefinition.ParameterType.FullName != Weaver.NetworkIdentityType.FullName)
				{
					Log.Error(string.Concat(new string[]
					{
						actionType,
						" function [",
						this.m_td.FullName,
						":",
						md.Name,
						"] parameter [",
						parameterDefinition.Name,
						"] is of the type [",
						parameterDefinition.ParameterType.Name,
						"] which is a Component. You cannot pass a Component to a remote call. Try passing data from within the component."
					}));
					Weaver.fail = true;
					return false;
				}
			}
			return true;
		}

		private bool ProcessMethodsValidateCommand(MethodDefinition md, CustomAttribute ca)
		{
			if (md.Name.Length > 2 && md.Name.Substring(0, 3) != "Cmd")
			{
				Log.Error(string.Concat(new string[]
				{
					"Command function [",
					this.m_td.FullName,
					":",
					md.Name,
					"] doesnt have 'Cmd' prefix"
				}));
				Weaver.fail = true;
				return false;
			}
			return this.ProcessMethodsValidateFunction(md, ca, "Command") && this.ProcessMethodsValidateParameters(md, ca, "Command");
		}

		private bool ProcessMethodsValidateTargetRpc(MethodReference md, CustomAttribute ca)
		{
			int length = "Target".Length;
			if (md.Name.Length > length && md.Name.Substring(0, length) != "Target")
			{
				Log.Error(string.Concat(new string[]
				{
					"Target Rpc function [",
					this.m_td.FullName,
					":",
					md.Name,
					"] doesnt have 'Target' prefix"
				}));
				Weaver.fail = true;
				return false;
			}
			if (!this.ProcessMethodsValidateFunction(md, ca, "Target Rpc"))
			{
				return false;
			}
			if (md.Parameters.Count < 1)
			{
				Log.Error(string.Concat(new string[]
				{
					"Target Rpc function [",
					this.m_td.FullName,
					":",
					md.Name,
					"] must have a NetworkConnection as the first parameter"
				}));
				Weaver.fail = true;
				return false;
			}
			if (md.Parameters[0].ParameterType.FullName != Weaver.NetworkConnectionType.FullName)
			{
				Log.Error(string.Concat(new string[]
				{
					"Target Rpc function [",
					this.m_td.FullName,
					":",
					md.Name,
					"] first parameter must be a NetworkConnection"
				}));
				Weaver.fail = true;
				return false;
			}
			return this.ProcessMethodsValidateParameters(md, ca, "Target Rpc");
		}

		private bool ProcessMethodsValidateRpc(MethodDefinition md, CustomAttribute ca)
		{
			if (md.Name.Length > 2 && md.Name.Substring(0, 3) != "Rpc")
			{
				Log.Error(string.Concat(new string[]
				{
					"Rpc function [",
					this.m_td.FullName,
					":",
					md.Name,
					"] doesnt have 'Rpc' prefix"
				}));
				Weaver.fail = true;
				return false;
			}
			return this.ProcessMethodsValidateFunction(md, ca, "Rpc") && this.ProcessMethodsValidateParameters(md, ca, "Rpc");
		}

		private void ProcessMethods()
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (MethodDefinition current in this.m_td.Methods)
			{
				Weaver.ResetRecursionCount();
				foreach (CustomAttribute current2 in current.CustomAttributes)
				{
					if (current2.AttributeType.FullName == Weaver.CommandType.FullName)
					{
						if (!this.ProcessMethodsValidateCommand(current, current2))
						{
							return;
						}
						if (hashSet.Contains(current.Name))
						{
							Log.Error(string.Concat(new string[]
							{
								"Duplicate Command name [",
								this.m_td.FullName,
								":",
								current.Name,
								"]"
							}));
							Weaver.fail = true;
							return;
						}
						hashSet.Add(current.Name);
						this.m_Cmds.Add(current);
						MethodDefinition methodDefinition = this.ProcessCommandInvoke(current);
						if (methodDefinition != null)
						{
							this.m_CmdInvocationFuncs.Add(methodDefinition);
						}
						MethodDefinition methodDefinition2 = this.ProcessCommandCall(current, current2);
						if (methodDefinition2 != null)
						{
							this.m_CmdCallFuncs.Add(methodDefinition2);
							Weaver.lists.replacedMethods.Add(current);
							Weaver.lists.replacementMethods.Add(methodDefinition2);
						}
						break;
					}
					else if (current2.AttributeType.FullName == Weaver.TargetRpcType.FullName)
					{
						if (!this.ProcessMethodsValidateTargetRpc(current, current2))
						{
							return;
						}
						if (hashSet.Contains(current.Name))
						{
							Log.Error(string.Concat(new string[]
							{
								"Duplicate Target Rpc name [",
								this.m_td.FullName,
								":",
								current.Name,
								"]"
							}));
							Weaver.fail = true;
							return;
						}
						hashSet.Add(current.Name);
						this.m_TargetRpcs.Add(current);
						MethodDefinition methodDefinition3 = this.ProcessTargetRpcInvoke(current);
						if (methodDefinition3 != null)
						{
							this.m_TargetRpcInvocationFuncs.Add(methodDefinition3);
						}
						MethodDefinition methodDefinition4 = this.ProcessTargetRpcCall(current, current2);
						if (methodDefinition4 != null)
						{
							this.m_TargetRpcCallFuncs.Add(methodDefinition4);
							Weaver.lists.replacedMethods.Add(current);
							Weaver.lists.replacementMethods.Add(methodDefinition4);
						}
						break;
					}
					else if (current2.AttributeType.FullName == Weaver.ClientRpcType.FullName)
					{
						if (!this.ProcessMethodsValidateRpc(current, current2))
						{
							return;
						}
						if (hashSet.Contains(current.Name))
						{
							Log.Error(string.Concat(new string[]
							{
								"Duplicate ClientRpc name [",
								this.m_td.FullName,
								":",
								current.Name,
								"]"
							}));
							Weaver.fail = true;
							return;
						}
						hashSet.Add(current.Name);
						this.m_Rpcs.Add(current);
						MethodDefinition methodDefinition5 = this.ProcessRpcInvoke(current);
						if (methodDefinition5 != null)
						{
							this.m_RpcInvocationFuncs.Add(methodDefinition5);
						}
						MethodDefinition methodDefinition6 = this.ProcessRpcCall(current, current2);
						if (methodDefinition6 != null)
						{
							this.m_RpcCallFuncs.Add(methodDefinition6);
							Weaver.lists.replacedMethods.Add(current);
							Weaver.lists.replacementMethods.Add(methodDefinition6);
						}
						break;
					}
				}
			}
			foreach (MethodDefinition current3 in this.m_CmdInvocationFuncs)
			{
				this.m_td.Methods.Add(current3);
			}
			foreach (MethodDefinition current4 in this.m_CmdCallFuncs)
			{
				this.m_td.Methods.Add(current4);
			}
			foreach (MethodDefinition current5 in this.m_RpcInvocationFuncs)
			{
				this.m_td.Methods.Add(current5);
			}
			foreach (MethodDefinition current6 in this.m_TargetRpcInvocationFuncs)
			{
				this.m_td.Methods.Add(current6);
			}
			foreach (MethodDefinition current7 in this.m_RpcCallFuncs)
			{
				this.m_td.Methods.Add(current7);
			}
			foreach (MethodDefinition current8 in this.m_TargetRpcCallFuncs)
			{
				this.m_td.Methods.Add(current8);
			}
		}

		private MethodDefinition ProcessEventInvoke(EventDefinition ed)
		{
			FieldDefinition fieldDefinition = null;
			foreach (FieldDefinition current in this.m_td.Fields)
			{
				if (current.FullName == ed.FullName)
				{
					fieldDefinition = current;
					break;
				}
			}
			if (fieldDefinition == null)
			{
				Weaver.DLog(this.m_td, "ERROR: no event field?!", new object[0]);
				Weaver.fail = true;
				return null;
			}
			MethodDefinition methodDefinition = new MethodDefinition("InvokeSyncEvent" + ed.Name, MethodAttributes.Family | MethodAttributes.Static | MethodAttributes.HideBySig, Weaver.voidType);
			ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
			Instruction label = iLProcessor.Create(OpCodes.Nop);
			Instruction instruction = iLProcessor.Create(OpCodes.Nop);
			NetworkBehaviourProcessor.WriteClientActiveCheck(iLProcessor, ed.Name, label, "Event");
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Castclass, this.m_td));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldfld, fieldDefinition));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Brtrue, instruction));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			iLProcessor.Append(instruction);
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Castclass, this.m_td));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldfld, fieldDefinition));
			MethodReference methodReference = Weaver.ResolveMethod(fieldDefinition.FieldType, "Invoke");
			if (!this.ProcessNetworkReaderParameters(methodReference.Resolve(), iLProcessor, false))
			{
				return null;
			}
			iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, methodReference));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			NetworkBehaviourProcessor.AddInvokeParameters(methodDefinition.Parameters);
			return methodDefinition;
		}

		private MethodDefinition ProcessEventCall(EventDefinition ed, CustomAttribute ca)
		{
			MethodReference methodReference = Weaver.ResolveMethod(ed.EventType, "Invoke");
			MethodDefinition methodDefinition = new MethodDefinition("Call" + ed.Name, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig, Weaver.voidType);
			foreach (ParameterDefinition current in methodReference.Parameters)
			{
				methodDefinition.Parameters.Add(new ParameterDefinition(current.Name, ParameterAttributes.None, current.ParameterType));
			}
			ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
			Instruction label = iLProcessor.Create(OpCodes.Nop);
			NetworkBehaviourProcessor.WriteSetupLocals(iLProcessor);
			NetworkBehaviourProcessor.WriteServerActiveCheck(iLProcessor, ed.Name, label, "Event");
			NetworkBehaviourProcessor.WriteCreateWriter(iLProcessor);
			NetworkBehaviourProcessor.WriteMessageSize(iLProcessor);
			NetworkBehaviourProcessor.WriteMessageId(iLProcessor, 7);
			FieldDefinition fieldDefinition = new FieldDefinition("kEvent" + ed.Name, FieldAttributes.Private | FieldAttributes.Static, Weaver.int32Type);
			this.m_td.Fields.Add(fieldDefinition);
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldsfld, fieldDefinition));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.NetworkWriterWritePacked32));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.getComponentReference));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.getUNetIdReference));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.NetworkWriterWriteNetworkInstanceId));
			if (!NetworkBehaviourProcessor.WriteArguments(iLProcessor, methodReference.Resolve(), "SyncEvent", false))
			{
				return null;
			}
			int value = 0;
			foreach (CustomAttributeNamedArgument current2 in ca.Fields)
			{
				if (current2.Name == "channel")
				{
					value = (int)current2.Argument.Value;
				}
			}
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4, value));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldstr, ed.Name));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.sendEventInternal));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			return methodDefinition;
		}

		private void ProcessEvents()
		{
			foreach (EventDefinition current in this.m_td.Events)
			{
				foreach (CustomAttribute current2 in current.CustomAttributes)
				{
					if (current2.AttributeType.FullName == Weaver.SyncEventType.FullName)
					{
						if (current.Name.Length > 4 && current.Name.Substring(0, 5) != "Event")
						{
							Log.Error(string.Concat(new string[]
							{
								"Event  [",
								this.m_td.FullName,
								":",
								current.FullName,
								"] doesnt have 'Event' prefix"
							}));
							Weaver.fail = true;
							return;
						}
						if (current.EventType.Resolve().HasGenericParameters)
						{
							Log.Error(string.Concat(new string[]
							{
								"Event  [",
								this.m_td.FullName,
								":",
								current.FullName,
								"] cannot have generic parameters"
							}));
							Weaver.fail = true;
							return;
						}
						this.m_Events.Add(current);
						MethodDefinition methodDefinition = this.ProcessEventInvoke(current);
						if (methodDefinition == null)
						{
							return;
						}
						this.m_td.Methods.Add(methodDefinition);
						this.m_EventInvocationFuncs.Add(methodDefinition);
						Weaver.DLog(this.m_td, "ProcessEvent " + current, new object[0]);
						MethodDefinition item = this.ProcessEventCall(current, current2);
						this.m_td.Methods.Add(item);
						Weaver.lists.replacedEvents.Add(current);
						Weaver.lists.replacementEvents.Add(item);
						Weaver.DLog(this.m_td, "  Event: " + current.Name, new object[0]);
						break;
					}
				}
			}
		}

		private static MethodDefinition ProcessSyncVarGet(FieldDefinition fd, string originalName)
		{
			MethodDefinition methodDefinition = new MethodDefinition("get_Network" + originalName, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.SpecialName, fd.FieldType);
			ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldfld, fd));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			methodDefinition.Body.Variables.Add(new VariableDefinition("V_0", fd.FieldType));
			methodDefinition.Body.InitLocals = true;
			methodDefinition.SemanticsAttributes = MethodSemanticsAttributes.Getter;
			return methodDefinition;
		}

		private MethodDefinition ProcessSyncVarSet(FieldDefinition fd, string originalName, int dirtyBit, FieldDefinition netFieldId)
		{
			MethodDefinition methodDefinition = new MethodDefinition("set_Network" + originalName, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.SpecialName, Weaver.voidType);
			ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldflda, fd));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4, dirtyBit));
			MethodDefinition methodDefinition2;
			this.CheckForHookFunction(fd, out methodDefinition2);
			if (methodDefinition2 != null)
			{
				Instruction instruction = iLProcessor.Create(OpCodes.Nop);
				iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.NetworkServerGetLocalClientActive));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Brfalse, instruction));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.getSyncVarHookGuard));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Brtrue, instruction));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4_1));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.setSyncVarHookGuard));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Call, methodDefinition2));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4_0));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.setSyncVarHookGuard));
				iLProcessor.Append(instruction);
			}
			if (fd.FieldType.FullName == Weaver.gameObjectType.FullName)
			{
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldflda, netFieldId));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.setSyncVarGameObjectReference));
			}
			else
			{
				GenericInstanceMethod genericInstanceMethod = new GenericInstanceMethod(Weaver.setSyncVarReference);
				genericInstanceMethod.GenericArguments.Add(fd.FieldType);
				iLProcessor.Append(iLProcessor.Create(OpCodes.Call, genericInstanceMethod));
			}
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			methodDefinition.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.In, fd.FieldType));
			methodDefinition.SemanticsAttributes = MethodSemanticsAttributes.Setter;
			return methodDefinition;
		}

		private void ProcessSyncVar(FieldDefinition fd, int dirtyBit)
		{
			string name = fd.Name;
			Weaver.lists.replacedFields.Add(fd);
			Weaver.DLog(this.m_td, string.Concat(new object[]
			{
				"Sync Var ",
				fd.Name,
				" ",
				fd.FieldType,
				" ",
				Weaver.gameObjectType
			}), new object[0]);
			FieldDefinition fieldDefinition = null;
			if (fd.FieldType.FullName == Weaver.gameObjectType.FullName)
			{
				fieldDefinition = new FieldDefinition("___" + fd.Name + "NetId", FieldAttributes.Private, Weaver.NetworkInstanceIdType);
				this.m_SyncVarNetIds.Add(fieldDefinition);
				Weaver.lists.netIdFields.Add(fieldDefinition);
			}
			MethodDefinition methodDefinition = NetworkBehaviourProcessor.ProcessSyncVarGet(fd, name);
			MethodDefinition methodDefinition2 = this.ProcessSyncVarSet(fd, name, dirtyBit, fieldDefinition);
			PropertyDefinition item = new PropertyDefinition("Network" + name, PropertyAttributes.None, fd.FieldType)
			{
				GetMethod = methodDefinition,
				SetMethod = methodDefinition2
			};
			this.m_td.Methods.Add(methodDefinition);
			this.m_td.Methods.Add(methodDefinition2);
			this.m_td.Properties.Add(item);
			Weaver.lists.replacementProperties.Add(methodDefinition2);
		}

		private static MethodDefinition ProcessSyncListInvoke(FieldDefinition fd)
		{
			MethodDefinition methodDefinition = new MethodDefinition("InvokeSyncList" + fd.Name, MethodAttributes.Family | MethodAttributes.Static | MethodAttributes.HideBySig, Weaver.voidType);
			ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
			Instruction label = iLProcessor.Create(OpCodes.Nop);
			NetworkBehaviourProcessor.WriteClientActiveCheck(iLProcessor, fd.Name, label, "SyncList");
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Castclass, fd.DeclaringType));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldfld, fd));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
			GenericInstanceType genericInstanceType = (GenericInstanceType)fd.FieldType.Resolve().BaseType;
			genericInstanceType = (GenericInstanceType)Weaver.scriptDef.MainModule.Import(genericInstanceType);
			TypeReference typeReference = genericInstanceType.GenericArguments[0];
			MethodReference method = Helpers.MakeHostInstanceGeneric(Weaver.SyncListInitHandleMsg, new TypeReference[]
			{
				typeReference
			});
			iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, method));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			NetworkBehaviourProcessor.AddInvokeParameters(methodDefinition.Parameters);
			return methodDefinition;
		}

		private FieldDefinition ProcessSyncList(FieldDefinition fd, int dirtyBit)
		{
			MethodDefinition item = NetworkBehaviourProcessor.ProcessSyncListInvoke(fd);
			this.m_SyncListInvocationFuncs.Add(item);
			return new FieldDefinition("kList" + fd.Name, FieldAttributes.Private | FieldAttributes.Static, Weaver.int32Type);
		}

		private void ProcessSyncVars()
		{
			int num = 0;
			int num2 = Weaver.GetSyncVarStart(this.m_td.BaseType.FullName);
			this.m_SyncVarNetIds.Clear();
			List<FieldDefinition> list = new List<FieldDefinition>();
			foreach (FieldDefinition current in this.m_td.Fields)
			{
				foreach (CustomAttribute current2 in current.CustomAttributes)
				{
					if (current2.AttributeType.FullName == Weaver.SyncVarType.FullName)
					{
						TypeDefinition typeDefinition = current.FieldType.Resolve();
						if (Weaver.IsDerivedFrom(typeDefinition, Weaver.NetworkBehaviourType))
						{
							Log.Error("SyncVar [" + current.FullName + "] cannot be derived from NetworkBehaviour. Use a GameObject or NetworkInstanceId.");
							Weaver.fail = true;
							return;
						}
						if ((ushort)(current.Attributes & FieldAttributes.Static) != 0)
						{
							Log.Error("SyncVar [" + current.FullName + "] cannot be static.");
							Weaver.fail = true;
							return;
						}
						if (typeDefinition.HasGenericParameters)
						{
							Log.Error("SyncVar [" + current.FullName + "] cannot have generic parameters.");
							Weaver.fail = true;
							return;
						}
						if (typeDefinition.IsInterface)
						{
							Log.Error("SyncVar [" + current.FullName + "] cannot be an interface.");
							Weaver.fail = true;
							return;
						}
						string name = typeDefinition.Module.Name;
						if (name != Weaver.scriptDef.MainModule.Name && name != Weaver.m_UnityAssemblyDefinition.MainModule.Name && name != Weaver.m_UNetAssemblyDefinition.MainModule.Name && name != Weaver.corLib.Name && name != "System.Runtime.dll")
						{
							Log.Error(string.Concat(new string[]
							{
								"SyncVar [",
								current.FullName,
								"] from ",
								typeDefinition.Module.ToString(),
								" cannot be a different module."
							}));
							Weaver.fail = true;
							return;
						}
						if (current.FieldType.IsArray)
						{
							Log.Error("SyncVar [" + current.FullName + "] cannot be an array. Use a SyncList instead.");
							Weaver.fail = true;
							return;
						}
						this.m_SyncVars.Add(current);
						this.ProcessSyncVar(current, 1 << num2);
						num2++;
						num++;
						if (num2 == 32)
						{
							Log.Error(string.Concat(new object[]
							{
								"Script class [",
								this.m_td.FullName,
								"] has too many SyncVars (",
								32,
								"). (This could include base classes)"
							}));
							Weaver.fail = true;
							return;
						}
						break;
					}
				}
				if (current.FieldType.FullName.Contains("UnityEngine.Networking.SyncListStruct"))
				{
					Log.Error("SyncListStruct member variable [" + current.FullName + "] must use a dervied class, like \"class MySyncList : SyncListStruct<MyStruct> {}\".");
					Weaver.fail = true;
					return;
				}
				if (Weaver.IsDerivedFrom(current.FieldType.Resolve(), Weaver.SyncListType))
				{
					this.m_SyncVars.Add(current);
					this.m_SyncLists.Add(current);
					list.Add(this.ProcessSyncList(current, 1 << num2));
					num2++;
					num++;
					if (num2 == 32)
					{
						Log.Error(string.Concat(new object[]
						{
							"Script class [",
							this.m_td.FullName,
							"] has too many SyncVars (",
							32,
							"). (This could include base classes)"
						}));
						Weaver.fail = true;
						return;
					}
				}
			}
			foreach (FieldDefinition current3 in list)
			{
				this.m_td.Fields.Add(current3);
				this.m_SyncListStaticFields.Add(current3);
			}
			foreach (FieldDefinition current4 in this.m_SyncVarNetIds)
			{
				this.m_td.Fields.Add(current4);
			}
			foreach (MethodDefinition current5 in this.m_SyncListInvocationFuncs)
			{
				this.m_td.Methods.Add(current5);
			}
			Weaver.SetNumSyncVars(this.m_td.FullName, num);
		}

		private unsafe static int GetHashCode(string s)
		{
			int length = s.Length;
			fixed (char* ptr = s + RuntimeHelpers.OffsetToStringData / 2)
			{
				char* ptr2 = ptr;
				char* ptr3 = ptr2 + length - 1;
				int num = 0;
				while (ptr2 < ptr3)
				{
					num = (num << 5) - num + (int)(*ptr2);
					num = (num << 5) - num + (int)ptr2[1];
					ptr2 += 2;
				}
				ptr3++;
				if (ptr2 < ptr3)
				{
					num = (num << 5) - num + (int)(*ptr2);
				}
				return num;
			}
		}
	}
}
