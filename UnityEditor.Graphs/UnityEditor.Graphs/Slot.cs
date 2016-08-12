using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor.Graphs
{
	[Serializable]
	public class Slot
	{
		public SlotType type;

		[SerializeField]
		private string m_Title = string.Empty;

		[SerializeField]
		private string m_Name = string.Empty;

		[NonSerialized]
		public List<Edge> edges;

		[NonSerialized]
		private Node m_Node;

		[SerializeField]
		private string m_DataTypeString = string.Empty;

		[NonSerialized]
		internal Rect m_Position;

		public string name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				this.m_Name = value;
				this.m_Title = Slot.GetNiceTitle(this.m_Name);
			}
		}

		public string title
		{
			get
			{
				return this.m_Title;
			}
			set
			{
				this.m_Title = value;
			}
		}

		public Node node
		{
			get
			{
				return this.m_Node;
			}
			set
			{
				this.m_Node = value;
			}
		}

		public Type dataType
		{
			get
			{
				return SerializedType.FromString(this.m_DataTypeString);
			}
			set
			{
				this.m_DataTypeString = SerializedType.ToString(value);
			}
		}

		public string dataTypeString
		{
			get
			{
				return this.m_DataTypeString;
			}
		}

		public bool isFlowSlot
		{
			get
			{
				return !this.isDataSlot && !this.isTarget;
			}
		}

		public bool isDataSlot
		{
			get
			{
				return !string.IsNullOrEmpty(this.m_DataTypeString);
			}
		}

		public bool isInputDataSlot
		{
			get
			{
				return this.isDataSlot && this.type == SlotType.InputSlot;
			}
		}

		public bool isOutputDataSlot
		{
			get
			{
				return this.isDataSlot && this.type == SlotType.OutputSlot;
			}
		}

		public bool isInputSlot
		{
			get
			{
				return this.type == SlotType.InputSlot;
			}
		}

		public bool isOutputSlot
		{
			get
			{
				return this.type == SlotType.OutputSlot;
			}
		}

		public bool isGeneric
		{
			get
			{
				return SerializedType.IsBaseTypeGeneric(this.m_DataTypeString);
			}
		}

		public bool isTarget
		{
			get
			{
				return this.name == "$Target";
			}
		}

		public Slot()
		{
			this.Init();
		}

		public Slot(SlotType type)
		{
			this.Init();
			this.type = type;
		}

		public Slot(SlotType type, string name)
		{
			this.Init();
			this.name = name;
			this.type = type;
		}

		public Slot(SlotType type, string name, Type dataType)
		{
			this.Init();
			this.name = name;
			this.type = type;
			this.dataType = dataType;
		}

		public Slot(SlotType type, string name, string title)
		{
			this.Init();
			this.name = name;
			this.type = type;
			this.title = title;
		}

		public Slot(SlotType type, string name, string title, Type dataType)
		{
			this.Init();
			this.name = name;
			this.type = type;
			this.title = title;
			this.dataType = dataType;
		}

		private static string GetNiceTitle(string name)
		{
			switch (name)
			{
			case "$Target":
				return string.Empty;
			case "$FnIn":
				return "In";
			case "$FnOut":
				return "Out";
			case "$VarIn":
				return "Param";
			case "$VarOut":
				return "Value";
			}
			return ObjectNames.NicifyVariableName(name);
		}

		private void Init()
		{
			this.edges = new List<Edge>();
		}

		public void SetGenericArgumentType(Type type)
		{
			this.m_DataTypeString = SerializedType.SetGenericArgumentType(this.m_DataTypeString, type);
		}

		public void ResetGenericArgumentType()
		{
			this.m_DataTypeString = SerializedType.ResetGenericArgumentType(this.m_DataTypeString);
		}

		public void RemoveEdge(Edge e)
		{
			this.edges.Remove(e);
		}

		public void AddEdge(Edge e)
		{
			if (this.edges == null)
			{
				throw new NullReferenceException("Error - edges are null?");
			}
			this.edges.Add(e);
		}

		public Property GetProperty()
		{
			return this.node.GetProperty(this.name);
		}

		internal bool HasConnectionTo(Slot toSlot)
		{
			return this.edges.Any((Edge e) => e.toSlot == toSlot);
		}

		internal void WakeUp(Node owner)
		{
			if (this.edges == null)
			{
				this.edges = new List<Edge>();
			}
			this.m_Node = owner;
		}

		public override string ToString()
		{
			return this.name;
		}
	}
}
