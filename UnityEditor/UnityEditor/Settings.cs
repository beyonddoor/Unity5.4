using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace UnityEditor
{
	internal class Settings
	{
		[CompilerGenerated]
		private sealed class <Prefs>c__Iterator4<T> : IDisposable, IEnumerator, IEnumerable, IEnumerable<KeyValuePair<string, T>>, IEnumerator<KeyValuePair<string, T>> where T : IPrefType
		{
			internal IEnumerator<KeyValuePair<string, object>> <$s_463>__0;

			internal KeyValuePair<string, object> <kvp>__1;

			internal int $PC;

			internal KeyValuePair<string, T> $current;

			KeyValuePair<string, T> IEnumerator<KeyValuePair<string, T>>.Current
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
				return this.System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string,T>>.GetEnumerator();
			}

			[DebuggerHidden]
			IEnumerator<KeyValuePair<string, T>> IEnumerable<KeyValuePair<string, T>>.GetEnumerator()
			{
				if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
				{
					return this;
				}
				return new Settings.<Prefs>c__Iterator4<T>();
			}

			public bool MoveNext()
			{
				uint num = (uint)this.$PC;
				this.$PC = -1;
				bool flag = false;
				switch (num)
				{
				case 0u:
					Settings.Load();
					this.<$s_463>__0 = Settings.m_Prefs.GetEnumerator();
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
					}
					while (this.<$s_463>__0.MoveNext())
					{
						this.<kvp>__1 = this.<$s_463>__0.Current;
						if (this.<kvp>__1.Value is T)
						{
							this.$current = new KeyValuePair<string, T>(this.<kvp>__1.Key, (T)((object)this.<kvp>__1.Value));
							this.$PC = 1;
							flag = true;
							return true;
						}
					}
				}
				finally
				{
					if (!flag)
					{
						if (this.<$s_463>__0 != null)
						{
							this.<$s_463>__0.Dispose();
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
					}
					finally
					{
						if (this.<$s_463>__0 != null)
						{
							this.<$s_463>__0.Dispose();
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

		private static List<IPrefType> m_AddedPrefs = new List<IPrefType>();

		private static SortedList<string, object> m_Prefs = new SortedList<string, object>();

		internal static void Add(IPrefType value)
		{
			Settings.m_AddedPrefs.Add(value);
		}

		internal static T Get<T>(string name, T defaultValue) where T : IPrefType, new()
		{
			Settings.Load();
			if (defaultValue == null)
			{
				throw new ArgumentException("default can not be null", "defaultValue");
			}
			if (Settings.m_Prefs.ContainsKey(name))
			{
				return (T)((object)Settings.m_Prefs[name]);
			}
			string @string = EditorPrefs.GetString(name, string.Empty);
			if (@string == string.Empty)
			{
				Settings.Set<T>(name, defaultValue);
				return defaultValue;
			}
			defaultValue.FromUniqueString(@string);
			Settings.Set<T>(name, defaultValue);
			return defaultValue;
		}

		internal static void Set<T>(string name, T value) where T : IPrefType
		{
			Settings.Load();
			EditorPrefs.SetString(name, value.ToUniqueString());
			Settings.m_Prefs[name] = value;
		}

		[DebuggerHidden]
		internal static IEnumerable<KeyValuePair<string, T>> Prefs<T>() where T : IPrefType
		{
			Settings.<Prefs>c__Iterator4<T> <Prefs>c__Iterator = new Settings.<Prefs>c__Iterator4<T>();
			Settings.<Prefs>c__Iterator4<T> expr_07 = <Prefs>c__Iterator;
			expr_07.$PC = -2;
			return expr_07;
		}

		private static void Load()
		{
			if (!Settings.m_AddedPrefs.Any<IPrefType>())
			{
				return;
			}
			List<IPrefType> list = new List<IPrefType>(Settings.m_AddedPrefs);
			Settings.m_AddedPrefs.Clear();
			foreach (IPrefType current in list)
			{
				current.Load();
			}
		}
	}
}
