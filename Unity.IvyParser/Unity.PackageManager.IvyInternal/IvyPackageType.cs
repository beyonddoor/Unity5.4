using System;
using System.Xml.Serialization;

namespace Unity.PackageManager.IvyInternal
{
	[XmlType("packageType")]
	public enum IvyPackageType
	{
		[XmlEnum("Unknown")]
		Unknown,
		[XmlEnum("PlaybackEngine")]
		PlaybackEngine,
		[XmlEnum("UnityExtension")]
		UnityExtension,
		[XmlEnum("PackageManager")]
		PackageManager
	}
}
