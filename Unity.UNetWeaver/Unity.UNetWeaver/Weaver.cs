using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Mdb;
using Mono.Cecil.Pdb;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Unity.UNetWeaver
{
	internal class Weaver
	{
		private const int MaxRecursionCount = 128;

		public static TypeReference NetworkBehaviourType;

		public static TypeReference NetworkBehaviourType2;

		public static TypeReference MonoBehaviourType;

		public static TypeReference NetworkConnectionType;

		public static TypeReference MessageBaseType;

		public static TypeReference SyncListStructType;

		public static MethodReference NetworkBehaviourDirtyBitsReference;

		public static TypeReference NetworkClientType;

		public static TypeReference NetworkServerType;

		public static TypeReference NetworkCRCType;

		public static TypeReference NetworkReaderType;

		public static TypeDefinition NetworkReaderDef;

		public static TypeReference NetworkWriterType;

		public static TypeDefinition NetworkWriterDef;

		public static MethodReference NetworkWriterCtor;

		public static MethodReference NetworkReaderCtor;

		public static TypeReference MemoryStreamType;

		public static MethodReference MemoryStreamCtor;

		public static MethodReference getComponentReference;

		public static MethodReference getUNetIdReference;

		public static MethodReference getPlayerIdReference;

		public static TypeReference NetworkIdentityType;

		public static TypeReference NetworkInstanceIdType;

		public static TypeReference NetworkSceneIdType;

		public static TypeReference IEnumeratorType;

		public static TypeReference ClientSceneType;

		public static MethodReference FindLocalObjectReference;

		public static MethodReference RegisterBehaviourReference;

		public static MethodReference ReadyConnectionReference;

		public static TypeReference ComponentType;

		public static TypeReference CmdDelegateReference;

		public static MethodReference CmdDelegateConstructor;

		public static MethodReference NetworkReaderReadInt32;

		public static MethodReference NetworkWriterWriteInt32;

		public static MethodReference NetworkWriterWriteInt16;

		public static MethodReference NetworkServerGetActive;

		public static MethodReference NetworkServerGetLocalClientActive;

		public static MethodReference NetworkClientGetActive;

		public static MethodReference UBehaviourIsServer;

		public static MethodReference NetworkReaderReadPacked32;

		public static MethodReference NetworkReaderReadPacked64;

		public static MethodReference NetworkReaderReadByte;

		public static MethodReference NetworkWriterWritePacked32;

		public static MethodReference NetworkWriterWritePacked64;

		public static MethodReference NetworkWriterWriteNetworkInstanceId;

		public static MethodReference NetworkWriterWriteNetworkSceneId;

		public static MethodReference NetworkReaderReadNetworkInstanceId;

		public static MethodReference NetworkReaderReadNetworkSceneId;

		public static MethodReference NetworkInstanceIsEmpty;

		public static MethodReference NetworkReadUInt16;

		public static MethodReference NetworkWriteUInt16;

		public static TypeReference SyncVarType;

		public static TypeReference CommandType;

		public static TypeReference ClientRpcType;

		public static TypeReference TargetRpcType;

		public static TypeReference SyncEventType;

		public static TypeReference SyncListType;

		public static MethodReference SyncListInitBehaviourReference;

		public static MethodReference SyncListInitHandleMsg;

		public static MethodReference SyncListClear;

		public static TypeReference NetworkSettingsType;

		public static TypeReference SyncListFloatType;

		public static TypeReference SyncListIntType;

		public static TypeReference SyncListUIntType;

		public static TypeReference SyncListBoolType;

		public static TypeReference SyncListStringType;

		public static MethodReference SyncListFloatReadType;

		public static MethodReference SyncListIntReadType;

		public static MethodReference SyncListUIntReadType;

		public static MethodReference SyncListStringReadType;

		public static MethodReference SyncListBoolReadType;

		public static MethodReference SyncListFloatWriteType;

		public static MethodReference SyncListIntWriteType;

		public static MethodReference SyncListUIntWriteType;

		public static MethodReference SyncListBoolWriteType;

		public static MethodReference SyncListStringWriteType;

		public static TypeReference voidType;

		public static TypeReference singleType;

		public static TypeReference doubleType;

		public static TypeReference decimalType;

		public static TypeReference boolType;

		public static TypeReference stringType;

		public static TypeReference int64Type;

		public static TypeReference uint64Type;

		public static TypeReference int32Type;

		public static TypeReference uint32Type;

		public static TypeReference int16Type;

		public static TypeReference uint16Type;

		public static TypeReference byteType;

		public static TypeReference sbyteType;

		public static TypeReference charType;

		public static TypeReference objectType;

		public static TypeReference vector2Type;

		public static TypeReference vector3Type;

		public static TypeReference vector4Type;

		public static TypeReference colorType;

		public static TypeReference color32Type;

		public static TypeReference quaternionType;

		public static TypeReference rectType;

		public static TypeReference rayType;

		public static TypeReference planeType;

		public static TypeReference matrixType;

		public static TypeReference hashType;

		public static TypeReference typeType;

		public static TypeReference gameObjectType;

		public static TypeReference transformType;

		public static TypeReference unityObjectType;

		public static MethodReference gameObjectInequality;

		public static MethodReference setSyncVarReference;

		public static MethodReference setSyncVarHookGuard;

		public static MethodReference getSyncVarHookGuard;

		public static MethodReference setSyncVarGameObjectReference;

		public static MethodReference registerCommandDelegateReference;

		public static MethodReference registerRpcDelegateReference;

		public static MethodReference registerEventDelegateReference;

		public static MethodReference registerSyncListDelegateReference;

		public static MethodReference getTypeReference;

		public static MethodReference getTypeFromHandleReference;

		public static MethodReference logErrorReference;

		public static MethodReference logWarningReference;

		public static MethodReference sendCommandInternal;

		public static MethodReference sendRpcInternal;

		public static MethodReference sendTargetRpcInternal;

		public static MethodReference sendEventInternal;

		public static WeaverLists lists;

		public static AssemblyDefinition scriptDef;

		public static ModuleDefinition corLib;

		public static AssemblyDefinition m_UnityAssemblyDefinition;

		public static AssemblyDefinition m_UNetAssemblyDefinition;

		private static bool m_DebugFlag = true;

		public static bool fail;

		public static bool generateLogErrors;

		private static int s_RecursionCount;

		public static void ResetRecursionCount()
		{
			Weaver.s_RecursionCount = 0;
		}

		public static void DLog(TypeDefinition td, string fmt, params object[] args)
		{
			if (!Weaver.m_DebugFlag)
			{
				return;
			}
			Console.WriteLine("[" + td.Name + "] " + string.Format(fmt, args));
		}

		public static int GetSyncVarStart(string className)
		{
			if (Weaver.lists.numSyncVars.ContainsKey(className))
			{
				return Weaver.lists.numSyncVars[className];
			}
			return 0;
		}

		public static void SetNumSyncVars(string className, int num)
		{
			Weaver.lists.numSyncVars[className] = num;
		}

		public static MethodReference GetWriteFunc(TypeReference variable)
		{
			if (Weaver.s_RecursionCount++ > 128)
			{
				Log.Error("GetWriteFunc recursion depth exceeded for " + variable.Name + ". Check for self-referencing member variables.");
				Weaver.fail = true;
				return null;
			}
			if (Weaver.lists.writeFuncs.ContainsKey(variable.FullName))
			{
				MethodReference methodReference = Weaver.lists.writeFuncs[variable.FullName];
				if (methodReference.Parameters[0].ParameterType.IsArray == variable.IsArray)
				{
					return methodReference;
				}
			}
			if (variable.Resolve().IsEnum)
			{
				return Weaver.NetworkWriterWriteInt32;
			}
			if (variable.IsByReference)
			{
				Log.Error("GetWriteFunc variable.IsByReference error.");
				return null;
			}
			MethodDefinition methodDefinition;
			if (variable.IsArray)
			{
				TypeReference elementType = variable.GetElementType();
				MethodReference writeFunc = Weaver.GetWriteFunc(elementType);
				if (writeFunc == null)
				{
					return null;
				}
				methodDefinition = Weaver.GenerateArrayWriteFunc(variable, writeFunc);
			}
			else
			{
				methodDefinition = Weaver.GenerateWriterFunction(variable);
			}
			if (methodDefinition == null)
			{
				return null;
			}
			Weaver.RegisterWriteFunc(variable.FullName, methodDefinition);
			return methodDefinition;
		}

		public static void RegisterWriteFunc(string name, MethodDefinition newWriterFunc)
		{
			Weaver.lists.writeFuncs[name] = newWriterFunc;
			Weaver.lists.generatedWriteFunctions.Add(newWriterFunc);
			Weaver.ConfirmGeneratedCodeClass(Weaver.scriptDef.MainModule);
			Weaver.lists.generateContainerClass.Methods.Add(newWriterFunc);
		}

		public static MethodReference GetReadByReferenceFunc(TypeReference variable)
		{
			if (Weaver.lists.readByReferenceFuncs.ContainsKey(variable.FullName))
			{
				return Weaver.lists.readByReferenceFuncs[variable.FullName];
			}
			return null;
		}

		public static MethodReference GetReadFunc(TypeReference variable)
		{
			if (Weaver.lists.readFuncs.ContainsKey(variable.FullName))
			{
				MethodReference methodReference = Weaver.lists.readFuncs[variable.FullName];
				if (methodReference.ReturnType.IsArray == variable.IsArray)
				{
					return methodReference;
				}
			}
			TypeDefinition typeDefinition = variable.Resolve();
			if (typeDefinition == null)
			{
				Log.Error("GetReadFunc unsupported type " + variable.FullName);
				return null;
			}
			if (typeDefinition.IsEnum)
			{
				return Weaver.NetworkReaderReadInt32;
			}
			if (variable.IsByReference)
			{
				Log.Error("GetReadFunc variable.IsByReference error.");
				return null;
			}
			MethodDefinition methodDefinition;
			if (variable.IsArray)
			{
				TypeReference elementType = variable.GetElementType();
				MethodReference readFunc = Weaver.GetReadFunc(elementType);
				if (readFunc == null)
				{
					return null;
				}
				methodDefinition = Weaver.GenerateArrayReadFunc(variable, readFunc);
			}
			else
			{
				methodDefinition = Weaver.GenerateReadFunction(variable);
			}
			if (methodDefinition == null)
			{
				Log.Error("GetReadFunc unable to generate function for:" + variable.FullName);
				return null;
			}
			Weaver.RegisterReadFunc(variable.FullName, methodDefinition);
			return methodDefinition;
		}

		public static void RegisterReadByReferenceFunc(string name, MethodDefinition newReaderFunc)
		{
			Weaver.lists.readByReferenceFuncs[name] = newReaderFunc;
			Weaver.lists.generatedReadFunctions.Add(newReaderFunc);
			Weaver.ConfirmGeneratedCodeClass(Weaver.scriptDef.MainModule);
			Weaver.lists.generateContainerClass.Methods.Add(newReaderFunc);
		}

		public static void RegisterReadFunc(string name, MethodDefinition newReaderFunc)
		{
			Weaver.lists.readFuncs[name] = newReaderFunc;
			Weaver.lists.generatedReadFunctions.Add(newReaderFunc);
			Weaver.ConfirmGeneratedCodeClass(Weaver.scriptDef.MainModule);
			Weaver.lists.generateContainerClass.Methods.Add(newReaderFunc);
		}

		private static MethodDefinition GenerateArrayReadFunc(TypeReference variable, MethodReference elementReadFunc)
		{
			string text = "_ReadArray" + variable.GetElementType().Name + "_";
			if (variable.DeclaringType != null)
			{
				text += variable.DeclaringType.Name;
			}
			else
			{
				text += "None";
			}
			MethodDefinition methodDefinition = new MethodDefinition(text, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static | MethodAttributes.HideBySig, variable);
			methodDefinition.Parameters.Add(new ParameterDefinition("reader", ParameterAttributes.None, Weaver.scriptDef.MainModule.Import(Weaver.NetworkReaderType)));
			methodDefinition.Body.Variables.Add(new VariableDefinition("v0", Weaver.int32Type));
			methodDefinition.Body.Variables.Add(new VariableDefinition("v1", variable));
			methodDefinition.Body.Variables.Add(new VariableDefinition("v2", Weaver.int32Type));
			methodDefinition.Body.InitLocals = true;
			ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.NetworkReadUInt16));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
			Instruction instruction = iLProcessor.Create(OpCodes.Nop);
			iLProcessor.Append(iLProcessor.Create(OpCodes.Brtrue, instruction));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Newarr, variable.GetElementType()));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			iLProcessor.Append(instruction);
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Newarr, variable.GetElementType()));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_1));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_2));
			Instruction instruction2 = iLProcessor.Create(OpCodes.Nop);
			iLProcessor.Append(iLProcessor.Create(OpCodes.Br, instruction2));
			Instruction instruction3 = iLProcessor.Create(OpCodes.Nop);
			iLProcessor.Append(instruction3);
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_1));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_2));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldelema, variable.GetElementType()));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Call, elementReadFunc));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Stobj, variable.GetElementType()));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_2));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4_1));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Add));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_2));
			iLProcessor.Append(instruction2);
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_2));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Blt, instruction3));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_1));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			return methodDefinition;
		}

		private static MethodDefinition GenerateArrayWriteFunc(TypeReference variable, MethodReference elementWriteFunc)
		{
			string text = "_WriteArray" + variable.GetElementType().Name + "_";
			if (variable.DeclaringType != null)
			{
				text += variable.DeclaringType.Name;
			}
			else
			{
				text += "None";
			}
			MethodDefinition methodDefinition = new MethodDefinition(text, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static | MethodAttributes.HideBySig, Weaver.voidType);
			methodDefinition.Parameters.Add(new ParameterDefinition("writer", ParameterAttributes.None, Weaver.scriptDef.MainModule.Import(Weaver.NetworkWriterType)));
			methodDefinition.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, Weaver.scriptDef.MainModule.Import(variable)));
			methodDefinition.Body.Variables.Add(new VariableDefinition("v0", Weaver.uint16Type));
			methodDefinition.Body.Variables.Add(new VariableDefinition("v1", Weaver.uint16Type));
			methodDefinition.Body.InitLocals = true;
			ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
			Instruction instruction = iLProcessor.Create(OpCodes.Nop);
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Brtrue, instruction));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.NetworkWriteUInt16));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			iLProcessor.Append(instruction);
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldlen));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Conv_I4));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Conv_U2));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.NetworkWriteUInt16));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_1));
			Instruction instruction2 = iLProcessor.Create(OpCodes.Nop);
			iLProcessor.Append(iLProcessor.Create(OpCodes.Br, instruction2));
			Instruction instruction3 = iLProcessor.Create(OpCodes.Nop);
			iLProcessor.Append(instruction3);
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_1));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldelema, variable.GetElementType()));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldobj, variable.GetElementType()));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Call, elementWriteFunc));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_1));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4_1));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Add));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Conv_U2));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_1));
			iLProcessor.Append(instruction2);
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_1));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldlen));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Conv_I4));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Blt, instruction3));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			return methodDefinition;
		}

		private static MethodDefinition GenerateWriterFunction(TypeReference variable)
		{
			string text = "_Write" + variable.Name + "_";
			if (variable.DeclaringType != null)
			{
				text += variable.DeclaringType.Name;
			}
			else
			{
				text += "None";
			}
			MethodDefinition methodDefinition = new MethodDefinition(text, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static | MethodAttributes.HideBySig, Weaver.voidType);
			methodDefinition.Parameters.Add(new ParameterDefinition("writer", ParameterAttributes.None, Weaver.scriptDef.MainModule.Import(Weaver.NetworkWriterType)));
			methodDefinition.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, Weaver.scriptDef.MainModule.Import(variable)));
			ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
			foreach (FieldDefinition current in variable.Resolve().Fields)
			{
				if (!current.IsStatic && !current.IsPrivate)
				{
					if (current.FieldType.Resolve().HasGenericParameters)
					{
						Weaver.fail = true;
						Log.Error(string.Concat(new object[]
						{
							"WriteReadFunc for ",
							current.Name,
							" [",
							current.FieldType,
							"/",
							current.FieldType.FullName,
							"]. Cannot have generic parameters."
						}));
						MethodDefinition result = null;
						return result;
					}
					if (current.FieldType.Resolve().IsInterface)
					{
						Weaver.fail = true;
						Log.Error(string.Concat(new object[]
						{
							"WriteReadFunc for ",
							current.Name,
							" [",
							current.FieldType,
							"/",
							current.FieldType.FullName,
							"]. Cannot be an interface."
						}));
						MethodDefinition result = null;
						return result;
					}
					MethodReference writeFunc = Weaver.GetWriteFunc(current.FieldType);
					if (writeFunc == null)
					{
						Log.Error(string.Concat(new object[]
						{
							"WriteReadFunc for ",
							current.Name,
							" type ",
							current.FieldType,
							" no supported"
						}));
						Weaver.fail = true;
						MethodDefinition result = null;
						return result;
					}
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldfld, current));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Call, writeFunc));
				}
			}
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			return methodDefinition;
		}

		private static MethodDefinition GenerateReadFunction(TypeReference variable)
		{
			string text = "_Read" + variable.Name + "_";
			if (variable.DeclaringType != null)
			{
				text += variable.DeclaringType.Name;
			}
			else
			{
				text += "None";
			}
			MethodDefinition methodDefinition = new MethodDefinition(text, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static | MethodAttributes.HideBySig, variable);
			methodDefinition.Body.Variables.Add(new VariableDefinition("result", variable));
			methodDefinition.Body.InitLocals = true;
			methodDefinition.Parameters.Add(new ParameterDefinition("reader", ParameterAttributes.None, Weaver.scriptDef.MainModule.Import(Weaver.NetworkReaderType)));
			ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
			if (variable.IsValueType)
			{
				iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloca, 0));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Initobj, variable));
			}
			else
			{
				MethodReference method = Weaver.ResolveMethod(variable, ".ctor");
				iLProcessor.Append(iLProcessor.Create(OpCodes.Newobj, method));
				iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_0));
			}
			foreach (FieldDefinition current in variable.Resolve().Fields)
			{
				if (!current.IsStatic && !current.IsPrivate)
				{
					if (variable.IsValueType)
					{
						iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloca, 0));
					}
					else
					{
						iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc, 0));
					}
					MethodReference readFunc = Weaver.GetReadFunc(current.FieldType);
					if (readFunc == null)
					{
						Log.Error(string.Concat(new object[]
						{
							"GetReadFunc for ",
							current.Name,
							" type ",
							current.FieldType,
							" no supported"
						}));
						Weaver.fail = true;
						return null;
					}
					iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Call, readFunc));
					iLProcessor.Append(iLProcessor.Create(OpCodes.Stfld, current));
				}
			}
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
			iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
			return methodDefinition;
		}

		private static Instruction GetEventLoadInstruction(ModuleDefinition moduleDef, TypeDefinition td, MethodDefinition md, int iCount, FieldReference foundEventField)
		{
			while (iCount > 0)
			{
				iCount--;
				Instruction instruction = md.Body.Instructions[iCount];
				if (instruction.OpCode == OpCodes.Ldfld && instruction.Operand == foundEventField)
				{
					Weaver.DLog(td, "    " + instruction.Operand, new object[0]);
					return instruction;
				}
			}
			return null;
		}

		private static void ProcessInstructionMethod(ModuleDefinition moduleDef, TypeDefinition td, MethodDefinition md, Instruction instr, MethodReference opMethodRef, int iCount)
		{
			if (opMethodRef.Name == "Invoke")
			{
				bool flag = false;
				while (iCount > 0 && !flag)
				{
					iCount--;
					Instruction instruction = md.Body.Instructions[iCount];
					if (instruction.OpCode == OpCodes.Ldfld)
					{
						FieldReference fieldReference = instruction.Operand as FieldReference;
						for (int i = 0; i < Weaver.lists.replacedEvents.Count; i++)
						{
							EventDefinition eventDefinition = Weaver.lists.replacedEvents[i];
							if (eventDefinition.Name == fieldReference.Name)
							{
								instr.Operand = Weaver.lists.replacementEvents[i];
								instruction.OpCode = OpCodes.Nop;
								flag = true;
								break;
							}
						}
					}
				}
			}
			else if (Weaver.lists.replacementMethodNames.Contains(opMethodRef.FullName))
			{
				for (int j = 0; j < Weaver.lists.replacedMethods.Count; j++)
				{
					MethodDefinition methodDefinition = Weaver.lists.replacedMethods[j];
					if (opMethodRef.FullName == methodDefinition.FullName)
					{
						instr.Operand = Weaver.lists.replacementMethods[j];
						break;
					}
				}
			}
		}

		private static void ConfirmGeneratedCodeClass(ModuleDefinition moduleDef)
		{
			if (Weaver.lists.generateContainerClass == null)
			{
				Weaver.lists.generateContainerClass = new TypeDefinition("Unity", "GeneratedNetworkCode", TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.BeforeFieldInit, Weaver.objectType);
				MethodDefinition methodDefinition = new MethodDefinition(".ctor", MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, Weaver.voidType);
				methodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
				methodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Call, Weaver.ResolveMethod(Weaver.objectType, ".ctor")));
				methodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
				Weaver.lists.generateContainerClass.Methods.Add(methodDefinition);
			}
		}

		private static void ProcessInstructionField(TypeDefinition td, MethodDefinition md, Instruction i, FieldDefinition opField)
		{
			if (md.Name == ".ctor" || md.Name == "OnDeserialize")
			{
				return;
			}
			for (int j = 0; j < Weaver.lists.replacedFields.Count; j++)
			{
				FieldDefinition fieldDefinition = Weaver.lists.replacedFields[j];
				if (opField == fieldDefinition)
				{
					i.OpCode = OpCodes.Call;
					i.Operand = Weaver.lists.replacementProperties[j];
					break;
				}
			}
		}

		private static void ProcessInstruction(ModuleDefinition moduleDef, TypeDefinition td, MethodDefinition md, Instruction i, int iCount)
		{
			if (i.OpCode == OpCodes.Call || i.OpCode == OpCodes.Callvirt)
			{
				MethodReference methodReference = i.Operand as MethodReference;
				if (methodReference != null)
				{
					Weaver.ProcessInstructionMethod(moduleDef, td, md, i, methodReference, iCount);
				}
			}
			if (i.OpCode == OpCodes.Stfld)
			{
				FieldDefinition fieldDefinition = i.Operand as FieldDefinition;
				if (fieldDefinition != null)
				{
					Weaver.ProcessInstructionField(td, md, i, fieldDefinition);
				}
			}
		}

		private static void InjectGuardParameters(MethodDefinition md, ILProcessor worker, Instruction top)
		{
			for (int i = 0; i < md.Parameters.Count; i++)
			{
				ParameterDefinition parameterDefinition = md.Parameters[i];
				if (parameterDefinition.IsOut)
				{
					TypeReference elementType = parameterDefinition.ParameterType.GetElementType();
					if (elementType.IsPrimitive)
					{
						worker.InsertBefore(top, worker.Create(OpCodes.Ldarg, i + 1));
						worker.InsertBefore(top, worker.Create(OpCodes.Ldc_I4_0));
						worker.InsertBefore(top, worker.Create(OpCodes.Stind_I4));
					}
					else
					{
						md.Body.Variables.Add(new VariableDefinition("__out" + i, elementType));
						md.Body.InitLocals = true;
						worker.InsertBefore(top, worker.Create(OpCodes.Ldarg, i + 1));
						worker.InsertBefore(top, worker.Create(OpCodes.Ldloca_S, (byte)(md.Body.Variables.Count - 1)));
						worker.InsertBefore(top, worker.Create(OpCodes.Initobj, elementType));
						worker.InsertBefore(top, worker.Create(OpCodes.Ldloc, md.Body.Variables.Count - 1));
						worker.InsertBefore(top, worker.Create(OpCodes.Stobj, elementType));
					}
				}
			}
		}

		private static void InjectGuardReturnValue(MethodDefinition md, ILProcessor worker, Instruction top)
		{
			if (md.ReturnType.FullName != Weaver.voidType.FullName)
			{
				if (md.ReturnType.IsPrimitive)
				{
					worker.InsertBefore(top, worker.Create(OpCodes.Ldc_I4_0));
				}
				else
				{
					md.Body.Variables.Add(new VariableDefinition("__ret", md.ReturnType));
					md.Body.InitLocals = true;
					worker.InsertBefore(top, worker.Create(OpCodes.Ldloca_S, (byte)(md.Body.Variables.Count - 1)));
					worker.InsertBefore(top, worker.Create(OpCodes.Initobj, md.ReturnType));
					worker.InsertBefore(top, worker.Create(OpCodes.Ldloc, md.Body.Variables.Count - 1));
				}
			}
		}

		private static void InjectServerGuard(ModuleDefinition moduleDef, TypeDefinition td, MethodDefinition md, bool logWarning)
		{
			if (!Weaver.IsNetworkBehaviour(td))
			{
				Log.Error("[Server] guard on non-NetworkBehaviour script at [" + md.FullName + "]");
				return;
			}
			ILProcessor iLProcessor = md.Body.GetILProcessor();
			Instruction instruction = md.Body.Instructions[0];
			iLProcessor.InsertBefore(instruction, iLProcessor.Create(OpCodes.Call, Weaver.NetworkServerGetActive));
			iLProcessor.InsertBefore(instruction, iLProcessor.Create(OpCodes.Brtrue, instruction));
			if (logWarning)
			{
				iLProcessor.InsertBefore(instruction, iLProcessor.Create(OpCodes.Ldstr, "[Server] function '" + md.FullName + "' called on client"));
				iLProcessor.InsertBefore(instruction, iLProcessor.Create(OpCodes.Call, Weaver.logWarningReference));
			}
			Weaver.InjectGuardParameters(md, iLProcessor, instruction);
			Weaver.InjectGuardReturnValue(md, iLProcessor, instruction);
			iLProcessor.InsertBefore(instruction, iLProcessor.Create(OpCodes.Ret));
		}

		private static void InjectClientGuard(ModuleDefinition moduleDef, TypeDefinition td, MethodDefinition md, bool logWarning)
		{
			if (!Weaver.IsNetworkBehaviour(td))
			{
				Log.Error("[Client] guard on non-NetworkBehaviour script at [" + md.FullName + "]");
				return;
			}
			ILProcessor iLProcessor = md.Body.GetILProcessor();
			Instruction instruction = md.Body.Instructions[0];
			iLProcessor.InsertBefore(instruction, iLProcessor.Create(OpCodes.Call, Weaver.NetworkClientGetActive));
			iLProcessor.InsertBefore(instruction, iLProcessor.Create(OpCodes.Brtrue, instruction));
			if (logWarning)
			{
				iLProcessor.InsertBefore(instruction, iLProcessor.Create(OpCodes.Ldstr, "[Client] function '" + md.FullName + "' called on server"));
				iLProcessor.InsertBefore(instruction, iLProcessor.Create(OpCodes.Call, Weaver.logWarningReference));
			}
			Weaver.InjectGuardParameters(md, iLProcessor, instruction);
			Weaver.InjectGuardReturnValue(md, iLProcessor, instruction);
			iLProcessor.InsertBefore(instruction, iLProcessor.Create(OpCodes.Ret));
		}

		private static void ProcessSiteMethod(ModuleDefinition moduleDef, TypeDefinition td, MethodDefinition md)
		{
			if (md.Name == ".cctor" || md.Name == "OnUnserializeVars")
			{
				return;
			}
			string a = md.Name.Substring(0, Math.Min(md.Name.Length, 4));
			if (a == "UNet")
			{
				return;
			}
			a = md.Name.Substring(0, Math.Min(md.Name.Length, 7));
			if (a == "CallCmd")
			{
				return;
			}
			a = md.Name.Substring(0, Math.Min(md.Name.Length, 9));
			if (a == "InvokeCmd" || a == "InvokeRpc" || a == "InvokeSyn")
			{
				return;
			}
			if (md.Body != null && md.Body.Instructions != null)
			{
				foreach (CustomAttribute current in md.CustomAttributes)
				{
					if (current.Constructor.DeclaringType.ToString() == "UnityEngine.Networking.ServerAttribute")
					{
						Weaver.InjectServerGuard(moduleDef, td, md, true);
					}
					else if (current.Constructor.DeclaringType.ToString() == "UnityEngine.Networking.ServerCallbackAttribute")
					{
						Weaver.InjectServerGuard(moduleDef, td, md, false);
					}
					else if (current.Constructor.DeclaringType.ToString() == "UnityEngine.Networking.ClientAttribute")
					{
						Weaver.InjectClientGuard(moduleDef, td, md, true);
					}
					else if (current.Constructor.DeclaringType.ToString() == "UnityEngine.Networking.ClientCallbackAttribute")
					{
						Weaver.InjectClientGuard(moduleDef, td, md, false);
					}
				}
				int num = 0;
				foreach (Instruction current2 in md.Body.Instructions)
				{
					Weaver.ProcessInstruction(moduleDef, td, md, current2, num);
					num++;
				}
			}
		}

		private static void ProcessSiteClass(ModuleDefinition moduleDef, TypeDefinition td)
		{
			foreach (MethodDefinition current in td.Methods)
			{
				Weaver.ProcessSiteMethod(moduleDef, td, current);
			}
			foreach (TypeDefinition current2 in td.NestedTypes)
			{
				Weaver.ProcessSiteClass(moduleDef, current2);
			}
		}

		private static void ProcessSitesModule(ModuleDefinition moduleDef)
		{
			DateTime now = DateTime.Now;
			foreach (TypeDefinition current in moduleDef.Types)
			{
				if (current.IsClass)
				{
					Weaver.ProcessSiteClass(moduleDef, current);
				}
			}
			if (Weaver.lists.generateContainerClass != null)
			{
				moduleDef.Types.Add(Weaver.lists.generateContainerClass);
				Weaver.scriptDef.MainModule.Import(Weaver.lists.generateContainerClass);
				foreach (MethodDefinition current2 in Weaver.lists.generatedReadFunctions)
				{
					Weaver.scriptDef.MainModule.Import(current2);
				}
				foreach (MethodDefinition current3 in Weaver.lists.generatedWriteFunctions)
				{
					Weaver.scriptDef.MainModule.Import(current3);
				}
			}
			Console.WriteLine(string.Concat(new object[]
			{
				"  ProcessSitesModule ",
				moduleDef.Name,
				" elapsed time:",
				DateTime.Now - now
			}));
		}

		private static void ProcessPropertySites()
		{
			Weaver.ProcessSitesModule(Weaver.scriptDef.MainModule);
		}

		private static bool ProcessMessageType(TypeDefinition td)
		{
			MessageClassProcessor messageClassProcessor = new MessageClassProcessor(td);
			messageClassProcessor.Process();
			return true;
		}

		private static bool ProcessSyncListStructType(TypeDefinition td)
		{
			SyncListStructProcessor syncListStructProcessor = new SyncListStructProcessor(td);
			syncListStructProcessor.Process();
			return true;
		}

		private static void ProcessMonoBehaviourType(TypeDefinition td)
		{
			MonoBehaviourProcessor monoBehaviourProcessor = new MonoBehaviourProcessor(td);
			monoBehaviourProcessor.Process();
		}

		private static bool ProcessNetworkBehaviourType(TypeDefinition td)
		{
			foreach (MethodDefinition current in td.Resolve().Methods)
			{
				if (current.Name == "UNetVersion")
				{
					Weaver.DLog(td, " Already processed", new object[0]);
					return false;
				}
			}
			Weaver.DLog(td, "Found NetworkBehaviour " + td.FullName, new object[0]);
			NetworkBehaviourProcessor networkBehaviourProcessor = new NetworkBehaviourProcessor(td);
			networkBehaviourProcessor.Process();
			return true;
		}

		public static MethodReference ResolveMethod(TypeReference t, string name)
		{
			if (t == null)
			{
				Log.Error("Type missing for " + name);
				Weaver.fail = true;
				return null;
			}
			foreach (MethodDefinition current in t.Resolve().Methods)
			{
				if (current.Name == name)
				{
					return Weaver.scriptDef.MainModule.Import(current);
				}
			}
			Log.Error(string.Concat(new object[]
			{
				"ResolveMethod failed ",
				t.Name,
				"::",
				name,
				" ",
				t.Resolve()
			}));
			foreach (MethodDefinition current2 in t.Resolve().Methods)
			{
				Log.Error("Method " + current2.Name);
			}
			Weaver.fail = true;
			return null;
		}

		private static MethodReference ResolveMethodWithArg(TypeReference t, string name, TypeReference argType)
		{
			foreach (MethodDefinition current in t.Resolve().Methods)
			{
				if (current.Name == name && current.Parameters.Count == 1 && current.Parameters[0].ParameterType.FullName == argType.FullName)
				{
					return Weaver.scriptDef.MainModule.Import(current);
				}
			}
			Log.Error(string.Concat(new object[]
			{
				"ResolveMethodWithArg failed ",
				t.Name,
				"::",
				name,
				" ",
				argType
			}));
			Weaver.fail = true;
			return null;
		}

		private static GenericInstanceMethod ResolveMethodGeneric(TypeReference t, string name, TypeReference genericType)
		{
			foreach (MethodDefinition current in t.Resolve().Methods)
			{
				if (current.Name == name && current.Parameters.Count == 0 && current.GenericParameters.Count == 1)
				{
					MethodReference method = Weaver.scriptDef.MainModule.Import(current);
					GenericInstanceMethod genericInstanceMethod = new GenericInstanceMethod(method);
					genericInstanceMethod.GenericArguments.Add(genericType);
					if (genericInstanceMethod.GenericArguments[0].FullName == genericType.FullName)
					{
						return genericInstanceMethod;
					}
				}
			}
			Log.Error(string.Concat(new object[]
			{
				"ResolveMethodGeneric failed ",
				t.Name,
				"::",
				name,
				" ",
				genericType
			}));
			Weaver.fail = true;
			return null;
		}

		public static FieldReference ResolveField(TypeReference t, string name)
		{
			foreach (FieldDefinition current in t.Resolve().Fields)
			{
				if (current.Name == name)
				{
					return Weaver.scriptDef.MainModule.Import(current);
				}
			}
			return null;
		}

		public static MethodReference ResolveProperty(TypeReference t, string name)
		{
			foreach (PropertyDefinition current in t.Resolve().Properties)
			{
				if (current.Name == name)
				{
					return Weaver.scriptDef.MainModule.Import(current.GetMethod);
				}
			}
			return null;
		}

		private static void SetupUnityTypes()
		{
			Weaver.vector2Type = Weaver.m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Vector2");
			Weaver.vector3Type = Weaver.m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Vector3");
			Weaver.vector4Type = Weaver.m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Vector4");
			Weaver.colorType = Weaver.m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Color");
			Weaver.color32Type = Weaver.m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Color32");
			Weaver.quaternionType = Weaver.m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Quaternion");
			Weaver.rectType = Weaver.m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Rect");
			Weaver.planeType = Weaver.m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Plane");
			Weaver.rayType = Weaver.m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Ray");
			Weaver.matrixType = Weaver.m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Matrix4x4");
			Weaver.gameObjectType = Weaver.m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.GameObject");
			Weaver.transformType = Weaver.m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Transform");
			Weaver.unityObjectType = Weaver.m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Object");
			Weaver.hashType = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkHash128");
			Weaver.NetworkClientType = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkClient");
			Weaver.NetworkServerType = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkServer");
			Weaver.NetworkCRCType = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkCRC");
			Weaver.SyncVarType = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.SyncVarAttribute");
			Weaver.CommandType = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.CommandAttribute");
			Weaver.ClientRpcType = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.ClientRpcAttribute");
			Weaver.TargetRpcType = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.TargetRpcAttribute");
			Weaver.SyncEventType = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.SyncEventAttribute");
			Weaver.SyncListType = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.SyncList`1");
			Weaver.NetworkSettingsType = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkSettingsAttribute");
			Weaver.SyncListFloatType = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.SyncListFloat");
			Weaver.SyncListIntType = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.SyncListInt");
			Weaver.SyncListUIntType = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.SyncListUInt");
			Weaver.SyncListBoolType = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.SyncListBool");
			Weaver.SyncListStringType = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.SyncListString");
		}

		private static void SetupCorLib()
		{
			AssemblyNameReference name = AssemblyNameReference.Parse("mscorlib");
			ReaderParameters parameters = new ReaderParameters
			{
				AssemblyResolver = Weaver.scriptDef.MainModule.AssemblyResolver
			};
			Weaver.corLib = Weaver.scriptDef.MainModule.AssemblyResolver.Resolve(name, parameters).MainModule;
		}

		private static TypeReference ImportCorLibType(string fullName)
		{
			TypeDefinition type = Weaver.corLib.GetType(fullName) ?? Weaver.corLib.ExportedTypes.First((ExportedType t) => t.FullName == fullName).Resolve();
			return Weaver.scriptDef.MainModule.Import(type);
		}

		private static void SetupTargetTypes()
		{
			Weaver.SetupCorLib();
			Weaver.voidType = Weaver.ImportCorLibType("System.Void");
			Weaver.singleType = Weaver.ImportCorLibType("System.Single");
			Weaver.doubleType = Weaver.ImportCorLibType("System.Double");
			Weaver.decimalType = Weaver.ImportCorLibType("System.Decimal");
			Weaver.boolType = Weaver.ImportCorLibType("System.Boolean");
			Weaver.stringType = Weaver.ImportCorLibType("System.String");
			Weaver.int64Type = Weaver.ImportCorLibType("System.Int64");
			Weaver.uint64Type = Weaver.ImportCorLibType("System.UInt64");
			Weaver.int32Type = Weaver.ImportCorLibType("System.Int32");
			Weaver.uint32Type = Weaver.ImportCorLibType("System.UInt32");
			Weaver.int16Type = Weaver.ImportCorLibType("System.Int16");
			Weaver.uint16Type = Weaver.ImportCorLibType("System.UInt16");
			Weaver.byteType = Weaver.ImportCorLibType("System.Byte");
			Weaver.sbyteType = Weaver.ImportCorLibType("System.SByte");
			Weaver.charType = Weaver.ImportCorLibType("System.Char");
			Weaver.objectType = Weaver.ImportCorLibType("System.Object");
			Weaver.typeType = Weaver.ImportCorLibType("System.Type");
			Weaver.IEnumeratorType = Weaver.ImportCorLibType("System.Collections.IEnumerator");
			Weaver.MemoryStreamType = Weaver.ImportCorLibType("System.IO.MemoryStream");
			Weaver.NetworkReaderType = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkReader");
			Weaver.NetworkReaderDef = Weaver.NetworkReaderType.Resolve();
			Weaver.NetworkReaderCtor = Weaver.ResolveMethod(Weaver.NetworkReaderDef, ".ctor");
			Weaver.NetworkWriterType = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkWriter");
			Weaver.NetworkWriterDef = Weaver.NetworkWriterType.Resolve();
			Weaver.NetworkWriterCtor = Weaver.ResolveMethod(Weaver.NetworkWriterDef, ".ctor");
			Weaver.MemoryStreamCtor = Weaver.ResolveMethod(Weaver.MemoryStreamType, ".ctor");
			Weaver.NetworkInstanceIdType = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkInstanceId");
			Weaver.NetworkSceneIdType = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkSceneId");
			Weaver.NetworkInstanceIdType = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkInstanceId");
			Weaver.NetworkSceneIdType = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkSceneId");
			Weaver.NetworkServerGetActive = Weaver.ResolveMethod(Weaver.NetworkServerType, "get_active");
			Weaver.NetworkServerGetLocalClientActive = Weaver.ResolveMethod(Weaver.NetworkServerType, "get_localClientActive");
			Weaver.NetworkClientGetActive = Weaver.ResolveMethod(Weaver.NetworkClientType, "get_active");
			Weaver.NetworkReaderReadInt32 = Weaver.ResolveMethod(Weaver.NetworkReaderType, "ReadInt32");
			Weaver.NetworkWriterWriteInt32 = Weaver.ResolveMethodWithArg(Weaver.NetworkWriterType, "Write", Weaver.int32Type);
			Weaver.NetworkWriterWriteInt16 = Weaver.ResolveMethodWithArg(Weaver.NetworkWriterType, "Write", Weaver.int16Type);
			Weaver.NetworkReaderReadPacked32 = Weaver.ResolveMethod(Weaver.NetworkReaderType, "ReadPackedUInt32");
			Weaver.NetworkReaderReadPacked64 = Weaver.ResolveMethod(Weaver.NetworkReaderType, "ReadPackedUInt64");
			Weaver.NetworkReaderReadByte = Weaver.ResolveMethod(Weaver.NetworkReaderType, "ReadByte");
			Weaver.NetworkWriterWritePacked32 = Weaver.ResolveMethod(Weaver.NetworkWriterType, "WritePackedUInt32");
			Weaver.NetworkWriterWritePacked64 = Weaver.ResolveMethod(Weaver.NetworkWriterType, "WritePackedUInt64");
			Weaver.NetworkWriterWriteNetworkInstanceId = Weaver.ResolveMethodWithArg(Weaver.NetworkWriterType, "Write", Weaver.NetworkInstanceIdType);
			Weaver.NetworkWriterWriteNetworkSceneId = Weaver.ResolveMethodWithArg(Weaver.NetworkWriterType, "Write", Weaver.NetworkSceneIdType);
			Weaver.NetworkReaderReadNetworkInstanceId = Weaver.ResolveMethod(Weaver.NetworkReaderType, "ReadNetworkId");
			Weaver.NetworkReaderReadNetworkSceneId = Weaver.ResolveMethod(Weaver.NetworkReaderType, "ReadSceneId");
			Weaver.NetworkInstanceIsEmpty = Weaver.ResolveMethod(Weaver.NetworkInstanceIdType, "IsEmpty");
			Weaver.NetworkReadUInt16 = Weaver.ResolveMethod(Weaver.NetworkReaderType, "ReadUInt16");
			Weaver.NetworkWriteUInt16 = Weaver.ResolveMethodWithArg(Weaver.NetworkWriterType, "Write", Weaver.uint16Type);
			Weaver.CmdDelegateReference = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkBehaviour/CmdDelegate");
			Weaver.CmdDelegateConstructor = Weaver.ResolveMethod(Weaver.CmdDelegateReference, ".ctor");
			Weaver.scriptDef.MainModule.Import(Weaver.gameObjectType);
			Weaver.scriptDef.MainModule.Import(Weaver.transformType);
			TypeReference type = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkIdentity");
			Weaver.NetworkIdentityType = Weaver.scriptDef.MainModule.Import(type);
			Weaver.NetworkInstanceIdType = Weaver.scriptDef.MainModule.Import(Weaver.NetworkInstanceIdType);
			Weaver.SyncListFloatReadType = Weaver.ResolveMethod(Weaver.SyncListFloatType, "ReadReference");
			Weaver.SyncListIntReadType = Weaver.ResolveMethod(Weaver.SyncListIntType, "ReadReference");
			Weaver.SyncListUIntReadType = Weaver.ResolveMethod(Weaver.SyncListUIntType, "ReadReference");
			Weaver.SyncListBoolReadType = Weaver.ResolveMethod(Weaver.SyncListBoolType, "ReadReference");
			Weaver.SyncListStringReadType = Weaver.ResolveMethod(Weaver.SyncListStringType, "ReadReference");
			Weaver.SyncListFloatWriteType = Weaver.ResolveMethod(Weaver.SyncListFloatType, "WriteInstance");
			Weaver.SyncListIntWriteType = Weaver.ResolveMethod(Weaver.SyncListIntType, "WriteInstance");
			Weaver.SyncListUIntWriteType = Weaver.ResolveMethod(Weaver.SyncListUIntType, "WriteInstance");
			Weaver.SyncListBoolWriteType = Weaver.ResolveMethod(Weaver.SyncListBoolType, "WriteInstance");
			Weaver.SyncListStringWriteType = Weaver.ResolveMethod(Weaver.SyncListStringType, "WriteInstance");
			Weaver.NetworkBehaviourType = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkBehaviour");
			Weaver.NetworkBehaviourType2 = Weaver.scriptDef.MainModule.Import(Weaver.NetworkBehaviourType);
			Weaver.NetworkConnectionType = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkConnection");
			Weaver.MonoBehaviourType = Weaver.m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.MonoBehaviour");
			Weaver.NetworkConnectionType = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkConnection");
			Weaver.NetworkConnectionType = Weaver.scriptDef.MainModule.Import(Weaver.NetworkConnectionType);
			Weaver.MessageBaseType = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.MessageBase");
			Weaver.SyncListStructType = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.SyncListStruct`1");
			Weaver.NetworkBehaviourDirtyBitsReference = Weaver.ResolveProperty(Weaver.NetworkBehaviourType, "syncVarDirtyBits");
			Weaver.ComponentType = Weaver.m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Component");
			Weaver.ClientSceneType = Weaver.m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.ClientScene");
			Weaver.FindLocalObjectReference = Weaver.ResolveMethod(Weaver.ClientSceneType, "FindLocalObject");
			Weaver.RegisterBehaviourReference = Weaver.ResolveMethod(Weaver.NetworkCRCType, "RegisterBehaviour");
			Weaver.ReadyConnectionReference = Weaver.ResolveMethod(Weaver.ClientSceneType, "get_readyConnection");
			Weaver.getComponentReference = Weaver.ResolveMethodGeneric(Weaver.ComponentType, "GetComponent", Weaver.NetworkIdentityType);
			Weaver.getUNetIdReference = Weaver.ResolveMethod(type, "get_netId");
			Weaver.gameObjectInequality = Weaver.ResolveMethod(Weaver.unityObjectType, "op_Inequality");
			Weaver.UBehaviourIsServer = Weaver.ResolveMethod(Weaver.NetworkBehaviourType, "get_isServer");
			Weaver.getPlayerIdReference = Weaver.ResolveMethod(Weaver.NetworkBehaviourType, "get_playerControllerId");
			Weaver.setSyncVarReference = Weaver.ResolveMethod(Weaver.NetworkBehaviourType, "SetSyncVar");
			Weaver.setSyncVarHookGuard = Weaver.ResolveMethod(Weaver.NetworkBehaviourType, "set_syncVarHookGuard");
			Weaver.getSyncVarHookGuard = Weaver.ResolveMethod(Weaver.NetworkBehaviourType, "get_syncVarHookGuard");
			Weaver.setSyncVarGameObjectReference = Weaver.ResolveMethod(Weaver.NetworkBehaviourType, "SetSyncVarGameObject");
			Weaver.registerCommandDelegateReference = Weaver.ResolveMethod(Weaver.NetworkBehaviourType, "RegisterCommandDelegate");
			Weaver.registerRpcDelegateReference = Weaver.ResolveMethod(Weaver.NetworkBehaviourType, "RegisterRpcDelegate");
			Weaver.registerEventDelegateReference = Weaver.ResolveMethod(Weaver.NetworkBehaviourType, "RegisterEventDelegate");
			Weaver.registerSyncListDelegateReference = Weaver.ResolveMethod(Weaver.NetworkBehaviourType, "RegisterSyncListDelegate");
			Weaver.getTypeReference = Weaver.ResolveMethod(Weaver.objectType, "GetType");
			Weaver.getTypeFromHandleReference = Weaver.ResolveMethod(Weaver.typeType, "GetTypeFromHandle");
			Weaver.logErrorReference = Weaver.ResolveMethod(Weaver.m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Debug"), "LogError");
			Weaver.logWarningReference = Weaver.ResolveMethod(Weaver.m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Debug"), "LogWarning");
			Weaver.sendCommandInternal = Weaver.ResolveMethod(Weaver.NetworkBehaviourType, "SendCommandInternal");
			Weaver.sendRpcInternal = Weaver.ResolveMethod(Weaver.NetworkBehaviourType, "SendRPCInternal");
			Weaver.sendTargetRpcInternal = Weaver.ResolveMethod(Weaver.NetworkBehaviourType, "SendTargetRPCInternal");
			Weaver.sendEventInternal = Weaver.ResolveMethod(Weaver.NetworkBehaviourType, "SendEventInternal");
			Weaver.SyncListType = Weaver.scriptDef.MainModule.Import(Weaver.SyncListType);
			Weaver.SyncListInitBehaviourReference = Weaver.ResolveMethod(Weaver.SyncListType, "InitializeBehaviour");
			Weaver.SyncListInitHandleMsg = Weaver.ResolveMethod(Weaver.SyncListType, "HandleMsg");
			Weaver.SyncListClear = Weaver.ResolveMethod(Weaver.SyncListType, "Clear");
		}

		private static void SetupReadFunctions()
		{
			Weaver.lists.readFuncs = new Dictionary<string, MethodReference>
			{
				{
					Weaver.singleType.FullName,
					Weaver.ResolveMethod(Weaver.NetworkReaderType, "ReadSingle")
				},
				{
					Weaver.doubleType.FullName,
					Weaver.ResolveMethod(Weaver.NetworkReaderType, "ReadDouble")
				},
				{
					Weaver.boolType.FullName,
					Weaver.ResolveMethod(Weaver.NetworkReaderType, "ReadBoolean")
				},
				{
					Weaver.stringType.FullName,
					Weaver.ResolveMethod(Weaver.NetworkReaderType, "ReadString")
				},
				{
					Weaver.int64Type.FullName,
					Weaver.NetworkReaderReadPacked64
				},
				{
					Weaver.uint64Type.FullName,
					Weaver.NetworkReaderReadPacked64
				},
				{
					Weaver.int32Type.FullName,
					Weaver.NetworkReaderReadPacked32
				},
				{
					Weaver.uint32Type.FullName,
					Weaver.NetworkReaderReadPacked32
				},
				{
					Weaver.int16Type.FullName,
					Weaver.NetworkReaderReadPacked32
				},
				{
					Weaver.uint16Type.FullName,
					Weaver.NetworkReaderReadPacked32
				},
				{
					Weaver.byteType.FullName,
					Weaver.NetworkReaderReadPacked32
				},
				{
					Weaver.sbyteType.FullName,
					Weaver.NetworkReaderReadPacked32
				},
				{
					Weaver.charType.FullName,
					Weaver.NetworkReaderReadPacked32
				},
				{
					Weaver.decimalType.FullName,
					Weaver.ResolveMethod(Weaver.NetworkReaderType, "ReadDecimal")
				},
				{
					Weaver.vector2Type.FullName,
					Weaver.ResolveMethod(Weaver.NetworkReaderType, "ReadVector2")
				},
				{
					Weaver.vector3Type.FullName,
					Weaver.ResolveMethod(Weaver.NetworkReaderType, "ReadVector3")
				},
				{
					Weaver.vector4Type.FullName,
					Weaver.ResolveMethod(Weaver.NetworkReaderType, "ReadVector4")
				},
				{
					Weaver.colorType.FullName,
					Weaver.ResolveMethod(Weaver.NetworkReaderType, "ReadColor")
				},
				{
					Weaver.color32Type.FullName,
					Weaver.ResolveMethod(Weaver.NetworkReaderType, "ReadColor32")
				},
				{
					Weaver.quaternionType.FullName,
					Weaver.ResolveMethod(Weaver.NetworkReaderType, "ReadQuaternion")
				},
				{
					Weaver.rectType.FullName,
					Weaver.ResolveMethod(Weaver.NetworkReaderType, "ReadRect")
				},
				{
					Weaver.planeType.FullName,
					Weaver.ResolveMethod(Weaver.NetworkReaderType, "ReadPlane")
				},
				{
					Weaver.rayType.FullName,
					Weaver.ResolveMethod(Weaver.NetworkReaderType, "ReadRay")
				},
				{
					Weaver.matrixType.FullName,
					Weaver.ResolveMethod(Weaver.NetworkReaderType, "ReadMatrix4x4")
				},
				{
					Weaver.hashType.FullName,
					Weaver.ResolveMethod(Weaver.NetworkReaderType, "ReadNetworkHash128")
				},
				{
					Weaver.gameObjectType.FullName,
					Weaver.ResolveMethod(Weaver.NetworkReaderType, "ReadGameObject")
				},
				{
					Weaver.NetworkIdentityType.FullName,
					Weaver.ResolveMethod(Weaver.NetworkReaderType, "ReadNetworkIdentity")
				},
				{
					Weaver.NetworkInstanceIdType.FullName,
					Weaver.NetworkReaderReadNetworkInstanceId
				},
				{
					Weaver.NetworkSceneIdType.FullName,
					Weaver.NetworkReaderReadNetworkSceneId
				},
				{
					Weaver.transformType.FullName,
					Weaver.ResolveMethod(Weaver.NetworkReaderType, "ReadTransform")
				},
				{
					"System.Byte[]",
					Weaver.ResolveMethod(Weaver.NetworkReaderType, "ReadBytesAndSize")
				}
			};
			Weaver.lists.readByReferenceFuncs = new Dictionary<string, MethodReference>
			{
				{
					Weaver.SyncListFloatType.FullName,
					Weaver.SyncListFloatReadType
				},
				{
					Weaver.SyncListIntType.FullName,
					Weaver.SyncListIntReadType
				},
				{
					Weaver.SyncListUIntType.FullName,
					Weaver.SyncListUIntReadType
				},
				{
					Weaver.SyncListBoolType.FullName,
					Weaver.SyncListBoolReadType
				},
				{
					Weaver.SyncListStringType.FullName,
					Weaver.SyncListStringReadType
				}
			};
		}

		private static void SetupWriteFunctions()
		{
			Weaver.lists.writeFuncs = new Dictionary<string, MethodReference>
			{
				{
					Weaver.singleType.FullName,
					Weaver.ResolveMethodWithArg(Weaver.NetworkWriterType, "Write", Weaver.singleType)
				},
				{
					Weaver.doubleType.FullName,
					Weaver.ResolveMethodWithArg(Weaver.NetworkWriterType, "Write", Weaver.doubleType)
				},
				{
					Weaver.boolType.FullName,
					Weaver.ResolveMethodWithArg(Weaver.NetworkWriterType, "Write", Weaver.boolType)
				},
				{
					Weaver.stringType.FullName,
					Weaver.ResolveMethodWithArg(Weaver.NetworkWriterType, "Write", Weaver.stringType)
				},
				{
					Weaver.int64Type.FullName,
					Weaver.NetworkWriterWritePacked64
				},
				{
					Weaver.uint64Type.FullName,
					Weaver.NetworkWriterWritePacked64
				},
				{
					Weaver.int32Type.FullName,
					Weaver.NetworkWriterWritePacked32
				},
				{
					Weaver.uint32Type.FullName,
					Weaver.NetworkWriterWritePacked32
				},
				{
					Weaver.int16Type.FullName,
					Weaver.NetworkWriterWritePacked32
				},
				{
					Weaver.uint16Type.FullName,
					Weaver.NetworkWriterWritePacked32
				},
				{
					Weaver.byteType.FullName,
					Weaver.NetworkWriterWritePacked32
				},
				{
					Weaver.sbyteType.FullName,
					Weaver.NetworkWriterWritePacked32
				},
				{
					Weaver.charType.FullName,
					Weaver.NetworkWriterWritePacked32
				},
				{
					Weaver.decimalType.FullName,
					Weaver.ResolveMethodWithArg(Weaver.NetworkWriterType, "Write", Weaver.decimalType)
				},
				{
					Weaver.vector2Type.FullName,
					Weaver.ResolveMethodWithArg(Weaver.NetworkWriterType, "Write", Weaver.vector2Type)
				},
				{
					Weaver.vector3Type.FullName,
					Weaver.ResolveMethodWithArg(Weaver.NetworkWriterType, "Write", Weaver.vector3Type)
				},
				{
					Weaver.vector4Type.FullName,
					Weaver.ResolveMethodWithArg(Weaver.NetworkWriterType, "Write", Weaver.vector4Type)
				},
				{
					Weaver.colorType.FullName,
					Weaver.ResolveMethodWithArg(Weaver.NetworkWriterType, "Write", Weaver.colorType)
				},
				{
					Weaver.color32Type.FullName,
					Weaver.ResolveMethodWithArg(Weaver.NetworkWriterType, "Write", Weaver.color32Type)
				},
				{
					Weaver.quaternionType.FullName,
					Weaver.ResolveMethodWithArg(Weaver.NetworkWriterType, "Write", Weaver.quaternionType)
				},
				{
					Weaver.rectType.FullName,
					Weaver.ResolveMethodWithArg(Weaver.NetworkWriterType, "Write", Weaver.rectType)
				},
				{
					Weaver.planeType.FullName,
					Weaver.ResolveMethodWithArg(Weaver.NetworkWriterType, "Write", Weaver.planeType)
				},
				{
					Weaver.rayType.FullName,
					Weaver.ResolveMethodWithArg(Weaver.NetworkWriterType, "Write", Weaver.rayType)
				},
				{
					Weaver.matrixType.FullName,
					Weaver.ResolveMethodWithArg(Weaver.NetworkWriterType, "Write", Weaver.matrixType)
				},
				{
					Weaver.hashType.FullName,
					Weaver.ResolveMethodWithArg(Weaver.NetworkWriterType, "Write", Weaver.hashType)
				},
				{
					Weaver.gameObjectType.FullName,
					Weaver.ResolveMethodWithArg(Weaver.NetworkWriterType, "Write", Weaver.gameObjectType)
				},
				{
					Weaver.NetworkIdentityType.FullName,
					Weaver.ResolveMethodWithArg(Weaver.NetworkWriterType, "Write", Weaver.NetworkIdentityType)
				},
				{
					Weaver.NetworkInstanceIdType.FullName,
					Weaver.NetworkWriterWriteNetworkInstanceId
				},
				{
					Weaver.NetworkSceneIdType.FullName,
					Weaver.NetworkWriterWriteNetworkSceneId
				},
				{
					Weaver.transformType.FullName,
					Weaver.ResolveMethodWithArg(Weaver.NetworkWriterType, "Write", Weaver.transformType)
				},
				{
					"System.Byte[]",
					Weaver.ResolveMethod(Weaver.NetworkWriterType, "WriteBytesFull")
				},
				{
					Weaver.SyncListFloatType.FullName,
					Weaver.SyncListFloatWriteType
				},
				{
					Weaver.SyncListIntType.FullName,
					Weaver.SyncListIntWriteType
				},
				{
					Weaver.SyncListUIntType.FullName,
					Weaver.SyncListUIntWriteType
				},
				{
					Weaver.SyncListBoolType.FullName,
					Weaver.SyncListBoolWriteType
				},
				{
					Weaver.SyncListStringType.FullName,
					Weaver.SyncListStringWriteType
				}
			};
		}

		private static bool IsNetworkBehaviour(TypeDefinition td)
		{
			if (!td.IsClass)
			{
				return false;
			}
			TypeReference baseType = td.BaseType;
			while (baseType != null)
			{
				if (baseType.FullName == Weaver.NetworkBehaviourType.FullName)
				{
					return true;
				}
				try
				{
					baseType = baseType.Resolve().BaseType;
				}
				catch (AssemblyResolutionException)
				{
					break;
				}
			}
			return false;
		}

		public static bool IsDerivedFrom(TypeDefinition td, TypeReference baseClass)
		{
			if (!td.IsClass)
			{
				return false;
			}
			TypeReference baseType = td.BaseType;
			while (baseType != null)
			{
				string text = baseType.FullName;
				int num = text.IndexOf('<');
				if (num != -1)
				{
					text = text.Substring(0, num);
				}
				if (text == baseClass.FullName)
				{
					return true;
				}
				try
				{
					baseType = baseType.Resolve().BaseType;
				}
				catch (AssemblyResolutionException)
				{
					break;
				}
			}
			return false;
		}

		private static void CheckMonoBehaviour(TypeDefinition td)
		{
			if (Weaver.IsDerivedFrom(td, Weaver.MonoBehaviourType))
			{
				Weaver.ProcessMonoBehaviourType(td);
			}
		}

		private static bool CheckNetworkBehaviour(TypeDefinition td)
		{
			if (!td.IsClass)
			{
				return false;
			}
			if (!Weaver.IsNetworkBehaviour(td))
			{
				Weaver.CheckMonoBehaviour(td);
				return false;
			}
			List<TypeDefinition> list = new List<TypeDefinition>();
			TypeDefinition typeDefinition = td;
			while (typeDefinition != null)
			{
				if (typeDefinition.FullName == Weaver.NetworkBehaviourType.FullName)
				{
					break;
				}
				try
				{
					list.Insert(0, typeDefinition);
					typeDefinition = typeDefinition.BaseType.Resolve();
				}
				catch (AssemblyResolutionException)
				{
					break;
				}
			}
			bool flag = false;
			foreach (TypeDefinition current in list)
			{
				flag |= Weaver.ProcessNetworkBehaviourType(current);
			}
			return flag;
		}

		private static bool CheckMessageBase(TypeDefinition td)
		{
			if (!td.IsClass)
			{
				return false;
			}
			bool flag = false;
			TypeReference baseType = td.BaseType;
			while (baseType != null)
			{
				if (baseType.FullName == Weaver.MessageBaseType.FullName)
				{
					flag |= Weaver.ProcessMessageType(td);
					break;
				}
				try
				{
					baseType = baseType.Resolve().BaseType;
				}
				catch (AssemblyResolutionException)
				{
					break;
				}
			}
			foreach (TypeDefinition current in td.NestedTypes)
			{
				flag |= Weaver.CheckMessageBase(current);
			}
			return flag;
		}

		private static bool CheckSyncListStruct(TypeDefinition td)
		{
			if (!td.IsClass)
			{
				return false;
			}
			bool flag = false;
			TypeReference baseType = td.BaseType;
			while (baseType != null)
			{
				if (baseType.FullName.Contains("SyncListStruct"))
				{
					flag |= Weaver.ProcessSyncListStructType(td);
					break;
				}
				try
				{
					baseType = baseType.Resolve().BaseType;
				}
				catch (AssemblyResolutionException)
				{
					break;
				}
			}
			foreach (TypeDefinition current in td.NestedTypes)
			{
				flag |= Weaver.CheckSyncListStruct(current);
			}
			return flag;
		}

		private static bool Weave(string assName, IEnumerable<string> dependencies, IAssemblyResolver assemblyResolver, string unityEngineDLLPath, string unityUNetDLLPath, string outputDir)
		{
			if (assName.IndexOf("Assembly-") == -1)
			{
				return true;
			}
			if (assName.IndexOf("-Editor.") != -1)
			{
				return true;
			}
			ReaderParameters readerParameters = Helpers.ReaderParameters(assName, dependencies, assemblyResolver, unityEngineDLLPath, unityUNetDLLPath);
			Weaver.scriptDef = AssemblyDefinition.ReadAssembly(assName, readerParameters);
			Weaver.SetupTargetTypes();
			Weaver.SetupReadFunctions();
			Weaver.SetupWriteFunctions();
			ModuleDefinition mainModule = Weaver.scriptDef.MainModule;
			Console.WriteLine("Script Module: {0}", mainModule.Name);
			bool flag = false;
			foreach (TypeDefinition current in mainModule.Types)
			{
				if (current.IsClass)
				{
					try
					{
						flag |= Weaver.CheckSyncListStruct(current);
						flag |= Weaver.CheckNetworkBehaviour(current);
						flag |= Weaver.CheckMessageBase(current);
					}
					catch (Exception ex)
					{
						if (Weaver.scriptDef.MainModule.SymbolReader != null)
						{
							Weaver.scriptDef.MainModule.SymbolReader.Dispose();
						}
						Weaver.fail = true;
						throw ex;
					}
				}
				if (Weaver.fail)
				{
					if (Weaver.scriptDef.MainModule.SymbolReader != null)
					{
						Weaver.scriptDef.MainModule.SymbolReader.Dispose();
					}
					bool result = false;
					return result;
				}
			}
			if (flag)
			{
				foreach (MethodDefinition current2 in Weaver.lists.replacedMethods)
				{
					Weaver.lists.replacementMethodNames.Add(current2.FullName);
				}
				try
				{
					Weaver.ProcessPropertySites();
				}
				catch (Exception arg)
				{
					Log.Error("ProcessPropertySites exception: " + arg);
					if (Weaver.scriptDef.MainModule.SymbolReader != null)
					{
						Weaver.scriptDef.MainModule.SymbolReader.Dispose();
					}
					bool result = false;
					return result;
				}
				if (Weaver.fail)
				{
					if (Weaver.scriptDef.MainModule.SymbolReader != null)
					{
						Weaver.scriptDef.MainModule.SymbolReader.Dispose();
					}
					return false;
				}
				string fileName = Helpers.DestinationFileFor(outputDir, assName);
				WriterParameters writerParameters = Helpers.GetWriterParameters(readerParameters);
				if (writerParameters.SymbolWriterProvider is PdbWriterProvider)
				{
					writerParameters.SymbolWriterProvider = new MdbWriterProvider();
					string path = Path.ChangeExtension(assName, ".pdb");
					File.Delete(path);
				}
				Weaver.scriptDef.Write(fileName, writerParameters);
			}
			if (Weaver.scriptDef.MainModule.SymbolReader != null)
			{
				Weaver.scriptDef.MainModule.SymbolReader.Dispose();
			}
			return true;
		}

		public static bool WeaveAssemblies(IEnumerable<string> assemblies, IEnumerable<string> dependencies, IAssemblyResolver assemblyResolver, string outputDir, string unityEngineDLLPath, string unityUNetDLLPath)
		{
			Weaver.fail = false;
			Weaver.lists = new WeaverLists();
			Console.WriteLine("WeaveAssemblies unityPath= " + unityEngineDLLPath);
			Weaver.m_UnityAssemblyDefinition = AssemblyDefinition.ReadAssembly(unityEngineDLLPath);
			Console.WriteLine("WeaveAssemblies unetPath= " + unityUNetDLLPath);
			Weaver.m_UNetAssemblyDefinition = AssemblyDefinition.ReadAssembly(unityUNetDLLPath);
			Weaver.SetupUnityTypes();
			try
			{
				foreach (string current in assemblies)
				{
					if (!Weaver.Weave(current, dependencies, assemblyResolver, unityEngineDLLPath, unityUNetDLLPath, outputDir))
					{
						bool result = false;
						return result;
					}
				}
			}
			catch (Exception arg)
			{
				Log.Error("Exception :" + arg);
				bool result = false;
				return result;
			}
			Weaver.corLib = null;
			return true;
		}
	}
}
