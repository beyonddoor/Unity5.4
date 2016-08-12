using System;
using System.Xml.Serialization;

namespace Unity.PackageManager.IvyInternal
{
	[XmlType("ivy-repository")]
	public class ModuleRepository
	{
		[XmlElement("ivy-module")]
		public IvyModules Modules
		{
			get;
			set;
		}
	}
}
