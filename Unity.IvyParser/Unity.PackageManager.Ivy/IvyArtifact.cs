using System;
using System.Collections.Generic;
using System.IO;

namespace Unity.PackageManager.Ivy
{
	public class IvyArtifact
	{
		public string Name;

		public ArtifactType Type;

		public string Extension;

		public Uri Url;

		public Uri PublicUrl;

		public string Guid;

		private List<string> configurations;

		public List<string> Configurations
		{
			get
			{
				if (this.configurations == null)
				{
					this.configurations = new List<string>();
				}
				return this.configurations;
			}
		}

		public string Filename
		{
			get
			{
				return (!string.IsNullOrEmpty(this.Extension)) ? string.Format("{0}.{1}", this.Name, this.Extension) : this.Name;
			}
		}

		public string MD5Filename
		{
			get
			{
				return string.Format("{0}.md5", this.Name);
			}
		}

		public Uri MD5Uri
		{
			get
			{
				if (this.Url == null)
				{
					return null;
				}
				return new Uri(this.Url.ToString().Replace(this.Filename, this.MD5Filename));
			}
		}

		public IvyArtifact()
		{
		}

		public IvyArtifact(string filename) : this(filename, ArtifactType.None)
		{
		}

		public IvyArtifact(string filename, ArtifactType type)
		{
			this.Name = Path.GetFileNameWithoutExtension(filename);
			this.Extension = Path.GetExtension(filename);
			this.Type = type;
		}

		public string WriteToDisk(Guid jobId, string basePath, byte[] bytes)
		{
			string text = Path.Combine(basePath, jobId.ToString());
			Directory.CreateDirectory(text);
			File.WriteAllBytes(Path.Combine(text, this.Filename), bytes);
			return Path.Combine(text, this.Filename);
		}

		public IvyArtifact Clone()
		{
			return Cloner.CloneObject<IvyArtifact>(this);
		}
	}
}
