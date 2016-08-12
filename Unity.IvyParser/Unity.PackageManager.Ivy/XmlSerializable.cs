using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Unity.DataContract;
using Unity.PackageManager.IvyInternal;

namespace Unity.PackageManager.Ivy
{
	public class XmlSerializable : IXmlSerializable
	{
		private static readonly PackageVersion compatibilityVersion = new PackageVersion("4.5.0b2");

		private static Dictionary<string, KeyValuePair<Func<object, object>, Action<object, object>>> attributeMappings = new Dictionary<string, KeyValuePair<Func<object, object>, Action<object, object>>>();

		private static List<string> processed = new List<string>();

		private static Dictionary<string, Type> typeMappings = new Dictionary<string, Type>();

		private static object cacheLock = new object();

		private static Dictionary<Type, XmlSerializer> serializerCache = new Dictionary<Type, XmlSerializer>();

		public virtual XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			this.ProcessTypes();
			this.ProcessType();
			bool isEmptyElement = reader.IsEmptyElement;
			reader.MoveToContent();
			if (reader.HasAttributes)
			{
				string str = base.GetType().FullName + ".";
				for (int i = 0; i < reader.AttributeCount; i++)
				{
					reader.MoveToAttribute(i);
					string localName = reader.LocalName;
					string text = reader.Value;
					if (localName == "packageType")
					{
						if (text == "PlaybackEngines")
						{
							text = "PlaybackEngine";
						}
						if (text == "EditorExtension")
						{
							text = "UnityExtension";
						}
					}
					KeyValuePair<Func<object, object>, Action<object, object>> keyValuePair;
					if (XmlSerializable.attributeMappings.TryGetValue(str + localName, out keyValuePair))
					{
						keyValuePair.Value(this, text);
					}
				}
			}
			if (reader.IsStartElement() && !reader.IsEmptyElement)
			{
				reader.ReadStartElement();
			}
			while (reader.IsStartElement() && !isEmptyElement)
			{
				string localName2 = reader.LocalName;
				string text2 = localName2;
				Type type;
				if (!XmlSerializable.typeMappings.TryGetValue(text2, out type))
				{
					text2 = base.GetType().Name + "." + text2;
					if (!XmlSerializable.typeMappings.TryGetValue(text2, out type))
					{
						reader.Skip();
						continue;
					}
				}
				XmlSerializer serializer = XmlSerializable.GetSerializer(type);
				object arg = serializer.Deserialize(reader);
				text2 = base.GetType().FullName + "." + localName2;
				KeyValuePair<Func<object, object>, Action<object, object>> keyValuePair2;
				if (XmlSerializable.attributeMappings.TryGetValue(text2, out keyValuePair2))
				{
					keyValuePair2.Value(this, arg);
				}
			}
			if (!isEmptyElement)
			{
				reader.ReadEndElement();
			}
			else
			{
				reader.ReadStartElement();
			}
		}

