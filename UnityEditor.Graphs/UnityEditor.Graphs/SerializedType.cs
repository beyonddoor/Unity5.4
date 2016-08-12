using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEditor.Graphs
{
	public class SerializedType
	{
		private struct SerializedTypeData
		{
			public string typeName;

			public string genericTypeName;

			public bool isGeneric;
		}

		private static string StripTypeNameString(string str, int index)
		{
			int num = index + 1;
			while (num < str.Length && str[num] != ',' && str[num] != ']')
			{
				num++;
			}
			return str.Remove(index, num - index);
		}

		private static string StripAllFromTypeNameString(string str, string toStrip)
		{
			for (int num = str.IndexOf(toStrip); num != -1; num = str.IndexOf(toStrip, num))
			{
				str = SerializedType.StripTypeNameString(str, num);
			}
			return str;
		}

		private static string ToShortTypeName(Type t)
		{
			string text = t.AssemblyQualifiedName;
			if (string.IsNullOrEmpty(text))
			{
				return string.Empty;
			}
			text = SerializedType.StripAllFromTypeNameString(text, ", Version");
			text = SerializedType.StripAllFromTypeNameString(text, ", Culture");
			return SerializedType.StripAllFromTypeNameString(text, ", PublicKeyToken");
		}

		private static string SafeTypeName(Type type)
		{
			return (type.FullName == null) ? null : type.FullName.Replace('+', '.');
		}

		private static SerializedType.SerializedTypeData SplitTypeString(string serializedTypeString)
		{
			if (string.IsNullOrEmpty(serializedTypeString))
			{
				throw new ArgumentException("Cannot parse serialized type string, it is empty.");
			}
			SerializedType.SerializedTypeData result;
			result.isGeneric = SerializedType.IsGeneric(serializedTypeString);
			result.typeName = serializedTypeString.Substring(0, serializedTypeString.IndexOf('#'));
			result.genericTypeName = serializedTypeString.Substring(result.typeName.Length + 1, serializedTypeString.IndexOf('#', result.typeName.Length + 1) - result.typeName.Length - 1);
			return result;
		}

		private static string ToString(SerializedType.SerializedTypeData data)
		{
			return string.Concat(new string[]
			{
				data.typeName,
				"#",
				data.genericTypeName,
				"#",
				(!data.isGeneric) ? "0" : "1"
			});
		}

		private static Type FromString(SerializedType.SerializedTypeData data)
		{
			return Type.GetType(data.typeName, true);
		}

		public static Type GenericType(Type t)
		{
			if (t.IsArray)
			{
				return t.GetElementType();
			}
			if (!t.IsGenericType)
			{
				return t;
			}
			Type[] genericArguments = t.GetGenericArguments();
			if (genericArguments.Length != 1)
			{
				throw new ArgumentException("Internal error: got generic type with more than one generic argument.");
			}
			return genericArguments[0];
		}

		public static bool IsListType(Type t)
		{
			return typeof(IList).IsAssignableFrom(t);
		}

		public static string GetFullName(Type t)
		{
			if (!t.IsGenericType)
			{
				return SerializedType.SafeTypeName(t);
			}
			if (t.GetGenericTypeDefinition() != typeof(List<>))
			{
				throw new ArgumentException("Internal error: got unsupported generic type");
			}
			return string.Format("System.Collections.Generic.List<{0}>", SerializedType.SafeTypeName(t.GetGenericArguments()[0]));
		}

		public static string ToString(Type t)
		{
			SerializedType.SerializedTypeData data = default(SerializedType.SerializedTypeData);
			if (t == null)
			{
				return string.Empty;
			}
			data.typeName = string.Empty;
			data.isGeneric = t.ContainsGenericParameters;
			if (data.isGeneric && t.IsGenericType)
			{
				data.typeName = SerializedType.ToShortTypeName(t.GetGenericTypeDefinition());
			}
			else if (data.isGeneric && t.IsArray)
			{
				data.typeName = "T[]";
			}
			else if (data.isGeneric)
			{
				data.typeName = "T";
			}
			else
			{
				data.typeName = SerializedType.ToShortTypeName(t);
			}
			return SerializedType.ToString(data);
		}

		public static Type FromString(string serializedTypeString)
		{
			if (string.IsNullOrEmpty(serializedTypeString) || SerializedType.IsGeneric(serializedTypeString))
			{
				return null;
			}
			return Type.GetType(SerializedType.SplitTypeString(serializedTypeString).typeName, true);
		}

		public static bool IsGeneric(string serializedTypeString)
		{
			return !string.IsNullOrEmpty(serializedTypeString) && serializedTypeString[serializedTypeString.Length - 1] == '1';
		}

		public static bool IsBaseTypeGeneric(string serializedTypeString)
		{
			if (string.IsNullOrEmpty(serializedTypeString))
			{
				return false;
			}
			SerializedType.SerializedTypeData serializedTypeData = SerializedType.SplitTypeString(serializedTypeString);
			return serializedTypeData.isGeneric || serializedTypeData.genericTypeName != string.Empty;
		}

		public static string SetGenericArgumentType(string serializedTypeString, Type type)
		{
			if (SerializedType.IsGeneric(serializedTypeString))
			{
				SerializedType.SerializedTypeData data = SerializedType.SplitTypeString(serializedTypeString);
				data.genericTypeName = data.typeName;
				data.isGeneric = false;
				string typeName = data.typeName;
				if (typeName != null)
				{
					if (SerializedType.<>f__switch$map1 == null)
					{
						SerializedType.<>f__switch$map1 = new Dictionary<string, int>(2)
						{
							{
								"T",
								0
							},
							{
								"T[]",
								1
							}
						};
					}
					int num;
					if (SerializedType.<>f__switch$map1.TryGetValue(typeName, out num))
					{
						if (num == 0)
						{
							data.typeName = SerializedType.ToShortTypeName(type);
							goto IL_105;
						}
						if (num == 1)
						{
							data.typeName = SerializedType.ToShortTypeName(type.MakeArrayType());
							goto IL_105;
						}
					}
				}
				data.typeName = SerializedType.ToShortTypeName(Type.GetType(data.typeName, true).GetGenericTypeDefinition().MakeGenericType(new Type[]
				{
					type
				}));
				IL_105:
				return SerializedType.ToString(data);
			}
			if (SerializedType.IsBaseTypeGeneric(serializedTypeString))
			{
				throw new ArgumentException("Trying to set a different generic type. Reset old one first.");
			}
			throw new ArgumentException("Trying to set generic argument type for non generic type.");
		}

		public static string ResetGenericArgumentType(string serializedTypeString)
		{
			if (string.IsNullOrEmpty(serializedTypeString))
			{
				throw new ArgumentException("Cannot reset generic argument type for null type.");
			}
			SerializedType.SerializedTypeData data = SerializedType.SplitTypeString(serializedTypeString);
			if (string.IsNullOrEmpty(data.genericTypeName))
			{
				throw new ArgumentException("Cannot reset generic argument type, previous generic type unknown.");
			}
			data.typeName = data.genericTypeName;
			data.isGeneric = true;
			data.genericTypeName = string.Empty;
			return SerializedType.ToString(data);
		}

		public static bool CanAssignFromGenericType(string serializedTypeString, Type t)
		{
			SerializedType.SerializedTypeData data = SerializedType.SplitTypeString(serializedTypeString);
			if (!data.isGeneric)
			{
				return false;
			}
			if (!t.IsGenericType)
			{
				return data.typeName == "T" || data.typeName == "T[]";
			}
			if (data.typeName == "T" || data.typeName == "T[]")
			{
				return false;
			}
			Type[] genericArguments = t.GetGenericArguments();
			return genericArguments.Length == 1 && !genericArguments[0].IsGenericType && t.GetGenericTypeDefinition() == SerializedType.FromString(data).GetGenericTypeDefinition();
		}
	}
}
