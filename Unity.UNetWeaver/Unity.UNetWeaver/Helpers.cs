using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Mdb;
using Mono.Cecil.Pdb;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Unity.UNetWeaver
{
	internal class Helpers
	{
		private class AddSearchDirectoryHelper
		{
			private delegate void AddSearchDirectoryDelegate(string directory);

			private readonly Helpers.AddSearchDirectoryHelper.AddSearchDirectoryDelegate _addSearchDirectory;

			public AddSearchDirectoryHelper(IAssemblyResolver assemblyResolver)
			{
				MethodInfo method = assemblyResolver.GetType().GetMethod("AddSearchDirectory", BindingFlags.Instance | BindingFlags.Public, null, new Type[]
				{
					typeof(string)
				}, null);
				if (method == null)
				{
					throw new Exception("Assembly resolver doesn't implement AddSearchDirectory method.");
				}
				this._addSearchDirectory = (Helpers.AddSearchDirectoryHelper.AddSearchDirectoryDelegate)Delegate.CreateDelegate(typeof(Helpers.AddSearchDirectoryHelper.AddSearchDirectoryDelegate), assemblyResolver, method);
			}

			public void AddSearchDirectory(string directory)
			{
				this._addSearchDirectory(directory);
			}
		}

		public static string UnityEngineDLLDirectoryName()
		{
			string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
			return (directoryName == null) ? null : directoryName.Replace("file:\\", string.Empty);
		}

		public static ISymbolReaderProvider GetSymbolReaderProvider(string inputFile)
		{
			string text = inputFile.Substring(0, inputFile.Length - 4);
			if (File.Exists(text + ".pdb"))
			{
				Console.WriteLine("Symbols will be read from " + text + ".pdb");
				return new PdbReaderProvider();
			}
			if (File.Exists(text + ".dll.mdb"))
			{
				Console.WriteLine("Symbols will be read from " + text + ".dll.mdb");
				return new MdbReaderProvider();
			}
			Console.WriteLine("No symbols for " + inputFile);
			return null;
		}

		public static string DestinationFileFor(string outputDir, string assemblyPath)
		{
			string fileName = Path.GetFileName(assemblyPath);
			return Path.Combine(outputDir, fileName);
		}

		public static ReaderParameters ReaderParameters(string assemblyPath, IEnumerable<string> extraPaths, IAssemblyResolver assemblyResolver, string unityEngineDLLPath, string unityUNetDLLPath)
		{
			ReaderParameters readerParameters = new ReaderParameters();
			if (assemblyResolver == null)
			{
				assemblyResolver = new DefaultAssemblyResolver();
			}
			Helpers.AddSearchDirectoryHelper addSearchDirectoryHelper = new Helpers.AddSearchDirectoryHelper(assemblyResolver);
			addSearchDirectoryHelper.AddSearchDirectory(Path.GetDirectoryName(assemblyPath));
			addSearchDirectoryHelper.AddSearchDirectory(Helpers.UnityEngineDLLDirectoryName());
			addSearchDirectoryHelper.AddSearchDirectory(Path.GetDirectoryName(unityEngineDLLPath));
			addSearchDirectoryHelper.AddSearchDirectory(Path.GetDirectoryName(unityUNetDLLPath));
			if (extraPaths != null)
			{
				foreach (string current in extraPaths)
				{
					addSearchDirectoryHelper.AddSearchDirectory(current);
				}
			}
			readerParameters.AssemblyResolver = assemblyResolver;
			readerParameters.SymbolReaderProvider = Helpers.GetSymbolReaderProvider(assemblyPath);
			return readerParameters;
		}

		public static WriterParameters GetWriterParameters(ReaderParameters readParams)
		{
			WriterParameters writerParameters = new WriterParameters();
			if (readParams.SymbolReaderProvider is PdbReaderProvider)
			{
				writerParameters.SymbolWriterProvider = new PdbWriterProvider();
			}
			else if (readParams.SymbolReaderProvider is MdbReaderProvider)
			{
				writerParameters.SymbolWriterProvider = new MdbWriterProvider();
			}
			return writerParameters;
		}

		public static TypeReference MakeGenericType(TypeReference self, params TypeReference[] arguments)
		{
			if (self.GenericParameters.Count != arguments.Length)
			{
				throw new ArgumentException();
			}
			GenericInstanceType genericInstanceType = new GenericInstanceType(self);
			for (int i = 0; i < arguments.Length; i++)
			{
				TypeReference item = arguments[i];
				genericInstanceType.GenericArguments.Add(item);
			}
			return genericInstanceType;
		}

		public static MethodReference MakeHostInstanceGeneric(MethodReference self, params TypeReference[] arguments)
		{
			MethodReference methodReference = new MethodReference(self.Name, self.ReturnType, Helpers.MakeGenericType(self.DeclaringType, arguments))
			{
				HasThis = self.HasThis,
				ExplicitThis = self.ExplicitThis,
				CallingConvention = self.CallingConvention
			};
			foreach (ParameterDefinition current in self.Parameters)
			{
				methodReference.Parameters.Add(new ParameterDefinition(current.ParameterType));
			}
			foreach (GenericParameter current2 in self.GenericParameters)
			{
				methodReference.GenericParameters.Add(new GenericParameter(current2.Name, methodReference));
			}
			return methodReference;
		}
	}
}
