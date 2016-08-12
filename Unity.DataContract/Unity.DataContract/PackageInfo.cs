using System;
using System.Collections.Generic;

namespace Unity.DataContract
{
	public class PackageInfo
	{
		public string organisation;

		public string name;

		public PackageVersion version;

		public PackageVersion unityVersion;

		public string basePath;

		public PackageType type;

		public string description;

		public string releaseNotes;

		public bool loaded;

		private Dictionary<string, PackageFileData> m_FileDict;

		public Dictionary<string, PackageFileData> files
		{
			get
			{
				return this.m_FileDict;
			}
			set
			{
				this.m_FileDict = value;
			}
		}

		public string packageName
		{
			get
			{
				return string.Format("{0}.{1}", this.organisation, this.name);
			}
		}

		public override string ToString()
		{
			return string.Format("{0} {1} ({2}) v{3} for Unity v{4}", new object[]
			{
				this.organisation,
				this.name,
				this.type,
				(!(this.version != null)) ? null : this.version.text,
				(!(this.unityVersion != null)) ? null : this.basePath
			});
		}

		public override int GetHashCode()
		{
			int num = 17;
			num = num * 23 + this.organisation.GetHashCode();
			num = num * 23 + this.name.GetHashCode();
			num = num * 23 + this.type.GetHashCode();
			num = num * 23 + this.version.GetHashCode();
			return num * 23 + this.unityVersion.GetHashCode();
		}

		public override bool Equals(object other)
		{
			return this == other as PackageInfo;
		}

		public static bool operator ==(PackageInfo a, PackageInfo z)
		{
			return (a == null && z == null) || (a != null && z != null && a.GetHashCode() == z.GetHashCode());
		}

		public static bool operator !=(PackageInfo a, PackageInfo z)
		{
			return !(a == z);
		}
	}
}
