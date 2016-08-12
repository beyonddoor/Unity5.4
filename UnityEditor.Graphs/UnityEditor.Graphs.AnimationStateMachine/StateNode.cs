using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityEditor.Graphs.AnimationStateMachine
{
	internal class StateNode : Node
	{
		public AnimatorState state;

		public override UnityEngine.Object selectionObject
		{
			get
			{
				return this.state;
			}
		}

		public override UnityEngine.Object undoableObject
		{
			get
			{
				return AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine;
			}
		}

		public override string title
		{
			get
			{
				return this.state.name;
			}
			set
			{
			}
		}

		private void CopyStateCallback()
		{
			this.graphGUI.CopySelectionToPasteboard();
		}

		public override void NodeUI(UnityEditor.Graphs.GraphGUI host)
		{
			this.graphGUI = (host as GraphGUI);
			Assert.NotNull(this.graphGUI);
			Event current = Event.current;
			if (Node.IsLeftClick())
			{
				host.edgeGUI.EndSlotDragging(base.inputSlots.First<Slot>(), true);
			}
			if (Node.IsDoubleClick())
			{
				Motion stateEffectiveMotion = AnimatorControllerTool.tool.animatorController.GetStateEffectiveMotion(this.state, AnimatorControllerTool.tool.selectedLayerIndex);
				if (stateEffectiveMotion is BlendTree)
				{
					this.graphGUI.tool.AddBreadCrumb(this.state);
				}
				else if (stateEffectiveMotion is AnimationClip)
				{
					Selection.activeObject = stateEffectiveMotion;
					AnimationClipEditor.EditWithImporter(stateEffectiveMotion as AnimationClip);
				}
				current.Use();
			}
			if (Node.IsRightClick())
			{
				GenericMenu genericMenu = new GenericMenu();
				genericMenu.AddItem(new GUIContent("Make Transition"), false, new GenericMenu.MenuFunction(this.MakeTransitionCallback));
				if (this.graphGUI.rootStateMachine.defaultState == this.state)
				{
					genericMenu.AddDisabledItem(new GUIContent("Set as Layer Default State"));
				}
				else
				{
					genericMenu.AddItem(new GUIContent("Set as Layer Default State"), false, new GenericMenu.MenuFunction(this.SetDefaultCallback));
				}
				genericMenu.AddItem(new GUIContent("Copy"), false, new GenericMenu.MenuFunction(this.CopyStateCallback));
				genericMenu.AddItem(new GUIContent("Create new BlendTree in State"), false, new GenericMenu.MenuFunction(this.AddNewBlendTreeCallback));
				genericMenu.AddItem(new GUIContent("Delete"), false, new GenericMenu.MenuFunction(this.DeleteStateCallback));
				genericMenu.ShowAsContext();
				current.Use();
			}
			Rect rect = GUILayoutUtility.GetRect(200f, 10f);
			if (Event.current.type == EventType.Repaint && (this.graphGUI.liveLinkInfo.currentState == this.state || this.graphGUI.liveLinkInfo.nextState == this.state))
			{
				GUIStyle gUIStyle = "MeLivePlayBackground";
				GUIStyle gUIStyle2 = "MeLivePlayBar";
				float num = (!(this.graphGUI.liveLinkInfo.currentState == this.state)) ? this.graphGUI.liveLinkInfo.nextStateNormalizedTime : this.graphGUI.liveLinkInfo.currentStateNormalizedTime;
				bool flag = (!(this.graphGUI.liveLinkInfo.currentState == this.state)) ? this.graphGUI.liveLinkInfo.nextStateLoopTime : this.graphGUI.liveLinkInfo.currentStateLoopTime;
				rect = gUIStyle.margin.Remove(rect);
				Rect position = gUIStyle.padding.Remove(rect);
				if (flag)
				{
					if (num < 0f)
					{
						position.width = position.width * (1f - Mathf.Abs(num) % 1f) + 2f;
					}
					else
					{
						position.width = position.width * (num % 1f) + 2f;
					}
				}
				else
				{
					position.width = position.width * Mathf.Clamp(num, 0f, 1f) + 2f;
				}
				gUIStyle2.Draw(position, false, false, false, false);
				gUIStyle.Draw(rect, false, false, false, false);
			}
		}

		private void SetDefaultCallback()
		{
			Undo.RegisterCompleteObjectUndo(this.graphGUI.rootStateMachine, "Set Default State");
			this.graphGUI.rootStateMachine.defaultState = this.state;
			AnimatorControllerTool.tool.RebuildGraph();
		}

		private void MakeTransitionCallback()
		{
			this.graphGUI.edgeGUI.BeginSlotDragging(base.outputSlots.First<Slot>(), true, false);
		}

		private void AddNewBlendTreeCallback()
		{
			Motion stateEffectiveMotion = AnimatorControllerTool.tool.animatorController.GetStateEffectiveMotion(this.state, AnimatorControllerTool.tool.selectedLayerIndex);
			BlendTree blendTree = stateEffectiveMotion as BlendTree;
			AnimatorStateMachine stateMachine = AnimatorControllerTool.tool.animatorController.layers[AnimatorControllerTool.tool.selectedLayerIndex].stateMachine;
			bool flag = true;
			if (blendTree != null)
			{
				string title = "This will delete current BlendTree in state.";
				string message = "You cannot undo this action.";
				if (EditorUtility.DisplayDialog(title, message, "Delete", "Cancel"))
				{
					MecanimUtilities.DestroyBlendTreeRecursive(blendTree);
				}
				else
				{
					flag = false;
				}
			}
			else
			{
				Undo.RegisterCompleteObjectUndo(stateMachine, "Blend Tree Added");
			}
			if (flag)
			{
				BlendTree blendTree2 = new BlendTree();
				blendTree2.hideFlags = HideFlags.HideInHierarchy;
				if (AssetDatabase.GetAssetPath(AnimatorControllerTool.tool.animatorController) != string.Empty)
				{
					AssetDatabase.AddObjectToAsset(blendTree2, AssetDatabase.GetAssetPath(AnimatorControllerTool.tool.animatorController));
				}
				blendTree2.name = "Blend Tree";
				BlendTree arg_10B_0 = blendTree2;
				string defaultBlendTreeParameter = AnimatorControllerTool.tool.animatorController.GetDefaultBlendTreeParameter();
				blendTree2.blendParameterY = defaultBlendTreeParameter;
				arg_10B_0.blendParameter = defaultBlendTreeParameter;
				AnimatorControllerTool.tool.animatorController.SetStateEffectiveMotion(this.state, blendTree2, AnimatorControllerTool.tool.selectedLayerIndex);
			}
		}

		public void DeleteStateCallback()
		{
			if (!this.graphGUI.selection.Contains(this))
			{
				this.graphGUI.selection.Add(this);
			}
			this.graphGUI.DeleteSelection();
			AnimatorControllerTool.tool.RebuildGraph();
		}

		public override void OnDrag()
		{
			base.OnDrag();
			AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine.SetStatePosition(this.state, new Vector2(this.position.x, this.position.y));
		}

		public override void EndDrag()
		{
			base.EndDrag();
			AnimatorStateMachine hoveredStateMachine = AnimatorControllerTool.tool.stateMachineGraphGUI.hoveredStateMachine;
			if (hoveredStateMachine != null)
			{
				Undo.RecordObjects(new List<UnityEngine.Object>
				{
					AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine,
					hoveredStateMachine
				}.ToArray(), "Move in StateMachine");
				AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine.MoveState(this.state, hoveredStateMachine);
			}
		}

		public override void Connect(Node toNode, Edge edge)
		{
			if (toNode is StateNode)
			{
				this.state.AddTransition((toNode as StateNode).state, true);
				this.graphGUI.stateMachineGraph.RebuildGraph();
			}
			else if (toNode is StateMachineNode)
			{
				Node.GenericMenuForStateMachineNode(toNode as StateMachineNode, true, delegate(object data)
				{
					if (data is AnimatorState)
					{
						this.state.AddTransition(data as AnimatorState, true);
					}
					else
					{
						this.state.AddTransition(data as AnimatorStateMachine, true);
					}
					this.graphGUI.stateMachineGraph.RebuildGraph();
				});
			}
			else if (toNode is ExitNode)
			{
				this.state.AddExitTransition(true);
				this.graphGUI.stateMachineGraph.RebuildGraph();
			}
		}
	}
}
