using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.Graphs
{
	public class EdgeGUI : IEdgeGUI
	{
		public enum EdgeStyle
		{
			Angular,
			Curvy
		}

		private const float kEdgeWidth = 3f;

		private const float kEdgeSlotYOffset = 9f;

		private const float kEdgeCurveRadius = 5f;

		private const float kEdgeCurveTangentRatio = 0.6f;

		public EdgeGUI.EdgeStyle edgeStyle = EdgeGUI.EdgeStyle.Curvy;

		public static readonly Color kFunctionEdgeColor = new Color(1f, 1f, 1f);

		public static readonly Color kObjectTypeEdgeColor = new Color(0.65f, 1f, 0.65f);

		public static readonly Color kSimpleTypeEdgeColor = new Color(0.6f, 0.75f, 1f);

		private static Slot s_DragSourceSlot;

		private static Slot s_DropTarget;

		private Edge dontDrawEdge
		{
			get;
			set;
		}

		private Edge moveEdge
		{
			get;
			set;
		}

		public List<int> edgeSelection
		{
			get;
			set;
		}

		public GraphGUI host
		{
			get;
			set;
		}

		public EdgeGUI()
		{
			this.edgeSelection = new List<int>();
		}

		public void DoEdges()
		{
			int num = 0;
			int num2 = 0;
			if (Event.current.type == EventType.Repaint)
			{
				foreach (Edge current in this.host.graph.edges)
				{
					if (current != this.dontDrawEdge && current != this.moveEdge)
					{
						Texture2D tex = (Texture2D)Styles.connectionTexture.image;
						if (num < this.edgeSelection.Count && this.edgeSelection[num] == num2)
						{
							num++;
							tex = (Texture2D)Styles.selectedConnectionTexture.image;
						}
						Color color = (!current.toSlot.isFlowSlot) ? EdgeGUI.kSimpleTypeEdgeColor : EdgeGUI.kFunctionEdgeColor;
						if (!current.toSlot.isFlowSlot && current.toSlot.dataType.IsSubclassOf(typeof(UnityEngine.Object)))
						{
							color = EdgeGUI.kObjectTypeEdgeColor;
						}
						color *= current.color;
						EdgeGUI.DrawEdge(current, tex, color, this.edgeStyle);
						num2++;
					}
				}
			}
			if (EdgeGUI.s_DragSourceSlot != null && Event.current.type == EventType.MouseUp)
			{
				if (this.moveEdge != null)
				{
					this.host.graph.RemoveEdge(this.moveEdge);
					this.moveEdge = null;
				}
				if (EdgeGUI.s_DropTarget == null)
				{
					this.EndDragging();
					Event.current.Use();
				}
			}
		}

		public void DoDraggedEdge()
		{
			if (EdgeGUI.s_DragSourceSlot != null)
			{
				EventType typeForControl = Event.current.GetTypeForControl(0);
				if (typeForControl != EventType.MouseDrag)
				{
					if (typeForControl == EventType.Repaint)
					{
						Rect position = EdgeGUI.s_DragSourceSlot.m_Position;
						Vector2 end = Event.current.mousePosition;
						if (EdgeGUI.s_DropTarget != null)
						{
							Rect position2 = EdgeGUI.s_DropTarget.m_Position;
							end = GUIClip.Clip(new Vector2(position2.x, position2.y + 9f));
						}
						EdgeGUI.DrawEdge(GUIClip.Clip(new Vector2(position.xMax, position.y + 9f)), end, (Texture2D)Styles.selectedConnectionTexture.image, Color.white, this.edgeStyle);
					}
				}
				else
				{
					EdgeGUI.s_DropTarget = null;
					this.dontDrawEdge = null;
					Event.current.Use();
				}
			}
		}

		public void BeginSlotDragging(Slot slot, bool allowStartDrag, bool allowEndDrag)
		{
			if (allowStartDrag)
			{
				EdgeGUI.s_DragSourceSlot = slot;
				Event.current.Use();
			}
			if (allowEndDrag && slot.edges.Count > 0)
			{
				this.moveEdge = slot.edges[slot.edges.Count - 1];
				EdgeGUI.s_DragSourceSlot = this.moveEdge.fromSlot;
				EdgeGUI.s_DropTarget = slot;
				Event.current.Use();
			}
		}

		public void SlotDragging(Slot slot, bool allowEndDrag, bool allowMultiple)
		{
			if (allowEndDrag && EdgeGUI.s_DragSourceSlot != null && EdgeGUI.s_DragSourceSlot != slot)
			{
				if (EdgeGUI.s_DropTarget != slot && slot.node.graph.CanConnect(EdgeGUI.s_DragSourceSlot, slot) && !slot.node.graph.Connected(EdgeGUI.s_DragSourceSlot, slot))
				{
					EdgeGUI.s_DropTarget = slot;
					if (slot.edges.Count > 0 && !allowMultiple)
					{
						this.dontDrawEdge = slot.edges[slot.edges.Count - 1];
					}
				}
				Event.current.Use();
			}
		}

		public void EndSlotDragging(Slot slot, bool allowMultiple)
		{
			if (EdgeGUI.s_DropTarget == slot)
			{
				if (this.moveEdge != null)
				{
					slot.node.graph.RemoveEdge(this.moveEdge);
				}
				while (EdgeGUI.s_DropTarget.edges.Count > 0)
				{
					if (allowMultiple)
					{
						break;
					}
					slot.node.graph.RemoveEdge(EdgeGUI.s_DropTarget.edges[0]);
				}
				try
				{
					slot.node.graph.Connect(EdgeGUI.s_DragSourceSlot, slot);
				}
				finally
				{
					this.EndDragging();
					slot.node.graph.Dirty();
					Event.current.Use();
				}
				GUIUtility.ExitGUI();
			}
		}

		public void EndDragging()
		{
			EdgeGUI.s_DragSourceSlot = (EdgeGUI.s_DropTarget = null);
			Edge edge = null;
			this.moveEdge = edge;
			this.dontDrawEdge = edge;
		}

		private static void GetEdgeEndPoints(Edge e, out Vector2 start, out Vector2 end)
		{
			start = GUIClip.Clip(new Vector2(e.fromSlot.m_Position.xMax, e.fromSlot.m_Position.y + 9f));
			end = GUIClip.Clip(new Vector2(e.toSlot.m_Position.x, e.toSlot.m_Position.y + 9f));
		}

		private static void DrawEdge(Edge e, Texture2D tex, Color color, EdgeGUI.EdgeStyle style)
		{
			Vector2 start;
			Vector2 end;
			EdgeGUI.GetEdgeEndPoints(e, out start, out end);
			EdgeGUI.DrawEdge(start, end, tex, color, style);
		}

		private static void DrawEdge(Vector2 start, Vector2 end, Texture2D tex, Color color, EdgeGUI.EdgeStyle style)
		{
			if (style != EdgeGUI.EdgeStyle.Angular)
			{
				if (style == EdgeGUI.EdgeStyle.Curvy)
				{
					Vector3[] array;
					Vector3[] array2;
					EdgeGUI.GetCurvyConnectorValues(start, end, out array, out array2);
					Handles.DrawBezier(array[0], array[1], array2[0], array2[1], color, tex, 3f);
				}
			}
			else
			{
				Vector3[] array;
				Vector3[] array2;
				EdgeGUI.GetAngularConnectorValues(start, end, out array, out array2);
				EdgeGUI.DrawRoundedPolyLine(array, array2, tex, color);
			}
		}

		private static void GetCurvyConnectorValues(Vector2 start, Vector2 end, out Vector3[] points, out Vector3[] tangents)
		{
			points = new Vector3[]
			{
				start,
				end
			};
			tangents = new Vector3[2];
			float arg_56_0 = (start.y >= end.y) ? 0.7f : 0.3f;
			float num = 0.5f;
			float num2 = 1f - num;
			float num3 = 0f;
			if (start.x > end.x)
			{
				num = (num2 = -0.25f);
				float f = (start.x - end.x) / (start.y - end.y);
				if (Mathf.Abs(f) > 0.5f)
				{
					float num4 = (Mathf.Abs(f) - 0.5f) / 8f;
					num4 = Mathf.Sqrt(num4);
					num3 = Mathf.Min(num4 * 80f, 80f);
					if (start.y > end.y)
					{
						num3 = -num3;
					}
				}
			}
			float d = Mathf.Clamp01(((start - end).magnitude - 10f) / 50f);
			tangents[0] = start + new Vector2((end.x - start.x) * num + 30f, num3) * d;
			tangents[1] = end + new Vector2((end.x - start.x) * -num2 - 30f, -num3) * d;
		}

		private static void GetAngularConnectorValues(Vector2 start, Vector2 end, out Vector3[] points, out Vector3[] tangents)
		{
			Vector2 a = start - end;
			Vector2 vector = a / 2f + end;
			Vector2 vector2 = new Vector2(Mathf.Sign(a.x), Mathf.Sign(a.y));
			Vector2 vector3 = new Vector2(Mathf.Min(Mathf.Abs(a.x / 2f), 5f), Mathf.Min(Mathf.Abs(a.y / 2f), 5f));
			points = new Vector3[]
			{
				start,
				new Vector3(vector.x + vector3.x * vector2.x, start.y),
				new Vector3(vector.x, start.y - vector3.y * vector2.y),
				new Vector3(vector.x, end.y + vector3.y * vector2.y),
				new Vector3(vector.x - vector3.x * vector2.x, end.y),
				end
			};
			tangents = new Vector3[]
			{
				(points[1] - points[0]).normalized * vector3.x * 0.6f + points[1],
				(points[2] - points[3]).normalized * vector3.y * 0.6f + points[2],
				(points[3] - points[2]).normalized * vector3.y * 0.6f + points[3],
				(points[4] - points[5]).normalized * vector3.x * 0.6f + points[4]
			};
		}

		private static void DrawRoundedPolyLine(Vector3[] points, Vector3[] tangets, Texture2D tex, Color color)
		{
			Handles.color = color;
			for (int i = 0; i < points.Length; i += 2)
			{
				Handles.DrawAAPolyLine(tex, 3f, new Vector3[]
				{
					points[i],
					points[i + 1]
				});
			}
			for (int j = 0; j < tangets.Length; j += 2)
			{
				Handles.DrawBezier(points[j + 1], points[j + 2], tangets[j], tangets[j + 1], color, tex, 3f);
			}
		}

		public Edge FindClosestEdge()
		{
			Vector2 mousePosition = Event.current.mousePosition;
			float num = float.PositiveInfinity;
			Edge result = null;
			foreach (Edge current in this.host.graph.edges)
			{
				Vector2 start;
				Vector2 end;
				EdgeGUI.GetEdgeEndPoints(current, out start, out end);
				Vector3[] array;
				if (this.edgeStyle == EdgeGUI.EdgeStyle.Angular)
				{
					Vector3[] array2;
					EdgeGUI.GetAngularConnectorValues(start, end, out array, out array2);
				}
				else
				{
					Vector3[] array2;
					EdgeGUI.GetCurvyConnectorValues(start, end, out array, out array2);
				}
				for (int i = 0; i < array.Length; i += 2)
				{
					float num2 = HandleUtility.DistancePointLine(mousePosition, array[i], array[i + 1]);
					if (num2 < num)
					{
						num = num2;
						result = current;
					}
				}
			}
			if (num > 10f)
			{
				result = null;
			}
			return result;
		}
	}
}
