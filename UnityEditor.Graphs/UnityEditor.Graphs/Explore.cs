using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityEditor.Graphs
{
	public sealed class Explore
	{
		private enum NodeState
		{
			White,
			Grey,
			Black
		}

		public struct SearchEvent
		{
			public Node node;

			public Edge edge;

			public SearchEvent(Node n, Edge e)
			{
				this.node = n;
				this.edge = e;
			}
		}

		public enum SearchDirection
		{
			Forward,
			Backward
		}

		public delegate void SearchHandler(Explore.SearchEvent e);

		private static Dictionary<Node, Explore.NodeState> m_NodeStates = new Dictionary<Node, Explore.NodeState>();

		public static event Explore.SearchHandler OnDiscoverNode
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				Explore.OnDiscoverNode = (Explore.SearchHandler)Delegate.Combine(Explore.OnDiscoverNode, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				Explore.OnDiscoverNode = (Explore.SearchHandler)Delegate.Remove(Explore.OnDiscoverNode, value);
			}
		}

		public static event Explore.SearchHandler OnDiscoverEdge
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				Explore.OnDiscoverEdge = (Explore.SearchHandler)Delegate.Combine(Explore.OnDiscoverEdge, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				Explore.OnDiscoverEdge = (Explore.SearchHandler)Delegate.Remove(Explore.OnDiscoverEdge, value);
			}
		}

		private Explore()
		{
		}

		public static void Traverse(Node v, Explore.SearchDirection direction)
		{
			Explore.m_NodeStates.Clear();
			Queue<Node> queue = new Queue<Node>();
			queue.Enqueue(v);
			Explore.m_NodeStates[v] = Explore.NodeState.Grey;
			while (queue.Count != 0)
			{
				Node node = queue.Dequeue();
				IEnumerable<Edge> arg_4A_0;
				if (direction == Explore.SearchDirection.Forward)
				{
					IEnumerable<Edge> outputEdges = node.outputEdges;
					arg_4A_0 = outputEdges;
				}
				else
				{
					arg_4A_0 = node.inputEdges;
				}
				foreach (Edge current in arg_4A_0)
				{
					if (Explore.OnDiscoverEdge != null)
					{
						Explore.OnDiscoverEdge(new Explore.SearchEvent(node, current));
					}
					Node node2 = (direction != Explore.SearchDirection.Forward) ? current.fromSlot.node : current.toSlot.node;
					if (!Explore.m_NodeStates.ContainsKey(node2))
					{
						if (Explore.OnDiscoverNode != null)
						{
							Explore.OnDiscoverNode(new Explore.SearchEvent(node2, current));
						}
						queue.Enqueue(node2);
						Explore.m_NodeStates[node2] = Explore.NodeState.Grey;
					}
				}
				Explore.m_NodeStates[node] = Explore.NodeState.Black;
			}
		}
	}
}
