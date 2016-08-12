using System;
using System.Linq;
using UnityEngine;

namespace UnityEditor.Graphs.AnimationStateMachine
{
	internal class ExitNode : Node
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
				return AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine;
			}
		}

		public override void NodeUI(UnityEditor.Graphs.GraphGUI host)
		{
			if (Node.IsLeftClick())
			{
				host.edgeGUI.EndSlotDragging(base.inputSlots.First<Slot>(), true);
			}
		}

		private void MakeTransitionCallback()
		{
			this.graphGUI.edgeGUI.BeginSlotDragging(base.outputSlots.First<Slot>(), true, false);
		}

		public override void OnDrag()
		{
			base.OnDrag();
			AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine.exitPosition = new Vector2(this.position.x, this.position.y);
		}
	}
}
