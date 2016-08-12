using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Unity.PackageManager.IvyInternal
{
	[XmlType("dependencies")]
	public class IvyDependencies : List<IvyDependency>
	{
	}
}
