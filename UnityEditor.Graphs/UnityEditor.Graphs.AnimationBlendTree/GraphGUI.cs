using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor.Graphs.AnimationBlendTree
{
	internal class GraphGUI : UnityEditor.Graphs.GraphGUI
	{
		private float[] m_Weights = new float[0];

		public Graph blendTreeGraph
		{
			get
			{
				return base.graph as Graph;
			}
		}

		private AnimatorControllerTool m_Tool
		{
			get
			{
				return this.m_Host as AnimatorControllerTool;
			}
		}

		private void HandleNodeInput(Node node)
		{
			Event current = Event.current;
			if (current.type == EventType.MouseDown && current.button == 0)
			{
				this.selection.Clear();
				this.selection.Add(node);
				Selection.activeObject = node.motion;
				Node rootNode = this.blendTreeGraph.rootNode;
				if (current.clickCount == 2 && node.motion is UnityEditor.Animations.BlendTree && rootNode != node.motion)
				{
					this.selection.Clear();
					Stack<Node> stack = new Stack<Node>();
					Node node2 = node;
					while (node2.motion is UnityEditor.Animations.BlendTree && node2 != rootNode)
					{
						stack.Push(node2);
						node2 = node2.parent;
					}
					foreach (Node current2 in stack)
					{
						this.m_Tool.AddBreadCrumb(current2.motion);
					}
				}
				current.Use();
			}
			if (current.type == EventType.MouseDown && current.button == 1)
			{
				GenericMenu genericMenu = new GenericMenu();
				UnityEditor.Animations.BlendTree blendTree = node.motion as UnityEditor.Animations.BlendTree;
				if (blendTree)
				{
					genericMenu.AddItem(new GUIContent("Add Motion"), false, new GenericMenu.MenuFunction2(this.CreateMotionCallback), blendTree);
					genericMenu.AddItem(new GUIContent("Add Blend Tree"), false, new GenericMenu.MenuFunction2(this.CreateBlendTreeCallback), blendTree);
				}
				if (node != this.blendTreeGraph.rootNode)
				{
					genericMenu.AddItem(new GUIContent("Delete"), false, new GenericMenu.MenuFunction2(this.DeleteNodeCallback), node);
				}
				genericMenu.ShowAsContext();
				Event.current.Use();
			}
		}

		private string LimitStringWidth(string content, float width, GUIStyle style)
		{
			int numCharactersThatFitWithinWidth = style.GetNumCharactersThatFitWithinWidth(content, width);
			if (content.Length > numCharactersThatFitWithinWidth)
			{
				return content.Substring(0, Mathf.Min(numCharactersThatFitWithinWidth - 3, content.Length)) + "...";
			}
			return content;
		}

		public override void NodeGUI(UnityEditor.Graphs.Node n)
		{
			Node node = n as Node;
			UnityEditor.Animations.BlendTree blendTree = node.motion as UnityEditor.Animations.BlendTree;
			GUILayout.BeginVertical(new GUILayoutOption[]
			{
				GUILayout.Width(200f)
			});
			foreach (Slot current in n.inputSlots)
			{
				base.LayoutSlot(current, this.LimitStringWidth(current.title, 180f, Styles.varPinIn), false, false, false, Styles.varPinIn);
			}
			foreach (Slot current2 in n.outputSlots)
			{
				base.LayoutSlot(current2, this.LimitStringWidth(current2.title, 180f, Styles.varPinOut), false, false, false, Styles.varPinOut);
			}
			n.NodeUI(this);
			EditorGUIUtility.labelWidth = 50f;
			if (blendTree)
			{
				int recursiveBlendParameterCount = blendTree.recursiveBlendParameterCount;
				if (recursiveBlendParameterCount > 0)
				{
					for (int i = 0; i < blendTree.recursiveBlendParameterCount; i++)
					{
						string recursiveBlendParameter = blendTree.GetRecursiveBlendParameter(i);
						float recursiveBlendParameterMin = blendTree.GetRecursiveBlendParameterMin(i);
						float num = blendTree.GetRecursiveBlendParameterMax(i);
						EventType type = Event.current.type;
						if (Event.current.button != 0 && Event.current.isMouse)
						{
							Event.current.type = EventType.Ignore;
						}
						if (Mathf.Approximately(num, recursiveBlendParameterMin))
						{
							num = recursiveBlendParameterMin + 1f;
						}
						EditorGUI.BeginChangeCheck();
						float parameterValue = EditorGUILayout.Slider(GUIContent.Temp(recursiveBlendParameter, recursiveBlendParameter), this.blendTreeGraph.GetParameterValue(recursiveBlendParameter), recursiveBlendParameterMin, num, new GUILayoutOption[0]);
						if (EditorGUI.EndChangeCheck())
						{
							this.blendTreeGraph.SetParameterValue(recursiveBlendParameter, parameterValue);
							InspectorWindow.RepaintAllInspectors();
						}
						if (Event.current.button != 0)
						{
							Event.current.type = type;
						}
					}
				}
				else
				{
					EditorGUILayout.LabelField("No blend parameter to display", new GUILayoutOption[0]);
				}
				if (node.animator != null)
				{
					List<Edge> list = new List<Edge>(n.outputEdges);
					node.UpdateAnimator();
					if (this.m_Weights.Length != list.Count)
					{
						this.m_Weights = new float[list.Count];
					}
					BlendTreePreviewUtility.GetRootBlendTreeChildWeights(node.animator, 0, node.animator.GetCurrentAnimatorStateInfo(0).fullPathHash, this.m_Weights);
					for (int j = 0; j < list.Count; j++)
					{
						Node node2 = list[j].toSlot.node as Node;
						node2.weight = node.weight * this.m_Weights[j];
						list[j].color = node2.weightEdgeColor;
					}
				}
			}
			GUILayout.EndVertical();
			this.HandleNodeInput(n as Node);
		}

		public override void OnGraphGUI()
		{
			this.blendTreeGraph.PopulateParameterValues();
			bool flag = false;
			using (List<UnityEditor.Graphs.Node>.Enumerator enumerator = base.graph.nodes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Node node = (Node)enumerator.Current;
					if (node.controllerDirty)
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				this.blendTreeGraph.BuildFromBlendTree(this.blendTreeGraph.rootBlendTree);
			}
			this.m_Host.BeginWindows();
			using (List<UnityEditor.Graphs.Node>.Enumerator enumerator2 = base.graph.nodes.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					Node node2 = (Node)enumerator2.Current;
					Node n2 = node2;
					bool flag2 = this.selection.Contains(node2);
					Color color = GUI.color;
					GUI.color = ((!flag2) ? node2.weightColor : Color.white);
					GUIStyle nodeStyle = Styles.GetNodeStyle(node2.style, node2.color, flag2);
					Rect screenRect = new Rect(node2.position.x, node2.position.y, 0f, 0f);
					node2.position = GUILayout.Window(node2.GetInstanceID(), screenRect, delegate
					{
						this.NodeGUI(n2);
					}, this.LimitStringWidth(node2.title, 180f, nodeStyle), nodeStyle, new GUILayoutOption[0]);
					GUI.color = color;
				}
			}
			this.m_Host.EndWindows();
			this.edgeGUI.DoEdges();
			this.HandleGraphInput();
		}

		public override void SyncGraphToUnitySelection()
		{
			if (GUIUtility.hotControl != 0)
			{
				return;
			}
			this.selection.Clear();
			UnityEngine.Object[] objects = Selection.objects;
			for (int i = 0; i < objects.Length; i++)
			{
				UnityEngine.Object @object = objects[i];
				Node node = null;
				Motion motion = @object as Motion;
				if (motion != null)
				{
					node = this.blendTreeGraph.FindNode(motion);
				}
				if (node != null)
				{
					this.selection.Add(node);
				}
			}
		}

		private void HandleGraphInput()
		{
			Event current = Event.current;
			EventType type = current.type;
			if (type != EventType.ValidateCommand)
			{
				if (type != EventType.ExecuteCommand)
				{
					if (type != EventType.MouseDown)
					{
						if (type == EventType.KeyDown)
						{
							if (current.keyCode == KeyCode.Delete)
							{
								this.DeleteSelection();
								current.Use();
							}
						}
					}
					else if (current.button == 0 && !current.alt && Event.current.clickCount == 1 && (Application.platform != RuntimePlatform.OSXEditor || !current.control))
					{
						this.DoBackgroundClickAction();
						current.Use();
					}
				}
				else if (current.commandName == "SoftDelete" || current.commandName == "Delete")
				{
					this.DeleteSelection();
					current.Use();
				}
			}
			else if (current.commandName == "SoftDelete" || current.commandName == "Delete")
			{
				current.Use();
			}
		}

		private void DeleteNodeCallback(object obj)
		{
			Node node = obj as Node;
			if (!node)
			{
				return;
			}
			string[] toDelete = new string[]
			{
				node.motion.name
			};
			if (GraphGUI.DeleteNodeDialog(toDelete))
			{
				this.blendTreeGraph.RemoveNodeMotions(new Node[]
				{
					node
				});
				this.blendTreeGraph.BuildFromBlendTree(this.blendTreeGraph.rootBlendTree);
			}
		}

		private void CreateMotionCallback(object obj)
		{
			UnityEditor.Animations.BlendTree blendTree = obj as UnityEditor.Animations.BlendTree;
			if (!blendTree)
			{
				return;
			}
			blendTree.AddChild(null);
			blendTree.SetDirectBlendTreeParameter(blendTree.children.Length - 1, this.m_Tool.animatorController.GetDefaultBlendTreeParameter());
		}

		private void CreateBlendTreeCallback(object obj)
		{
			UnityEditor.Animations.BlendTree blendTree = obj as UnityEditor.Animations.BlendTree;
			if (!blendTree)
			{
				return;
			}
			UnityEditor.Animations.BlendTree blendTree2 = blendTree.CreateBlendTreeChild(0f);
			if (this.m_Tool && this.m_Tool.animatorController != null)
			{
				UnityEditor.Animations.BlendTree arg_5F_0 = blendTree2;
				string defaultBlendTreeParameter = this.m_Tool.animatorController.GetDefaultBlendTreeParameter();
				blendTree2.blendParameterY = defaultBlendTreeParameter;
				arg_5F_0.blendParameter = defaultBlendTreeParameter;
				blendTree.SetDirectBlendTreeParameter(blendTree.children.Length - 1, this.m_Tool.animatorController.GetDefaultBlendTreeParameter());
			}
			else
			{
				blendTree2.blendParameter = blendTree.blendParameter;
				blendTree2.blendParameterY = blendTree.blendParameterY;
			}
		}

		private List<string> CollectSelectionNames()
		{
			List<string> list = new List<string>();
			using (List<UnityEditor.Graphs.Node>.Enumerator enumerator = this.selection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Node node = (Node)enumerator.Current;
					list.Add(node.motion.name);
				}
			}
			return list;
		}

		private void DeleteSelection()
		{
			List<string> list = this.CollectSelectionNames();
			if (list.Count == 0)
			{
				return;
			}
			if (GraphGUI.DeleteNodeDialog(list.ToArray()))
			{
				this.blendTreeGraph.RemoveNodeMotions(this.selection);
				this.blendTreeGraph.BuildFromBlendTree(this.blendTreeGraph.rootBlendTree);
			}
			this.ClearSelection();
			this.UpdateUnitySelection();
		}

		public static bool DeleteNodeDialog(string[] toDelete)
		{
			string text = "Delete selected Blend Tree asset";
			if (toDelete.Length > 1)
			{
				text += "s";
			}
			text += "?";
			string text2 = string.Empty;
			for (int i = 0; i < toDelete.Length; i++)
			{
				string str = toDelete[i];
				text2 = text2 + str + "\n";
			}
			return EditorUtility.DisplayDialog(text, text2, "Delete", "Cancel");
		}

		public override void ClearSelection()
		{
			this.selection.Clear();
			this.edgeGUI.edgeSelection.Clear();
		}

		public override void DoBackgroundClickAction()
		{
			this.selection.Clear();
			Node rootNode = (base.graph as Graph).rootNode;
			this.selection.Add(rootNode);
			Selection.activeObject = rootNode.motion;
		}
	}
}
