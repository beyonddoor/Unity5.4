using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Unity.PackageManager.IvyInternal
{
	[XmlType("publications")]
	public class IvyArtifacts : List<IvyArtifact>
	{
	}
}
