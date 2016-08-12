using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Unity.PackageManager.IvyInternal
{
	[XmlType("modules")]
	public class IvyModules : List<IvyModule>
	{
	}
}
