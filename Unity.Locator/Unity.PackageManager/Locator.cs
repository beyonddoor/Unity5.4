using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Unity.DataContract;
using Unity.PackageManager.Ivy;

namespace Unity.PackageManager
{
	public class Locator
	{
		private const int kMaxLevels = 4;

		private static List<InternalPackageInfo> s_Tree = new List<InternalPackageInfo>();

		private static Func<bool> s_ScanningCallback;

		private static bool s_Cancelled;

		private static string s_InstallLocation;

		private static string m_ModuleLocation;

		private static string moduleFile
		{
			get
			{
				return "ivy.xml";
			}
		}

		private static bool teamcity
		{
			get
			{
				return Environment.GetEnvironmentVariable("UNITY_THISISABUILDMACHINE") == "1";
			}
		}

		private static bool isLinux
		{
			get
			{
				return Environment.OSVersion.Platform == PlatformID.Unix && Directory.Exists("/proc");
			}
		}

		public static bool Completed
		{
			get;
			private set;
		}

		public static string installLocation
		{
			get
			{
				if (Locator.s_InstallLocation == null)
				{
					if (Locator.isLinux)
					{
						Locator.s_InstallLocation = Path.Combine(Locator.moduleLocation, "unity3d");
					}
					else
					{
						Locator.s_InstallLocation = Path.Combine(Locator.moduleLocation, "Unity");
					}
				}
				return Locator.s_InstallLocation;
			}
			set
			{
				Locator.s_InstallLocation = value;
			}
		}

		public static string moduleLocation
		{
			get
			{
				if (Locator.m_ModuleLocation == null)
				{
					switch (Environment.OSVersion.Platform)
					{
					case PlatformID.Unix:
					case PlatformID.MacOSX:
						Locator.m_ModuleLocation = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
						goto IL_50;
					}
					Locator.m_ModuleLocation = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
				}
				IL_50:
				return Locator.m_ModuleLocation;
			}
			set
			{
				Locator.m_ModuleLocation = value;
			}
		}

		public static void Scan(string editorInstallPath, string unityVersion)
		{
			Locator.Completed = false;
			PackageVersion packageVersion = new PackageVersion(unityVersion);
			Locator.s_Cancelled = false;
			Locator.s_Tree = new List<InternalPackageInfo>();
			if (Directory.Exists(editorInstallPath))
			{
				Locator.ScanDirectory(editorInstallPath, editorInstallPath, 0);
			}
			string text = Locator.CombinePaths(new string[]
			{
				Locator.installLocation,
				string.Format("{0}.{1}", packageVersion.major, packageVersion.minor)
			});
			if (Directory.Exists(text))
			{
				Locator.ScanDirectory(text, text, 0);
			}
			Locator.Completed = true;
		}

		public static void Scan(string[] scanPaths, string unityVersion)
		{
			Locator.Completed = false;
			PackageVersion packageVersion = new PackageVersion(unityVersion);
			Locator.s_Cancelled = false;
			Locator.s_Tree = new List<InternalPackageInfo>();
			for (int i = 0; i < scanPaths.Length; i++)
			{
				string text = scanPaths[i];
				if (text != null && Directory.Exists(text))
				{
					Locator.ScanDirectory(text, text, 0);
				}
			}
			string text2 = Locator.CombinePaths(new string[]
			{
				Locator.installLocation,
				string.Format("{0}.{1}", packageVersion.major, packageVersion.minor)
			});
			if (Directory.Exists(text2))
			{
				Locator.ScanDirectory(text2, text2, 0);
			}
			Locator.Completed = true;
		}

		public static void Scan(string editorInstallPath, string unityVersion, Func<bool> scanInProgressCallback, Action scanDoneCallback)
		{
			Locator.s_ScanningCallback = scanInProgressCallback;
			Locator.Scan(editorInstallPath, unityVersion);
			if (scanDoneCallback != null)
			{
				scanDoneCallback();
			}
		}

		public static IEnumerable<PackageInfo> QueryAll()
		{
			return from t in Locator.s_Tree
			select t.package;
		}

		public static IEnumerable<object> QueryAllModules()
		{
			return from t in Locator.s_Tree
			select t.module;
		}

		public static PackageInfo GetPackageManager(string unityVersion)
		{
			PackageVersion version = new PackageVersion(unityVersion);
			InternalPackageInfo internalPackageInfo = (from p in Locator.s_Tree
			where p.package.type == PackageType.PackageManager && p.module.UnityVersion.IsCompatibleWith(version)
			orderby p.package.version descending
			select p).FirstOrDefault<InternalPackageInfo>();
			if (internalPackageInfo != null)
			{
				return internalPackageInfo.package;
			}
			return null;
		}

		private static bool UserWantsToContinue()
		{
			if (Locator.s_ScanningCallback != null)
			{
				Locator.s_Cancelled = !Locator.s_ScanningCallback();
				if (Locator.s_Cancelled)
				{
					Locator.s_ScanningCallback = null;
				}
			}
			return !Locator.s_Cancelled;
		}

		private static InternalPackageInfo Parse(string moduleFile)
		{
			if (moduleFile == null)
			{
				return null;
			}
			IvyModule ivyModule = IvyParser.ParseFile<IvyModule>(moduleFile);
			if (IvyParser.HasErrors)
			{
				Console.WriteLine("Error parsing module description from {0}. {1}", moduleFile, IvyParser.ErrorMessage);
				return null;
			}
			return new InternalPackageInfo
			{
				module = ivyModule,
				package = ivyModule.ToPackageInfo()
			};
		}

		public static string CombinePaths(params string[] paths)
		{
			if (paths == null)
			{
				throw new ArgumentNullException("paths");
			}
			if (paths.Length == 1)
			{
				return paths[0];
			}
			StringBuilder stringBuilder = new StringBuilder(paths[0]);
			for (int i = 1; i < paths.Length; i++)
			{
				stringBuilder.AppendFormat("{0}{1}", Path.DirectorySeparatorChar, paths[i]);
			}
			return stringBuilder.ToString();
		}

		private static void ScanDirectory(string rootPath, string path, int level)
		{
			if (!Locator.UserWantsToContinue())
			{
				return;
			}
			if (level > 4)
			{
				return;
			}
			if (path == null || rootPath == null)
			{
				return;
			}
			if (File.Exists(Path.Combine(path, Locator.moduleFile)))
			{
				try
				{
					InternalPackageInfo internalPackageInfo = Locator.Parse(Locator.CombinePaths(new string[]
					{
						path,
						Locator.moduleFile
					}));
					if (internalPackageInfo != null)
					{
						Locator.s_Tree.Add(internalPackageInfo);
						return;
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error parsing module from {0}. {1}", Locator.CombinePaths(new string[]
					{
						path,
						Locator.moduleFile
					}), ex.Message);
					return;
				}
			}
			string[] directories = Directory.GetDirectories(path);
			if (directories.Length == 0)
			{
				return;
			}
			string[] array = directories;
			for (int i = 0; i < array.Length; i++)
			{
				string path2 = array[i];
				Locator.ScanDirectory(rootPath, path2, level + 1);
			}
		}
	}
}
