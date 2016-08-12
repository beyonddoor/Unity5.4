using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityEditor.Graphs.AnimationStateMachine
{
	internal class StateMachineNode : Node
	{
		public AnimatorStateMachine stateMachine;

		public override UnityEngine.Object selectionObject
		{
			get
			{
				return this.stateMachine;
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
				return base.title + this.stateMachine.name;
			}
			set
			{
				base.title = value;
			}
		}

		public override void NodeUI(UnityEditor.Graphs.GraphGUI host)
		{
			this.graphGUI = (host as GraphGUI);
			Assert.NotNull(this.graphGUI);
			if (Node.IsLeftClick())
			{
				host.edgeGUI.EndSlotDragging(base.inputSlots.First<Slot>(), true);
			}
			if (Node.IsDoubleClick())
			{
				if (this.stateMachine == this.graphGUI.stateMachineGraph.parentStateMachine)
				{
					this.graphGUI.tool.GoToBreadCrumbTarget(this.stateMachine);
				}
				else
				{
					this.graphGUI.tool.AddBreadCrumb(this.stateMachine);
				}
				Event.current.Use();
			}
			if (Node.IsRightClick() && this.stateMachine != this.graphGUI.stateMachineGraph.parentStateMachine)
			{
				GenericMenu genericMenu = new GenericMenu();
				genericMenu.AddItem(new GUIContent("Make Transition"), false, new GenericMenu.MenuFunction(this.MakeTransitionCallback));
				genericMenu.AddItem(new GUIContent("Copy"), false, new GenericMenu.MenuFunction(this.CopyStateMachineCallback));
				genericMenu.AddItem(new GUIContent("Delete"), false, new GenericMenu.MenuFunction(this.DeleteStateMachineCallback));
				genericMenu.ShowAsContext();
				Event.current.Use();
			}
		}

		private void MakeTransitionCallback()
		{
			this.graphGUI.edgeGUI.BeginSlotDragging(base.outputSlots.First<Slot>(), true, false);
		}

		private void CopyStateMachineCallback()
		{
			this.graphGUI.CopySelectionToPasteboard();
		}

		public void DeleteStateMachineCallback()
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
			if (this.stateMachine == AnimatorControllerTool.tool.stateMachineGraph.parentStateMachine)
			{
				AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine.parentStateMachinePosition = new Vector2(this.position.x, this.position.y);
			}
			else
			{
				AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine.SetStateMachinePosition(this.stateMachine, new Vector2(this.position.x, this.position.y));
			}
		}

		public override void EndDrag()
		{
			base.EndDrag();
			AnimatorStateMachine hoveredStateMachine = AnimatorControllerTool.tool.stateMachineGraphGUI.hoveredStateMachine;
			if (hoveredStateMachine != null && hoveredStateMachine != this.stateMachine && this.stateMachine != this.graphGUI.stateMachineGraph.parentStateMachine)
			{
				Undo.RecordObjects(new List<UnityEngine.Object>
				{
					AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine,
					hoveredStateMachine
				}.ToArray(), "Move in StateMachine");
				AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine.MoveStateMachine(this.stateMachine, hoveredStateMachine);
				AnimatorControllerTool.tool.RebuildGraph();
			}
		}

		public override void Connect(Node toNode, Edge edge)
		{
			if (toNode is StateNode)
			{
				this.graphGUI.stateMachineGraph.activeStateMachine.AddStateMachineTransition(this.stateMachine, (toNode as StateNode).state);
				this.graphGUI.stateMachineGraph.RebuildGraph();
			}
			else if (toNode is StateMachineNode)
			{
				Node.GenericMenuForStateMachineNode(toNode as StateMachineNode, true, delegate(object data)
				{
					if (data is AnimatorState)
					{
						this.graphGUI.stateMachineGraph.activeStateMachine.AddStateMachineTransition(this.stateMachine, data as AnimatorState);
					}
					else
					{
						this.graphGUI.stateMachineGraph.activeStateMachine.AddStateMachineTransition(this.stateMachine, data as AnimatorStateMachine);
					}
					this.graphGUI.stateMachineGraph.RebuildGraph();
				});
			}
			else if (toNode is ExitNode)
			{
				this.graphGUI.stateMachineGraph.activeStateMachine.AddStateMachineExitTransition(this.stateMachine);
				this.graphGUI.stateMachineGraph.RebuildGraph();
			}
		}
	}
}
