using System;
using System.Xml.Serialization;

namespace Unity.PackageManager.IvyInternal
{
	[XmlType("dependency")]
	public class IvyDependency
	{
		[XmlAttribute("org")]
		public string Organisation;

		[XmlAttribute("name")]
		public string Name;

		[XmlAttribute("branch")]
		public string Branch;

		[XmlAttribute("rev")]
		public string Revision;

		[XmlAttribute("revConstraint")]
		public string RevisionConstraint;

		[XmlAttribute("force")]
		public bool Force;
	}
}