		public void WriteXml(XmlWriter writer)
		{
			this.ProcessTypes();
			this.ProcessType();
			Type type = base.GetType();
			string str = type.FullName + ".";
			MemberInfo[] members = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			List<string> list = new List<string>();
			Dictionary<string, XmlAttributeAttribute> dictionary = new Dictionary<string, XmlAttributeAttribute>();
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
			MemberInfo[] array = members;
			for (int i = 0; i < array.Length; i++)
			{
				MemberInfo memberInfo = array[i];
				if (memberInfo.MemberType == MemberTypes.Field || memberInfo.MemberType == MemberTypes.Property)
				{
					object[] customAttributes = memberInfo.GetCustomAttributes(false);
					object[] array2 = customAttributes;
					for (int j = 0; j < array2.Length; j++)
					{
						object obj = array2[j];
						if (!(obj is XmlIgnoreAttribute))
						{
							if (obj is XmlElementAttribute)
							{
								XmlElementAttribute xmlElementAttribute = (XmlElementAttribute)obj;
								string elementName = xmlElementAttribute.ElementName;
								if (xmlElementAttribute.Order > 0)
								{
									if (list.Count < xmlElementAttribute.Order)
									{
										list.AddRange(new string[xmlElementAttribute.Order - list.Count]);
									}
									list[xmlElementAttribute.Order - 1] = str + elementName;
								}
								else
								{
									list.Add(str + elementName);
								}
								dictionary2.Add(str + elementName, elementName);
								break;
							}
							if (obj is XmlAttributeAttribute)
							{
								XmlAttributeAttribute xmlAttributeAttribute = (XmlAttributeAttribute)obj;
								dictionary.Add(str + xmlAttributeAttribute.AttributeName, xmlAttributeAttribute);
								object[] customAttributes2 = memberInfo.GetCustomAttributes(typeof(DefaultValueAttribute), false);
								if (customAttributes2.Length > 0)
								{
									dictionary3.Add(str + xmlAttributeAttribute.AttributeName, ((DefaultValueAttribute)customAttributes2[0]).Value);
								}
								break;
							}
						}
					}
				}
			}
			foreach (KeyValuePair<string, XmlAttributeAttribute> current in dictionary)
			{
				if (XmlSerializable.attributeMappings.ContainsKey(current.Key))
				{
					object obj2 = XmlSerializable.attributeMappings[current.Key].Key(this);
					if (obj2 != null)
					{
						string text = obj2.ToString();
						object obj3 = null;
						if (!dictionary3.TryGetValue(current.Key, out obj3) || obj2 != obj3)
						{
							if (obj2 is bool)
							{
								if (!(bool)obj2)
								{
									continue;
								}
								text = text.ToLower();
							}
							if (obj2 is IvyPackageType && this is Unity.PackageManager.IvyInternal.IvyInfo)
							{
								PackageVersion a = new PackageVersion(((Unity.PackageManager.IvyInternal.IvyInfo)this).UnityVersion);
								if (a <= XmlSerializable.compatibilityVersion)
								{
									IvyPackageType ivyPackageType = (IvyPackageType)((int)obj2);
									if (ivyPackageType != IvyPackageType.PlaybackEngine)
									{
										if (ivyPackageType == IvyPackageType.UnityExtension)
										{
											text = "EditorExtension";
										}
									}
									else
									{
										text = "PlaybackEngines";
									}
								}
							}
							writer.WriteAttributeString((current.Value.Namespace == null) ? string.Empty : "e", current.Value.AttributeName, current.Value.Namespace, text);
						}
					}
				}
			}
			foreach (string current2 in list)
			{
				if (XmlSerializable.attributeMappings.ContainsKey(current2))
				{
					object obj4 = XmlSerializable.attributeMappings[current2].Key(this);
					if (obj4 != null)
					{
						if (obj4 is List<Unity.PackageManager.IvyInternal.IvyRepository>)
						{
							IList list2 = obj4 as IList;
							foreach (object current3 in list2)
							{
								XmlSerializer serializer = XmlSerializable.GetSerializer(current3.GetType());
								serializer.Serialize(writer, current3, IvyParser.Namespaces);
							}
						}
						else
						{
							XmlSerializer serializer2 = XmlSerializable.GetSerializer(obj4.GetType());
							serializer2.Serialize(writer, obj4, IvyParser.Namespaces);
						}
					}
				}
			}
		}

		private void ProcessTypes()
		{
			Dictionary<string, Type> obj = XmlSerializable.typeMappings;
			lock (obj)
			{
				if (XmlSerializable.typeMappings.Count <= 0)
				{
					Type[] types = Assembly.GetAssembly(base.GetType()).GetTypes();
					Type[] array = types;
					for (int i = 0; i < array.Length; i++)
					{
						Type type = array[i];
						if (!(type.Namespace != "Unity.PackageManager.IvyInternal"))
						{
							object[] customAttributes = type.GetCustomAttributes(false);
							object[] array2 = customAttributes;
							int j = 0;
							while (j < array2.Length)
							{
								object obj2 = array2[j];
								string text;
								string @namespace;
								if (obj2 is XmlTypeAttribute)
								{
									text = ((XmlTypeAttribute)obj2).TypeName;
									@namespace = ((XmlTypeAttribute)obj2).Namespace;
									goto IL_DC;
								}
								if (obj2 is XmlRootAttribute)
								{
									text = ((XmlRootAttribute)obj2).ElementName;
									@namespace = ((XmlRootAttribute)obj2).Namespace;
									goto IL_DC;
								}
								IL_105:
								j++;
								continue;
								IL_DC:
								if (!string.IsNullOrEmpty(@namespace))
								{
									text = @namespace + "." + text;
								}
								XmlSerializable.typeMappings.Add(text, type);
								goto IL_105;
							}
						}
					}
				}
			}
		}

