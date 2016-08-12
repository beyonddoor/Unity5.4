using System;
using Unity.DataContract;
using Unity.PackageManager.Ivy;

namespace Unity.PackageManager
{
	internal class InternalPackageInfo
	{
		public IvyModule module;

		public PackageInfo package;
	}
}
