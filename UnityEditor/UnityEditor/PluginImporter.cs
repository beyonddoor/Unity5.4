using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	public sealed class PluginImporter : AssetImporter
	{
		[CompilerGenerated]
		private sealed class <GetExtensionPlugins>c__Iterator0 : IDisposable, IEnumerator, IEnumerable<PluginDesc>, IEnumerator<PluginDesc>, IEnumerable
		{
			internal BuildTarget target;

			internal IEnumerable<IEnumerable<PluginDesc>> <pluginDescriptions>__0;

			internal IEnumerator<IEnumerable<PluginDesc>> <$s_162>__1;

			internal IEnumerable<PluginDesc> <extensionPlugins>__2;

			internal IEnumerator<PluginDesc> <$s_163>__3;

			internal PluginDesc <pluginDesc>__4;

			internal int $PC;

			internal PluginDesc $current;

			internal BuildTarget <$>target;

			PluginDesc IEnumerator<PluginDesc>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.$current;
				}
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.$current;
				}
			}

			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<UnityEditorInternal.PluginDesc>.GetEnumerator();
			}

			[DebuggerHidden]
			IEnumerator<PluginDesc> IEnumerable<PluginDesc>.GetEnumerator()
			{
				if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
				{
					return this;
				}
				PluginImporter.<GetExtensionPlugins>c__Iterator0 <GetExtensionPlugins>c__Iterator = new PluginImporter.<GetExtensionPlugins>c__Iterator0();
				<GetExtensionPlugins>c__Iterator.target = this.<$>target;
				return <GetExtensionPlugins>c__Iterator;
			}

			public bool MoveNext()
			{
				uint num = (uint)this.$PC;
				this.$PC = -1;
				bool flag = false;
				switch (num)
				{
				case 0u:
					this.<pluginDescriptions>__0 = AttributeHelper.CallMethodsWithAttribute<IEnumerable<PluginDesc>>(typeof(RegisterPluginsAttribute), new object[]
					{
						this.target
					});
					this.<$s_162>__1 = this.<pluginDescriptions>__0.GetEnumerator();
					num = 4294967293u;
					break;
				case 1u:
					break;
				default:
					return false;
				}
				try
				{
					switch (num)
					{
					case 1u:
						Block_4:
						try
						{
							switch (num)
							{
							}
							if (this.<$s_163>__3.MoveNext())
							{
								this.<pluginDesc>__4 = this.<$s_163>__3.Current;
								this.$current = this.<pluginDesc>__4;
								this.$PC = 1;
								flag = true;
								return true;
							}
						}
						finally
						{
							if (!flag)
							{
								if (this.<$s_163>__3 != null)
								{
									this.<$s_163>__3.Dispose();
								}
							}
						}
						break;
					}
					if (this.<$s_162>__1.MoveNext())
					{
						this.<extensionPlugins>__2 = this.<$s_162>__1.Current;
						this.<$s_163>__3 = this.<extensionPlugins>__2.GetEnumerator();
						num = 4294967293u;
						goto Block_4;
					}
				}
				finally
				{
					if (!flag)
					{
						if (this.<$s_162>__1 != null)
						{
							this.<$s_162>__1.Dispose();
						}
					}
				}
				this.$PC = -1;
				return false;
			}

			[DebuggerHidden]
			public void Dispose()
			{
				uint num = (uint)this.$PC;
				this.$PC = -1;
				switch (num)
				{
				case 1u:
					try
					{
						try
						{
						}
						finally
						{
							if (this.<$s_163>__3 != null)
							{
								this.<$s_163>__3.Dispose();
							}
						}
					}
					finally
					{
						if (this.<$s_162>__1 != null)
						{
							this.<$s_162>__1.Dispose();
						}
					}
					break;
				}
			}

			[DebuggerHidden]
			public void Reset()
			{
				throw new NotSupportedException();
			}
		}

		public extern bool isNativePlugin
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern DllType dllType
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetCompatibleWithAnyPlatform(bool enable);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetCompatibleWithAnyPlatform();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetCompatibleWithEditor(bool enable);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetCompatibleWithEditor();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetIsPreloaded(bool isPreloaded);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool GetIsPreloaded();

		public void SetCompatibleWithPlatform(BuildTarget platform, bool enable)
		{
			this.SetCompatibleWithPlatform(BuildPipeline.GetBuildTargetName(platform), enable);
		}

		public bool GetCompatibleWithPlatform(BuildTarget platform)
		{
			return this.GetCompatibleWithPlatform(BuildPipeline.GetBuildTargetName(platform));
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetCompatibleWithPlatform(string platformName, bool enable);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetCompatibleWithPlatform(string platformName);

		public void SetPlatformData(BuildTarget platform, string key, string value)
		{
			this.SetPlatformData(BuildPipeline.GetBuildTargetName(platform), key, value);
		}

		public string GetPlatformData(BuildTarget platform, string key)
		{
			return this.GetPlatformData(BuildPipeline.GetBuildTargetName(platform), key);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPlatformData(string platformName, string key, string value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetPlatformData(string platformName, string key);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetEditorData(string key, string value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetEditorData(string key);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern PluginImporter[] GetAllImporters();

		private static bool IsCompatible(PluginImporter imp, string platformName)
		{
			return !string.IsNullOrEmpty(imp.assetPath) && (imp.GetCompatibleWithPlatform(platformName) || imp.GetCompatibleWithAnyPlatform());
		}

		public static PluginImporter[] GetImporters(string platformName)
		{
			return (from imp in PluginImporter.GetAllImporters()
			where PluginImporter.IsCompatible(imp, platformName)
			select imp).ToArray<PluginImporter>();
		}

		public static PluginImporter[] GetImporters(BuildTarget platform)
		{
			return PluginImporter.GetImporters(BuildPipeline.GetBuildTargetName(platform));
		}

		[DebuggerHidden]
		internal static IEnumerable<PluginDesc> GetExtensionPlugins(BuildTarget target)
		{
			PluginImporter.<GetExtensionPlugins>c__Iterator0 <GetExtensionPlugins>c__Iterator = new PluginImporter.<GetExtensionPlugins>c__Iterator0();
			<GetExtensionPlugins>c__Iterator.target = target;
			<GetExtensionPlugins>c__Iterator.<$>target = target;
			PluginImporter.<GetExtensionPlugins>c__Iterator0 expr_15 = <GetExtensionPlugins>c__Iterator;
			expr_15.$PC = -2;
			return expr_15;
		}
	}
}