		private void ProcessType()
		{
			Type type = base.GetType();
			List<string> obj = XmlSerializable.processed;
			lock (obj)
			{
				if (!XmlSerializable.processed.Contains(type.FullName))
				{
					string str = type.FullName + ".";
					MemberInfo[] members = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					MemberInfo[] array = members;
					MemberInfo memberInfo;
					for (int i = 0; i < array.Length; i++)
					{
						memberInfo = array[i];
						if (memberInfo.MemberType == MemberTypes.Field || memberInfo.MemberType == MemberTypes.Property)
						{
							string text = null;
							object[] customAttributes = memberInfo.GetCustomAttributes(false);
							object[] array2 = customAttributes;
							for (int j = 0; j < array2.Length; j++)
							{
								object obj2 = array2[j];
								if (!(obj2 is XmlIgnoreAttribute))
								{
									if (obj2 is XmlElementAttribute)
									{
										text = ((XmlElementAttribute)obj2).ElementName;
										break;
									}
									if (obj2 is XmlAttributeAttribute)
									{
										text = ((XmlAttributeAttribute)obj2).AttributeName;
										break;
									}
								}
							}
							if (text != null)
							{
								text = str + text;
								if (!XmlSerializable.attributeMappings.ContainsKey(text))
								{
									if (memberInfo.MemberType == MemberTypes.Field)
									{
										FieldInfo field = type.GetField(memberInfo.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
										XmlSerializable.attributeMappings.Add(text, new KeyValuePair<Func<object, object>, Action<object, object>>((object who) => field.GetValue(who), delegate(object who, object what)
										{
											TypeConverter converter = TypeDescriptor.GetConverter(field.FieldType);
											object obj3 = what;
											if (obj3 != null && converter.CanConvertFrom(obj3.GetType()))
											{
												obj3 = converter.ConvertFrom(what);
											}
											field.SetValue(who, obj3);
										}));
									}
									else if (memberInfo.MemberType == MemberTypes.Property)
									{
										PropertyInfo prop = type.GetProperty(memberInfo.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
										XmlSerializable.attributeMappings.Add(text, new KeyValuePair<Func<object, object>, Action<object, object>>((object who) => prop.GetGetMethod(true).Invoke(who, null), delegate(object who, object what)
										{
											if (!(what is IList) && typeof(IList).IsAssignableFrom(prop.PropertyType))
											{
												if (what == null)
												{
													return;
												}
												IList list = (IList)prop.GetGetMethod(true).Invoke(who, null);
												if (list == null && memberInfo.Name.StartsWith("xml"))
												{
													PropertyInfo property = type.GetProperty(memberInfo.Name.Substring(3), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
													if (property != null)
													{
														list = (IList)property.GetGetMethod(true).Invoke(who, null);
													}
												}
												if (list != null)
												{
													list.Add(what);
												}
											}
											else
											{
												prop.GetSetMethod(true).Invoke(who, new object[]
												{
													what
												});
											}
										}));
									}
								}
							}
						}
					}
					XmlSerializable.processed.Add(type.FullName);
				}
			}
		}

		internal static XmlSerializer GetSerializer(Type type)
		{
			object obj = XmlSerializable.cacheLock;
			XmlSerializer result;
			lock (obj)
			{
				if (!XmlSerializable.serializerCache.ContainsKey(type))
				{
					XmlSerializable.serializerCache.Add(type, new XmlSerializer(type));
				}
				result = XmlSerializable.serializerCache[type];
			}
			return result;
		}
	}
}
