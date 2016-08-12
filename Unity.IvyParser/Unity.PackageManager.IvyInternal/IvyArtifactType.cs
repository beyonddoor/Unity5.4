using System;
using System.Xml.Serialization;

namespace Unity.PackageManager.IvyInternal
{
	[XmlType("type")]
	public enum IvyArtifactType
	{
		[XmlEnum("none")]
		None,
		[XmlEnum("package")]
		Package,
		[XmlEnum("ivy")]
		Ivy,
		[XmlEnum("dll")]
		Dll,
		[XmlEnum("notes")]
		ReleaseNotes,
		[XmlEnum("debug")]
		DebugSymbols
	}
}
