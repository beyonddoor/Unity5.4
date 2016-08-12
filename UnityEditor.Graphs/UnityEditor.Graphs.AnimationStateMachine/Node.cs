using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityEditor.Graphs.AnimationStateMachine
{
	internal class Node : UnityEditor.Graphs.Node
	{
		protected GraphGUI graphGUI;

		public virtual UnityEngine.Object selectionObject
		{
			get
			{
				return null;
			}
		}

		public virtual UnityEngine.Object undoableObject
		{
			get
			{
				return null;
			}
		}

		protected static bool IsRightClick()
		{
			Event current = Event.current;
			return current.type == EventType.ContextClick;
		}

		protected static bool IsDoubleClick()
		{
			Event current = Event.current;
			return current.type == EventType.MouseDown && current.button == 0 && current.clickCount == 2;
		}

		protected static bool IsLeftClick()
		{
			Event current = Event.current;
			return current.type == EventType.MouseDown && current.button == 0 && current.clickCount == 1;
		}

		public override void BeginDrag()
		{
			base.BeginDrag();
			Undo.RegisterCompleteObjectUndo(this.undoableObject, "Moved " + this.title);
		}

		public virtual void Connect(Node toNode, Edge edge)
		{
		}

		private static void PopulateSubNodeList(StateMachineNode toStateMachineNode, ref List<ChildAnimatorState> stateList, ref List<ChildAnimatorStateMachine> stateMachineList, ref AnimatorStateMachine parentStateMachine)
		{
			if (toStateMachineNode.stateMachine == AnimatorControllerTool.tool.stateMachineGraph.parentStateMachine)
			{
				stateList = AnimatorControllerTool.tool.stateMachineGraph.rootStateMachine.statesRecursive;
				stateList.RemoveAll((ChildAnimatorState s) => AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine.HasState(s.state));
				stateMachineList = AnimatorControllerTool.tool.stateMachineGraph.rootStateMachine.stateMachinesRecursive;
				stateMachineList.RemoveAll((ChildAnimatorStateMachine s) => AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine.HasStateMachine(s.stateMachine) || AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine == s.stateMachine);
				parentStateMachine = AnimatorControllerTool.tool.stateMachineGraph.rootStateMachine;
			}
			else
			{
				parentStateMachine = toStateMachineNode.stateMachine;
				stateList = toStateMachineNode.stateMachine.statesRecursive;
				stateMachineList = parentStateMachine.stateMachinesRecursive;
			}
			ChildAnimatorStateMachine item = default(ChildAnimatorStateMachine);
			item.stateMachine = parentStateMachine;
			stateMachineList.Add(item);
		}

		public static void GenericMenuForStateMachineNode(StateMachineNode toStateMachineNode, bool showStateMachine, GenericMenu.MenuFunction2 func)
		{
			AnimatorStateMachine y = null;
			List<ChildAnimatorState> list = new List<ChildAnimatorState>();
			List<ChildAnimatorStateMachine> list2 = new List<ChildAnimatorStateMachine>();
			Node.PopulateSubNodeList(toStateMachineNode, ref list, ref list2, ref y);
			if (list.Count == 0 && list2.Count == 1)
			{
				func(list2[0].stateMachine);
				return;
			}
			GenericMenu genericMenu = new GenericMenu();
			foreach (ChildAnimatorState current in list)
			{
				string text = current.state.name;
				AnimatorStateMachine currentParent = current.state.FindParent(AnimatorControllerTool.tool.stateMachineGraph.rootStateMachine);
				while (currentParent != null && currentParent != y)
				{
					text = text.Insert(0, currentParent.name + "/");
					currentParent = list2.Find((ChildAnimatorStateMachine sm) => sm.stateMachine.IsDirectParent(currentParent)).stateMachine;
				}
				if (list.Count > 0 && list2.Count > 0 && showStateMachine)
				{
					text = text.Insert(0, "States/");
				}
				genericMenu.AddItem(new GUIContent(text), false, func, current.state);
			}
			if (showStateMachine)
			{
				foreach (ChildAnimatorStateMachine current2 in list2)
				{
					string text2 = current2.stateMachine.name;
					if (list.Count > 0 && list2.Count > 0)
					{
						text2 = text2.Insert(0, "StateMachine/");
					}
					genericMenu.AddItem(new GUIContent(text2), false, func, current2.stateMachine);
				}
			}
			genericMenu.ShowAsContext();
		}
	}
}
