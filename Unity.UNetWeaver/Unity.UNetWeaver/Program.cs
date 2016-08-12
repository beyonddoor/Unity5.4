using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;

namespace Unity.UNetWeaver
{
	public class Program
	{
		public static bool Process(string unityEngine, string unetDLL, string outputDirectory, string[] assemblies, string[] extraAssemblyPaths, IAssemblyResolver assemblyResolver, Action<string> printWarning, Action<string> printError)
		{
			Program.CheckDLLPath(unityEngine);
			Program.CheckDLLPath(unetDLL);
			Program.CheckOutputDirectory(outputDirectory);
			Program.CheckAssemblies(assemblies);
			Log.WarningMethod = printWarning;
			Log.ErrorMethod = printError;
			return Weaver.WeaveAssemblies(assemblies, extraAssemblyPaths, assemblyResolver, outputDirectory, unityEngine, unetDLL);
		}

		private static void CheckDLLPath(string path)
		{
			if (!File.Exists(path))
			{
				throw new Exception("dll could not be located at " + path + "!");
			}
		}

		private static void CheckAssemblies(IEnumerable<string> assemblyPaths)
		{
			foreach (string current in assemblyPaths)
			{
				Program.CheckAssemblyPath(current);
			}
		}

		private static void CheckAssemblyPath(string assemblyPath)
		{
			if (!File.Exists(assemblyPath))
			{
				throw new Exception("Assembly " + assemblyPath + " does not exist!");
			}
		}

		private static void CheckOutputDirectory(string outputDir)
		{
			if (!Directory.Exists(outputDir))
			{
				Directory.CreateDirectory(outputDir);
			}
		}
	}
}
