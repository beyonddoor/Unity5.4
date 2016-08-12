using System;
using System.Collections.Generic;
using System.IO;
using Unity.DataContract;

namespace Unity.PackageManager.Ivy
{
	public class ModuleRepository
	{
		private List<IvyModule> modules;

		public List<IvyModule> Modules
		{
			get
			{
				if (this.modules == null)
				{
					this.modules = new List<IvyModule>();
				}
				return this.modules;
			}
		}

		public IvyModule GetPackage(PackageType type)
		{
			foreach (IvyModule current in this.modules)
			{
				if (current.Info.Type == type)
				{
					return current;
				}
			}
			return null;
		}

		public IvyModule GetPackage(string org, string name, string version)
		{
			foreach (IvyModule current in this.modules)
			{
				if (current.Info.FullName == string.Format("{0}.{1}.{2}", org, name, version))
				{
					return current;
				}
			}
			return null;
		}

		public override string ToString()
		{
			return IvyParser.Serialize(this);
		}

		public static ModuleRepository FromIvyFile(string fullpath)
		{
			return IvyParser.ParseFile<ModuleRepository>(fullpath);
		}

		public string WriteIvyFile(string outputPath)
		{
			return this.WriteIvyFile(outputPath, null);
		}

		public string WriteIvyFile(string outputPath, string filename)
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
			string value = IvyParser.Serialize(this);
			using (StreamWriter streamWriter = File.CreateText(text))
			{
				streamWriter.Write(value);
			}
			return text;
		}

		public ModuleRepository Clone()
		{
			return Cloner.CloneObject<ModuleRepository>(this);
		}
	}
}
