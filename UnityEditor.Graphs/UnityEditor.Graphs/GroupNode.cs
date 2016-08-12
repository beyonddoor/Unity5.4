using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.Graphs
{
	public class GroupNode : Node
	{
		[SerializeField]
		private Graph m_SubGraph;

		[NonSerialized]
		private ProxyNode m_ProxyInNode;

		[NonSerialized]
		private ProxyNode m_ProxyOutNode;

		public Graph subGraph
		{
			get
			{
				return this.m_SubGraph;
			}
		}

		internal ProxyNode ProxyInNode
		{
			get
			{
				return this.m_ProxyInNode;
			}
		}

		internal ProxyNode ProxyOutNode
		{
			get
			{
				return this.m_ProxyOutNode;
			}
		}

		public GroupNode()
		{
		}

		private GroupNode(string name, Graph parentGraph, Type graphType)
		{
			this.m_SubGraph = (Graph)Activator.CreateInstance(graphType);
			base.name = name;
			this.m_SubGraph.name = "SubGraph";
		}

		public override void NodeUI(GraphGUI host)
		{
			GUILayout.Label("Internal Nodes:" + this.m_SubGraph.nodes.Count, new GUILayoutOption[0]);
			if (GUILayout.Button("Ungroup", new GUILayoutOption[0]))
			{
				this.UnGroup();
			}
			if (GUILayout.Button("Edit", new GUILayoutOption[0]))
			{
				host.ZoomToGraph(this.m_SubGraph);
			}
		}

		public void AddChildNode(Node node)
		{
			this.m_SubGraph.AddNode(node, false);
		}

		public void DestroyChildNode(Node node)
		{
			this.m_SubGraph.DestroyNode(node);
		}

		public static GroupNode FromNodes(string name, List<Node> nodes, Type graphType)
		{
			if (nodes.Count == 0)
			{
				throw new ArgumentException("No nodes to group");
			}
			Graph graph = nodes[0].graph;
			if (graph == null)
			{
				throw new ArgumentException("Nodes needs to be attached to a graph");
			}
			GroupNode groupNode = new GroupNode(name, graph, graphType);
			graph.AddNode(groupNode);
			groupNode.m_ProxyInNode = ProxyNode.Instance(true);
			groupNode.subGraph.AddNode(groupNode.m_ProxyInNode);
			groupNode.m_ProxyOutNode = ProxyNode.Instance(false);
			groupNode.subGraph.AddNode(groupNode.m_ProxyOutNode);
			List<Edge> list = new List<Edge>();
			foreach (Node current in nodes)
			{
				list.AddRange(current.outputEdges);
				list.AddRange(current.inputEdges);
				groupNode.AddChildNode(current);
				graph.nodes.Remove(current);
			}
			foreach (Edge current2 in list)
			{
				if (current2.fromSlot.node.graph == groupNode.subGraph && current2.toSlot.node.graph == groupNode.subGraph)
				{
					if (!groupNode.subGraph.Connected(current2.fromSlot, current2.toSlot))
					{
						groupNode.subGraph.Connect(current2.fromSlot, current2.toSlot);
					}
				}
				else if (current2.fromSlot.node.graph == groupNode.subGraph && current2.toSlot.node.graph != groupNode.subGraph)
				{
					string name2 = current2.fromSlot.name;
					int num = 0;
					while (groupNode.m_ProxyInNode[name2] != null)
					{
						name2 = current2.fromSlot.name + "_" + num++;
					}
					Slot slot = new Slot(SlotType.InputSlot, name2);
					groupNode.m_ProxyInNode.AddSlot(slot);
					groupNode.subGraph.Connect(current2.fromSlot, slot);
					Slot slot2 = new Slot(SlotType.OutputSlot, name2);
					groupNode.AddSlot(slot2);
					groupNode.graph.Connect(slot2, current2.toSlot);
				}
				else if (current2.fromSlot.node.graph != groupNode.subGraph && current2.toSlot.node.graph == groupNode.subGraph)
				{
					string name3 = current2.toSlot.name;
					int num2 = 0;
					while (groupNode.m_ProxyOutNode[name3] != null)
					{
						name3 = current2.toSlot.name + "_" + num2++;
					}
					Slot slot3 = new Slot(SlotType.OutputSlot, name3);
					groupNode.m_ProxyOutNode.AddSlot(slot3);
					groupNode.subGraph.Connect(slot3, current2.toSlot);
					Slot slot4 = new Slot(SlotType.InputSlot, name3);
					groupNode.AddSlot(slot4);
					groupNode.graph.Connect(current2.fromSlot, slot4);
				}
				groupNode.graph.RemoveEdge(current2);
			}
			return groupNode;
		}

		public void UnGroup()
		{
			List<Node> list = this.m_SubGraph.nodes.FindAll((Node n) => !(n is ProxyNode));
			foreach (Node current in list)
			{
				base.graph.AddNode(current, false);
				this.subGraph.nodes.Remove(current);
			}
			List<Edge> list2 = this.m_SubGraph.edges.FindAll((Edge e) => !(e.toSlot.node is ProxyNode) && !(e.fromSlot.node is ProxyNode));
			foreach (Edge current2 in list2)
			{
				base.graph.edges.Add(current2);
				this.subGraph.edges.Remove(current2);
			}
			List<Edge> list3 = new List<Edge>();
			List<Edge> list4 = new List<Edge>();
			foreach (Slot current3 in base.inputSlots)
			{
				int num = 0;
				foreach (Edge current4 in current3.edges)
				{
					Slot slot = this.ProxyOutNode[current3.name];
					Edge edge = slot.edges[num];
					base.graph.Connect(current4.fromSlot, edge.toSlot);
					list3.Add(current4);
					list4.Add(edge);
					num++;
				}
			}
			foreach (Slot current5 in base.outputSlots)
			{
				int num2 = 0;
				foreach (Edge current6 in current5.edges)
				{
					Slot slot2 = this.ProxyInNode[current5.name];
					Edge edge2 = slot2.edges[num2];
					base.graph.Connect(edge2.fromSlot, current6.toSlot);
					list3.Add(current6);
					list4.Add(edge2);
					num2++;
				}
			}
			foreach (Edge current7 in list4)
			{
				this.subGraph.edges.Remove(current7);
			}
			foreach (Edge current8 in list3)
			{
				base.graph.edges.Remove(current8);
			}
			this.subGraph.RemoveNode(this.ProxyInNode, false);
			this.subGraph.RemoveNode(this.ProxyOutNode, false);
			base.graph.RemoveNode(this, false);
		}

		internal override void WakeUp(Graph owner)
		{
			if (this.m_Slots == null)
			{
				this.m_Slots = new List<Slot>();
			}
			if (this.m_SubGraph != null)
			{
				this.m_SubGraph.WakeUp();
			}
			else
			{
				Debug.LogError("Subgraph is null????");
			}
			if (this.m_SubGraph != null)
			{
				this.m_ProxyInNode = (ProxyNode)this.m_SubGraph.nodes.Find((Node n) => n is ProxyNode && ((ProxyNode)n).isIn);
				this.m_ProxyOutNode = (ProxyNode)this.m_SubGraph.nodes.Find((Node n) => n is ProxyNode && !((ProxyNode)n).isIn);
			}
			base.WakeUp(owner);
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				base.ToString(),
				" \"",
				this.m_SubGraph,
				"\" "
			});
		}
	}
}
