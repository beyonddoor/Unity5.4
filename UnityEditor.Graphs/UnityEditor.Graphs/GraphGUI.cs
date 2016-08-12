using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor.Graphs
{
	[Serializable]
	public abstract class GraphGUI : ScriptableObject
	{
		public class NodeTool
		{
			public delegate Node CreateNodeFuncDelegate();

			public string category;

			public GUIContent content;

			public bool visible;

			public GraphGUI.NodeTool.CreateNodeFuncDelegate createNodeFunc;

			public NodeTool(string category, string title, GraphGUI.NodeTool.CreateNodeFuncDelegate createNodeFunc)
			{
				this.createNodeFunc = createNodeFunc;
				this.category = category;
				this.content = new GUIContent(title);
				this.visible = false;
			}
		}

		private enum SelectionDragMode
		{
			None,
			Rect,
			Pick
		}

		private class ContextMenuData
		{
			public GUIContent[] items;

			public Vector2 mousePosition;
		}

		private const int kNodePositionIncrement = 40;

		private const int kNodePositionXMax = 700;

		private const int kNodePositionYMax = 250;

		protected const float kGraphPaddingMultiplier = 0.6f;

		protected const float kNodeGridSize = 12f;

		private const float kMajorGridSize = 120f;

		[SerializeField]
		protected Vector2 m_ScrollPosition;

		private Rect m_LastGraphExtents;

		protected bool m_CenterGraph;

		protected Rect m_GraphClientArea;

		protected Vector2? m_contextMenuMouseDownPosition;

		[SerializeField]
		protected Graph m_Graph;

		protected EditorWindow m_Host;

		protected IEdgeGUI m_EdgeGUI;

		private Vector2 m_LastNodeAddedPosition = new Vector2(40f, 0f);

		protected List<GraphGUI.NodeTool> m_Tools = new List<GraphGUI.NodeTool>();

		private Vector2 m_DragStartPoint;

		public List<Node> selection = new List<Node>();

		private GraphGUI.SelectionDragMode m_IsDraggingSelection;

		private List<Node> m_OldSelection;

		private static readonly Color kGridMinorColorDark = new Color(0f, 0f, 0f, 0.18f);

		private static readonly Color kGridMajorColorDark = new Color(0f, 0f, 0f, 0.28f);

		private static readonly Color kGridMinorColorLight = new Color(0f, 0f, 0f, 0.1f);

		private static readonly Color kGridMajorColorLight = new Color(0f, 0f, 0f, 0.15f);

		private Vector2 m_LastMousePosition;

		private Vector2 m_DragNodeDistance;

		private readonly Dictionary<Node, Rect> m_InitialDragNodePositions = new Dictionary<Node, Rect>();

		private static readonly GUIContent kTempContent = new GUIContent();

		private static readonly int kDragGraphControlID = "DragGraph".GetHashCode();

		private static readonly int kDragNodesControlID = "DragNodes".GetHashCode();

		private static readonly int kDragSelectionControlID = "DragSelection".GetHashCode();

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

		public virtual IEdgeGUI edgeGUI
		{
			get
			{
				if (this.m_EdgeGUI == null)
				{
					this.m_EdgeGUI = new EdgeGUI
					{
						host = this
					};
				}
				return this.m_EdgeGUI;
			}
		}

		private static Color gridMinorColor
		{
			get
			{
				if (EditorGUIUtility.isProSkin)
				{
					return GraphGUI.kGridMinorColorDark;
				}
				return GraphGUI.kGridMinorColorLight;
			}
		}

		private static Color gridMajorColor
		{
			get
			{
				if (EditorGUIUtility.isProSkin)
				{
					return GraphGUI.kGridMajorColorDark;
				}
				return GraphGUI.kGridMajorColorLight;
			}
		}

		public virtual void OnGraphGUI()
		{
			this.m_Host.BeginWindows();
			foreach (Node current in this.m_Graph.nodes)
			{
				Node n2 = current;
				bool on = this.selection.Contains(current);
				Styles.Color color = (!current.nodeIsInvalid) ? current.color : Styles.Color.Red;
				current.position = GUILayout.Window(current.GetInstanceID(), current.position, delegate
				{
					this.NodeGUI(n2);
				}, current.title, Styles.GetNodeStyle(current.style, color, on), new GUILayoutOption[]
				{
					GUILayout.Width(0f),
					GUILayout.Height(0f)
				});
			}
			this.m_Host.EndWindows();
			this.edgeGUI.DoEdges();
			this.edgeGUI.DoDraggedEdge();
			this.DragSelection(new Rect(-5000f, -5000f, 10000f, 10000f));
			this.ShowContextMenu();
			this.HandleMenuEvents();
		}

		public void BeginGraphGUI(EditorWindow host, Rect position)
		{
			this.m_GraphClientArea = position;
			this.m_Host = host;
			if (Event.current.type == EventType.Repaint)
			{
				Styles.graphBackground.Draw(position, false, false, false, false);
			}
			this.m_ScrollPosition = GUI.BeginScrollView(position, this.m_ScrollPosition, this.m_Graph.graphExtents, GUIStyle.none, GUIStyle.none);
			this.DrawGrid();
		}

		public void EndGraphGUI()
		{
			this.UpdateGraphExtents();
			this.UpdateScrollPosition();
			this.DragGraph();
			GUI.EndScrollView();
		}

		private void UpdateScrollPosition()
		{
			this.m_ScrollPosition.x = this.m_ScrollPosition.x + (this.m_LastGraphExtents.x - this.graph.graphExtents.x);
			this.m_ScrollPosition.y = this.m_ScrollPosition.y + (this.m_LastGraphExtents.y - this.graph.graphExtents.y);
			this.m_LastGraphExtents = this.graph.graphExtents;
			if (this.m_CenterGraph && Event.current.type == EventType.Layout)
			{
				this.m_ScrollPosition = new Vector2(this.graph.graphExtents.width / 2f - this.m_Host.position.width / 2f, this.graph.graphExtents.height / 2f - this.m_Host.position.height / 2f);
				this.m_CenterGraph = false;
			}
		}

		private void UpdateGraphExtents()
		{
			this.graph.graphExtents = GUILayoutUtility.GetWindowsBounds();
			Graph expr_1B_cp_0 = this.graph;
			expr_1B_cp_0.graphExtents.xMin = expr_1B_cp_0.graphExtents.xMin - this.m_Host.position.width * 0.6f;
			Graph expr_4B_cp_0 = this.graph;
			expr_4B_cp_0.graphExtents.xMax = expr_4B_cp_0.graphExtents.xMax + this.m_Host.position.width * 0.6f;
			Graph expr_7B_cp_0 = this.graph;
			expr_7B_cp_0.graphExtents.yMin = expr_7B_cp_0.graphExtents.yMin - this.m_Host.position.height * 0.6f;
			Graph expr_AB_cp_0 = this.graph;
			expr_AB_cp_0.graphExtents.yMax = expr_AB_cp_0.graphExtents.yMax + this.m_Host.position.height * 0.6f;
		}

		private void DrawGrid()
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			Profiler.BeginSample("DrawGrid");
			HandleUtility.ApplyWireMaterial();
			GL.PushMatrix();
			GL.Begin(1);
			this.DrawGridLines(12f, GraphGUI.gridMinorColor);
			this.DrawGridLines(120f, GraphGUI.gridMajorColor);
			GL.End();
			GL.PopMatrix();
			Profiler.EndSample();
		}

		private void DrawGridLines(float gridSize, Color gridColor)
		{
			GL.Color(gridColor);
			for (float num = this.m_Graph.graphExtents.xMin - this.m_Graph.graphExtents.xMin % gridSize; num < this.m_Graph.graphExtents.xMax; num += gridSize)
			{
				this.DrawLine(new Vector2(num, this.m_Graph.graphExtents.yMin), new Vector2(num, this.m_Graph.graphExtents.yMax));
			}
			GL.Color(gridColor);
			for (float num2 = this.m_Graph.graphExtents.yMin - this.m_Graph.graphExtents.yMin % gridSize; num2 < this.m_Graph.graphExtents.yMax; num2 += gridSize)
			{
				this.DrawLine(new Vector2(this.m_Graph.graphExtents.xMin, num2), new Vector2(this.m_Graph.graphExtents.xMax, num2));
			}
		}

		private void DrawLine(Vector2 p1, Vector2 p2)
		{
			GL.Vertex(p1);
			GL.Vertex(p2);
		}

		private void DragGraph()
		{
			int controlID = GUIUtility.GetControlID(GraphGUI.kDragGraphControlID, FocusType.Passive);
			Event current = Event.current;
			if (current.button != 2 && (current.button != 0 || !current.alt))
			{
				return;
			}
			switch (current.GetTypeForControl(controlID))
			{
			case EventType.MouseDown:
				GUIUtility.hotControl = controlID;
				current.Use();
				EditorGUIUtility.SetWantsMouseJumping(1);
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID)
				{
					GUIUtility.hotControl = 0;
					current.Use();
					EditorGUIUtility.SetWantsMouseJumping(0);
				}
				break;
			case EventType.MouseMove:
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID)
				{
					this.m_ScrollPosition -= current.delta;
					current.Use();
				}
				break;
			}
		}

		private void ContextMenuClick(object userData, string[] options, int selected)
		{
			if (selected < 0)
			{
				return;
			}
			GraphGUI.ContextMenuData contextMenuData = (GraphGUI.ContextMenuData)userData;
			string text = contextMenuData.items[selected].text;
			string text2 = text;
			if (text2 != null)
			{
				if (GraphGUI.<>f__switch$map3 == null)
				{
					GraphGUI.<>f__switch$map3 = new Dictionary<string, int>(5)
					{
						{
							"Cut",
							0
						},
						{
							"Copy",
							0
						},
						{
							"Duplicate",
							0
						},
						{
							"Delete",
							0
						},
						{
							"Paste",
							1
						}
					};
				}
				int num;
				if (GraphGUI.<>f__switch$map3.TryGetValue(text2, out num))
				{
					if (num != 0)
					{
						if (num == 1)
						{
							this.m_contextMenuMouseDownPosition = new Vector2?(contextMenuData.mousePosition);
							this.m_Host.SendEvent(EditorGUIUtility.CommandEvent(text));
						}
					}
					else
					{
						this.m_Host.SendEvent(EditorGUIUtility.CommandEvent(text));
					}
				}
			}
		}

		protected void ShowContextMenu()
		{
			if (Event.current.type != EventType.MouseDown || Event.current.button != 1 || Event.current.clickCount != 1)
			{
				return;
			}
			Event.current.Use();
			Vector2 mousePosition = Event.current.mousePosition;
			Rect position = new Rect(mousePosition.x, mousePosition.y, 0f, 0f);
			List<GUIContent> list = new List<GUIContent>();
			if (this.selection.Count != 0)
			{
				list.Add(new GUIContent("Cut"));
				list.Add(new GUIContent("Copy"));
				list.Add(new GUIContent("Duplicate"));
				list.Add(new GUIContent(string.Empty));
				list.Add(new GUIContent("Delete"));
			}
			else
			{
				list.Add((this.edgeGUI.edgeSelection.Count != 0) ? new GUIContent("Delete") : new GUIContent("Paste"));
			}
			GUIContent[] options = list.ToArray();
			GraphGUI.ContextMenuData userData = new GraphGUI.ContextMenuData
			{
				items = list.ToArray(),
				mousePosition = mousePosition
			};
			this.m_contextMenuMouseDownPosition = null;
			EditorUtility.DisplayCustomMenu(position, options, -1, new EditorUtility.SelectMenuItemFunction(this.ContextMenuClick), userData);
		}

		protected Graph CopyNodesPasteboardData(out int[] ids)
		{
			Graph graph = ScriptableObject.CreateInstance(this.m_Graph.GetType()) as Graph;
			graph.nodes.AddRange(this.selection.ToArray());
			foreach (Edge current in this.m_Graph.edges)
			{
				if (this.selection.Contains(current.fromSlot.node) && this.selection.Contains(current.toSlot.node))
				{
					graph.Connect(current.fromSlot, current.toSlot);
				}
			}
			List<int> list = new List<int>();
			list.Add(graph.GetInstanceID());
			list.AddRange(Graph.GetNodeIdsForSerialization(graph));
			ids = list.ToArray();
			return graph;
		}

		protected virtual void CopyNodesToPasteboard()
		{
		}

		protected virtual void PasteNodesPasteboardData(Graph dummyGraph)
		{
		}

		protected virtual void PasteNodesFromPasteboard()
		{
		}

		protected static void OffsetPastedNodePositions(IEnumerable<Node> nodes, Vector2? pastePosition)
		{
			float num = 15f;
			float num2 = 15f;
			if (pastePosition.HasValue)
			{
				float num3 = 3.40282347E+38f;
				float num4 = 3.40282347E+38f;
				foreach (Node current in nodes)
				{
					num3 = Mathf.Min(num3, current.position.x);
					num4 = Mathf.Min(num4, current.position.y);
				}
				num = pastePosition.Value.x - num3;
				num2 = pastePosition.Value.y - num4;
			}
			foreach (Node current2 in nodes)
			{
				current2.position = new Rect(current2.position.x + num, current2.position.y + num2, current2.position.width, current2.position.height);
			}
		}

		protected virtual void DuplicateNodesThroughPasteboard()
		{
		}

		protected void HandleMenuEvents()
		{
			string[] source = new string[]
			{
				"SoftDelete",
				"Delete",
				"Cut",
				"Copy",
				"Paste",
				"Duplicate",
				"SelectAll"
			};
			Event current = Event.current;
			if (current.type != EventType.ValidateCommand && current.type != EventType.ExecuteCommand)
			{
				return;
			}
			if (!source.Contains(current.commandName))
			{
				return;
			}
			if (current.type == EventType.ValidateCommand)
			{
				current.Use();
				return;
			}
			string commandName = current.commandName;
			switch (commandName)
			{
			case "SoftDelete":
			case "Delete":
				if (this.selection.Count == 0 && this.edgeGUI.edgeSelection.Count == 0)
				{
					return;
				}
				if (current.type == EventType.ExecuteCommand)
				{
					this.DeleteNodesAndEdges(this.selection, (from i in this.edgeGUI.edgeSelection
					select this.m_Graph.edges[i]).ToList<Edge>());
				}
				current.Use();
				break;
			case "Cut":
				this.CopyNodesToPasteboard();
				this.DeleteNodesAndEdges(this.selection, (from i in this.edgeGUI.edgeSelection
				select this.m_Graph.edges[i]).ToList<Edge>());
				current.Use();
				break;
			case "Copy":
				this.CopyNodesToPasteboard();
				current.Use();
				break;
			case "Paste":
				this.PasteNodesFromPasteboard();
				current.Use();
				break;
			case "Duplicate":
				this.DuplicateNodesThroughPasteboard();
				current.Use();
				break;
			case "SelectAll":
				this.ClearSelection();
				this.selection.AddRange(this.m_Graph.nodes);
				current.Use();
				break;
			}
		}

		public void ZoomToGraph(Graph g)
		{
			throw new NotImplementedException();
		}

		private void DeleteNodesAndEdges(List<Node> nodes, List<Edge> edges)
		{
			foreach (Edge current in edges)
			{
				this.m_Graph.RemoveEdge(current);
			}
			this.edgeGUI.edgeSelection.Clear();
			foreach (Edge current2 in GraphGUI.FindEdgesOfNodes(nodes, false))
			{
				this.m_Graph.RemoveEdge(current2);
			}
			foreach (Node current3 in nodes)
			{
				this.m_Graph.DestroyNode(current3);
			}
		}

		private static List<Edge> FindEdgesOfNodes(List<Node> nodes, bool requireBoth)
		{
			Dictionary<Edge, int> dictionary = new Dictionary<Edge, int>();
			foreach (Node current in nodes)
			{
				foreach (Slot current2 in current.slots)
				{
					foreach (Edge current3 in current2.edges)
					{
						if (!dictionary.ContainsKey(current3))
						{
							dictionary.Add(current3, 1);
						}
						else
						{
							Dictionary<Edge, int> dictionary2;
							Dictionary<Edge, int> expr_6B = dictionary2 = dictionary;
							Edge key;
							Edge expr_70 = key = current3;
							int num = dictionary2[key];
							expr_6B[expr_70] = num + 1;
						}
					}
				}
			}
			List<Edge> list = new List<Edge>();
			int num2 = (!requireBoth) ? 1 : 2;
			foreach (KeyValuePair<Edge, int> current4 in dictionary)
			{
				if (current4.Value >= num2)
				{
					list.Add(current4.Key);
				}
			}
			return list;
		}

		public virtual void NodeGUI(Node n)
		{
			this.SelectNode(n);
			foreach (Slot current in n.inputSlots)
			{
				this.LayoutSlot(current, current.title, false, true, false, Styles.varPinIn);
			}
			n.NodeUI(this);
			foreach (Slot current2 in n.outputSlots)
			{
				this.LayoutSlot(current2, current2.title, true, false, true, Styles.varPinOut);
			}
			this.DragNodes();
		}

		protected void DragNodes()
		{
			Event current = Event.current;
			int controlID = GUIUtility.GetControlID(GraphGUI.kDragNodesControlID, FocusType.Passive);
			switch (current.GetTypeForControl(controlID))
			{
			case EventType.MouseDown:
				if (current.button == 0)
				{
					this.m_LastMousePosition = GUIClip.Unclip(current.mousePosition);
					this.m_DragNodeDistance = Vector2.zero;
					foreach (Node current2 in this.selection)
					{
						this.m_InitialDragNodePositions[current2] = current2.position;
						current2.BeginDrag();
					}
					GUIUtility.hotControl = controlID;
					current.Use();
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID)
				{
					foreach (Node current3 in this.selection)
					{
						current3.EndDrag();
					}
					this.m_InitialDragNodePositions.Clear();
					GUIUtility.hotControl = 0;
					current.Use();
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID)
				{
					this.m_DragNodeDistance += GUIClip.Unclip(current.mousePosition) - this.m_LastMousePosition;
					this.m_LastMousePosition = GUIClip.Unclip(current.mousePosition);
					foreach (Node current4 in this.selection)
					{
						Rect position = current4.position;
						Rect rect = this.m_InitialDragNodePositions[current4];
						position.x = rect.x + this.m_DragNodeDistance.x;
						position.y = rect.y + this.m_DragNodeDistance.y;
						current4.position = GraphGUI.SnapPositionToGrid(position);
						current4.OnDrag();
						current4.Dirty();
					}
					current.Use();
				}
				break;
			case EventType.KeyDown:
				if (GUIUtility.hotControl == controlID && current.keyCode == KeyCode.Escape)
				{
					foreach (Node current5 in this.selection)
					{
						current5.position = GraphGUI.SnapPositionToGrid(this.m_InitialDragNodePositions[current5]);
						current5.Dirty();
					}
					GUIUtility.hotControl = 0;
					current.Use();
				}
				break;
			}
		}

		protected static Rect SnapPositionToGrid(Rect position)
		{
			int num = Mathf.RoundToInt(position.x / 12f);
			int num2 = Mathf.RoundToInt(position.y / 12f);
			position.x = (float)num * 12f;
			position.y = (float)num2 * 12f;
			return position;
		}

		protected static float CeilValueToGrid(float value)
		{
			return Mathf.Ceil(value / 12f) * 12f;
		}

		protected void SelectNode(Node node)
		{
			Event current = Event.current;
			if (current.type == EventType.MouseDown && (current.button == 0 || current.button == 1) && current.clickCount == 1)
			{
				if (EditorGUI.actionKey || current.shift)
				{
					if (this.selection.Contains(node))
					{
						this.selection.Remove(node);
					}
					else
					{
						this.selection.Add(node);
					}
					current.Use();
				}
				else
				{
					if (!this.selection.Contains(node))
					{
						this.ClearSelection();
						this.selection.Add(node);
					}
					HandleUtility.Repaint();
				}
				this.UpdateUnitySelection();
				GUIUtility.keyboardControl = 0;
				EditorGUI.EndEditingActiveTextField();
			}
		}

		protected virtual void UpdateUnitySelection()
		{
			List<UnityEngine.Object> list = new List<UnityEngine.Object>(this.selection.ToArray());
			Selection.objects = list.ToArray();
		}

		protected void DragSelection(Rect position)
		{
			int controlID = GUIUtility.GetControlID(GraphGUI.kDragSelectionControlID, FocusType.Passive);
			Event current = Event.current;
			switch (current.GetTypeForControl(controlID))
			{
			case EventType.MouseDown:
				if (position.Contains(current.mousePosition) && current.button == 0 && current.clickCount != 2 && !current.alt)
				{
					GUIUtility.hotControl = controlID;
					this.m_DragStartPoint = current.mousePosition;
					this.m_OldSelection = new List<Node>(this.selection);
					Edge edge = this.edgeGUI.FindClosestEdge();
					if (edge != null)
					{
						if (!EditorGUI.actionKey && !current.shift)
						{
							this.ClearSelection();
						}
						int item = this.m_Graph.edges.IndexOf(edge);
						if (this.edgeGUI.edgeSelection.Contains(item))
						{
							this.edgeGUI.edgeSelection.Remove(item);
						}
						else
						{
							this.edgeGUI.edgeSelection.Add(item);
						}
						current.Use();
					}
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID)
				{
					GUIUtility.hotControl = 0;
					this.m_OldSelection.Clear();
					this.UpdateUnitySelection();
					this.m_IsDraggingSelection = GraphGUI.SelectionDragMode.None;
					current.Use();
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID)
				{
					if (!EditorGUI.actionKey && !current.shift && this.m_IsDraggingSelection == GraphGUI.SelectionDragMode.Pick)
					{
						this.ClearSelection();
					}
					this.m_IsDraggingSelection = GraphGUI.SelectionDragMode.Rect;
					this.SelectNodesInRect(GraphGUI.FromToRect(this.m_DragStartPoint, current.mousePosition));
					current.Use();
				}
				break;
			case EventType.KeyDown:
				if (this.m_IsDraggingSelection != GraphGUI.SelectionDragMode.None && current.keyCode == KeyCode.Escape)
				{
					this.selection = this.m_OldSelection;
					GUIUtility.hotControl = 0;
					this.m_IsDraggingSelection = GraphGUI.SelectionDragMode.None;
					current.Use();
				}
				break;
			case EventType.Repaint:
				if (this.m_IsDraggingSelection == GraphGUI.SelectionDragMode.Rect)
				{
					Styles.selectionRect.Draw(GraphGUI.FromToRect(this.m_DragStartPoint, current.mousePosition), false, false, false, false);
				}
				break;
			}
		}

		private void SelectNodesInRect(Rect r)
		{
			this.selection.Clear();
			foreach (Node current in this.m_Graph.nodes)
			{
				Rect position = current.position;
				if (position.xMax >= r.x && position.x <= r.xMax && position.yMax >= r.y && position.y <= r.yMax)
				{
					this.selection.Add(current);
				}
			}
			foreach (Edge current2 in this.m_Graph.edges)
			{
				if (this.selection.Contains(current2.fromSlot.node) && this.selection.Contains(current2.toSlot.node))
				{
					this.edgeGUI.edgeSelection.Add(this.m_Graph.edges.IndexOf(current2));
				}
			}
		}

		internal static Rect FromToRect(Vector2 start, Vector2 end)
		{
			Rect result = new Rect(start.x, start.y, end.x - start.x, end.y - start.y);
			if (result.width < 0f)
			{
				result.x += result.width;
				result.width = -result.width;
			}
			if (result.height < 0f)
			{
				result.y += result.height;
				result.height = -result.height;
			}
			return result;
		}

		public void LayoutSlot(Slot s, string title, bool allowStartDrag, bool allowEndDrag, bool allowMultiple, GUIStyle style)
		{
			int controlID = GUIUtility.GetControlID(FocusType.Passive);
			GraphGUI.kTempContent.text = title;
			Rect rect = GUILayoutUtility.GetRect(GraphGUI.kTempContent, style);
			if (Event.current.type == EventType.Layout || Event.current.type == EventType.Used)
			{
				return;
			}
			Color backgroundColor = GUI.backgroundColor;
			if (s.edges.Count > 0)
			{
				GUI.backgroundColor = s.edges[0].color;
			}
			this.DoSlot(controlID, rect, title, s, allowStartDrag, allowEndDrag, allowMultiple, style);
			GUI.backgroundColor = backgroundColor;
		}

		public void Slot(Rect position, string title, Slot s, bool allowStartDrag, bool allowEndDrag, bool allowMultiple, GUIStyle style)
		{
			int controlID = GUIUtility.GetControlID(FocusType.Passive);
			this.DoSlot(controlID, position, title, s, allowStartDrag, allowEndDrag, allowMultiple, style);
		}

		private void DoSlot(int id, Rect position, string title, Slot slot, bool allowStartDrag, bool allowEndDrag, bool allowMultiple, GUIStyle style)
		{
			slot.m_Position = GUIClip.Unclip(position);
			Event current = Event.current;
			switch (current.GetTypeForControl(id))
			{
			case EventType.MouseDown:
				if (position.Contains(Event.current.mousePosition) && current.button == 0)
				{
					this.edgeGUI.BeginSlotDragging(slot, allowStartDrag, allowEndDrag);
				}
				break;
			case EventType.MouseUp:
				if (position.Contains(current.mousePosition) && current.button == 0)
				{
					this.edgeGUI.EndSlotDragging(slot, allowMultiple);
				}
				break;
			case EventType.MouseDrag:
				if (position.Contains(current.mousePosition) && current.button == 0)
				{
					this.edgeGUI.SlotDragging(slot, allowEndDrag, allowMultiple);
				}
				break;
			case EventType.Repaint:
				style.Draw(position, new GUIContent(title), id, GraphGUI.IsSlotActive(slot));
				break;
			}
		}

		private static bool IsSlotActive(Slot slot)
		{
			return slot.edges.Count > 0 || (!slot.isFlowSlot && !slot.isOutputDataSlot && slot.dataType != null && slot.GetProperty().value != null);
		}

		public virtual void OnNodeLibraryGUI(EditorWindow host, Rect position)
		{
		}

		public void BeginToolbarGUI(Rect position)
		{
			GUI.BeginGroup(position);
			GUILayout.BeginHorizontal(GUIContent.none, EditorStyles.toolbar, new GUILayoutOption[0]);
		}

		public void EndToolbarGUI()
		{
			GUILayout.EndHorizontal();
			GUI.EndGroup();
		}

		public virtual void OnToolbarGUI()
		{
			GUI.enabled = (this.selection.Count > 0);
			if (GUILayout.Button("Group " + this.selection.Count, EditorStyles.toolbarButton, new GUILayoutOption[0]))
			{
				Vector2 midPoint = GraphGUI.GetMidPoint(this.selection);
				Debug.Log("Grouping" + this.selection.Count + " nodes");
				Node node = GroupNode.FromNodes("GroupNode", this.selection, this.m_Graph.GetType());
				node.position = new Rect(midPoint.x - node.position.x / 2f, midPoint.y - node.position.y / 2f, node.position.width, node.position.height);
			}
			GUI.enabled = true;
			GUILayout.FlexibleSpace();
			GUI.enabled = EditorApplication.isPlaying;
			if (GUILayout.Button("Reload", EditorStyles.toolbarButton, new GUILayoutOption[0]))
			{
				InternalEditorUtility.RequestScriptReload();
			}
			GUI.enabled = true;
			GUILayout.Space(5f);
		}

		protected virtual void AddNode(Node node)
		{
			this.m_Graph.AddNode(node);
			if (node.position.x == 0f && node.position.y == 0f)
			{
				this.m_LastNodeAddedPosition.y = this.m_LastNodeAddedPosition.y + 40f;
				this.m_LastNodeAddedPosition.x = this.m_LastNodeAddedPosition.x + 40f;
				if (this.m_LastNodeAddedPosition.y >= 250f)
				{
					this.m_LastNodeAddedPosition.y = 40f;
				}
				if (this.m_LastNodeAddedPosition.x >= 700f)
				{
					this.m_LastNodeAddedPosition.x = 40f;
				}
				node.position = new Rect(this.m_LastNodeAddedPosition.x, this.m_LastNodeAddedPosition.y, 0f, 0f);
			}
		}

		internal void InternalOnSelectionChange()
		{
			if (Selection.activeObject is Node)
			{
				return;
			}
			this.m_Tools.Clear();
			this.AddTools();
		}

		public virtual void AddTools()
		{
		}

		public virtual void OnEnable()
		{
			this.AddTools();
		}

		private static Vector2 GetMidPoint(List<Node> nodes)
		{
			float num = 3.40282347E+38f;
			float num2 = 3.40282347E+38f;
			float num3 = -3.40282347E+38f;
			float num4 = -3.40282347E+38f;
			foreach (Node current in nodes)
			{
				num = Math.Min(current.position.x, num);
				num2 = Math.Min(current.position.y, num2);
				num3 = Math.Max(current.position.x, num3);
				num4 = Math.Max(current.position.y, num4);
			}
			return new Vector2((num3 + num) / 2f, (num4 + num2) / 2f);
		}

		public void CenterGraph()
		{
			this.m_CenterGraph = true;
		}

		public virtual void ClearSelection()
		{
			this.selection.Clear();
			this.edgeGUI.edgeSelection.Clear();
		}

		public virtual void SyncGraphToUnitySelection()
		{
		}

		public virtual void DoBackgroundClickAction()
		{
		}
	}
}
