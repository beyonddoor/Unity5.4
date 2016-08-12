using Mono.Cecil;
using System;
using System.Collections.Generic;
using Unity.CecilTools.Extensions;

namespace Unity.SerializationLogic
{
	public class UnityEngineTypePredicates
	{
		private const string AnimationCurve = "UnityEngine.AnimationCurve";

		private const string Gradient = "UnityEngine.Gradient";

		private const string GUIStyle = "UnityEngine.GUIStyle";

		private const string RectOffset = "UnityEngine.RectOffset";

		protected const string UnityEngineObject = "UnityEngine.Object";

		public const string MonoBehaviour = "UnityEngine.MonoBehaviour";

		public const string ScriptableObject = "UnityEngine.ScriptableObject";

		protected const string Matrix4x4 = "UnityEngine.Matrix4x4";

		protected const string Color32 = "UnityEngine.Color32";

		private const string SerializeFieldAttribute = "UnityEngine.SerializeField";

		private static readonly HashSet<string> TypesThatShouldHaveHadSerializableAttribute = new HashSet<string>
		{
			"Vector3",
			"Vector2",
			"Vector4",
			"Rect",
			"Quaternion",
			"Matrix4x4",
			"Color",
			"Color32",
			"LayerMask",
			"Bounds"
		};

		private static string[] serializableStructs = new string[]
		{
			"UnityEngine.AnimationCurve",
			"UnityEngine.Color32",
			"UnityEngine.Gradient",
			"UnityEngine.GUIStyle",
			"UnityEngine.RectOffset",
			"UnityEngine.Matrix4x4"
		};

		public static bool IsMonoBehaviour(TypeReference type)
		{
			return UnityEngineTypePredicates.IsMonoBehaviour(type.CheckedResolve());
		}

		private static bool IsMonoBehaviour(TypeDefinition typeDefinition)
		{
			return typeDefinition.IsSubclassOf("UnityEngine.MonoBehaviour");
		}

		public static bool IsScriptableObject(TypeReference type)
		{
			return UnityEngineTypePredicates.IsScriptableObject(type.CheckedResolve());
		}

		private static bool IsScriptableObject(TypeDefinition temp)
		{
			return temp.IsSubclassOf("UnityEngine.ScriptableObject");
		}

		public static bool IsColor32(TypeReference type)
		{
			return type.IsAssignableTo("UnityEngine.Color32");
		}

		public static bool IsMatrix4x4(TypeReference type)
		{
			return type.IsAssignableTo("UnityEngine.Matrix4x4");
		}

		public static bool IsGradient(TypeReference type)
		{
			return type.IsAssignableTo("UnityEngine.Gradient");
		}

		public static bool IsGUIStyle(TypeReference type)
		{
			return type.IsAssignableTo("UnityEngine.GUIStyle");
		}

		public static bool IsRectOffset(TypeReference type)
		{
			return type.IsAssignableTo("UnityEngine.RectOffset");
		}

		public static bool IsSerializableUnityStruct(TypeReference type)
		{
			string[] array = UnityEngineTypePredicates.serializableStructs;
			for (int i = 0; i < array.Length; i++)
			{
				string typeName = array[i];
				if (type.IsAssignableTo(typeName))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsUnityEngineObject(TypeReference type)
		{
			if (type.IsArray)
			{
				return false;
			}
			TypeDefinition typeDefinition = type.Resolve();
			return typeDefinition != null && (type.FullName == "UnityEngine.Object" || typeDefinition.IsSubclassOf("UnityEngine.Object"));
		}

		public static bool ShouldHaveHadSerializableAttribute(TypeReference type)
		{
			return UnityEngineTypePredicates.IsUnityEngineValueType(type);
		}

		public static bool IsUnityEngineValueType(TypeReference type)
		{
			return type.SafeNamespace() == "UnityEngine" && UnityEngineTypePredicates.TypesThatShouldHaveHadSerializableAttribute.Contains(type.Name);
		}

		public static bool IsSerializeFieldAttribute(TypeReference attributeType)
		{
			return attributeType.FullName == "UnityEngine.SerializeField";
		}
	}
}
