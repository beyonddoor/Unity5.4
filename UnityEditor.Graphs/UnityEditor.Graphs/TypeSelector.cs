using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UnityEditor.Graphs
{
	public class TypeSelector
	{
		public enum TypeKind
		{
			Simple,
			List,
			Array
		}

		private static string[] s_TypeKindNames = new string[]
		{
			"Simple",
			"List",
			"Array"
		};

		private static string[] s_TypeNames = new string[]
		{
			"string (System.String)",
			"integer (System.Int32)",
			"float (System.Single)",
			"bool (System.Boolean)",
			"byte (System.Byte)",
			"char (System.Char)",
			"GameObject (UnityEngine.GameObject)",
			"Component (UnityEngine.Component)",
			"Material (UnityEngine.Material)",
			"Vector2 (UnityEngine.Vector2)",
			"Vector3 (UnityEngine.Vector3)",
			"Vector4 (UnityEngine.Vector4)",
			"Color (UnityEngine.Color)",
			"Quaternion (UnityEngine.Quaternion)",
			"Rectangle (UnityEngine.Rect)",
			"AnimationCurve (UnityEngine.AnimationCurve)",
			string.Empty,
			"Other..."
		};

		private static string[] s_ComponentTypeNames = new string[]
		{
			"Transform (UnityEngine.Transform)",
			"Rigidbody (UnityEngine.Rigidbody)",
			"Camera (UnityEngine.Camera)",
			"Light (UnityEngine.Light)",
			"Animation (UnityEngine.Animation)",
			"ConstantForce (UnityEngine.ConstantForce)",
			"Renderer (UnityEngine.Renderer)",
			"AudioSource (UnityEngine.AudioSource)",
			"GUIText (UnityEngine.GUIText)",
			"NetworkView (UnityEngine.NetworkView)",
			"GUITexture (UnityEngine.GUITexture)",
			"Collider (UnityEngine.Collider)",
			"HingeJoint (UnityEngine.HingeJoint)",
			"ParticleEmitter (UnityEngine.ParticleEmitter)",
			string.Empty,
			"Other..."
		};

		[SerializeField]
		private bool m_EditingOther;

		[SerializeField]
		private string m_OtherTypeName;

		[SerializeField]
		private string m_ShownError;

		[SerializeField]
		private string[] m_TypeNames;

		[SerializeField]
		private bool m_OnlyComponents;

		public Type selectedType = typeof(DummyNullType);

		public TypeSelector.TypeKind selectedTypeKind;

		private static Texture2D m_warningIcon;

		private static Texture2D warningIcon
		{
			get
			{
				Texture2D arg_1C_0;
				if ((arg_1C_0 = TypeSelector.m_warningIcon) == null)
				{
					arg_1C_0 = (TypeSelector.m_warningIcon = EditorGUIUtility.LoadIcon("console.warnicon"));
				}
				return arg_1C_0;
			}
		}

		public TypeSelector()
		{
			this.Init(TypeSelector.GenericTypeSelectorCommonTypes(), false);
		}

		public TypeSelector(string[] types)
		{
			this.Init(types, false);
		}

		public TypeSelector(bool onlyComponents)
		{
			this.Init((!onlyComponents) ? TypeSelector.GenericTypeSelectorCommonTypes() : TypeSelector.s_ComponentTypeNames, onlyComponents);
		}

		public bool DoGUI()
		{
			return (this.selectedType != typeof(DummyNullType) && this.selectedType != null) ? this.DoTypeClear() : this.DoTypeSelector();
		}

		private void Init(string[] types, bool onlyComponents)
		{
			this.m_TypeNames = types;
			this.m_OnlyComponents = onlyComponents;
		}

		public bool DoTypeKindGUI()
		{
			bool changed = GUI.changed;
			GUI.changed = false;
			this.selectedTypeKind = (TypeSelector.TypeKind)EditorGUILayout.Popup("Kind", (int)this.selectedTypeKind, TypeSelector.s_TypeKindNames, new GUILayoutOption[0]);
			bool changed2 = GUI.changed;
			GUI.enabled |= changed;
			return changed2;
		}

		public static Type GetFinalType(TypeSelector.TypeKind typeKind, Type baseType)
		{
			switch (typeKind)
			{
			case TypeSelector.TypeKind.Simple:
				return baseType;
			case TypeSelector.TypeKind.List:
				return typeof(List<>).MakeGenericType(new Type[]
				{
					baseType
				});
			case TypeSelector.TypeKind.Array:
				return baseType.MakeArrayType();
			default:
				throw new ArgumentException("Internal error: got weird type kind");
			}
		}

		public static Type GetBaseType(TypeSelector.TypeKind typeKind, Type finalType)
		{
			if (finalType == null)
			{
				return null;
			}
			switch (typeKind)
			{
			case TypeSelector.TypeKind.Simple:
				return finalType;
			case TypeSelector.TypeKind.List:
				return finalType.GetGenericArguments()[0];
			case TypeSelector.TypeKind.Array:
				return finalType.GetElementType();
			default:
				throw new ArgumentException("Internal error: got weird type kind");
			}
		}

		public static TypeSelector.TypeKind GetTypeKind(Type dataType)
		{
			if (dataType == null)
			{
				return TypeSelector.TypeKind.Simple;
			}
			if (dataType.IsArray)
			{
				return TypeSelector.TypeKind.Array;
			}
			if (dataType.IsGenericType)
			{
				return TypeSelector.TypeKind.List;
			}
			return TypeSelector.TypeKind.Simple;
		}

		private bool DoTypeClear()
		{
			bool result = false;
			EditorGUILayout.LabelField("Type", TypeSelector.DotNetTypeNiceName(this.selectedType), new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Reset", EditorStyles.miniButton, new GUILayoutOption[0]))
			{
				this.selectedType = typeof(DummyNullType);
				result = true;
			}
			GUILayout.EndHorizontal();
			return result;
		}

		private bool DoTypeSelector()
		{
			if (this.m_EditingOther)
			{
				return this.DoOtherEditing();
			}
			int num = EditorGUILayout.Popup("Choose a type", -1, this.m_TypeNames, new GUILayoutOption[0]);
			if (num == -1)
			{
				return false;
			}
			string text = this.m_TypeNames[num];
			if (text == "Other...")
			{
				this.m_EditingOther = true;
				this.m_OtherTypeName = string.Empty;
				return false;
			}
			int num2 = text.IndexOf('(');
			if (num2 != -1)
			{
				text = text.Substring(num2 + 1, text.IndexOf(')') - num2 - 1);
			}
			this.selectedType = TypeSelector.FindType(text);
			return true;
		}

		private static Type FindType(string typeName)
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				Assembly assembly = assemblies[i];
				Type type = assembly.GetType(typeName);
				if (type != null)
				{
					return type;
				}
			}
			throw new ArgumentException("Type '" + typeName + "' was not found.");
		}

		private bool DoOtherEditing()
		{
			bool result = false;
			this.m_OtherTypeName = EditorGUILayout.TextField("Full type Name", this.m_OtherTypeName, new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Set", EditorStyles.miniButton, new GUILayoutOption[0]))
			{
				try
				{
					this.selectedType = TypeSelector.FindType(this.m_OtherTypeName);
					if (!this.m_OnlyComponents || typeof(Component).IsAssignableFrom(this.selectedType))
					{
						this.m_OtherTypeName = string.Empty;
						this.m_EditingOther = false;
						this.m_ShownError = string.Empty;
						result = true;
					}
					this.m_ShownError = "Type must be derived from 'Component'.";
				}
				catch
				{
					this.m_ShownError = "Could not find a type '" + this.m_OtherTypeName + "'";
				}
			}
			if (GUILayout.Button("Cancel", EditorStyles.miniButton, new GUILayoutOption[0]))
			{
				this.m_OtherTypeName = string.Empty;
				this.m_EditingOther = false;
				this.m_ShownError = string.Empty;
			}
			GUILayout.EndHorizontal();
			if (!string.IsNullOrEmpty(this.m_ShownError))
			{
				TypeSelector.ShowError(this.m_ShownError);
			}
			return result;
		}

		private static void ShowError(string shownError)
		{
			GUIContent content = new GUIContent(shownError)
			{
				image = TypeSelector.warningIcon
			};
			GUILayout.Space(5f);
			GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
			GUILayout.Label(content, EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
			GUILayout.EndVertical();
		}

		private static string[] GenericTypeSelectorCommonTypes()
		{
			return TypeSelector.s_TypeNames;
		}

		public static string DotNetTypeNiceName(Type t)
		{
			if (t == null)
			{
				return string.Empty;
			}
			string[] array = TypeSelector.s_TypeNames;
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if (text.IndexOf(t.FullName) != -1)
				{
					return text;
				}
			}
			return t.FullName;
		}
	}
}
