using System;
using System.Xml.Serialization;

namespace Unity.PackageManager.IvyInternal
{
	[XmlType("repository")]
	public class IvyRepository
	{
		[XmlAttribute("name")]
		public string Name;

		[XmlAttribute("url")]
		public string Url;
	}
}
