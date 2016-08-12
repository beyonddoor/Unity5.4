using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor.Graphs
{
	public class Graph : ScriptableObject
	{
		[SerializeField]
		public List<Node> nodes = new List<Node>();

		[SerializeField]
		public List<Edge> edges = new List<Edge>();

		[SerializeField]
		internal List<Edge> m_InvalidEdges = new List<Edge>();

		[NonSerialized]
		internal Rect graphExtents;

		[NonSerialized]
		private bool m_ImAwake;

		[NonSerialized]
		private List<Slot> m_changingOutputSlotTypesCycleSlots = new List<Slot>();

		protected bool isAwake
		{
			get
			{
				return this.m_ImAwake;
			}
		}

		public Node this[string name]
		{
			get
			{
				return this.GetNodeByName(name);
			}
		}

		internal static int[] GetNodeIdsForSerialization(Graph graph)
		{
			return (from n in graph.nodes
			select n.GetInstanceID()).ToArray<int>();
		}

		internal virtual GraphGUI GetEditor()
		{
			GraphGUI graphGUI = ScriptableObject.CreateInstance<GraphGUI>();
			graphGUI.graph = this;
			return graphGUI;
		}

		public virtual bool CanConnect(Slot fromSlot, Slot toSlot)
		{
			return true;
		}

		public virtual bool Connected(Slot fromSlot, Slot toSlot)
		{
			return this.edges.Exists((Edge e) => e.fromSlot == fromSlot && e.toSlot == toSlot);
		}

		public virtual Edge Connect(Slot fromSlot, Slot toSlot)
		{
			bool flag = this.m_changingOutputSlotTypesCycleSlots.Count == 0;
			if (this.m_changingOutputSlotTypesCycleSlots.Contains(toSlot))
			{
				toSlot.edges.RemoveAll((Edge edg) => edg.fromSlot == fromSlot);
				fromSlot.edges.RemoveAll((Edge edg) => edg.toSlot == toSlot);
				this.edges.RemoveAll((Edge edg) => edg.fromSlot == fromSlot && edg.toSlot == toSlot);
				throw new ArgumentException("Connecting node data slots this way creates infinite cycle of changing node types");
			}
			this.m_changingOutputSlotTypesCycleSlots.Add(toSlot);
			if (fromSlot == null || toSlot == null)
			{
				throw new ArgumentException("to/from slot can't be null");
			}
			if (this.Connected(fromSlot, toSlot))
			{
				throw new ArgumentException("Already connected");
			}
			Edge edge = new Edge(fromSlot, toSlot);
			this.edges.Add(edge);
			Graph.SetGenericPropertyArgumentType(toSlot, fromSlot.dataType);
			this.Dirty();
			toSlot.node.InputEdgeChanged(edge);
			if (flag)
			{
				this.m_changingOutputSlotTypesCycleSlots.Clear();
			}
			return edge;
		}

		private static void SetGenericPropertyArgumentType(Slot toSlot, Type fromSlotType)
		{
			if (toSlot.isInputDataSlot && toSlot.node.genericType == null && toSlot.node.isGeneric && toSlot.node.inputDataSlots.First<Slot>() == toSlot)
			{
				toSlot.node.SetGenericPropertyArgumentType(SerializedType.GenericType(fromSlotType));
			}
		}

		private static void ResetGenericPropertyArgumentType(Slot toSlot)
		{
			if (toSlot == null)
			{
				return;
			}
			if (toSlot.isInputDataSlot && toSlot.node.genericType != null && toSlot.node.isGeneric && toSlot.node.inputDataSlots.First<Slot>() == toSlot)
			{
				toSlot.node.ResetGenericPropertyArgumentType();
			}
		}

		public virtual void RemoveEdge(Edge e)
		{
			this.edges.Remove(e);
			if (e.fromSlot != null)
			{
				e.fromSlot.RemoveEdge(e);
			}
			if (e.toSlot != null)
			{
				e.toSlot.RemoveEdge(e);
				e.toSlot.node.InputEdgeChanged(e);
			}
			Graph.ResetGenericPropertyArgumentType(e.toSlot);
			this.Dirty();
		}

		public virtual void AddNode(Node node)
		{
			this.AddNode(node, true);
			this.Dirty();
		}

		public virtual void AddNodes(params Node[] nodes)
		{
			for (int i = 0; i < nodes.Length; i++)
			{
				Node node = nodes[i];
				this.AddNode(node);
			}
		}

		internal virtual void AddNode(Node node, bool serialize)
		{
			this.nodes.Add(node);
			node.graph = this;
			this.Dirty();
			node.AddedToGraph();
		}

		public virtual void Clear(bool destroyNodes = false)
		{
			foreach (Node current in this.nodes)
			{
				current.RemovingFromGraph();
				this.RemoveEdgesFromNode(current);
				if (destroyNodes)
				{
					UnityEngine.Object.DestroyImmediate(current, true);
				}
			}
			this.nodes.Clear();
			this.Dirty();
		}

		public virtual void RemoveNodes(List<Node> nodesToRemove, bool destroyNodes = false)
		{
			foreach (Node current in nodesToRemove)
			{
				this.RemoveNode(current, destroyNodes);
			}
		}

		public virtual void RemoveNode(Node node, bool destroyNode = false)
		{
			if (node == null)
			{
				throw new ArgumentNullException("Node is null");
			}
			node.RemovingFromGraph();
			this.RemoveEdgesFromNode(node);
			this.nodes.Remove(node);
			if (destroyNode)
			{
				UnityEngine.Object.DestroyImmediate(node, true);
			}
			this.Dirty();
		}

		private void RemoveEdgesFromNode(Node node)
		{
			List<Edge> list = new List<Edge>();
			foreach (Edge current in node.inputEdges)
			{
				list.Add(current);
			}
			foreach (Edge current2 in node.outputEdges)
			{
				list.Add(current2);
			}
			foreach (Edge current3 in list)
			{
				this.RemoveEdge(current3);
			}
		}

		public virtual void DestroyNode(Node node)
		{
			this.RemoveNode(node, true);
		}

		public Node GetNodeByName(string name)
		{
			foreach (Node current in this.nodes)
			{
				if (current.name == name)
				{
					return current;
				}
			}
			return null;
		}

		public virtual void OnEnable()
		{
			this.WakeUp();
		}

		public void WakeUp()
		{
			this.WakeUp(false);
		}

		public virtual void WakeUp(bool force)
		{
			if (!force && this.m_ImAwake)
			{
				return;
			}
			for (int i = this.nodes.Count - 1; i >= 0; i--)
			{
				if (this.nodes[i] == null)
				{
					Debug.LogError("Removing null node");
					this.nodes.RemoveAt(i);
				}
			}
			foreach (Node current in this.nodes)
			{
				current.Awake();
			}
			this.WakeUpNodes();
			if (this.edges != null)
			{
				this.WakeUpEdges(false);
			}
			else
			{
				Debug.LogError("Edges are null?");
			}
			this.m_ImAwake = true;
		}

		public void WakeUpEdges(bool clearSlotEdges)
		{
			if (clearSlotEdges)
			{
				foreach (Slot current in this.nodes.SelectMany((Node n) => n.slots))
				{
					current.edges.Clear();
				}
			}
			List<Edge> ok = new List<Edge>();
			List<Edge> list = new List<Edge>();
			this.DoWakeUpEdges(this.edges, ok, list, true);
			this.DoWakeUpEdges(this.m_InvalidEdges, ok, list, false);
			this.edges = ok;
			this.m_InvalidEdges = list;
		}

		public void RevalidateInputDataEdges(Slot s)
		{
			if (!s.isDataSlot || s.type != SlotType.InputSlot)
			{
				throw new ArgumentException("Expected an input data slot");
			}
			if (s.edges.Count<Edge>() > 1)
			{
				throw new ArgumentException("Got input data slot with multiple input Edges. This should never happen.");
			}
			if (s.edges.Count<Edge>() == 1)
			{
				Edge edge = s.edges.First<Edge>();
				edge.fromSlot.edges.Remove(edge);
				s.edges.Clear();
				this.edges.Remove(edge);
				if (this.CanConnect(edge.fromSlot, edge.toSlot))
				{
					if (this.m_changingOutputSlotTypesCycleSlots.Contains(s))
					{
						this.m_changingOutputSlotTypesCycleSlots.Remove(s);
					}
					this.Connect(edge.fromSlot, s);
				}
				else
				{
					this.m_InvalidEdges.Add(edge);
					edge.toSlot.node.InputEdgeChanged(edge);
				}
				return;
			}
			foreach (Edge current in from e in this.m_InvalidEdges
			where e.toSlot == s
			select e)
			{
				if (this.CanConnect(current.fromSlot, current.toSlot))
				{
					this.Connect(current.fromSlot, current.toSlot);
					this.m_InvalidEdges.Remove(current);
					break;
				}
			}
		}

		public void RevalidateOutputDataEdges(Slot s)
		{
			if (!s.isDataSlot || s.type != SlotType.OutputSlot)
			{
				throw new ArgumentException("Expected an output data slot");
			}
			List<Edge> list = s.edges.ToList<Edge>();
			List<Edge> list2 = (from e in this.m_InvalidEdges
			where e.fromSlot == s
			select e).ToList<Edge>();
			foreach (Edge current in s.edges)
			{
				current.toSlot.edges.RemoveAll((Edge edg) => edg.fromSlot == s);
			}
			s.edges.Clear();
			this.edges.RemoveAll((Edge e) => e.fromSlot == s);
			this.m_InvalidEdges.RemoveAll((Edge e) => e.fromSlot == s);
			foreach (Edge current2 in list)
			{
				if (this.CanConnect(current2.fromSlot, current2.toSlot))
				{
					this.Connect(current2.fromSlot, current2.toSlot);
				}
				else
				{
					this.m_InvalidEdges.Add(current2);
					current2.toSlot.node.InputEdgeChanged(current2);
				}
			}
			foreach (Edge current3 in list2)
			{
				if (this.CanConnect(current3.fromSlot, current3.toSlot))
				{
					this.Connect(current3.fromSlot, current3.toSlot);
				}
				else
				{
					this.m_InvalidEdges.Add(current3);
				}
			}
		}

		protected virtual void WakeUpNodes()
		{
			foreach (Node current in this.nodes)
			{
				current.WakeUp(this);
			}
		}

		private void DoWakeUpEdges(List<Edge> inEdges, List<Edge> ok, List<Edge> error, bool inEdgesUsedToBeValid)
		{
			foreach (Edge current in inEdges)
			{
				if (current != null)
				{
					if (current.NodesNotNull())
					{
						if (current.WakeUp())
						{
							ok.Add(current);
							if (!inEdgesUsedToBeValid)
							{
								this.Dirty();
							}
						}
						else
						{
							error.Add(current);
							if (inEdgesUsedToBeValid)
							{
								this.Dirty();
							}
						}
					}
				}
				else
				{
					Debug.LogError("Edge is null?");
				}
			}
		}

		public override string ToString()
		{
			string text = "[";
			foreach (Node current in this.nodes)
			{
				text = text + current + "|";
			}
			text += "];[";
			foreach (Edge current2 in this.edges)
			{
				if (current2 != null)
				{
					text = text + current2 + "|";
				}
			}
			return text + "]";
		}

		public static Graph FlattenedCopy(Graph source)
		{
			Dictionary<Node, Node> dictionary = new Dictionary<Node, Node>();
			Graph graph = (Graph)Activator.CreateInstance(source.GetType());
			foreach (Node current in source.nodes)
			{
				Node node = (Node)Activator.CreateInstance(current.GetType());
				EditorUtility.CopySerialized(current, node);
				dictionary.Add(current, node);
				graph.AddNode(node);
			}
			graph.OnEnable();
			foreach (Edge current2 in source.edges)
			{
				Node node2 = current2.fromSlot.node;
				Node node3 = current2.toSlot.node;
				node2 = dictionary[node2];
				node3 = dictionary[node3];
				Slot fromSlot = node2[current2.fromSlot.name];
				Slot toSlot = node3[current2.toSlot.name];
				graph.Connect(fromSlot, toSlot);
			}
			return graph;
		}

		public void RedirectSlotEdges(Node node, string oldSlotName, string newSlotName)
		{
			foreach (Edge current in this.edges)
			{
				if (current.fromSlotName == oldSlotName)
				{
					current.fromSlotName = newSlotName;
				}
				if (current.toSlotName == oldSlotName)
				{
					current.toSlotName = newSlotName;
				}
			}
		}

		public virtual void Dirty()
		{
			EditorUtility.SetDirty(this);
		}

		public void RemoveInvalidEdgesForSlot(Slot slot)
		{
			this.m_InvalidEdges.RemoveAll((Edge e) => e.fromSlot == slot || e.toSlot == slot);
		}
	}
}
