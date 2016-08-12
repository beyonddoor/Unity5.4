using System;
using System.IO;
using System.Linq;
using Unity.DataContract;

namespace Unity.PackageManager.Ivy
{
	public class IvyModule
	{
		public string Timestamp;

		private IvyInfo info;

		private IvyArtifacts artifacts;

		private IvyDependencies dependencies;

		public bool Loaded;

		public string BasePath;

		public bool Selected;

		public string IvyFile;

		public string Name
		{
			get
			{
				return string.Format("{0}.{1}", this.Info.Organisation, this.Info.Module);
			}
		}

		public PackageVersion Version
		{
			get
			{
				return this.Info.Version;
			}
		}

		public PackageVersion UnityVersion
		{
			get
			{
				return this.Info.UnityVersion;
			}
		}

		public bool Public
		{
			get
			{
				return this.Info.Published;
			}
			set
			{
				this.Info.Published = value;
			}
		}

		public IvyInfo Info
		{
			get
			{
				if (this.info == null)
				{
					this.info = new IvyInfo();
				}
				return this.info;
			}
			set
			{
				this.info = value;
			}
		}

		public IvyArtifacts Artifacts
		{
			get
			{
				if (this.artifacts == null)
				{
					this.artifacts = new IvyArtifacts();
				}
				return this.artifacts;
			}
		}

		public IvyDependencies Dependencies
		{
			get
			{
				if (this.dependencies == null)
				{
					this.dependencies = new IvyDependencies();
				}
				return this.dependencies;
			}
		}

		public override bool Equals(object other)
		{
			if (other is PackageInfo)
			{
				return this == other as PackageInfo;
			}
			return this == other as IvyModule;
		}

		public override int GetHashCode()
		{
			int num = 17;
			num = num * 23 + this.Info.Organisation.GetHashCode();
			num = num * 23 + this.Info.Module.GetHashCode();
			num = num * 23 + this.Info.Type.GetHashCode();
			num = num * 23 + this.Info.Version.GetHashCode();
			return num * 23 + this.Info.UnityVersion.GetHashCode();
		}

		public override string ToString()
		{
			return IvyParser.Serialize(this);
		}

		public static IvyModule FromIvyFile(string fullpath)
		{
			return IvyParser.ParseFile<IvyModule>(fullpath);
		}

		public string WriteIvyFile()
		{
			if (this.BasePath == null)
			{
				throw new InvalidOperationException("Can't save IvyModule without path information");
			}
			return this.WriteIvyFile(this.BasePath, this.IvyFile, true);
		}

		public string WriteIvyFile(string outputPath)
		{
			return this.WriteIvyFile(outputPath, this.IvyFile, false);
		}

		public string WriteIvyFile(string outputPath, string filename)
		{
			return this.WriteIvyFile(outputPath, filename ?? this.IvyFile, false);
		}

		private string WriteIvyFile(string outputPath, string filename, bool savePath)
		{
			if (filename == null)
			{
				filename = "ivy.xml";
			}
			if (!Directory.Exists(outputPath))
			{
				Directory.CreateDirectory(outputPath);
			}
			string text = Path.Combine(outputPath, filename);
			if (!savePath)
			{
				this.BasePath = (this.IvyFile = null);
			}
			string value = IvyParser.Serialize(this);
			using (StreamWriter streamWriter = File.CreateText(text))
			{
				streamWriter.Write(value);
			}
			this.BasePath = outputPath;
			this.IvyFile = filename;
			return text;
		}

		public PackageInfo ToPackageInfo()
		{
			PackageInfo packageInfo = new PackageInfo();
			packageInfo.unityVersion = this.Info.UnityVersion;
			packageInfo.name = this.Info.Module;
			packageInfo.organisation = this.Info.Organisation;
			packageInfo.version = this.Info.Version;
			packageInfo.type = this.Info.Type;
			packageInfo.basePath = this.BasePath;
			packageInfo.description = this.Info.Description;
			packageInfo.loaded = this.Loaded;
			packageInfo.files = this.Artifacts.ToDictionary((IvyArtifact p) => p.Filename, (IvyArtifact e) => new PackageFileData((PackageFileType)e.Type, (!(e.Url != null)) ? string.Empty : e.Url.ToString(), e.Guid ?? string.Empty));
			PackageInfo packageInfo2 = packageInfo;
			IvyArtifact artifact = this.GetArtifact(ArtifactType.ReleaseNotes);
			if (artifact != null && this.BasePath != null)
			{
				string path = Path.Combine(this.BasePath, artifact.Filename);
				if (File.Exists(path))
				{
					packageInfo2.releaseNotes = File.ReadAllText(path);
				}
			}
			return packageInfo2;
		}

		public static IvyModule FromPackageInfo(PackageInfo package)
		{
			IvyModule ivyModule = new IvyModule();
			ivyModule.Info.Organisation = package.organisation;
			ivyModule.Info.UnityVersion = package.unityVersion;
			ivyModule.Info.Module = package.name;
			ivyModule.Info.Version = package.version;
			ivyModule.Info.Type = package.type;
			ivyModule.BasePath = package.basePath;
			ivyModule.Loaded = package.loaded;
			ivyModule.Artifacts.Add(new IvyArtifact
			{
				Name = ivyModule.Info.FullName,
				Type = ArtifactType.Ivy,
				Extension = "xml"
			});
			if (package.files != null)
			{
				ivyModule.Artifacts.AddRange(from f in package.files.Keys
				select new IvyArtifact(f)
				{
					Type = (ArtifactType)package.files[f].type,
					Url = new Uri(package.files[f].url)
				});
			}
			return ivyModule;
		}

		public IvyArtifact GetArtifact(ArtifactType type)
		{
			return this.Artifacts.FirstOrDefault((IvyArtifact x) => x.Type == type);
		}

		public IvyArtifact GetArtifact(string filename)
		{
			return this.Artifacts.FirstOrDefault((IvyArtifact x) => x.Filename == filename);
		}

		public IvyRepository GetRepository(string name)
		{
			return this.Info.repositories.FirstOrDefault((IvyRepository x) => x.Name == name);
		}

		public IvyModule Clone()
		{
			return Cloner.CloneObject<IvyModule>(this);
		}

		public static bool operator ==(IvyModule a, object z)
		{
			return (a == null && z == null) || (a != null && z != null && a.GetHashCode() == z.GetHashCode());
		}

		public static bool operator !=(IvyModule a, object z)
		{
			return !(a == z);
		}
	}
}
