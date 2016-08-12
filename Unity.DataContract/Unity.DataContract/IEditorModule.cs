using System;

namespace Unity.DataContract
{
	public interface IEditorModule : IDisposable
	{
		PackageInfo moduleInfo
		{
			get;
			set;
		}

		void Initialize();

		void Shutdown(bool wait);
	}
}
