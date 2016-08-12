using System;
using System.Collections.Generic;

namespace Unity.PackageManager.Ivy
{
	public class IvyArtifacts : List<IvyArtifact>
	{
		public IvyArtifacts Clone()
		{
			return Cloner.CloneObject<IvyArtifacts>(this);
		}
	}
}
