using System;

namespace Unity.DataContract
{
	public class PackageFileData
	{
		public PackageFileType type;

		public string url;

		public string guid;

		public PackageFileData()
		{
		}

		public PackageFileData(PackageFileType type, string url)
		{
			this.type = type;
			this.url = url;
		}

		public PackageFileData(PackageFileType type, string url, string guid) : this(type, url)
		{
			this.guid = guid;
		}
	}
}
