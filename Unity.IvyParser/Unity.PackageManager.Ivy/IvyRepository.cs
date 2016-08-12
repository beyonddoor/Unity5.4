using System;

namespace Unity.PackageManager.Ivy
{
	public class IvyRepository
	{
		public string Name;

		public Uri Url;

		public string Pattern = "packages/";

		public IvyRepository()
		{
		}

		public IvyRepository(string name, Uri url)
		{
			this.Name = name;
			this.Url = url;
		}

		public Uri BuildFullUrl(string file)
		{
			if (this.Url == null)
			{
				return new Uri(new Uri("http://replace.me"), this.Pattern + file);
			}
			return new Uri(this.Url, this.Pattern + file);
		}

		public IvyRepository Clone()
		{
			return Cloner.CloneObject<IvyRepository>(this);
		}
	}
}
