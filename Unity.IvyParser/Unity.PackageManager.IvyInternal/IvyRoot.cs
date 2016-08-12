using System;
using System.Xml.Serialization;
using Unity.PackageManager.Ivy;

namespace Unity.PackageManager.IvyInternal
{
	[XmlRoot("root")]
	public class IvyRoot : XmlSerializable
	{
		[XmlElement("ivy-repository", Namespace = "http://ant.apache.org/ivy/extra")]
		public ModuleRepository Repository;

		[XmlElement("ivy-module")]
		public IvyModule Module;
	}
}
