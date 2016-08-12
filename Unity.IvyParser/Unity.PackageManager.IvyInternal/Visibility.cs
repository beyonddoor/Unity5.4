using System;
using System.Xml.Serialization;

namespace Unity.PackageManager.IvyInternal
{
	[XmlType("visibility")]
	public enum Visibility
	{
		[XmlEnum("private")]
		Private,
		[XmlEnum("public")]
		Public
	}
}
