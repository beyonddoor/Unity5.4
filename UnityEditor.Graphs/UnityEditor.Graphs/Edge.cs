using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor.Graphs
{
	[Serializable]
	public sealed class Edge
	{
		[SerializeField]
		private Node m_FromNode;

		[SerializeField]
		private Node m_ToNode;

		[SerializeField]
		private string m_FromSlotName;

		[SerializeField]
		private string m_ToSlotName;

		[SerializeField]
		public Color color = Color.white;

		[NonSerialized]
		private Slot m_FromSlot;

		[NonSerialized]
		private Slot m_ToSlot;

		public Slot fromSlot
		{
			get
			{
				return this.m_FromSlot;
			}
			set
			{
				if (this.m_FromSlot != null)
				{
					this.m_FromSlot.RemoveEdge(this);
				}
				this.m_FromSlot = value;
				if (value != null)
				{
					this.m_FromNode = value.node;
					this.m_FromSlotName = value.name;
					value.AddEdge(this);
				}
			}
		}

		public Slot toSlot
		{
			get
			{
				return this.m_ToSlot;
			}
			set
			{
				if (this.m_ToSlot != null)
				{
					this.m_ToSlot.RemoveEdge(this);
				}
				this.m_ToSlot = value;
				if (value != null)
				{
					this.m_ToNode = value.node;
					this.m_ToSlotName = value.name;
					value.AddEdge(this);
				}
			}
		}

		public string fromSlotName
		{
			get
			{
				return this.m_FromSlotName;
			}
			set
			{
				this.m_FromSlotName = value;
			}
		}

		public string toSlotName
		{
			get
			{
				return this.m_ToSlotName;
			}
			set
			{
				this.m_ToSlotName = value;
			}
		}

		public Edge(Slot fromSlot, Slot toSlot)
		{
			this.fromSlot = fromSlot;
			this.toSlot = toSlot;
		}

		internal bool NodesNotNull()
		{
			if (this.m_FromNode == null)
			{
				Debug.LogError("Edge.fromNode is null");
				return false;
			}
			if (this.m_ToNode == null)
			{
				Debug.LogError("Edge.toNode is null");
				return false;
			}
			return true;
		}

		internal bool WakeUp()
		{
			this.m_FromSlot = Edge.FindSlotByName(this.m_FromNode.outputSlots, this.m_FromSlotName);
			if (this.m_FromSlot == null)
			{
				return false;
			}
			this.m_ToSlot = Edge.FindSlotByName(this.m_ToNode.inputSlots, this.m_ToSlotName);
			if (this.m_ToSlot == null)
			{
				return false;
			}
			if (!this.m_FromNode.graph.CanConnect(this.m_FromSlot, this.m_ToSlot))
			{
				return false;
			}
			this.m_ToSlot.AddEdge(this);
			this.m_FromSlot.AddEdge(this);
			return true;
		}

		private static Slot FindSlotByName(IEnumerable<Slot> slots, string name)
		{
			return slots.FirstOrDefault((Slot s) => s.name == name);
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				this.fromSlot.node.title,
				"[",
				this.fromSlot,
				"]-->",
				this.toSlot.node.title,
				"[",
				this.toSlot,
				"]"
			});
		}
	}
}
