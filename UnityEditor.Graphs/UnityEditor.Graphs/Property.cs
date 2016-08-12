using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace UnityEditor.Graphs
{
	[Serializable]
	public class Property
	{
		internal const char kListSeparator = '↓';

		[SerializeField]
		private string m_Name;

		[SerializeField]
		private string m_TypeString = string.Empty;

		[SerializeField]
		private string m_Value = string.Empty;

		[SerializeField]
		private List<UnityEngine.Object> m_RefValues;

		[NonSerialized]
		private bool m_HaveCachedDeserializedResult;

		[NonSerialized]
		private string m_CachedLastDeserializedString;

		[NonSerialized]
		private object m_CachedLastDeserializedValue;

		public Type type
		{
			get
			{
				return SerializedType.FromString(this.m_TypeString);
			}
			set
			{
				this.m_TypeString = SerializedType.ToString(value);
			}
		}

		public string typeString
		{
			get
			{
				return this.m_TypeString;
			}
		}

		public bool isGeneric
		{
			get
			{
				return SerializedType.IsGeneric(this.m_TypeString);
			}
		}

		private List<UnityEngine.Object> refValues
		{
			get
			{
				List<UnityEngine.Object> arg_1B_0;
				if ((arg_1B_0 = this.m_RefValues) == null)
				{
					arg_1B_0 = (this.m_RefValues = new List<UnityEngine.Object>());
				}
				return arg_1B_0;
			}
		}

		public bool hasValue
		{
			get
			{
				return (!this.isSceneReferenceType) ? (!string.IsNullOrEmpty(this.stringValue)) : (this.refValues.Count != 0);
			}
		}

		public bool hasDefaultValue
		{
			get
			{
				object value = this.value;
				return value == null || value.Equals(Property.TryGetDefaultValue(this.type));
			}
		}

		public string name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				this.m_Name = value;
			}
		}

		public bool isIList
		{
			get
			{
				return typeof(IList).IsAssignableFrom(this.type);
			}
		}

		public bool isSceneReferenceType
		{
			get
			{
				return !string.IsNullOrEmpty(this.m_TypeString) && !this.isGeneric && Property.IsSceneReferenceType(this.type);
			}
		}

		public int elementCount
		{
			get
			{
				if (!this.hasValue)
				{
					return 0;
				}
				if (this.isSceneReferenceType)
				{
					return this.refValues.Count;
				}
				return this.stringValue.Count((char t) => t == '↓');
			}
		}

		public Type elementType
		{
			get
			{
				if (!this.isSceneReferenceType)
				{
					return (!this.type.IsArray) ? this.type.GetGenericArguments()[0] : this.type.GetElementType();
				}
				if (this.type.IsArray)
				{
					return this.type.GetElementType();
				}
				if (this.type.IsGenericType && this.type.GetGenericTypeDefinition() == typeof(List<>))
				{
					return this.type.GetGenericArguments()[0];
				}
				return this.type;
			}
		}

		public string stringValue
		{
			get
			{
				return this.m_Value;
			}
		}

		public CodeExpression codeExpression
		{
			get
			{
				Type type = this.type;
				if (typeof(UnityEngine.Object).IsAssignableFrom(type))
				{
					throw new ArgumentException("Trying to get a code expression for Object type");
				}
				object value = this.value;
				if (value == null)
				{
					return this.GetDefaultCodeExpression();
				}
				return (!this.isIList) ? this.ConvertSingleValueToCodeExpression(value) : this.ConvertArrayToCodeExpression(value);
			}
		}

		public object value
		{
			get
			{
				if (this.m_HaveCachedDeserializedResult && this.m_CachedLastDeserializedString == this.m_Value)
				{
					return this.m_CachedLastDeserializedValue;
				}
				if (this.isGeneric)
				{
					this.m_CachedLastDeserializedValue = null;
				}
				else if (this.isSceneReferenceType)
				{
					this.m_CachedLastDeserializedValue = this.GetSceneReferenceValue();
				}
				else
				{
					this.m_CachedLastDeserializedValue = ((!this.isIList) ? this.ConvertToSingleValue() : this.ConvertToListOrArray());
				}
				this.m_HaveCachedDeserializedResult = true;
				this.m_CachedLastDeserializedString = this.m_Value;
				return this.m_CachedLastDeserializedValue;
			}
			set
			{
				this.m_HaveCachedDeserializedResult = false;
				if (this.isSceneReferenceType)
				{
					this.SetSceneReferenceValue(value);
					return;
				}
				if (!this.isIList)
				{
					this.m_Value = this.ConvertFromSingleValue(value);
					return;
				}
				this.m_Value = ((value != null) ? this.ConvertFromListOrArray(value) : string.Empty);
			}
		}

		public Property()
		{
			this.m_Name = string.Empty;
			this.m_TypeString = string.Empty;
		}

		public Property(string typeString, string name)
		{
			this.m_Name = name;
			this.m_TypeString = typeString;
			if (!this.isGeneric)
			{
				this.value = Property.TryGetDefaultValue(this.type);
			}
		}

		public Property(Type type, string name)
		{
			this.m_Name = name;
			this.type = type;
			if (!this.isGeneric)
			{
				this.value = Property.TryGetDefaultValue(type);
			}
		}

		public static bool IsSceneReferenceType(Type t)
		{
			if (t.IsArray)
			{
				t = t.GetElementType();
			}
			if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>))
			{
				t = t.GetGenericArguments()[0];
			}
			return typeof(UnityEngine.Object).IsAssignableFrom(t);
		}

		public void SetGenericArgumentType(Type type)
		{
			if (!this.isGeneric)
			{
				return;
			}
			this.m_TypeString = SerializedType.SetGenericArgumentType(this.m_TypeString, type);
			this.value = Property.TryGetDefaultValue(this.type);
		}

		public void ResetGenericArgumentType()
		{
			this.m_TypeString = SerializedType.ResetGenericArgumentType(this.m_TypeString);
		}

		private static object TryGetDefaultValue(Type type)
		{
			return (!type.IsValueType) ? null : Activator.CreateInstance(type);
		}

		private object GetSceneReferenceValue()
		{
			if (this.type.IsGenericType && this.type.GetGenericTypeDefinition() == typeof(List<>))
			{
				IList list = (IList)Activator.CreateInstance(this.type, new object[]
				{
					this.refValues.Count
				});
				foreach (UnityEngine.Object current in this.refValues)
				{
					list.Add((!(current == null)) ? current : null);
				}
				return list;
			}
			if (this.type.IsArray)
			{
				Array array = (Array)Activator.CreateInstance(this.type, new object[]
				{
					this.refValues.Count
				});
				for (int i = 0; i < array.Length; i++)
				{
					if (this.refValues[i] != null)
					{
						array.SetValue(this.refValues[i], i);
					}
				}
				return array;
			}
			return (this.refValues.Count != 0) ? this.refValues[0] : null;
		}

		private void SetSceneReferenceValue(object o)
		{
			this.refValues.Clear();
			if (o == null)
			{
				return;
			}
			if (this.type.IsArray || (this.type.IsGenericType && this.type.GetGenericTypeDefinition() == typeof(List<>)))
			{
				foreach (object current in ((IList)o))
				{
					this.refValues.Add((UnityEngine.Object)current);
				}
			}
			else
			{
				this.refValues.Add((UnityEngine.Object)o);
			}
		}

		private object ConvertToListOrArray()
		{
			object obj = Activator.CreateInstance(this.type, new object[]
			{
				this.elementCount
			});
			TypeConverter converter = Property.GetConverter(this.elementType);
			int num = 0;
			if (this.type.IsArray)
			{
				Array array = (Array)obj;
				if (!string.IsNullOrEmpty(this.m_Value))
				{
					string[] array2 = this.m_Value.Split(new char[]
					{
						'↓'
					});
					for (int i = 0; i < array2.Length; i++)
					{
						string text = array2[i];
						if (num == this.elementCount)
						{
							break;
						}
						array.SetValue(converter.ConvertFromString(text), num++);
					}
				}
			}
			else
			{
				MethodInfo method = this.type.GetMethod("Add");
				if (!string.IsNullOrEmpty(this.m_Value))
				{
					string[] array3 = this.m_Value.Split(new char[]
					{
						'↓'
					});
					for (int j = 0; j < array3.Length; j++)
					{
						string text2 = array3[j];
						if (num++ == this.elementCount)
						{
							break;
						}
						method.Invoke(obj, new object[]
						{
							converter.ConvertFromString(text2)
						});
					}
				}
			}
			return obj;
		}

		private object ConvertToSingleValue()
		{
			TypeConverter converter = Property.GetConverter(this.type);
			if (typeof(TypeConverter) == converter.GetType() || !converter.IsValid(this.m_Value))
			{
				return null;
			}
			return converter.ConvertFromString(this.m_Value);
		}

		private string ConvertFromSingleValue(object o)
		{
			TypeConverter converter = Property.GetConverter(this.type);
			if (!converter.IsValid(o))
			{
				throw new ArgumentException();
			}
			return converter.ConvertToString(o);
		}

		private string ConvertFromListOrArray(object o)
		{
			TypeConverter converter = Property.GetConverter(this.elementType);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object current in ((IEnumerable)o))
			{
				stringBuilder.Append(converter.ConvertToString(current));
				stringBuilder.Append('↓');
			}
			return stringBuilder.ToString();
		}

		private CodeExpression ConvertArrayToCodeExpression(object o)
		{
			CSharpCodeProvider cSharpCodeProvider = new CSharpCodeProvider();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object current in ((IEnumerable)o))
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.Append(',');
				}
				StringWriter stringWriter = new StringWriter();
				cSharpCodeProvider.GenerateCodeFromExpression(this.ConvertSingleValueToCodeExpression(current), stringWriter, new CodeGeneratorOptions());
				stringBuilder.AppendFormat(stringWriter.ToString(), new object[0]);
				stringWriter.Close();
			}
			return new CodeSnippetExpression(string.Format("new {0} {{ {1} }}", SerializedType.GetFullName(this.type), stringBuilder));
		}

		private CodeExpression ConvertSingleValueToCodeExpression(object o)
		{
			if (Property.IsPrimitive(o.GetType()))
			{
				return new CodePrimitiveExpression(o);
			}
			TypeConverter converter = Property.GetConverter(o.GetType());
			if (!converter.CanConvertTo(typeof(CodeExpression)))
			{
				return this.GetDefaultCodeExpression();
			}
			return converter.ConvertTo(o, typeof(CodeExpression)) as CodeExpression;
		}

		private CodeExpression GetDefaultCodeExpression()
		{
			if (this.type.IsValueType)
			{
				return new CodeObjectCreateExpression(this.type, new CodeExpression[0]);
			}
			return new CodePrimitiveExpression(null);
		}

		private static bool IsPrimitive(Type t)
		{
			return t.IsPrimitive || t == typeof(string);
		}

		public static bool ValidPropertyType(Type type)
		{
			if (typeof(IList).IsAssignableFrom(type))
			{
				if (type.IsArray && type.GetArrayRank() == 1)
				{
					type = type.GetElementType();
				}
				else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
				{
					type = type.GetGenericArguments()[0];
				}
			}
			if (Property.IsPrimitive(type))
			{
				return true;
			}
			TypeConverter converter = Property.GetConverter(type);
			return converter != null && converter.GetType() != typeof(TypeConverter) && converter.CanConvertTo(typeof(string)) && converter.CanConvertFrom(typeof(string)) && converter.CanConvertTo(typeof(CodeExpression));
		}

		public static object ConvertFromString(Type toType, string str)
		{
			object result;
			try
			{
				result = Property.GetConverter(toType).ConvertFromInvariantString(str);
			}
			catch
			{
				result = null;
			}
			return result;
		}

		private static TypeConverter GetConverter(Type t)
		{
			string name = t.Name;
			switch (name)
			{
			case "Vector2":
				return new GenericFloatVarsTypeConverter(t, new string[]
				{
					"x",
					"y"
				});
			case "Vector3":
				return new GenericFloatVarsTypeConverter(t, new string[]
				{
					"x",
					"y",
					"z"
				});
			case "Vector4":
				return new GenericFloatVarsTypeConverter(t, new string[]
				{
					"x",
					"y",
					"z",
					"w"
				});
			case "Color":
				return new GenericFloatVarsTypeConverter(t, new string[]
				{
					"r",
					"g",
					"b",
					"a"
				});
			case "Quaternion":
				return new GenericFloatVarsTypeConverter(t, new string[]
				{
					"x",
					"y",
					"z",
					"w"
				});
			case "Rect":
				return new GenericFloatVarsTypeConverter(t, new string[]
				{
					"x",
					"y",
					"width",
					"height"
				});
			case "AnimationCurve":
				return new AnimationCurveTypeConverter(t);
			}
			if (t.BaseType == typeof(Enum))
			{
				return new EnumTypeConverter(t);
			}
			return TypeDescriptor.GetConverter(t);
		}

		public void ChangeDataType(Type newDataType)
		{
			if (this.type == newDataType)
			{
				return;
			}
			object value = this.value;
			this.type = newDataType;
			this.m_Value = string.Empty;
			this.m_RefValues = null;
			this.m_HaveCachedDeserializedResult = false;
			this.m_CachedLastDeserializedString = string.Empty;
			this.m_CachedLastDeserializedValue = null;
			object obj = Property.ConvertActualValueIfPossible(value, newDataType);
			if (obj == null && newDataType.IsValueType && !this.isGeneric)
			{
				this.value = Property.TryGetDefaultValue(this.type);
			}
			else
			{
				this.value = obj;
			}
		}

		public static object ConvertActualValueIfPossible(object value, Type toType)
		{
			if (value == null)
			{
				return value;
			}
			Type type = value.GetType();
			if (type == toType)
			{
				return value;
			}
			if (Property.ConvertableUnityObjects(type, toType))
			{
				if (type == typeof(GameObject))
				{
					return ((GameObject)value).GetComponent(toType);
				}
				if (toType == typeof(GameObject))
				{
					return ((UnityEngine.Component)value).gameObject;
				}
				return ((UnityEngine.Component)value).GetComponent(toType);
			}
			else
			{
				if (toType.IsAssignableFrom(typeof(Vector3)))
				{
					if (typeof(UnityEngine.Component).IsAssignableFrom(type))
					{
						return ((UnityEngine.Component)value).transform.position;
					}
					if (typeof(GameObject).IsAssignableFrom(type))
					{
						return ((GameObject)value).transform.position;
					}
				}
				if (toType == typeof(string))
				{
					return value.ToString();
				}
				object result;
				try
				{
					result = Convert.ChangeType(value, toType);
				}
				catch (Exception)
				{
					result = null;
				}
				return result;
			}
		}

		public static bool ConvertableUnityObjects(Type t1, Type t2)
		{
			return (typeof(UnityEngine.Component).IsAssignableFrom(t1) || typeof(GameObject).IsAssignableFrom(t1)) && (typeof(UnityEngine.Component).IsAssignableFrom(t2) || typeof(GameObject).IsAssignableFrom(t2));
		}
	}
}
