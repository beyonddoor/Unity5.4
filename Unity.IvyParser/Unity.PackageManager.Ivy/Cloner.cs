using System;
using System.Collections;
using System.Reflection;
using Unity.DataContract;

namespace Unity.PackageManager.Ivy
{
	public static class Cloner
	{
		private static BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;

		public static ToType CloneObject<ToType>(object origin) where ToType : new()
		{
			Type type = origin.GetType();
			ToType toType = (default(ToType) == null) ? Activator.CreateInstance<ToType>() : default(ToType);
			Type type2 = toType.GetType();
			Cloner.CloneProperties(origin, type, toType, type2);
			Cloner.CloneFields(origin, type, toType, type2);
			return toType;
		}

		public static object CloneObject(object origin, Type targetType)
		{
			Type type = origin.GetType();
			object obj = null;
			ConstructorInfo constructorInfo = null;
			try
			{
				obj = Activator.CreateInstance(targetType);
			}
			catch
			{
			}
			if (obj == null && type != typeof(string))
			{
				constructorInfo = targetType.GetConstructor(new Type[]
				{
					type
				});
				if (constructorInfo != null)
				{
					obj = constructorInfo.Invoke(new object[]
					{
						origin
					});
				}
				if (obj == null)
				{
					constructorInfo = targetType.GetConstructor(new Type[]
					{
						typeof(string)
					});
				}
				if (constructorInfo != null)
				{
					obj = constructorInfo.Invoke(new object[]
					{
						origin.ToString()
					});
				}
			}
			if (obj == null)
			{
				return null;
			}
			if (constructorInfo == null)
			{
				Cloner.CloneProperties(origin, type, obj, targetType);
				Cloner.CloneFields(origin, type, obj, targetType);
			}
			return obj;
		}

		private static void CloneFields(object origin, Type originType, object target, Type targetType)
		{
			FieldInfo[] fields = originType.GetFields(Cloner.bindingFlags);
			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo fieldInfo = fields[i];
				object value = fieldInfo.GetValue(origin);
				Type fieldType = fieldInfo.FieldType;
				FieldInfo field = targetType.GetField(fieldInfo.Name, Cloner.bindingFlags);
				if (field != null)
				{
					Type fieldType2 = field.FieldType;
					Cloner.CloneThing(field, target, value, fieldType, fieldType2);
				}
				else
				{
					PropertyInfo property = targetType.GetProperty(fieldInfo.Name, Cloner.bindingFlags);
					if (property != null)
					{
						Type propertyType = property.PropertyType;
						Cloner.CloneThing(property, target, value, fieldType, propertyType);
					}
				}
			}
		}

		private static void CloneProperties(object origin, Type originType, object target, Type targetType)
		{
			PropertyInfo[] properties = originType.GetProperties(Cloner.bindingFlags);
			for (int i = 0; i < properties.Length; i++)
			{
				PropertyInfo propertyInfo = properties[i];
				object value = propertyInfo.GetValue(origin);
				Type propertyType = propertyInfo.PropertyType;
				PropertyInfo property = targetType.GetProperty(propertyInfo.Name, Cloner.bindingFlags);
				if (property != null)
				{
					Type propertyType2 = property.PropertyType;
					Cloner.CloneThing(property, target, value, propertyType, propertyType2);
				}
				else
				{
					FieldInfo field = targetType.GetField(propertyInfo.Name, Cloner.bindingFlags);
					if (field != null)
					{
						Type fieldType = field.FieldType;
						Cloner.CloneThing(field, target, value, propertyType, fieldType);
					}
				}
			}
		}

		private static void CloneThing(MemberInfo thing, object target, object originValue, Type originValueType, Type targetValueType)
		{
			if ((!thing.CanWrite() || (originValue != null && !targetValueType.IsAssignableFrom(originValueType))) && typeof(IList).IsAssignableFrom(originValueType))
			{
				IList list = thing.GetValue(target) as IList;
				if (list == null && thing.CanWrite())
				{
					list = (Activator.CreateInstance(targetValueType) as IList);
					thing.SetValue(target, list);
				}
				if (list != null)
				{
					IList list2 = originValue as IList;
					foreach (object current in list2)
					{
						if (current != null)
						{
							object value = Cloner.CloneObject(current, Cloner.GetElementType(list.GetType()));
							list.Add(value);
						}
					}
					return;
				}
			}
			if (!thing.CanWrite())
			{
				return;
			}
			if (originValue != null && !targetValueType.IsAssignableFrom(originValueType))
			{
				if (targetValueType.IsAssignableFrom(typeof(Uri)) && originValueType.IsAssignableFrom(typeof(string)))
				{
					Uri value2 = null;
					if (Uri.TryCreate((string)originValue, UriKind.RelativeOrAbsolute, out value2))
					{
						thing.SetValue(target, value2);
					}
				}
				else if (targetValueType.IsAssignableFrom(typeof(string)) && originValueType.IsAssignableFrom(typeof(Uri)))
				{
					string value3 = string.Empty;
					if (originValue != null)
					{
						value3 = originValue.ToString();
					}
					thing.SetValue(target, value3);
				}
				else if (targetValueType.IsAssignableFrom(typeof(PackageVersion)) && originValueType.IsAssignableFrom(typeof(string)))
				{
					try
					{
						PackageVersion value4 = new PackageVersion((string)originValue);
						thing.SetValue(target, value4);
					}
					catch
					{
					}
				}
				else if (targetValueType.IsAssignableFrom(typeof(string)) && originValueType.IsAssignableFrom(typeof(PackageVersion)))
				{
					string value5 = string.Empty;
					if (originValue != null)
					{
						value5 = originValue.ToString();
					}
					thing.SetValue(target, value5);
				}
				else if (targetValueType.IsClass)
				{
					object obj = Cloner.CloneObject(originValue, targetValueType);
					if (obj == null)
					{
						obj = thing.GetValue(target);
					}
					if (obj != null)
					{
						thing.SetValue(target, obj);
					}
					else
					{
						thing.SetValue(target, originValue);
					}
				}
				else if (targetValueType.IsEnum)
				{
					thing.SetValue(target, (int)originValue);
				}
			}
			else if (originValue == null)
			{
				thing.SetValue(target, originValue);
			}
			else if (targetValueType.IsClass && targetValueType != typeof(string))
			{
				object obj2 = Cloner.CloneObject(originValue, targetValueType);
				if (obj2 == null)
				{
					obj2 = thing.GetValue(target);
				}
				if (obj2 != null)
				{
					thing.SetValue(target, obj2);
				}
				else
				{
					thing.SetValue(target, originValue);
				}
			}
			else
			{
				thing.SetValue(target, originValue);
			}
		}

		private static Type GetElementType(Type type)
		{
			Type type2 = type.GetElementType();
			if (type2 == null)
			{
				if (type.IsGenericType)
				{
					Type[] genericArguments = type.GetGenericArguments();
					if (genericArguments.Length > 0)
					{
						type2 = genericArguments[0];
					}
				}
				else if (type.BaseType != null)
				{
					type2 = Cloner.GetElementType(type.BaseType);
				}
			}
			return type2;
		}
	}
}
