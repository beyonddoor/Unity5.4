using Mono.Cecil;
using System;

namespace Unity.UNetWeaver
{
	internal class MonoBehaviourProcessor
	{
		private TypeDefinition m_td;

		public MonoBehaviourProcessor(TypeDefinition td)
		{
			this.m_td = td;
		}

		public void Process()
		{
			this.ProcessSyncVars();
			this.ProcessMethods();
		}

		private void ProcessSyncVars()
		{
			foreach (FieldDefinition current in this.m_td.Fields)
			{
				foreach (CustomAttribute current2 in current.CustomAttributes)
				{
					if (current2.AttributeType.FullName == Weaver.SyncVarType.FullName)
					{
						Log.Error(string.Concat(new string[]
						{
							"Script ",
							this.m_td.FullName,
							" uses [SyncVar] ",
							current.Name,
							" but is not a NetworkBehaviour."
						}));
						Weaver.fail = true;
					}
				}
			}
		}

		private void ProcessMethods()
		{
			foreach (MethodDefinition current in this.m_td.Methods)
			{
				foreach (CustomAttribute current2 in current.CustomAttributes)
				{
					if (current2.AttributeType.FullName == Weaver.CommandType.FullName)
					{
						Log.Error(string.Concat(new string[]
						{
							"Script ",
							this.m_td.FullName,
							" uses [Command] ",
							current.Name,
							" but is not a NetworkBehaviour."
						}));
						Weaver.fail = true;
					}
					if (current2.AttributeType.FullName == Weaver.ClientRpcType.FullName)
					{
						Log.Error(string.Concat(new string[]
						{
							"Script ",
							this.m_td.FullName,
							" uses [ClientRpc] ",
							current.Name,
							" but is not a NetworkBehaviour."
						}));
						Weaver.fail = true;
					}
					if (current2.AttributeType.FullName == Weaver.TargetRpcType.FullName)
					{
						Log.Error(string.Concat(new string[]
						{
							"Script ",
							this.m_td.FullName,
							" uses [TargetRpc] ",
							current.Name,
							" but is not a NetworkBehaviour."
						}));
						Weaver.fail = true;
					}
					string a = current2.Constructor.DeclaringType.ToString();
					if (a == "UnityEngine.Networking.ServerAttribute")
					{
						Log.Error(string.Concat(new string[]
						{
							"Script ",
							this.m_td.FullName,
							" uses the attribute [Server] on the method ",
							current.Name,
							" but is not a NetworkBehaviour."
						}));
						Weaver.fail = true;
					}
					else if (a == "UnityEngine.Networking.ServerCallbackAttribute")
					{
						Log.Error(string.Concat(new string[]
						{
							"Script ",
							this.m_td.FullName,
							" uses the attribute [ServerCallback] on the method ",
							current.Name,
							" but is not a NetworkBehaviour."
						}));
						Weaver.fail = true;
					}
					else if (a == "UnityEngine.Networking.ClientAttribute")
					{
						Log.Error(string.Concat(new string[]
						{
							"Script ",
							this.m_td.FullName,
							" uses the attribute [Client] on the method ",
							current.Name,
							" but is not a NetworkBehaviour."
						}));
						Weaver.fail = true;
					}
					else if (a == "UnityEngine.Networking.ClientCallbackAttribute")
					{
						Log.Error(string.Concat(new string[]
						{
							"Script ",
							this.m_td.FullName,
							" uses the attribute [ClientCallback] on the method ",
							current.Name,
							" but is not a NetworkBehaviour."
						}));
						Weaver.fail = true;
					}
				}
			}
		}
	}
}
