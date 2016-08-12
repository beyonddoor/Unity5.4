using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor.Scripting
{
	internal class PragmaFixing30
	{
		[CompilerGenerated]
		private sealed class <SearchRecursive>c__Iterator9 : IDisposable, IEnumerator, IEnumerable, IEnumerable<string>, IEnumerator<string>
		{
			internal string dir;

			internal string[] <$s_2096>__0;

			internal int <$s_2097>__1;

			internal string <d>__2;

			internal string mask;

			internal IEnumerator<string> <$s_2098>__3;

			internal string <f>__4;

			internal string[] <$s_2099>__5;

			internal int <$s_2100>__6;

			internal string <f>__7;

			internal int $PC;

			internal string $current;

			internal string <$>dir;

			internal string <$>mask;

			string IEnumerator<string>.Current
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
				return this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();
			}

			[DebuggerHidden]
			IEnumerator<string> IEnumerable<string>.GetEnumerator()
			{
				if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
				{
					return this;
				}
				PragmaFixing30.<SearchRecursive>c__Iterator9 <SearchRecursive>c__Iterator = new PragmaFixing30.<SearchRecursive>c__Iterator9();
				<SearchRecursive>c__Iterator.dir = this.<$>dir;
				<SearchRecursive>c__Iterator.mask = this.<$>mask;
				return <SearchRecursive>c__Iterator;
			}

			public bool MoveNext()
			{
				uint num = (uint)this.$PC;
				this.$PC = -1;
				bool flag = false;
				switch (num)
				{
				case 0u:
					this.<$s_2096>__0 = Directory.GetDirectories(this.dir);
					this.<$s_2097>__1 = 0;
					break;
				case 1u:
					Block_2:
					try
					{
						switch (num)
						{
						}
						if (this.<$s_2098>__3.MoveNext())
						{
							this.<f>__4 = this.<$s_2098>__3.Current;
							this.$current = this.<f>__4;
							this.$PC = 1;
							flag = true;
							return true;
						}
					}
					finally
					{
						if (!flag)
						{
							if (this.<$s_2098>__3 != null)
							{
								this.<$s_2098>__3.Dispose();
							}
						}
					}
					this.<$s_2097>__1++;
					break;
				case 2u:
					this.<$s_2100>__6++;
					goto IL_15D;
				default:
					return false;
				}
				if (this.<$s_2097>__1 < this.<$s_2096>__0.Length)
				{
					this.<d>__2 = this.<$s_2096>__0[this.<$s_2097>__1];
					this.<$s_2098>__3 = PragmaFixing30.SearchRecursive(this.<d>__2, this.mask).GetEnumerator();
					num = 4294967293u;
					goto Block_2;
				}
				this.<$s_2099>__5 = Directory.GetFiles(this.dir, this.mask);
				this.<$s_2100>__6 = 0;
				IL_15D:
				if (this.<$s_2100>__6 < this.<$s_2099>__5.Length)
				{
					this.<f>__7 = this.<$s_2099>__5[this.<$s_2100>__6];
					this.$current = this.<f>__7;
					this.$PC = 2;
					return true;
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
						if (this.<$s_2098>__3 != null)
						{
							this.<$s_2098>__3.Dispose();
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

		private static void FixJavaScriptPragmas()
		{
			string[] array = PragmaFixing30.CollectBadFiles();
			if (array.Length == 0)
			{
				return;
			}
			if (!InternalEditorUtility.inBatchMode)
			{
				PragmaFixingWindow.ShowWindow(array);
			}
			else
			{
				PragmaFixing30.FixFiles(array);
			}
		}

		public static void FixFiles(string[] filesToFix)
		{
			for (int i = 0; i < filesToFix.Length; i++)
			{
				string text = filesToFix[i];
				try
				{
					PragmaFixing30.FixPragmasInFile(text);
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogError("Failed to fix pragmas in file '" + text + "'.\n" + ex.Message);
				}
			}
		}

		private static bool FileNeedsPragmaFixing(string fileName)
		{
			return PragmaFixing30.CheckOrFixPragmas(fileName, true);
		}

		private static void FixPragmasInFile(string fileName)
		{
			PragmaFixing30.CheckOrFixPragmas(fileName, false);
		}

		private static bool CheckOrFixPragmas(string fileName, bool onlyCheck)
		{
			string text = File.ReadAllText(fileName);
			StringBuilder sb = new StringBuilder(text);
			PragmaFixing30.LooseComments(sb);
			Match match = PragmaFixing30.PragmaMatch(sb, "strict");
			if (!match.Success)
			{
				return false;
			}
			bool success = PragmaFixing30.PragmaMatch(sb, "downcast").Success;
			bool success2 = PragmaFixing30.PragmaMatch(sb, "implicit").Success;
			if (success && success2)
			{
				return false;
			}
			if (!onlyCheck)
			{
				PragmaFixing30.DoFixPragmasInFile(fileName, text, match.Index + match.Length, success, success2);
			}
			return true;
		}

		private static void DoFixPragmasInFile(string fileName, string oldText, int fixPos, bool hasDowncast, bool hasImplicit)
		{
			string text = string.Empty;
			string str = (!PragmaFixing30.HasWinLineEndings(oldText)) ? "\n" : "\r\n";
			if (!hasImplicit)
			{
				text = text + str + "#pragma implicit";
			}
			if (!hasDowncast)
			{
				text = text + str + "#pragma downcast";
			}
			File.WriteAllText(fileName, oldText.Insert(fixPos, text));
		}

		private static bool HasWinLineEndings(string text)
		{
			return text.IndexOf("\r\n") != -1;
		}

		[DebuggerHidden]
		private static IEnumerable<string> SearchRecursive(string dir, string mask)
		{
			PragmaFixing30.<SearchRecursive>c__Iterator9 <SearchRecursive>c__Iterator = new PragmaFixing30.<SearchRecursive>c__Iterator9();
			<SearchRecursive>c__Iterator.dir = dir;
			<SearchRecursive>c__Iterator.mask = mask;
			<SearchRecursive>c__Iterator.<$>dir = dir;
			<SearchRecursive>c__Iterator.<$>mask = mask;
			PragmaFixing30.<SearchRecursive>c__Iterator9 expr_23 = <SearchRecursive>c__Iterator;
			expr_23.$PC = -2;
			return expr_23;
		}

		private static void LooseComments(StringBuilder sb)
		{
			Regex regex = new Regex("//");
			foreach (Match match in regex.Matches(sb.ToString()))
			{
				int index = match.Index;
				while (index < sb.Length && sb[index] != '\n' && sb[index] != '\r')
				{
					sb[index++] = ' ';
				}
			}
		}

		private static Match PragmaMatch(StringBuilder sb, string pragma)
		{
			return new Regex("#\\s*pragma\\s*" + pragma).Match(sb.ToString());
		}

		private static string[] CollectBadFiles()
		{
			List<string> list = new List<string>();
			foreach (string current in PragmaFixing30.SearchRecursive(Path.Combine(Directory.GetCurrentDirectory(), "Assets"), "*.js"))
			{
				try
				{
					if (PragmaFixing30.FileNeedsPragmaFixing(current))
					{
						list.Add(current);
					}
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogError("Failed to fix pragmas in file '" + current + "'.\n" + ex.Message);
				}
			}
			return list.ToArray();
		}
	}
}
