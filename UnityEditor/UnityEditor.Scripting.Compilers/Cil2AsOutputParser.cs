using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace UnityEditor.Scripting.Compilers
{
	internal class Cil2AsOutputParser : UnityScriptCompilerOutputParser
	{
		[CompilerGenerated]
		private sealed class <Parse>c__IteratorA : IDisposable, IEnumerator, IEnumerable, IEnumerable<CompilerMessage>, IEnumerator<CompilerMessage>
		{
			internal bool <parsingError>__0;

			internal StringBuilder <currentErrorBuffer>__1;

			internal string[] errorOutput;

			internal string[] <$s_2119>__2;

			internal int <$s_2120>__3;

			internal string <str>__4;

			internal int $PC;

			internal CompilerMessage $current;

			internal string[] <$>errorOutput;

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
				Cil2AsOutputParser.<Parse>c__IteratorA <Parse>c__IteratorA = new Cil2AsOutputParser.<Parse>c__IteratorA();
				<Parse>c__IteratorA.errorOutput = this.<$>errorOutput;
				return <Parse>c__IteratorA;
			}

			public bool MoveNext()
			{
				uint num = (uint)this.$PC;
				this.$PC = -1;
				switch (num)
				{
				case 0u:
					this.<parsingError>__0 = false;
					this.<currentErrorBuffer>__1 = new StringBuilder();
					this.<$s_2119>__2 = this.errorOutput;
					this.<$s_2120>__3 = 0;
					goto IL_103;
				case 1u:
					this.<currentErrorBuffer>__1.Length = 0;
					break;
				case 2u:
					IL_13E:
					this.$PC = -1;
					return false;
				default:
					return false;
				}
				IL_AB:
				this.<currentErrorBuffer>__1.AppendLine(this.<str>__4.Substring("ERROR: ".Length));
				this.<parsingError>__0 = true;
				IL_F5:
				this.<$s_2120>__3++;
				IL_103:
				if (this.<$s_2120>__3 >= this.<$s_2119>__2.Length)
				{
					if (!this.<parsingError>__0)
					{
						goto IL_13E;
					}
					this.$current = Cil2AsOutputParser.CompilerErrorFor(this.<currentErrorBuffer>__1);
					this.$PC = 2;
				}
				else
				{
					this.<str>__4 = this.<$s_2119>__2[this.<$s_2120>__3];
					if (this.<str>__4.StartsWith("ERROR: "))
					{
						if (!this.<parsingError>__0)
						{
							goto IL_AB;
						}
						this.$current = Cil2AsOutputParser.CompilerErrorFor(this.<currentErrorBuffer>__1);
						this.$PC = 1;
					}
					else
					{
						if (this.<parsingError>__0)
						{
							this.<currentErrorBuffer>__1.AppendLine(this.<str>__4);
							goto IL_F5;
						}
						goto IL_F5;
					}
				}
				return true;
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

		[DebuggerHidden]
		public override IEnumerable<CompilerMessage> Parse(string[] errorOutput, string[] standardOutput, bool compilationHadFailure)
		{
			Cil2AsOutputParser.<Parse>c__IteratorA <Parse>c__IteratorA = new Cil2AsOutputParser.<Parse>c__IteratorA();
			<Parse>c__IteratorA.errorOutput = errorOutput;
			<Parse>c__IteratorA.<$>errorOutput = errorOutput;
			Cil2AsOutputParser.<Parse>c__IteratorA expr_15 = <Parse>c__IteratorA;
			expr_15.$PC = -2;
			return expr_15;
		}

		private static CompilerMessage CompilerErrorFor(StringBuilder currentErrorBuffer)
		{
			return new CompilerMessage
			{
				type = CompilerMessageType.Error,
				message = currentErrorBuffer.ToString()
			};
		}
	}
}
