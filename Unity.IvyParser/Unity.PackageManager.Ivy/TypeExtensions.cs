using System;
using System.Reflection;

namespace Unity.PackageManager.Ivy
{
	internal static class TypeExtensions
	{
		public static bool CanWrite(this MemberInfo member)
		{
			return !(member is PropertyInfo) || ((PropertyInfo)member).CanWrite;
		}

		public static void SetValue(this MemberInfo member, object target, object value)
		{
			if (member is PropertyInfo)
			{
				((PropertyInfo)member).GetSetMethod().Invoke(target, new object[]
				{
					value
				});
			}
			else
			{
				((FieldInfo)member).SetValue(target, value);
			}
		}

		public static object GetValue(this MemberInfo member, object target)
		{
			if (member is PropertyInfo)
			{
				return ((PropertyInfo)member).GetGetMethod().Invoke(target, null);
			}
			return ((FieldInfo)member).GetValue(target);
		}
	}
}
