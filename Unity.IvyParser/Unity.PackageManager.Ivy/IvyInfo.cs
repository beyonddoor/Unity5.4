using System;
using System.Collections.Generic;
using Unity.DataContract;

namespace Unity.PackageManager.Ivy
{
	public class IvyInfo
	{
		public PackageVersion Version;

		public string Organisation;

		public string Module;

		public string Revision;

		public string Branch;

		public string Status;

		public string Publication;

		public PackageType Type;

		public PackageVersion UnityVersion;

		public string Title;

		public string Description;

		public bool Published;

		internal List<IvyRepository> repositories;

		public List<IvyRepository> Repositories
		{
			get
			{
				if (this.repositories == null)
				{
					this.repositories = new List<IvyRepository>();
				}
				return this.repositories;
			}
		}

		public string FullName
		{
			get
			{
				return string.Format("{0}.{1}.{2}", this.Organisation, this.Module, this.Version);
			}
		}

		public IvyInfo Clone()
		{
			return Cloner.CloneObject<IvyInfo>(this);
		}
	}
}
