using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.Graphs.AnimationStateMachine
{
	internal class EdgeGUI : IEdgeGUI
	{
		private const float kEdgeWidth = 5f;

		private const float kArrowEdgeWidth = 2f;

		private const float kEdgeClickWidth = 10f;

		private const float kEdgeToSelfOffset = 30f;

		private Edge m_DraggingEdge;

		private static Slot s_TargetDraggingSlot;

		public UnityEditor.Graphs.GraphGUI host
		{
			get;
			set;
		}

		private GraphGUI smHost
		{
			get
			{
				return this.host as GraphGUI;
			}
		}

		public List<int> edgeSelection
		{
			get;
			set;
		}

		private static Vector3 edgeToSelfOffsetVector
		{
			get
			{
				return new Vector3(0f, 30f, 0f);
			}
		}

		private static Color selectedEdgeColor
		{
			get
			{
				return new Color(0.42f, 0.7f, 1f, 1f);
			}
		}

		private static Color selectorTransitionColor
		{
			get
			{
				return new Color(0.5f, 0.5f, 0.5f, 1f);
			}
		}

		private static Color defaultTransitionColor
		{
			get
			{
				return new Color(0.6f, 0.4f, 0f, 1f);
			}
		}

		public EdgeGUI()
		{
			this.edgeSelection = new List<int>();
		}

		public void DoEdges()
		{
			if (Event.current.type == EventType.Repaint)
			{
				int num = 0;
				foreach (Edge current in this.host.graph.edges)
				{
					Texture2D tex = (Texture2D)Styles.connectionTexture.image;
					Color color = current.color;
					EdgeInfo edgeInfo = this.smHost.stateMachineGraph.GetEdgeInfo(current);
					if (edgeInfo != null)
					{
						if (edgeInfo.hasDefaultState)
						{
							color = EdgeGUI.defaultTransitionColor;
						}
						else if (edgeInfo.edgeType == EdgeType.Transition)
						{
							color = EdgeGUI.selectorTransitionColor;
						}
					}
					bool flag = false;
					int num2 = 0;
					while (num2 < this.edgeSelection.Count && !flag)
					{
						if (this.edgeSelection[num2] == num)
						{
							color = EdgeGUI.selectedEdgeColor;
							flag = true;
						}
						num2++;
					}
					this.DrawEdge(current, tex, color, edgeInfo);
					num++;
				}
			}
			if (this.IsDragging())
			{
				EdgeGUI.s_TargetDraggingSlot = null;
				Event.current.Use();
			}
			if (this.ShouldStopDragging())
			{
				this.EndDragging();
				Event.current.Use();
			}
		}

		public void EndDragging()
		{
			if (this.m_DraggingEdge == null)
			{
				return;
			}
			this.host.graph.RemoveEdge(this.m_DraggingEdge);
			this.m_DraggingEdge = null;
			this.smHost.tool.Repaint();
		}

		private bool IsDragging()
		{
			return Event.current.type == EventType.MouseMove && this.m_DraggingEdge != null;
		}

		private bool ShouldStopDragging()
		{
			return Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape && this.m_DraggingEdge != null;
		}

		private void DrawEdge(Edge edge, Texture2D tex, Color color, EdgeInfo info)
		{
			Vector3 cross;
			Vector3[] edgePoints = EdgeGUI.GetEdgePoints(edge, out cross);
			Handles.color = color;
			if (edgePoints[0] == edgePoints[1])
			{
				EdgeGUI.DrawArrows(color, -Vector3.right, new Vector3[]
				{
					edgePoints[0] + new Vector3(0f, 31f, 0f),
					edgePoints[0] + new Vector3(0f, 30f, 0f)
				}, info, true);
			}
			else
			{
				Handles.DrawAAPolyLine(tex, 5f, new Vector3[]
				{
					edgePoints[0],
					edgePoints[1]
				});
				EdgeGUI.DrawArrows(color, cross, edgePoints, info, false);
				if (info != null)
				{
					bool flag = this.smHost.liveLinkInfo.srcNode == edge.fromSlot.node;
					bool flag2 = this.smHost.liveLinkInfo.dstNode == edge.toSlot.node;
					if ((flag && flag2) || (flag2 && this.smHost.liveLinkInfo.transitionInfo.entry && edge.fromSlot.node is EntryNode) || (flag && this.smHost.liveLinkInfo.transitionInfo.exit && edge.toSlot.node is ExitNode) || (flag2 && this.smHost.liveLinkInfo.transitionInfo.anyState && edge.fromSlot.node is AnyStateNode))
					{
						float num = this.smHost.liveLinkInfo.transitionInfo.normalizedTime;
						if (this.smHost.liveLinkInfo.currentStateMachine != this.smHost.liveLinkInfo.nextStateMachine)
						{
							num = num % 0.5f / 0.5f;
						}
						Handles.color = EdgeGUI.selectedEdgeColor;
						Handles.DrawAAPolyLine(10f, new Vector3[]
						{
							edgePoints[0],
							edgePoints[1] * num + edgePoints[0] * (1f - num)
						});
					}
				}
			}
		}

		private static void DrawArrows(Color color, Vector3 cross, Vector3[] edgePoints, EdgeInfo info, bool isSelf)
		{
			Vector3 a = edgePoints[1] - edgePoints[0];
			Vector3 normalized = a.normalized;
			Vector3 a2 = a * 0.5f + edgePoints[0];
			a2 -= cross * 0.5f;
			int num = 1;
			if (info != null && info.hasMultipleTransitions)
			{
				num = 3;
			}
			for (int i = 0; i < num; i++)
			{
				Color color2 = color;
				if (info != null)
				{
					if (info.debugState == EdgeDebugState.MuteAll)
					{
						color2 = Color.red;
					}
					else if (info.debugState == EdgeDebugState.SoloAll)
					{
						color2 = Color.green;
					}
					else if (i == 0)
					{
						if (info.debugState == EdgeDebugState.MuteSome || info.debugState == EdgeDebugState.MuteAndSolo)
						{
							color2 = Color.red;
						}
						if (info.debugState == EdgeDebugState.SoloSome)
						{
							color2 = Color.green;
						}
					}
					else if (i == 2 && info.debugState == EdgeDebugState.MuteAndSolo)
					{
						color2 = Color.green;
					}
					if (i == 1 && info.edgeType == EdgeType.MixedTransition)
					{
						color2 = EdgeGUI.selectorTransitionColor;
					}
				}
				Vector3 center = a2 + (float)((num != 1) ? (i - 1) : i) * 13f * ((!isSelf) ? normalized : cross);
				EdgeGUI.DrawArrow(color2, cross, normalized, center);
			}
		}

		private static void DrawArrow(Color color, Vector3 cross, Vector3 direction, Vector3 center)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			Vector3[] array = new Vector3[4];
			array[0] = center + direction * 5f;
			array[1] = center - direction * 5f + cross * 5f;
			array[2] = center - direction * 5f - cross * 5f;
			array[3] = array[0];
			Shader.SetGlobalColor("_HandleColor", color);
			HandleUtility.ApplyWireMaterial();
			GL.Begin(4);
			GL.Color(color);
			GL.Vertex(array[0]);
			GL.Vertex(array[1]);
			GL.Vertex(array[2]);
			GL.End();
			Handles.color = color;
			Handles.DrawAAPolyLine((Texture2D)Styles.connectionTexture.image, 2f, array);
		}

		private static Vector3[] GetEdgePoints(Edge edge)
		{
			Vector3 vector;
			return EdgeGUI.GetEdgePoints(edge, out vector);
		}

		private static Vector3[] GetEdgePoints(Edge edge, out Vector3 cross)
		{
			Vector3[] array = new Vector3[]
			{
				EdgeGUI.GetEdgeStartPosition(edge),
				EdgeGUI.GetEdgeEndPosition(edge)
			};
			cross = Vector3.Cross((array[0] - array[1]).normalized, Vector3.forward);
			array[0] += cross * 5f;
			if (!EdgeGUI.IsEdgeBeingDragged(edge))
			{
				array[1] += cross * 5f;
			}
			return array;
		}

		private static Vector3 GetEdgeStartPosition(Edge edge)
		{
			return EdgeGUI.GetNodeCenterFromSlot(edge.fromSlot);
		}

		private static Vector3 GetEdgeEndPosition(Edge edge)
		{
			if (!EdgeGUI.IsEdgeBeingDragged(edge))
			{
				return EdgeGUI.GetNodeCenterFromSlot(edge.toSlot);
			}
			if (EdgeGUI.s_TargetDraggingSlot != null)
			{
				return EdgeGUI.GetNodeCenterFromSlot(EdgeGUI.s_TargetDraggingSlot);
			}
			return Event.current.mousePosition;
		}

		private static bool IsEdgeBeingDragged(Edge edge)
		{
			return edge.toSlot == null;
		}

		private static Vector3 GetNodeCenterFromSlot(Slot slot)
		{
			return slot.node.position.center;
		}

		public void DoDraggedEdge()
		{
		}

		public void BeginSlotDragging(Slot slot, bool allowStartDrag, bool allowEndDrag)
		{
			this.EndDragging();
			Edge edge = new Edge(slot, null);
			this.host.graph.edges.Add(edge);
			this.m_DraggingEdge = edge;
			this.smHost.tool.wantsMouseMove = true;
		}

		public void SlotDragging(Slot slot, bool allowEndDrag, bool allowMultiple)
		{
			if (this.m_DraggingEdge == null)
			{
				return;
			}
			if (slot.node is AnyStateNode)
			{
				return;
			}
			EdgeGUI.s_TargetDraggingSlot = slot;
			Event.current.Use();
		}

		public void EndSlotDragging(Slot slot, bool allowMultiple)
		{
			if (this.m_DraggingEdge == null)
			{
				return;
			}
			if (slot.node is AnyStateNode)
			{
				return;
			}
			Node node = this.m_DraggingEdge.fromSlot.node as Node;
			Node toNode = slot.node as Node;
			if (slot == this.m_DraggingEdge.fromSlot)
			{
				this.host.graph.RemoveEdge(this.m_DraggingEdge);
			}
			else
			{
				this.m_DraggingEdge.toSlot = slot;
				this.host.selection.Clear();
				this.host.selection.Add(node);
				Selection.activeObject = node.selectionObject;
				node.Connect(toNode, this.m_DraggingEdge);
			}
			this.m_DraggingEdge = null;
			EdgeGUI.s_TargetDraggingSlot = null;
			Event.current.Use();
			this.smHost.tool.wantsMouseMove = false;
			AnimatorControllerTool.tool.RebuildGraph();
		}

		public Edge FindClosestEdge()
		{
			Edge result = null;
			float num = float.PositiveInfinity;
			Vector3 vector = Event.current.mousePosition;
			foreach (Edge current in this.host.graph.edges)
			{
				Vector3[] edgePoints = EdgeGUI.GetEdgePoints(current);
				float num2;
				if (edgePoints[0] == edgePoints[1])
				{
					num2 = Vector3.Distance(EdgeGUI.edgeToSelfOffsetVector + edgePoints[0], vector);
				}
				else
				{
					num2 = HandleUtility.DistancePointLine(vector, edgePoints[0], edgePoints[1]);
				}
				if (num2 < num && num2 < 10f)
				{
					num = num2;
					result = current;
				}
			}
			return result;
		}
	}
}
