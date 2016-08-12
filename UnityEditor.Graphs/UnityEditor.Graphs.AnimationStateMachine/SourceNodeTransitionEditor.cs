using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor.Graphs.AnimationStateMachine
{
	internal class SourceNodeTransitionEditor
	{
		private Editor m_Host;

		private List<TransitionEditionContext> m_Transitions;

		private ReorderableList m_TransitionList;

		private AnimatorTransitionInspectorBase m_TransitionInspector;

		private TransitionType m_Type;

		private bool m_LockList;

		private AnimatorState m_State;

		private AnimatorStateMachine m_StateMachine;

		private UnityEditor.Animations.AnimatorController m_Controller;

		private AnimatorStateMachine m_ActiveStateMachine;

		public SourceNodeTransitionEditor(AnimatorState state, Editor host)
		{
			this.m_Type = TransitionType.eState;
			this.m_Host = host;
			this.m_State = state;
		}

		public SourceNodeTransitionEditor(AnimatorStateMachine stateMachine, TransitionType type, Editor host)
		{
			this.m_Type = type;
			this.m_Host = host;
			this.m_StateMachine = stateMachine;
		}

		public SourceNodeTransitionEditor(TransitionType type, Editor host)
		{
			this.m_Type = type;
			this.m_Host = host;
		}

		public void OnEnable()
		{
			AnimatorControllerTool.graphDirtyCallback = (Action)Delegate.Combine(AnimatorControllerTool.graphDirtyCallback, new Action(this.OnGraphDirty));
			this.AcquireActiveStateMachine();
			this.AcquireController();
		}

		public void AcquireController()
		{
			this.m_Controller = ((!AnimatorControllerTool.tool) ? null : AnimatorControllerTool.tool.animatorController);
			if (this.m_Controller)
			{
				UnityEditor.Animations.AnimatorController expr_3B = this.m_Controller;
				expr_3B.OnAnimatorControllerDirty = (Action)Delegate.Combine(expr_3B.OnAnimatorControllerDirty, new Action(this.ResetTransitionList));
			}
		}

		public void AcquireActiveStateMachine()
		{
			this.m_ActiveStateMachine = ((!AnimatorControllerTool.tool) ? null : ((this.m_Type != TransitionType.eAnyState) ? AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine : AnimatorControllerTool.tool.stateMachineGraph.rootStateMachine));
			if (this.m_ActiveStateMachine)
			{
				this.ResetTransitionList();
			}
		}

		public void OnInspectorGUI()
		{
			if (this.m_TransitionList == null)
			{
				this.ResetTransitionList();
			}
			if (this.m_ActiveStateMachine == null)
			{
				this.AcquireActiveStateMachine();
			}
			if (this.m_Controller == null)
			{
				this.AcquireController();
			}
			EditorGUI.BeginChangeCheck();
			this.m_TransitionList.DoLayoutList();
			if (EditorGUI.EndChangeCheck())
			{
				AnimatorControllerTool.tool.RebuildGraph();
				GUIUtility.ExitGUI();
			}
			if (this.m_TransitionInspector)
			{
				this.m_TransitionInspector.OnInspectorGUI();
			}
		}

		private void GetTransitionContexts()
		{
			this.m_Transitions = new List<TransitionEditionContext>();
			switch (this.m_Type)
			{
			case TransitionType.eState:
			{
				if (this.m_State == null)
				{
					return;
				}
				AnimatorStateTransition[] transitions = this.m_State.transitions;
				AnimatorStateTransition[] array = transitions;
				for (int i = 0; i < array.Length; i++)
				{
					AnimatorStateTransition aTransition = array[i];
					this.m_Transitions.Add(new TransitionEditionContext(aTransition, this.m_State, null, null));
				}
				break;
			}
			case TransitionType.eAnyState:
				if (this.m_ActiveStateMachine)
				{
					AnimatorStateTransition[] anyStateTransitions = this.m_ActiveStateMachine.anyStateTransitions;
					AnimatorStateTransition[] array2 = anyStateTransitions;
					for (int j = 0; j < array2.Length; j++)
					{
						AnimatorStateTransition aTransition2 = array2[j];
						this.m_Transitions.Add(new TransitionEditionContext(aTransition2, null, null, this.m_ActiveStateMachine));
					}
				}
				break;
			case TransitionType.eStateMachine:
				if (this.m_ActiveStateMachine)
				{
					AnimatorTransition[] stateMachineTransitions = this.m_ActiveStateMachine.GetStateMachineTransitions(this.m_StateMachine);
					AnimatorTransition[] array3 = stateMachineTransitions;
					for (int k = 0; k < array3.Length; k++)
					{
						AnimatorTransition aTransition3 = array3[k];
						this.m_Transitions.Add(new TransitionEditionContext(aTransition3, null, this.m_StateMachine, this.m_ActiveStateMachine));
					}
				}
				break;
			case TransitionType.eEntry:
			{
				if (this.m_StateMachine == null)
				{
					return;
				}
				AnimatorTransition[] entryTransitions = this.m_StateMachine.entryTransitions;
				AnimatorTransition[] array4 = entryTransitions;
				for (int l = 0; l < array4.Length; l++)
				{
					AnimatorTransition aTransition4 = array4[l];
					this.m_Transitions.Add(new TransitionEditionContext(aTransition4, null, null, this.m_StateMachine));
				}
				break;
			}
			}
		}

		private void ResetTransitionList()
		{
			if (this.m_LockList)
			{
				return;
			}
			this.GetTransitionContexts();
			this.m_TransitionList = new ReorderableList(this.m_Transitions, typeof(TransitionEditionContext), true, true, false, true);
			this.m_TransitionList.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(AnimatorTransitionInspectorBase.DrawTransitionHeaderCommon);
			this.m_TransitionList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.OnTransitionElement);
			this.m_TransitionList.onSelectCallback = new ReorderableList.SelectCallbackDelegate(this.SelectTransition);
			this.m_TransitionList.onReorderCallback = new ReorderableList.ReorderCallbackDelegate(this.ReorderTransition);
			this.m_TransitionList.onRemoveCallback = new ReorderableList.RemoveCallbackDelegate(this.RemoveTransition);
			this.m_TransitionList.displayAdd = false;
			this.m_Host.Repaint();
		}

		public void OnDestroy()
		{
			UnityEngine.Object.DestroyImmediate(this.m_TransitionInspector);
		}

		public void OnDisable()
		{
			AnimatorControllerTool.graphDirtyCallback = (Action)Delegate.Remove(AnimatorControllerTool.graphDirtyCallback, new Action(this.OnGraphDirty));
			if (this.m_Controller)
			{
				UnityEditor.Animations.AnimatorController expr_36 = this.m_Controller;
				expr_36.OnAnimatorControllerDirty = (Action)Delegate.Remove(expr_36.OnAnimatorControllerDirty, new Action(this.ResetTransitionList));
			}
		}

		private void RemoveTransition(ReorderableList transitionList)
		{
			UnityEngine.Object.DestroyImmediate(this.m_TransitionInspector);
			this.m_Transitions[this.m_TransitionList.index].Remove(true);
			this.ResetTransitionList();
		}

		private void SelectTransition(ReorderableList list)
		{
			AnimatorTransitionBase transition = this.m_Transitions[list.index].transition;
			if (transition == null)
			{
				return;
			}
			if (this.m_Type == TransitionType.eState || this.m_Type == TransitionType.eAnyState)
			{
				this.m_TransitionInspector = (Editor.CreateEditor(transition as AnimatorStateTransition) as AnimatorStateTransitionInspector);
			}
			else
			{
				this.m_TransitionInspector = (Editor.CreateEditor(transition as AnimatorTransition) as AnimatorTransitionInspector);
			}
			this.m_TransitionInspector.SetTransitionContext(this.m_Transitions[list.index]);
			this.m_TransitionInspector.showTransitionList = false;
		}

		private void ReorderTransition(ReorderableList list)
		{
			this.m_LockList = true;
			switch (this.m_Type)
			{
			case TransitionType.eState:
				Undo.RegisterCompleteObjectUndo(this.m_State, "Reorder transition");
				this.m_State.transitions = Array.ConvertAll<TransitionEditionContext, AnimatorStateTransition>(this.m_Transitions.ToArray(), (TransitionEditionContext t) => t.transition as AnimatorStateTransition);
				break;
			case TransitionType.eAnyState:
				Undo.RegisterCompleteObjectUndo(this.m_ActiveStateMachine, "Reorder transition");
				this.m_ActiveStateMachine.anyStateTransitions = Array.ConvertAll<TransitionEditionContext, AnimatorStateTransition>(this.m_Transitions.ToArray(), (TransitionEditionContext t) => t.transition as AnimatorStateTransition);
				break;
			case TransitionType.eStateMachine:
				Undo.RegisterCompleteObjectUndo(this.m_ActiveStateMachine, "Reorder transition");
				this.m_ActiveStateMachine.SetStateMachineTransitions(this.m_StateMachine, Array.ConvertAll<TransitionEditionContext, AnimatorTransition>(this.m_Transitions.ToArray(), (TransitionEditionContext t) => t.transition as AnimatorTransition));
				break;
			case TransitionType.eEntry:
				Undo.RegisterCompleteObjectUndo(this.m_StateMachine, "Reorder transition");
				this.m_StateMachine.entryTransitions = Array.ConvertAll<TransitionEditionContext, AnimatorTransition>(this.m_Transitions.ToArray(), (TransitionEditionContext t) => t.transition as AnimatorTransition);
				break;
			}
			this.m_LockList = false;
		}

		private void OnTransitionElement(Rect rect, int index, bool selected, bool focused)
		{
			AnimatorTransitionInspectorBase.DrawTransitionElementCommon(rect, this.m_Transitions[index], selected, focused);
		}

		private void OnGraphDirty()
		{
			this.ResetTransitionList();
		}

		public bool HasPreviewGUI()
		{
			return this.m_TransitionInspector != null && this.m_TransitionInspector.HasPreviewGUI();
		}

		public void OnPreviewSettings()
		{
			this.m_TransitionInspector.OnPreviewSettings();
		}

		public void OnInteractivePreviewGUI(Rect r, GUIStyle background)
		{
			this.m_TransitionInspector.OnInteractivePreviewGUI(r, background);
		}
	}
}
