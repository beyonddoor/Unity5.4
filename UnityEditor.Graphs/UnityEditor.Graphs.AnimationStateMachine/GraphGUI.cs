using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Experimental.Director;

namespace UnityEditor.Graphs.AnimationStateMachine
{
	internal class GraphGUI : UnityEditor.Graphs.GraphGUI
	{
		public struct LiveLinkInfo
		{
			private AnimatorState m_CurrentState;

			private float m_CurrentStateNormalizedTime;

			private bool m_CurrentStateLoopTime;

			private AnimatorState m_NextState;

			private float m_NextStateNormalizedTime;

			private bool m_NextStateLoopTime;

			private AnimatorStateMachine m_CurrentStateMachine;

			private AnimatorStateMachine m_NextStateMachine;

			private AnimatorTransitionInfo m_TransitionInfo;

			public Node srcNode;

			public Node dstNode;

			public AnimatorState currentState
			{
				get
				{
					return this.m_CurrentState;
				}
				set
				{
					this.m_CurrentState = value;
				}
			}

			public float currentStateNormalizedTime
			{
				get
				{
					return this.m_CurrentStateNormalizedTime;
				}
				set
				{
					this.m_CurrentStateNormalizedTime = value;
				}
			}

			public bool currentStateLoopTime
			{
				get
				{
					return this.m_CurrentStateLoopTime;
				}
				set
				{
					this.m_CurrentStateLoopTime = value;
				}
			}

			public AnimatorState nextState
			{
				get
				{
					return this.m_NextState;
				}
				set
				{
					this.m_NextState = value;
				}
			}

			public float nextStateNormalizedTime
			{
				get
				{
					return this.m_NextStateNormalizedTime;
				}
				set
				{
					this.m_NextStateNormalizedTime = value;
				}
			}

			public bool nextStateLoopTime
			{
				get
				{
					return this.m_NextStateLoopTime;
				}
				set
				{
					this.m_NextStateLoopTime = value;
				}
			}

			public AnimatorStateMachine currentStateMachine
			{
				get
				{
					return this.m_CurrentStateMachine;
				}
				set
				{
					this.m_CurrentStateMachine = value;
				}
			}

			public AnimatorStateMachine nextStateMachine
			{
				get
				{
					return this.m_NextStateMachine;
				}
				set
				{
					this.m_NextStateMachine = value;
				}
			}

			public AnimatorTransitionInfo transitionInfo
			{
				get
				{
					return this.m_TransitionInfo;
				}
				set
				{
					this.m_TransitionInfo = value;
				}
			}

			public void Clear()
			{
				this.m_CurrentState = null;
				this.m_NextState = null;
				this.m_CurrentStateNormalizedTime = 0f;
				this.m_NextStateNormalizedTime = 0f;
				this.m_TransitionInfo = default(AnimatorTransitionInfo);
			}
		}

		private AnimatorDefaultTransition m_DefaultTransition;

		private StateMachineNode m_HoveredStateMachineNode;

		private GraphGUI.LiveLinkInfo m_LiveLinkInfo;

		public Graph stateMachineGraph
		{
			get
			{
				return base.graph as Graph;
			}
		}

		public AnimatorControllerTool tool
		{
			get
			{
				return this.m_Host as AnimatorControllerTool;
			}
		}

		public AnimatorStateMachine activeStateMachine
		{
			get
			{
				return this.stateMachineGraph.activeStateMachine;
			}
		}

		public AnimatorStateMachine rootStateMachine
		{
			get
			{
				return this.stateMachineGraph.rootStateMachine;
			}
		}

		public AnimatorStateMachine parentStateMachine
		{
			get
			{
				return this.stateMachineGraph.parentStateMachine;
			}
		}

		public AnimatorDefaultTransition defaultTransition
		{
			get
			{
				return this.m_DefaultTransition;
			}
		}

		private bool isSelectionMoving
		{
			get
			{
				return this.selection.Count > 0 && this.selection[0].isDragging;
			}
		}

		public AnimatorStateMachine hoveredStateMachine
		{
			get
			{
				return (!(this.m_HoveredStateMachineNode != null)) ? null : this.m_HoveredStateMachineNode.stateMachine;
			}
		}

		public GraphGUI.LiveLinkInfo liveLinkInfo
		{
			get
			{
				return this.m_LiveLinkInfo;
			}
		}

