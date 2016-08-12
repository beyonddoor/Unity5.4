using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace UnityEditor.Graphs
{
	public sealed class TopologicalSort
	{
		private enum NodeState
		{
			Visited
		}

		[CompilerGenerated]
		private sealed class <>c__Iterator0 : IDisposable, IEnumerator, IEnumerable, IEnumerable<Node>, IEnumerator<Node>
		{
			internal List<Node>.Enumerator <$s_55>__0;

			internal Node <n>__1;

			internal int $PC;

			internal Node $current;

			Node IEnumerator<Node>.Current
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
				return this.System.Collections.Generic.IEnumerable<UnityEditor.Graphs.Node>.GetEnumerator();
			}

			[DebuggerHidden]
			IEnumerator<Node> IEnumerable<Node>.GetEnumerator()
			{
				if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
				{
					return this;
				}
				return new TopologicalSort.<>c__Iterator0();
			}

			public bool MoveNext()
			{
				uint num = (uint)this.$PC;
				this.$PC = -1;
				bool flag = false;
				switch (num)
				{
				case 0u:
					if (TopologicalSort.s_Graph == null)
					{
						return false;
					}
					this.<$s_55>__0 = TopologicalSort.s_Graph.nodes.GetEnumerator();
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
					while (this.<$s_55>__0.MoveNext())
					{
						this.<n>__1 = this.<$s_55>__0.Current;
						if (!TopologicalSort.s_SortedNodes.Contains(this.<n>__1))
						{
							this.$current = this.<n>__1;
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
						((IDisposable)this.<$s_55>__0).Dispose();
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
						((IDisposable)this.<$s_55>__0).Dispose();
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

		private static List<Node> s_SortedNodes;

		private static Graph s_Graph;

		private static Dictionary<Node, TopologicalSort.NodeState> s_NodeStates;

		public static List<Node> SortedNodes
		{
			get
			{
				return TopologicalSort.s_SortedNodes;
			}
		}

		public static IEnumerable<Node> deadNodes
		{
			get
			{
				TopologicalSort.<>c__Iterator0 <>c__Iterator = new TopologicalSort.<>c__Iterator0();
				TopologicalSort.<>c__Iterator0 expr_07 = <>c__Iterator;
				expr_07.$PC = -2;
				return expr_07;
			}
		}

		private TopologicalSort()
		{
		}

		private static void Visit(Node n)
		{
			if (!TopologicalSort.s_NodeStates.ContainsKey(n))
			{
				TopologicalSort.s_NodeStates[n] = TopologicalSort.NodeState.Visited;
				foreach (Edge current in n.outputEdges)
				{
					TopologicalSort.Visit(current.toSlot.node);
				}
				TopologicalSort.s_SortedNodes.Add(n);
			}
		}

		public static void Sort(Graph g)
		{
			TopologicalSort.s_Graph = g;
			TopologicalSort.s_SortedNodes = new List<Node>();
			TopologicalSort.s_NodeStates = new Dictionary<Node, TopologicalSort.NodeState>();
			foreach (Node current in g.nodes)
			{
				TopologicalSort.Visit(current);
			}
		}
	}
}
