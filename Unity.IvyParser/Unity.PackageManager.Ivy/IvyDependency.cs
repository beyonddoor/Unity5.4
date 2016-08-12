using System;

namespace Unity.PackageManager.Ivy
{
	public class IvyDependency
	{
		public string Organisation;

		public string Name;

		public string Branch;

		public string Revision;

		public string RevisionConstraint;

		public bool Force;

		public IvyDependency Clone()
		{
			return Cloner.CloneObject<IvyDependency>(this);
		}
	}
}