		public override IEdgeGUI edgeGUI
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

		public override void NodeGUI(UnityEditor.Graphs.Node n)
		{
			GUILayoutUtility.GetRect(160f, 0f);
			base.SelectNode(n);
			n.NodeUI(this);
			base.DragNodes();
		}

		public override void ClearSelection()
		{
			this.selection.Clear();
			this.edgeGUI.edgeSelection.Clear();
			this.UpdateUnitySelection();
		}

		private void SetHoveredStateMachine()
		{
			Vector2 mousePosition = Event.current.mousePosition;
			this.m_HoveredStateMachineNode = null;
			using (List<UnityEditor.Graphs.Node>.Enumerator enumerator = this.m_Graph.nodes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Node node = (Node)enumerator.Current;
					StateMachineNode stateMachineNode = node as StateMachineNode;
					if (stateMachineNode && stateMachineNode.position.Contains(mousePosition) && !this.selection.Contains(stateMachineNode))
					{
						this.m_HoveredStateMachineNode = stateMachineNode;
						break;
					}
				}
			}
		}

		protected override void UpdateUnitySelection()
		{
			List<UnityEngine.Object> list = new List<UnityEngine.Object>();
			foreach (UnityEditor.Graphs.Node current in this.selection)
			{
				if (current is StateNode)
				{
					list.Add((current as StateNode).state);
				}
				if (current is StateMachineNode)
				{
					list.Add((current as StateMachineNode).stateMachine);
				}
				if (current is AnyStateNode)
				{
					list.Add(current);
				}
				if (current is EntryNode)
				{
					list.Add(current);
				}
				if (current is ExitNode)
				{
					list.Add(current);
				}
			}
			foreach (int current2 in this.edgeGUI.edgeSelection)
			{
				EdgeInfo edgeInfo = this.stateMachineGraph.GetEdgeInfo(base.graph.edges[current2]);
				foreach (TransitionEditionContext current3 in edgeInfo.transitions)
				{
					if (current3.transition != null)
					{
						list.Add(current3.transition);
					}
					else
					{
						this.m_DefaultTransition = ScriptableObject.CreateInstance<AnimatorDefaultTransition>();
						list.Add(this.m_DefaultTransition);
					}
				}
			}
			if (list.Count > 0)
			{
				Selection.objects = list.ToArray();
			}
		}

		public override void SyncGraphToUnitySelection()
		{
			if (GUIUtility.hotControl != 0)
			{
				return;
			}
			this.selection.Clear();
			this.edgeGUI.edgeSelection.Clear();
			UnityEngine.Object[] objects = Selection.objects;
			for (int i = 0; i < objects.Length; i++)
			{
				UnityEngine.Object @object = objects[i];
				Node node = null;
				AnimatorState animatorState = @object as AnimatorState;
				AnimatorStateMachine animatorStateMachine = @object as AnimatorStateMachine;
				AnimatorTransitionBase animatorTransitionBase = @object as AnimatorTransitionBase;
				if (animatorState != null)
				{
					node = this.stateMachineGraph.FindNode(animatorState);
				}
				else if (animatorStateMachine != null)
				{
					node = this.stateMachineGraph.FindNode(animatorStateMachine);
				}
				else if (animatorTransitionBase != null)
				{
					foreach (Edge current in this.m_Graph.edges)
					{
						EdgeInfo edgeInfo = this.stateMachineGraph.GetEdgeInfo(current);
						foreach (TransitionEditionContext current2 in edgeInfo.transitions)
						{
							if (current2.transition == animatorTransitionBase)
							{
								int item = this.m_Graph.edges.IndexOf(current);
								if (!this.edgeGUI.edgeSelection.Contains(item))
								{
									this.edgeGUI.edgeSelection.Add(item);
								}
							}
						}
					}
				}
				else
				{
					node = (@object as Node);
				}
				if (node != null)
				{
					this.selection.Add(node);
				}
			}
		}

		private bool IsCurrentStateMachineNodeLiveLinked(Node n)
		{
			StateMachineNode stateMachineNode = n as StateMachineNode;
			if (stateMachineNode != null)
			{
				AnimatorState currentState = this.liveLinkInfo.currentState;
				bool flag = this.activeStateMachine.HasState(currentState, true);
				bool flag2 = stateMachineNode.stateMachine.HasState(currentState, true);
				bool flag3 = stateMachineNode.stateMachine.HasStateMachine(this.activeStateMachine, false);
				if ((flag3 && flag2 && !flag) || (!flag3 && flag2))
				{
					return true;
				}
			}
			return false;
		}

