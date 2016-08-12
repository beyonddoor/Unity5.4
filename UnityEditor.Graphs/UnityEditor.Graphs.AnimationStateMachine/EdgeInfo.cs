using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityEditor.Graphs.AnimationStateMachine
{
	internal class EdgeInfo
	{
		public readonly List<TransitionEditionContext> transitions = new List<TransitionEditionContext>();

		public bool hasMultipleTransitions
		{
			get
			{
				return this.transitions.Count > 1;
			}
		}

		public bool hasDefaultState
		{
			get
			{
				foreach (TransitionEditionContext current in this.transitions)
				{
					if (current.transition == null)
					{
						return true;
					}
				}
				return false;
			}
		}

		public EdgeType edgeType
		{
			get
			{
				int num = 0;
				int num2 = 0;
				foreach (TransitionEditionContext current in this.transitions)
				{
					if (current.transition is AnimatorStateTransition)
					{
						num++;
					}
					else
					{
						num2++;
					}
				}
				if (num2 > 0 && num > 0)
				{
					return EdgeType.MixedTransition;
				}
				if (num > 0)
				{
					return EdgeType.StateTransition;
				}
				return EdgeType.Transition;
			}
		}

		public EdgeDebugState debugState
		{
			get
			{
				int num = 0;
				int num2 = 0;
				foreach (TransitionEditionContext current in this.transitions)
				{
					if (current.transition != null)
					{
						if (current.transition.mute)
						{
							num++;
						}
						else if (current.transition.solo)
						{
							num2++;
						}
					}
				}
				if (num == this.transitions.Count)
				{
					return EdgeDebugState.MuteAll;
				}
				if (num2 == this.transitions.Count)
				{
					return EdgeDebugState.SoloAll;
				}
				if (num > 0 && num2 > 0)
				{
					return EdgeDebugState.MuteAndSolo;
				}
				if (num > 0)
				{
					return EdgeDebugState.MuteSome;
				}
				if (num2 > 0)
				{
					return EdgeDebugState.SoloSome;
				}
				return EdgeDebugState.Normal;
			}
		}

		public EdgeInfo(TransitionEditionContext context)
		{
			this.Add(context);
		}

		public void Add(TransitionEditionContext context)
		{
			this.transitions.Add(context);
		}

		public bool HasTransition(int nameHash)
		{
			foreach (TransitionEditionContext current in this.transitions)
			{
				if (Animator.StringToHash(current.fullName) == nameHash)
				{
					return true;
				}
			}
			return false;
		}

		public bool HasTransition(AnimatorTransitionBase transition)
		{
			foreach (TransitionEditionContext current in this.transitions)
			{
				if (current.transition == transition)
				{
					return true;
				}
			}
			return false;
		}
	}
}
