using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace UnityEditor.Scripting.Compilers
{
	internal class GendarmeOutputParser : UnityScriptCompilerOutputParser
	{
		[CompilerGenerated]
		private sealed class <Parse>c__IteratorB : IDisposable, IEnumerator, IEnumerable, IEnumerable<CompilerMessage>, IEnumerator<CompilerMessage>
		{
			internal int <i>__0;

			internal string[] standardOutput;

			internal GendarmeRuleData <grd>__1;

			internal CompilerMessage <compilerErrorFor>__2;

			internal int $PC;

			internal CompilerMessage $current;

			internal string[] <$>standardOutput;

			CompilerMessage IEnumerator<CompilerMessage>.Current
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
				return this.System.Collections.Generic.IEnumerable<UnityEditor.Scripting.Compilers.CompilerMessage>.GetEnumerator();
			}

			[DebuggerHidden]
			IEnumerator<CompilerMessage> IEnumerable<CompilerMessage>.GetEnumerator()
			{
				if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
				{
					return this;
				}
				GendarmeOutputParser.<Parse>c__IteratorB <Parse>c__IteratorB = new GendarmeOutputParser.<Parse>c__IteratorB();
				<Parse>c__IteratorB.standardOutput = this.<$>standardOutput;
				return <Parse>c__IteratorB;
			}

			public bool MoveNext()
			{
				uint num = (uint)this.$PC;
				this.$PC = -1;
				switch (num)
				{
				case 0u:
					this.<i>__0 = 0;
					goto IL_AF;
				case 1u:
					this.<i>__0 = this.<grd>__1.LastIndex + 1;
					break;
				default:
					return false;
				}
				IL_A1:
				this.<i>__0++;
				IL_AF:
				if (this.<i>__0 >= this.standardOutput.Length)
				{
					this.$PC = -1;
				}
				else
				{
					if (!this.standardOutput[this.<i>__0].StartsWith("Problem:"))
					{
						goto IL_A1;
					}
					this.<grd>__1 = GendarmeOutputParser.GetGendarmeRuleDataFor(this.standardOutput, this.<i>__0);
					this.<compilerErrorFor>__2 = GendarmeOutputParser.CompilerErrorFor(this.<grd>__1);
					this.$current = this.<compilerErrorFor>__2;
					this.$PC = 1;
					return true;
				}
				return false;
			}

			[DebuggerHidden]
			public void Dispose()
			{
				this.$PC = -1;
			}

			[DebuggerHidden]
			public void Reset()
			{
				throw new NotSupportedException();
			}
		}

		public override IEnumerable<CompilerMessage> Parse(string[] errorOutput, bool compilationHadFailure)
		{
			throw new ArgumentException("Gendarme Output Parser needs standard out");
		}

		[DebuggerHidden]
		public override IEnumerable<CompilerMessage> Parse(string[] errorOutput, string[] standardOutput, bool compilationHadFailure)
		{
			GendarmeOutputParser.<Parse>c__IteratorB <Parse>c__IteratorB = new GendarmeOutputParser.<Parse>c__IteratorB();
			<Parse>c__IteratorB.standardOutput = standardOutput;
			<Parse>c__IteratorB.<$>standardOutput = standardOutput;
			GendarmeOutputParser.<Parse>c__IteratorB expr_15 = <Parse>c__IteratorB;
			expr_15.$PC = -2;
			return expr_15;
		}

		private static CompilerMessage CompilerErrorFor(GendarmeRuleData gendarmeRuleData)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(gendarmeRuleData.Problem);
			stringBuilder.AppendLine(gendarmeRuleData.Details);
			stringBuilder.AppendLine(string.IsNullOrEmpty(gendarmeRuleData.Location) ? string.Format("{0} at line : {1}", gendarmeRuleData.Source, gendarmeRuleData.Line) : gendarmeRuleData.Location);
			string message = stringBuilder.ToString();
			return new CompilerMessage
			{
				type = CompilerMessageType.Error,
				message = message,
				file = gendarmeRuleData.File,
				line = gendarmeRuleData.Line,
				column = 1
			};
		}

		private static GendarmeRuleData GetGendarmeRuleDataFor(IList<string> output, int index)
		{
			GendarmeRuleData gendarmeRuleData = new GendarmeRuleData();
			for (int i = index; i < output.Count; i++)
			{
				string text = output[i];
				if (text.StartsWith("Problem:"))
				{
					gendarmeRuleData.Problem = text.Substring(text.LastIndexOf("Problem:", StringComparison.Ordinal) + "Problem:".Length);
				}
				else if (text.StartsWith("* Details"))
				{
					gendarmeRuleData.Details = text;
				}
				else if (text.StartsWith("* Source"))
				{
					gendarmeRuleData.IsAssemblyError = false;
					gendarmeRuleData.Source = text;
					gendarmeRuleData.Line = GendarmeOutputParser.GetLineNumberFrom(text);
					gendarmeRuleData.File = GendarmeOutputParser.GetFileNameFrome(text);
				}
				else if (text.StartsWith("* Severity"))
				{
					gendarmeRuleData.Severity = text;
				}
				else if (text.StartsWith("* Location"))
				{
					gendarmeRuleData.IsAssemblyError = true;
					gendarmeRuleData.Location = text;
				}
				else
				{
					if (!text.StartsWith("* Target"))
					{
						gendarmeRuleData.LastIndex = i;
						break;
					}
					gendarmeRuleData.Target = text;
				}
			}
			return gendarmeRuleData;
		}

		private static string GetFileNameFrome(string currentLine)
		{
			int num = currentLine.LastIndexOf("* Source:") + "* Source:".Length;
			int num2 = currentLine.IndexOf("(");
			if (num != -1 && num2 != -1)
			{
				return currentLine.Substring(num, num2 - num).Trim();
			}
			return string.Empty;
		}

		private static int GetLineNumberFrom(string currentLine)
		{
			int num = currentLine.IndexOf("(") + 2;
			int num2 = currentLine.IndexOf(")");
			if (num != -1 && num2 != -1)
			{
				return int.Parse(currentLine.Substring(num, num2 - num));
			}
			return 0;
		}
	}
}
