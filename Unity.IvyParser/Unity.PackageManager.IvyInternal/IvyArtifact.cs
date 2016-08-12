using System;
using System.Xml.Serialization;

namespace Unity.PackageManager.IvyInternal
{
	[XmlType("artifact")]
	public class IvyArtifact
	{
		[XmlAttribute("name")]
		public string Name;

		[XmlAttribute("type")]
		public IvyArtifactType Type;

		[XmlAttribute("ext")]
		public string Extension;

		[XmlAttribute("url")]
		public string Url;

		[XmlAttribute("guid", Namespace = "http://ant.apache.org/ivy/extra")]
		public string Guid;
	}
}
