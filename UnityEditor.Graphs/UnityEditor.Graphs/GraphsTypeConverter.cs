using System;
using System.CodeDom;
using System.ComponentModel;

namespace UnityEditor.Graphs
{
	public class GraphsTypeConverter : TypeConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(string) || destinationType.IsAssignableFrom(typeof(CodeExpression));
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}
	}
}
