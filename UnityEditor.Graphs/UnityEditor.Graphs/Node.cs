using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor.Graphs
{
	public class Node : ScriptableObject
	{
		[SerializeField]
		protected List<Slot> m_Slots = new List<Slot>();

		[NonSerialized]
		internal Graph m_Graph;

		private string m_NodeInvalidError = string.Empty;

		[SerializeField]
		internal List<Property> m_Properties = new List<Property>();

		[NonSerialized]
		internal List<int> m_SettingProperties = new List<int>();

		[NonSerialized]
		internal List<string> m_SettingPropertyTitles = new List<string>();

		[NonSerialized]
		internal List<int> m_HiddenProperties = new List<int>();

		[SerializeField]
		protected string m_GenericTypeString = string.Empty;

		[SerializeField]
		public Styles.Color color;

		[SerializeField]
		public string style = "node";

		[SerializeField]
		public Rect position;

		[SerializeField]
		internal bool showEmptySlots = true;

		private bool m_IsDragging;

		[NonSerialized]
		private string m_Title = string.Empty;

		public bool nodeIsInvalid
		{
			get
			{
				return this.m_NodeInvalidError != string.Empty;
			}
		}

		public string nodeInvalidError
		{
			get
			{
				return this.m_NodeInvalidError;
			}
			set
			{
				this.m_NodeInvalidError = value;
			}
		}

		public Graph graph
		{
			get
			{
				return this.m_Graph;
			}
			set
			{
				this.m_Graph = value;
			}
		}

		public Type genericType
		{
			get
			{
				return SerializedType.FromString(this.m_GenericTypeString);
			}
			set
			{
				this.m_GenericTypeString = SerializedType.ToString(value);
			}
		}

		public bool isGeneric
		{
			get
			{
				return this.slots.Any((Slot s) => s.isGeneric);
			}
		}

		public bool isDragging
		{
			get
			{
				return this.m_IsDragging;
			}
		}

		public virtual string title
		{
			get
			{
				return (!(this.m_Title == string.Empty)) ? this.m_Title : base.name;
			}
			set
			{
				this.m_Title = value;
				this.Dirty();
			}
		}

		public bool hasTitle
		{
			get
			{
				return !string.IsNullOrEmpty(this.m_Title);
			}
		}

		public virtual string windowTitle
		{
			get
			{
				return base.GetType().Name;
			}
		}

		public List<Slot> slots
		{
			get
			{
				return this.m_Slots;
			}
		}

		public IEnumerable<Slot> inputSlots
		{
			get
			{
				return from s in this.m_Slots
				where s.type == SlotType.InputSlot
				select s;
			}
		}

		public IEnumerable<Slot> outputSlots
		{
			get
			{
				return from s in this.m_Slots
				where s.type == SlotType.OutputSlot
				select s;
			}
		}

		public IEnumerable<Slot> inputDataSlots
		{
			get
			{
				return from s in this.m_Slots
				where s.type == SlotType.InputSlot && s.isDataSlot && s.name != "$Target"
				select s;
			}
		}

		public IEnumerable<Slot> outputDataSlots
		{
			get
			{
				return from s in this.m_Slots
				where s.type == SlotType.OutputSlot && s.isDataSlot
				select s;
			}
		}

		public IEnumerable<Slot> inputFlowSlots
		{
			get
			{
				return from s in this.m_Slots
				where s.type == SlotType.InputSlot && s.isFlowSlot
				select s;
			}
		}

		public IEnumerable<Slot> outputFlowSlots
		{
			get
			{
				return from s in this.m_Slots
				where s.type == SlotType.OutputSlot && s.isFlowSlot
				select s;
			}
		}

		public List<Property> properties
		{
			get
			{
				return this.m_Properties;
			}
		}

		public IEnumerable<Property> settingProperties
		{
			get
			{
				return from i in this.m_SettingProperties
				select this.m_Properties[i];
			}
		}

		public IEnumerable<Edge> outputEdges
		{
			get
			{
				return this.outputSlots.SelectMany((Slot s) => s.edges);
			}
		}

		public IEnumerable<Edge> inputEdges
		{
			get
			{
				return this.inputSlots.SelectMany((Slot s) => s.edges);
			}
		}

		public IEnumerable<Edge> outputFlowEdges
		{
			get
			{
				return this.outputFlowSlots.SelectMany((Slot s) => s.edges);
			}
		}

		public IEnumerable<Edge> inputFlowEdges
		{
			get
			{
				return this.inputFlowSlots.SelectMany((Slot s) => s.edges);
			}
		}

		public IEnumerable<Edge> outputDataEdges
		{
			get
			{
				return this.outputDataSlots.SelectMany((Slot s) => s.edges);
			}
		}

		public IEnumerable<Edge> inputDataEdges
		{
			get
			{
				return this.inputDataSlots.SelectMany((Slot s) => s.edges);
			}
		}

		public Slot this[string name]
		{
			get
			{
				return this.m_Slots.FirstOrDefault((Slot s) => s.name == name);
			}
		}

		public Slot this[int index]
		{
			get
			{
				return this.m_Slots[index];
			}
		}

		public static T Instance<T>() where T : Node, new()
		{
			return ScriptableObject.CreateInstance<T>();
		}

		public static Node Instance()
		{
			return Node.Instance<Node>();
		}

		public Slot AddInputSlot(string name)
		{
			return this.AddInputSlot(name, null);
		}

		public Slot AddInputSlot(string name, Type type)
		{
			Slot slot = new Slot(SlotType.InputSlot, name, type);
			this.AddSlot(slot);
			return slot;
		}

		public Slot AddOutputSlot(string name)
		{
			return this.AddOutputSlot(name, null);
		}

		public Slot AddOutputSlot(string name, Type type)
		{
			Slot slot = new Slot(SlotType.OutputSlot, name, type);
			this.AddSlot(slot);
			return slot;
		}

		public void AddSlot(Slot s)
		{
			this.AddSlot(s, -1);
		}

		public virtual void AddSlot(Slot s, int index)
		{
			if (index != -1)
			{
				this.m_Slots.Insert(index, s);
			}
			else
			{
				this.m_Slots.Add(s);
			}
			if (s.type == SlotType.InputSlot && !s.isFlowSlot)
			{
				this.AddOrModifyPropertyForSlot(s);
			}
			if (s.node == null)
			{
				s.node = this;
				this.Dirty();
				return;
			}
			throw new ArgumentException("Slot already attached to another node");
		}

		public virtual void RemoveSlot(Slot s)
		{
			this.m_Slots.Remove(s);
			foreach (Edge current in s.edges)
			{
				if (current.fromSlot != s)
				{
					current.fromSlot.RemoveEdge(current);
				}
				if (current.toSlot != s)
				{
					current.toSlot.RemoveEdge(current);
				}
				this.graph.edges.Remove(current);
				this.graph.Dirty();
			}
			s.edges.Clear();
			if (this.graph != null)
			{
				this.graph.RemoveInvalidEdgesForSlot(s);
			}
			this.Dirty();
		}

		public virtual void ChangeSlotType(Slot s, Type toType)
		{
			Type dataType = s.dataType;
			if (dataType == toType)
			{
				return;
			}
			s.dataType = toType;
			if (s.isInputDataSlot)
			{
				Property property = this.GetProperty(s.name);
				property.ChangeDataType(toType);
			}
			if (this.graph != null && s.isDataSlot)
			{
				if (s.type == SlotType.InputSlot)
				{
					this.graph.RevalidateInputDataEdges(s);
				}
				else
				{
					this.graph.RevalidateOutputDataEdges(s);
				}
			}
			this.Dirty();
		}

		public virtual void RenameProperty(string oldName, string newName, Type newType)
		{
			Property property = this.GetProperty(oldName);
			property.name = newName;
			property.ChangeDataType(newType);
			this.Dirty();
		}

		public virtual void SetGenericPropertyArgumentType(Type type)
		{
			foreach (Slot current in from s in this.inputDataSlots
			where s.isGeneric
			select s)
			{
				Property property = this.GetProperty(current.name);
				current.SetGenericArgumentType(type);
				property.SetGenericArgumentType(type);
			}
			foreach (Slot current2 in from s in this.outputDataSlots
			where s.isGeneric
			select s)
			{
				current2.SetGenericArgumentType(type);
			}
			this.genericType = type;
			this.Dirty();
		}

		public virtual void ResetGenericPropertyArgumentType()
		{
			this.m_GenericTypeString = string.Empty;
			foreach (Slot current in this.slots)
			{
				if (current.isGeneric && current.isInputDataSlot)
				{
					Property property = this.GetProperty(current.name);
					property.ResetGenericArgumentType();
					current.ResetGenericArgumentType();
					while (current.edges.Any<Edge>())
					{
						this.graph.RemoveEdge(current.edges.First<Edge>());
					}
				}
			}
			foreach (Slot current2 in from s in this.slots
			where s.isGeneric && s.isOutputDataSlot
			select s)
			{
				current2.ResetGenericArgumentType();
				while (current2.edges.Any<Edge>())
				{
					this.graph.RemoveEdge(current2.edges.First<Edge>());
				}
			}
			this.Dirty();
		}

		internal void HideProperty(Property p)
		{
			int num = this.properties.IndexOf(p);
			if (num == -1)
			{
				throw new ArgumentException("Could not find property to hide.");
			}
			this.m_HiddenProperties.Add(num);
			this.Dirty();
		}

		internal void MakeSettingProperty(Property p, string title)
		{
			int num = this.properties.IndexOf(p);
			if (num == -1)
			{
				throw new ArgumentException("Failed to find property to turn into a setting property.");
			}
			this.m_SettingProperties.Add(num);
			this.m_SettingPropertyTitles.Add(title);
			this.Dirty();
		}

		internal bool IsPropertyHidden(Property p)
		{
			int num = this.properties.IndexOf(p);
			return num != -1 && this.m_HiddenProperties.Contains(num);
		}

		internal virtual void Awake()
		{
		}

		internal virtual void WakeUp(Graph owner)
		{
			this.m_Graph = owner;
			if (this.m_Slots == null)
			{
				Debug.LogError("Slots are null - should not happen");
				this.m_Slots = new List<Slot>();
			}
			foreach (Slot current in this.slots)
			{
				if (current != null)
				{
					current.WakeUp(this);
				}
				else
				{
					Debug.LogError("NULL SLOT");
				}
			}
		}

		public virtual void InputEdgeChanged(Edge e)
		{
		}

		public virtual void AddedToGraph()
		{
		}

		public virtual void RemovingFromGraph()
		{
		}

		public override string ToString()
		{
			string text = "[" + base.GetType().Name + "   " + this.title;
			text += "| ";
			text = this.outputSlots.Aggregate(text, (string current, Slot slot) => current + "o[" + slot.name + "]");
			text = this.outputSlots.Aggregate(text, (string current, Slot slot) => current + "i[" + slot.name + "]");
			return text + "]";
		}

		public virtual void NodeUI(GraphGUI host)
		{
		}

		public Property ConstructAndAddProperty(Type type, string name)
		{
			Property property = new Property(type, name);
			this.AddProperty(property);
			return property;
		}

		public Property ConstructAndAddProperty(string serializedTypeString, string name)
		{
			Property property = new Property(serializedTypeString, name);
			this.AddProperty(property);
			return property;
		}

		public Property AddOrModifyProperty(Type dataType, string name)
		{
			Property property = this.TryGetProperty(name);
			if (property != null)
			{
				if (!property.isGeneric && property.type != dataType)
				{
					property.ChangeDataType(dataType);
				}
			}
			else
			{
				property = this.ConstructAndAddProperty(dataType, name);
			}
			return property;
		}

		public Property AddOrModifyPropertyForSlot(Slot s)
		{
			Property property = this.TryGetProperty(s.name);
			if (property != null)
			{
				if (!s.isGeneric && property.type != s.dataType)
				{
					property.ChangeDataType(s.dataType);
				}
			}
			else
			{
				property = this.ConstructAndAddProperty(s.dataTypeString, s.name);
			}
			return property;
		}

		public void AddProperty(Property p)
		{
			this.AssertNotDuplicateName(p.name);
			this.m_Properties.Add(p);
			this.Dirty();
		}

		public string GetSettingPropertyTitle(Property property)
		{
			int num = this.m_Properties.IndexOf(property);
			if (num == -1)
			{
				return string.Empty;
			}
			num = this.m_SettingProperties.IndexOf(num);
			if (num == -1)
			{
				return string.Empty;
			}
			return this.m_SettingPropertyTitles[num];
		}

		public void RemoveProperty(string name)
		{
			Property property = this.TryGetProperty(name);
			if (property == null)
			{
				Debug.LogError("Trying to remove non-existant property " + name);
				return;
			}
			this.RemoveProperty(property);
		}

		public void RemoveProperty(Property p)
		{
			if (!this.m_Properties.Contains(p))
			{
				Debug.LogError("Trying to remove non-existant property " + base.name);
				return;
			}
			int num = this.m_Properties.IndexOf(p);
			this.m_Properties.RemoveAt(num);
			int num2 = this.m_SettingProperties.IndexOf(num);
			if (num2 != -1)
			{
				this.m_SettingProperties.RemoveAt(num2);
				this.m_SettingPropertyTitles.RemoveAt(num2);
			}
			int num3 = this.m_HiddenProperties.IndexOf(num);
			if (num3 != -1)
			{
				this.m_HiddenProperties.RemoveAt(num3);
			}
			this.Dirty();
		}

		public void SetPropertyValueOrCreateAndAddProperty(string name, Type type, object value)
		{
			Property property = this.TryGetProperty(name);
			if (property == null)
			{
				property = new Property(type, name);
				this.AddProperty(property);
			}
			else
			{
				property.ChangeDataType(type);
			}
			property.value = value;
			this.Dirty();
		}

		public void SetPropertyValue(string name, object value)
		{
			Property property = this.GetProperty(name);
			property.value = value;
			this.Dirty();
		}

		public object GetPropertyValue(string name)
		{
			return this.GetProperty(name).value;
		}

		public object GetSlotValue(string slotName)
		{
			return this.GetPropertyValue(slotName);
		}

		public object TryGetSlotPropertyValue(Slot slot)
		{
			Property property = this.TryGetProperty(slot.name);
			return (property != null) ? property.value : null;
		}

		public Property GetProperty(string name)
		{
			Property property = this.TryGetProperty(name);
			if (property == null)
			{
				throw new ArgumentException("Property '" + name + "' not found.");
			}
			return property;
		}

		private void AssertNotDuplicateName(string name)
		{
			if (this.TryGetProperty(name) != null)
			{
				throw new ArgumentException("Property '" + name + "' already exists.");
			}
		}

		public Property TryGetProperty(string name)
		{
			return this.m_Properties.FirstOrDefault((Property p) => p.name == name);
		}

		public Property GetOrCreateAndAddProperty(Type type, string name)
		{
			Property property = this.TryGetProperty(name);
			if (property == null)
			{
				return this.ConstructAndAddProperty(type, name);
			}
			property.ChangeDataType(type);
			return property;
		}

		public virtual void Dirty()
		{
			EditorUtility.SetDirty(this);
		}

		public virtual void BeginDrag()
		{
		}

		public virtual void OnDrag()
		{
			this.m_IsDragging = true;
		}

		public virtual void EndDrag()
		{
			this.m_IsDragging = false;
		}
	}
}
