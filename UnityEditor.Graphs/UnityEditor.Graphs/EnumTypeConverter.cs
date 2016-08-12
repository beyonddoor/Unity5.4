using System;
using System.CodeDom;
using System.ComponentModel;
using System.Globalization;

namespace UnityEditor.Graphs
{
	public class EnumTypeConverter : GraphsTypeConverter
	{
		private Type m_Type;

		private EnumConverter m_Converter;

		public EnumTypeConverter(Type type)
		{
			this.m_Type = type;
			this.m_Converter = new EnumConverter(type);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(CodeExpression))
			{
				return new CodeSnippetExpression(this.m_Type.FullName.Replace('+', '.') + "." + value.ToString());
			}
			return this.m_Converter.ConvertTo(context, culture, value, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return this.m_Converter.ConvertFrom(context, culture, value);
		}

		public override bool IsValid(ITypeDescriptorContext context, object value)
		{
			return this.m_Converter.IsValid(context, value);
		}
	}
}
