using System;
using System.Collections.Generic;

namespace Unity.PackageManager.Ivy
{
	public class IvyDependencies : List<IvyDependency>
	{
		public IvyDependencies Clone()
		{
			return Cloner.CloneObject<IvyDependencies>(this);
		}
	}
}
