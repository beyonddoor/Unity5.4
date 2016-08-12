using NUnit.Framework;
using System;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityEditor.Graphs.AnimationStateMachine
{
	internal class EntryNode : Node
	{
		private bool draggingDefaultState;

		private AnimatorStateMachine m_StateMachine;

		public AnimatorStateMachine stateMachine
		{
			get
			{
				return this.m_StateMachine;
			}
			set
			{
				this.m_StateMachine = value;
			}
		}

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
				return this.m_StateMachine;
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
				genericMenu.AddItem(new GUIContent("Set StateMachine Default State"), false, new GenericMenu.MenuFunction(this.MakeDefaultStateCallback));
				genericMenu.ShowAsContext();
				current.Use();
			}
			if (Node.IsLeftClick())
			{
				host.edgeGUI.EndSlotDragging(base.inputSlots.First<Slot>(), true);
			}
		}

		private void MakeTransitionCallback()
		{
			this.graphGUI.edgeGUI.BeginSlotDragging(base.outputSlots.First<Slot>(), true, false);
		}

		private void MakeDefaultStateCallback()
		{
			this.draggingDefaultState = true;
			this.graphGUI.edgeGUI.BeginSlotDragging(base.outputSlots.First<Slot>(), true, false);
		}

		public override void OnDrag()
		{
			base.OnDrag();
			this.m_StateMachine.entryPosition = new Vector2(this.position.x, this.position.y);
		}

		public override void Connect(Node toNode, Edge edge)
		{
			if (toNode is StateNode)
			{
				if (this.draggingDefaultState)
				{
					this.m_StateMachine.defaultState = (toNode as StateNode).state;
				}
				else
				{
					this.m_StateMachine.AddEntryTransition((toNode as StateNode).state);
				}
				this.graphGUI.stateMachineGraph.RebuildGraph();
			}
			else if (toNode is StateMachineNode)
			{
				bool isDragginDefaultState = this.draggingDefaultState;
				Node.GenericMenuForStateMachineNode(toNode as StateMachineNode, !this.draggingDefaultState, delegate(object data)
				{
					if (data is AnimatorState)
					{
						if (isDragginDefaultState)
						{
							this.m_StateMachine.defaultState = (data as AnimatorState);
						}
						else
						{
							this.m_StateMachine.AddEntryTransition(data as AnimatorState);
						}
					}
					else if (data is AnimatorStateMachine)
					{
						this.m_StateMachine.AddEntryTransition(data as AnimatorStateMachine);
					}
					this.graphGUI.stateMachineGraph.RebuildGraph();
				});
			}
			this.draggingDefaultState = false;
		}
	}
}
