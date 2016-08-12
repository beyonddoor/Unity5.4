using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace UnityEditor.Graphs
{
	public class GenericFloatVarsTypeConverter : GraphsTypeConverter
	{
		private Type m_Type;

		private string[] m_VarNames;

		public GenericFloatVarsTypeConverter(Type type, params string[] varNames)
		{
			this.m_Type = type;
			this.m_VarNames = varNames;
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(CodeExpression))
			{
				return this.ConvertToCodeExpression(value);
			}
			return this.ConvertToString(value);
		}

		private new object ConvertToString(object target)
		{
			string text = string.Empty;
			foreach (float num in this.GetAllValues(target))
			{
				if (text.Length != 0)
				{
					text += ",";
				}
				text += num.ToString(CultureInfo.InvariantCulture);
			}
			return text;
		}

		private object ConvertToCodeExpression(object target)
		{
			CodePrimitiveExpression[] array = new CodePrimitiveExpression[this.m_VarNames.Length];
			int num = 0;
			foreach (float num2 in this.GetAllValues(target))
			{
				array[num++] = new CodePrimitiveExpression(num2);
			}
			return new CodeObjectCreateExpression(this.m_Type, array);
		}

		private IEnumerable<float> GetAllValues(object target)
		{
			return from varName in this.m_VarNames
			select this.GetVariableValue(varName, target);
		}

		private float GetVariableValue(string varName, object target)
		{
			FieldInfo field = this.m_Type.GetField(varName);
			if (field != null)
			{
				return (float)field.GetValue(target);
			}
			PropertyInfo property = this.m_Type.GetProperty(varName);
			return (float)property.GetValue(target, null);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string[] array = ((string)value).Split(new char[]
			{
				','
			});
			object[] array2 = new object[this.m_VarNames.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = float.Parse(array[i], CultureInfo.InvariantCulture);
			}
			return Activator.CreateInstance(this.m_Type, array2);
		}

		public override bool IsValid(ITypeDescriptorContext context, object value)
		{
			if (value.GetType() == this.m_Type)
			{
				return true;
			}
			if (value.GetType() != typeof(string))
			{
				return false;
			}
			string[] array = ((string)value).Split(new char[]
			{
				','
			});
			float dummy;
			return array.Length == this.m_VarNames.Length && array.All((string t) => float.TryParse(t, NumberStyles.Float, CultureInfo.InvariantCulture, out dummy));
		}
	}
}