		public override void OnGraphGUI()
		{
			if (this.stateMachineGraph.DisplayDirty())
			{
				this.stateMachineGraph.RebuildGraph();
			}
			this.SyncGraphToUnitySelection();
			this.LiveLink();
			this.SetHoveredStateMachine();
			this.m_Host.BeginWindows();
			using (List<UnityEditor.Graphs.Node>.Enumerator enumerator = this.m_Graph.nodes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Node node = (Node)enumerator.Current;
					Node n2 = node;
					bool on = this.selection.Contains(node);
					node.position = GUILayout.Window(node.GetInstanceID(), node.position, delegate
					{
						this.NodeGUI(n2);
					}, node.title, Styles.GetNodeStyle(node.style, (!this.IsCurrentStateMachineNodeLiveLinked(node)) ? node.color : Styles.Color.Blue, on), new GUILayoutOption[]
					{
						GUILayout.Width(0f),
						GUILayout.Height(0f)
					});
					if (Event.current.type == EventType.MouseMove && node.position.Contains(Event.current.mousePosition))
					{
						this.edgeGUI.SlotDragging(node.inputSlots.First<Slot>(), true, true);
					}
				}
			}
			this.edgeGUI.DoEdges();
			this.m_Host.EndWindows();
			if (Event.current.type == EventType.MouseDown && Event.current.button != 2)
			{
				this.edgeGUI.EndDragging();
			}
			this.HandleEvents();
			this.HandleContextMenu();
			this.HandleObjectDragging();
			base.DragSelection(new Rect(-5000f, -5000f, 10000f, 10000f));
		}

		private List<AnimationClip> ComputeDraggedClipsFromModelImporter()
		{
			List<AnimationClip> list = new List<AnimationClip>();
			List<GameObject> list2 = DragAndDrop.objectReferences.OfType<GameObject>().ToList<GameObject>();
			for (int i = 0; i < list2.Count; i++)
			{
				ModelImporter modelImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(list2[i])) as ModelImporter;
				if (modelImporter)
				{
					UnityEngine.Object[] array = AssetDatabase.LoadAllAssetsAtPath(modelImporter.assetPath);
					UnityEngine.Object[] array2 = array;
					for (int j = 0; j < array2.Length; j++)
					{
						UnityEngine.Object @object = array2[j];
						if ((@object.hideFlags & HideFlags.HideInHierarchy) == HideFlags.None)
						{
							AnimationClip animationClip = @object as AnimationClip;
							if (animationClip)
							{
								list.Add(animationClip);
							}
						}
					}
				}
			}
			return list;
		}

		private void HandleObjectDragging()
		{
			Event current = Event.current;
			List<Motion> list = DragAndDrop.objectReferences.OfType<Motion>().ToList<Motion>();
			List<AnimatorState> list2 = DragAndDrop.objectReferences.OfType<AnimatorState>().ToList<AnimatorState>();
			List<AnimatorStateMachine> list3 = DragAndDrop.objectReferences.OfType<AnimatorStateMachine>().ToList<AnimatorStateMachine>();
			List<AnimationClip> list4 = this.ComputeDraggedClipsFromModelImporter();
			EventType type = current.type;
			switch (type)
			{
			case EventType.Repaint:
				if (this.isSelectionMoving && this.m_HoveredStateMachineNode && !this.selection.Contains(this.m_HoveredStateMachineNode) && !this.SelectionOnlyUpNode())
				{
					EditorGUIUtility.AddCursorRect(this.m_HoveredStateMachineNode.position, MouseCursor.ArrowPlus);
				}
				return;
			case EventType.Layout:
				IL_5F:
				if (type != EventType.DragExited)
				{
					return;
				}
				current.Use();
				return;
			case EventType.DragUpdated:
			case EventType.DragPerform:
				if (list.Count > 0 || list2.Count > 0 || list3.Count > 0 || list4.Count > 0)
				{
					DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
				}
				else
				{
					DragAndDrop.visualMode = DragAndDropVisualMode.None;
				}
				if (current.type == EventType.DragPerform && DragAndDrop.visualMode != DragAndDropVisualMode.None)
				{
					DragAndDrop.AcceptDrag();
					Undo.RegisterCompleteObjectUndo(this.activeStateMachine, "Drag motion to state machine.");
					for (int i = 0; i < list.Count; i++)
					{
						AnimatorState state = this.activeStateMachine.AddState(list[i].name, current.mousePosition + new Vector2(12f, 12f) * (float)i);
						this.tool.animatorController.SetStateEffectiveMotion(state, list[i], this.tool.selectedLayerIndex);
					}
					for (int j = 0; j < list2.Count; j++)
					{
						this.activeStateMachine.AddState(list2[j], current.mousePosition + new Vector2(12f, 12f) * (float)j);
					}
					for (int k = 0; k < list3.Count; k++)
					{
						this.activeStateMachine.AddStateMachine(list3[k], current.mousePosition + new Vector2(12f, 12f) * (float)k);
					}
					for (int l = 0; l < list4.Count; l++)
					{
						AnimatorState state2 = this.activeStateMachine.AddState(list4[l].name, current.mousePosition + new Vector2(12f, 12f) * (float)l);
						this.tool.animatorController.SetStateEffectiveMotion(state2, list4[l], this.tool.selectedLayerIndex);
					}
					this.stateMachineGraph.RebuildGraph();
				}
				current.Use();
				return;
			}
			goto IL_5F;
		}

		private bool SelectionOnlyUpNode()
		{
			if (this.selection.Count == 1 && this.selection[0] is StateMachineNode)
			{
				AnimatorStateMachine stateMachine = (this.selection[0] as StateMachineNode).stateMachine;
				if (this.parentStateMachine == stateMachine)
				{
					return true;
				}
			}
			return false;
		}

		private bool MoveSelectionTo(StateMachineNode targetSM)
		{
			bool result = false;
			List<UnityEngine.Object> list = new List<UnityEngine.Object>();
			list.Add(this.rootStateMachine);
			foreach (ChildAnimatorStateMachine current in this.rootStateMachine.stateMachinesRecursive)
			{
				list.Add(current.stateMachine);
			}
			using (List<UnityEditor.Graphs.Node>.Enumerator enumerator2 = this.selection.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					Node node = (Node)enumerator2.Current;
					if (node is StateNode)
					{
						list.Add((node as StateNode).state);
					}
				}
			}
			Undo.RegisterCompleteObjectUndo(list.ToArray(), "Move in StateMachine");
			using (List<UnityEditor.Graphs.Node>.Enumerator enumerator3 = this.selection.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					Node node2 = (Node)enumerator3.Current;
					if (node2 is StateNode)
					{
						this.activeStateMachine.MoveState((node2 as StateNode).state, targetSM.stateMachine);
					}
					else if (node2 is StateMachineNode)
					{
						this.activeStateMachine.MoveStateMachine((node2 as StateMachineNode).stateMachine, targetSM.stateMachine);
					}
					result = true;
				}
			}
			return result;
		}

		internal string ResolveHash(AnimatorControllerPlayable controller, int fullPathHash)
		{
			return controller.ResolveHash(fullPathHash);
		}

		internal string ResolveHash(Animator controller, int fullPathHash)
		{
			return controller.ResolveHash(fullPathHash);
		}

		private void LiveLink()
		{
			this.m_LiveLinkInfo.Clear();
			if (!this.tool.liveLink)
			{
				return;
			}
			AnimatorControllerPlayable controller = AnimatorController.FindAnimatorControllerPlayable(this.tool.previewAnimator, this.tool.animatorController);
			if (!controller.node.IsValid())
			{
				return;
			}
			AnimatorStateInfo currentAnimatorStateInfo = controller.GetCurrentAnimatorStateInfo(AnimatorControllerTool.tool.selectedLayerIndex);
			AnimatorStateInfo nextAnimatorStateInfo = controller.GetNextAnimatorStateInfo(AnimatorControllerTool.tool.selectedLayerIndex);
			AnimatorTransitionInfo animatorTransitionInfo = controller.GetAnimatorTransitionInfo(AnimatorControllerTool.tool.selectedLayerIndex);
			int shortNameHash = currentAnimatorStateInfo.shortNameHash;
			int shortNameHash2 = nextAnimatorStateInfo.shortNameHash;
			this.m_LiveLinkInfo.currentStateMachine = ((shortNameHash == 0) ? null : this.rootStateMachine.FindStateMachine(this.ResolveHash(controller, currentAnimatorStateInfo.fullPathHash)));
			this.m_LiveLinkInfo.currentState = ((shortNameHash == 0) ? null : this.m_LiveLinkInfo.currentStateMachine.FindState(shortNameHash).state);
			this.m_LiveLinkInfo.currentStateNormalizedTime = currentAnimatorStateInfo.normalizedTime;
			this.m_LiveLinkInfo.currentStateLoopTime = currentAnimatorStateInfo.loop;
			if (this.m_LiveLinkInfo.currentState == null)
			{
				return;
			}
			this.m_LiveLinkInfo.nextStateMachine = ((shortNameHash2 == 0) ? null : this.rootStateMachine.FindStateMachine(this.ResolveHash(controller, nextAnimatorStateInfo.fullPathHash)));
			this.m_LiveLinkInfo.nextState = ((shortNameHash2 == 0) ? null : this.m_LiveLinkInfo.nextStateMachine.FindState(shortNameHash2).state);
			this.m_LiveLinkInfo.nextStateNormalizedTime = nextAnimatorStateInfo.normalizedTime;
			this.m_LiveLinkInfo.nextStateLoopTime = nextAnimatorStateInfo.loop;
			this.m_LiveLinkInfo.srcNode = this.stateMachineGraph.FindNode(this.m_LiveLinkInfo.currentState);
			this.m_LiveLinkInfo.dstNode = ((!this.m_LiveLinkInfo.nextState) ? null : this.stateMachineGraph.FindNode(this.m_LiveLinkInfo.nextState));
			this.m_LiveLinkInfo.transitionInfo = animatorTransitionInfo;
			if (this.tool.autoLiveLink)
			{
				AnimatorStateMachine animatorStateMachine = this.m_LiveLinkInfo.currentStateMachine;
				if (this.m_LiveLinkInfo.currentState != null && this.m_LiveLinkInfo.nextState != null && ((double)this.m_LiveLinkInfo.transitionInfo.normalizedTime > 0.5 || animatorTransitionInfo.anyState))
				{
					animatorStateMachine = this.m_LiveLinkInfo.nextStateMachine;
				}
				if (shortNameHash != 0 && animatorStateMachine != this.activeStateMachine && Event.current.type == EventType.Repaint)
				{
					List<AnimatorStateMachine> hierarchy = new List<AnimatorStateMachine>();
					MecanimUtilities.StateMachineRelativePath(this.rootStateMachine, animatorStateMachine, ref hierarchy);
					this.tool.BuildBreadCrumbsFromSMHierarchy(hierarchy);
				}
			}
		}

		private void HandleEvents()
		{
			Event current = Event.current;
			EventType type = current.type;
			if (type != EventType.ValidateCommand)
			{
				if (type != EventType.ExecuteCommand)
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
				else
				{
					if (current.commandName == "SoftDelete" || current.commandName == "Delete")
					{
						this.DeleteSelection();
						current.Use();
					}
					if (current.commandName == "Copy")
					{
						this.CopySelectionToPasteboard();
						current.Use();
					}
					else if (current.commandName == "Paste")
					{
						Unsupported.PasteToStateMachineFromPasteboard(this.activeStateMachine, this.tool.animatorController, this.tool.selectedLayerIndex, Vector3.zero);
						current.Use();
					}
					else if (current.commandName == "Duplicate" && this.CopySelectionToPasteboard())
					{
						Vector3 zero = Vector3.zero;
						if (this.selection.Count > 0)
						{
							zero.Set(this.selection[0].position.x, this.selection[0].position.y, 0f);
						}
						Unsupported.PasteToStateMachineFromPasteboard(this.activeStateMachine, this.tool.animatorController, this.tool.selectedLayerIndex, zero + new Vector3(40f, 40f, 0f));
						current.Use();
					}
				}
			}
			else if (current.commandName == "SoftDelete" || current.commandName == "Delete" || current.commandName == "Copy" || current.commandName == "Paste" || current.commandName == "Duplicate")
			{
				current.Use();
			}
		}

		public void DeleteSelection()
		{
			List<string> list = this.CollectSelectionNames();
			if (list.Count == 0)
			{
				return;
			}
			this.DeleteSelectedEdges();
			this.DeleteSelectedNodes();
			this.ClearSelection();
			this.UpdateUnitySelection();
		}

		public bool CopySelectionToPasteboard()
		{
			UnityEngine.Object[] array = new UnityEngine.Object[0];
			Vector3[] monoPositions = new Vector3[0];
			using (List<UnityEditor.Graphs.Node>.Enumerator enumerator = this.selection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Node node = (Node)enumerator.Current;
					if (node is StateNode)
					{
						ArrayUtility.Add<UnityEngine.Object>(ref array, (node as StateNode).state);
						ArrayUtility.Add<Vector3>(ref monoPositions, new Vector3(node.position.x, node.position.y, 0f));
					}
					else if (node is StateMachineNode && (node as StateMachineNode).stateMachine != AnimatorControllerTool.tool.stateMachineGraph.parentStateMachine)
					{
						ArrayUtility.Add<UnityEngine.Object>(ref array, (node as StateMachineNode).stateMachine);
						ArrayUtility.Add<Vector3>(ref monoPositions, new Vector3(node.position.x, node.position.y, 0f));
					}
				}
			}
			foreach (int current in this.edgeGUI.edgeSelection)
			{
				EdgeInfo edgeInfo = this.stateMachineGraph.GetEdgeInfo(base.graph.edges[current]);
				foreach (TransitionEditionContext current2 in edgeInfo.transitions)
				{
					if (current2.transition != null)
					{
						ArrayUtility.Add<UnityEngine.Object>(ref array, current2.transition);
						ArrayUtility.Add<Vector3>(ref monoPositions, Vector3.zero);
					}
				}
			}
			Unsupported.CopyStateMachineDataToPasteboard(array, monoPositions, this.tool.animatorController, this.tool.selectedLayerIndex);
			return array.Length > 0;
		}

		public override void DoBackgroundClickAction()
		{
			Selection.objects = new List<UnityEngine.Object>
			{
				this.activeStateMachine
			}.ToArray();
		}

		private List<string> CollectSelectionNames()
		{
			List<string> list = new List<string>();
			using (List<UnityEditor.Graphs.Node>.Enumerator enumerator = this.selection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Node node = (Node)enumerator.Current;
					if (node is StateNode)
					{
						list.Add((node as StateNode).state.name);
					}
					else if (node is StateMachineNode)
					{
						AnimatorStateMachine stateMachine = (node as StateMachineNode).stateMachine;
						if (this.parentStateMachine != stateMachine)
						{
							list.Add(stateMachine.name);
						}
					}
				}
			}
			foreach (int current in this.edgeGUI.edgeSelection)
			{
				EdgeInfo edgeInfo = this.stateMachineGraph.GetEdgeInfo(base.graph.edges[current]);
				foreach (TransitionEditionContext current2 in edgeInfo.transitions)
				{
					if (!current2.isDefaultTransition)
					{
						list.Add(current2.displayName);
					}
				}
			}
			return list;
		}

		private void DeleteSelectedNodes()
		{
			using (List<UnityEditor.Graphs.Node>.Enumerator enumerator = this.selection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Node node = (Node)enumerator.Current;
					if (node is StateNode)
					{
						AnimatorState state = (node as StateNode).state;
						this.activeStateMachine.RemoveState(state);
						this.stateMachineGraph.RemoveNode(node, false);
					}
					if (node is StateMachineNode)
					{
						AnimatorStateMachine stateMachine = (node as StateMachineNode).stateMachine;
						if (this.parentStateMachine != stateMachine)
						{
							this.activeStateMachine.RemoveStateMachine(stateMachine);
							this.stateMachineGraph.RemoveNode(node, false);
						}
					}
				}
			}
		}

		private void DeleteSelectedEdges()
		{
			bool flag = false;
			List<Edge> list = new List<Edge>();
			foreach (int current in this.edgeGUI.edgeSelection)
			{
				list.Add(base.graph.edges[current]);
				flag = true;
			}
			foreach (Edge current2 in list)
			{
				EdgeInfo edgeInfo = this.stateMachineGraph.GetEdgeInfo(current2);
				foreach (TransitionEditionContext current3 in edgeInfo.transitions)
				{
					current3.Remove(false);
					flag = true;
				}
			}
			if (flag)
			{
				this.stateMachineGraph.RebuildGraph();
			}
			this.edgeGUI.edgeSelection.Clear();
		}

		private static bool HasMotionSelected()
		{
			return Selection.activeObject is Motion;
		}

		private void HandleContextMenu()
		{
			Event current = Event.current;
			if (current.type != EventType.ContextClick)
			{
				return;
			}
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent("Create State/Empty"), false, new GenericMenu.MenuFunction2(this.AddStateEmptyCallback), Event.current.mousePosition);
			if (GraphGUI.HasMotionSelected())
			{
				genericMenu.AddItem(new GUIContent("Create State/From Selected Clip"), false, new GenericMenu.MenuFunction2(this.AddStateFromSelectedMotionCallback), Event.current.mousePosition);
			}
			else
			{
				genericMenu.AddDisabledItem(new GUIContent("Create State/From Selected Clip"));
			}
			genericMenu.AddItem(new GUIContent("Create State/From New Blend Tree"), false, new GenericMenu.MenuFunction2(this.AddStateFromNewBlendTreeCallback), Event.current.mousePosition);
			genericMenu.AddItem(new GUIContent("Create Sub-State Machine"), false, new GenericMenu.MenuFunction2(this.AddStateMachineCallback), Event.current.mousePosition);
			if (Unsupported.HasStateMachineDataInPasteboard())
			{
				genericMenu.AddItem(new GUIContent("Paste"), false, new GenericMenu.MenuFunction2(this.PasteCallback), Event.current.mousePosition);
			}
			else
			{
				genericMenu.AddDisabledItem(new GUIContent("Paste"));
			}
			genericMenu.AddItem(new GUIContent("Copy current StateMachine"), false, new GenericMenu.MenuFunction2(this.CopyStateMachineCallback), Event.current.mousePosition);
			genericMenu.ShowAsContext();
		}

		private void AddStateMachineCallback(object data)
		{
			Undo.RegisterCompleteObjectUndo(this.activeStateMachine, "Sub-State Machine Added");
			this.activeStateMachine.AddStateMachine("New StateMachine", (Vector2)data);
			AnimatorControllerTool.tool.RebuildGraph();
		}

		private void PasteCallback(object data)
		{
			Undo.RegisterCompleteObjectUndo(this.activeStateMachine, "Paste");
			Unsupported.PasteToStateMachineFromPasteboard(this.activeStateMachine, this.tool.animatorController, this.tool.selectedLayerIndex, (Vector2)data);
			AnimatorControllerTool.tool.RebuildGraph();
		}

		private void AddStateFromNewBlendTreeCallback(object data)
		{
			Undo.RegisterCompleteObjectUndo(this.activeStateMachine, "Blend Tree State Added");
			AnimatorState state = this.activeStateMachine.AddState("Blend Tree", (Vector2)data);
			BlendTree blendTree = new BlendTree();
			blendTree.hideFlags = HideFlags.HideInHierarchy;
			if (AssetDatabase.GetAssetPath(this.tool.animatorController) != string.Empty)
			{
				AssetDatabase.AddObjectToAsset(blendTree, AssetDatabase.GetAssetPath(this.tool.animatorController));
			}
			blendTree.name = "Blend Tree";
			BlendTree arg_93_0 = blendTree;
			string defaultBlendTreeParameter = this.tool.animatorController.GetDefaultBlendTreeParameter();
			blendTree.blendParameterY = defaultBlendTreeParameter;
			arg_93_0.blendParameter = defaultBlendTreeParameter;
			this.tool.animatorController.SetStateEffectiveMotion(state, blendTree, this.tool.selectedLayerIndex);
			AnimatorControllerTool.tool.RebuildGraph();
		}

		private void AddStateFromSelectedMotionCallback(object data)
		{
			AnimationClip animationClip = Selection.activeObject as AnimationClip;
			AnimatorState state = this.activeStateMachine.AddState(animationClip.name, (Vector2)data);
			this.tool.animatorController.SetStateEffectiveMotion(state, animationClip, this.tool.selectedLayerIndex);
			AnimatorControllerTool.tool.RebuildGraph();
		}

		private void AddStateEmptyCallback(object data)
		{
			this.activeStateMachine.AddState("New State", (Vector2)data);
			AnimatorControllerTool.tool.RebuildGraph();
		}

		private void CopyStateMachineCallback(object data)
		{
			Unsupported.CopyStateMachineDataToPasteboard(this.activeStateMachine, AnimatorControllerTool.tool.animatorController, AnimatorControllerTool.tool.selectedLayerIndex);
		}
	}
}
