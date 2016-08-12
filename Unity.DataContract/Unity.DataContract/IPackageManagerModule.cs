using System;
using System.Collections.Generic;

namespace Unity.DataContract
{
	public interface IPackageManagerModule : IEditorModule, IDisposable
	{
		string editorInstallPath
		{
			get;
			set;
		}

		string unityVersion
		{
			get;
			set;
		}

		UpdateMode updateMode
		{
			get;
			set;
		}

		IEnumerable<PackageInfo> playbackEngines
		{
			get;
		}

		IEnumerable<PackageInfo> unityExtensions
		{
			get;
		}

		void CheckForUpdates();

		void LoadPackage(PackageInfo package);

		void SelectPackage(PackageInfo package);
	}
}
