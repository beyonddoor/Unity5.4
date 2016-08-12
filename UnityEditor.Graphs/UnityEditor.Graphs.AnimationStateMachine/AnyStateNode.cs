using NUnit.Framework;
using System;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityEditor.Graphs.AnimationStateMachine
{
	internal class AnyStateNode : Node
	{
		public override UnityEngine.Object selectionObject
		{
			get
			{
				return this;
			}
		}

		public override UnityEngine.Object undoableObject
		{
			get
			{
				return AnimatorControllerTool.tool.stateMachineGraph.rootStateMachine;
			}
		}

		public override void NodeUI(UnityEditor.Graphs.GraphGUI host)
		{
			this.graphGUI = (host as GraphGUI);
			Assert.NotNull(this.graphGUI);
			Event current = Event.current;
			if (Node.IsRightClick())
			{
				GenericMenu genericMenu = new GenericMenu();
				genericMenu.AddItem(new GUIContent("Make Transition"), false, new GenericMenu.MenuFunction(this.MakeTransitionCallback));
				genericMenu.ShowAsContext();
				current.Use();
			}
		}

		private void MakeTransitionCallback()
		{
			this.graphGUI.edgeGUI.BeginSlotDragging(base.outputSlots.First<Slot>(), true, false);
		}

		public override void OnDrag()
		{
			base.OnDrag();
			AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine.anyStatePosition = new Vector2(this.position.x, this.position.y);
		}

		public override void Connect(Node toNode, Edge edge)
		{
			if (toNode is StateNode)
			{
				this.graphGUI.stateMachineGraph.rootStateMachine.AddAnyStateTransition((toNode as StateNode).state);
				this.graphGUI.stateMachineGraph.RebuildGraph();
			}
			if (toNode is StateMachineNode)
			{
				StateMachineNode stateMachineNode = toNode as StateMachineNode;
				if (stateMachineNode.stateMachine != this.graphGUI.parentStateMachine)
				{
					Node.GenericMenuForStateMachineNode(toNode as StateMachineNode, true, delegate(object data)
					{
						if (data is AnimatorState)
						{
							this.graphGUI.stateMachineGraph.rootStateMachine.AddAnyStateTransition(data as AnimatorState);
						}
						else if (data is AnimatorStateMachine)
						{
							this.graphGUI.stateMachineGraph.rootStateMachine.AddAnyStateTransition(data as AnimatorStateMachine);
						}
						this.graphGUI.stateMachineGraph.RebuildGraph();
					});
				}
			}
			if (toNode is EntryNode)
			{
				this.graphGUI.stateMachineGraph.rootStateMachine.AddAnyStateTransition(this.graphGUI.activeStateMachine);
			}
		}
	}
}
