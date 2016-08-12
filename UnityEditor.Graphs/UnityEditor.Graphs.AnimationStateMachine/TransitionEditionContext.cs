using System;
using UnityEditor.Animations;

namespace UnityEditor.Graphs.AnimationStateMachine
{
	internal class TransitionEditionContext
	{
		public AnimatorTransitionBase transition;

		public AnimatorState sourceState;

		public AnimatorStateMachine sourceStateMachine;

		public AnimatorStateMachine ownerStateMachine;

		private string m_FullName;

		private string m_DisplayName;

		public string displayName
		{
			get
			{
				return this.m_DisplayName;
			}
		}

		public string fullName
		{
			get
			{
				return this.m_FullName;
			}
		}

		public bool isAnyStateTransition
		{
			get
			{
				return this.ownerStateMachine && this.transition is AnimatorStateTransition;
			}
		}

		public bool isDefaultTransition
		{
			get
			{
				return this.transition == null && this.sourceState == null && this.sourceStateMachine == null;
			}
		}

		public TransitionEditionContext(AnimatorTransitionBase aTransition, AnimatorState aSourceState, AnimatorStateMachine aSourceStateMachine, AnimatorStateMachine aOwnerStateMachine)
		{
			this.transition = aTransition;
			this.sourceState = aSourceState;
			this.sourceStateMachine = aSourceStateMachine;
			this.ownerStateMachine = aOwnerStateMachine;
			this.BuildNames();
		}

		private void BuildNames()
		{
			if (this.sourceState)
			{
				this.m_DisplayName = this.transition.GetDisplayName(this.sourceState);
			}
			else if (this.sourceStateMachine)
			{
				this.m_DisplayName = this.transition.GetDisplayName(this.sourceStateMachine);
			}
			else if (this.transition)
			{
				this.m_DisplayName = this.transition.GetDisplayName(null);
			}
			else
			{
				this.m_DisplayName = "To Default State";
			}
			this.m_FullName = string.Empty;
			if (AnimatorControllerTool.tool && AnimatorControllerTool.tool.stateMachineGraph && AnimatorControllerTool.tool.stateMachineGraph.rootStateMachine && this.transition != null)
			{
				string source = (!this.isAnyStateTransition) ? string.Empty : "AnyState";
				string text = string.Empty;
				Graph stateMachineGraph = AnimatorControllerTool.tool.stateMachineGraph;
				if (this.sourceState)
				{
					source = stateMachineGraph.GetStatePath(this.sourceState);
				}
				else if (this.sourceStateMachine)
				{
					source = stateMachineGraph.GetStateMachinePath(this.sourceStateMachine);
				}
				if (this.transition.destinationState)
				{
					text += stateMachineGraph.GetStatePath(this.transition.destinationState);
				}
				else if (this.transition.destinationStateMachine)
				{
					text += stateMachineGraph.GetStateMachinePath(this.transition.destinationStateMachine);
				}
				this.m_FullName = AnimatorTransitionBase.BuildTransitionName(source, text);
			}
		}

		public void Remove(bool rebuildGraph = true)
		{
			if (this.sourceState)
			{
				this.sourceState.RemoveTransition(this.transition as AnimatorStateTransition);
			}
			else if (this.ownerStateMachine)
			{
				if (this.transition is AnimatorStateTransition)
				{
					this.ownerStateMachine.RemoveAnyStateTransition(this.transition as AnimatorStateTransition);
				}
				else if (this.sourceStateMachine)
				{
					this.ownerStateMachine.RemoveStateMachineTransition(this.sourceStateMachine, this.transition as AnimatorTransition);
				}
				else
				{
					this.ownerStateMachine.RemoveEntryTransition(this.transition as AnimatorTransition);
				}
			}
			if (rebuildGraph)
			{
				AnimatorControllerTool.tool.RebuildGraph();
			}
		}
	}
}
