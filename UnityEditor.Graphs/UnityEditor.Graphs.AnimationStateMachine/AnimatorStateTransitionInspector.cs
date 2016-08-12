using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityEditor.Graphs.AnimationStateMachine
{
	[CanEditMultipleObjects, CustomEditor(typeof(AnimatorStateTransition))]
	internal class AnimatorStateTransitionInspector : AnimatorTransitionInspectorBase
	{
		private class Styles
		{
			public GUIContent hasExitTime = new GUIContent(EditorGUIUtility.TextContent("Has Exit Time|Transition has a fixed exit time"));

			public GUIContent exitTime = new GUIContent(EditorGUIUtility.TextContent("Exit Time|Exit time in normalized time from current state"));

			public GUIContent hasFixedDuration = new GUIContent(EditorGUIUtility.TextContent("Fixed Duration | Transition duration is independent of state length"));

			public GUIContent interruptionSource = new GUIContent(EditorGUIUtility.TextContent("Interruption Source|Can be interrupted by transitions from"));

			public GUIContent transitionOffset = new GUIContent(EditorGUIUtility.TextContent("Transition Offset|Normalized start time in the next state"));

			public GUIContent transitionDurationFixed = new GUIContent(EditorGUIUtility.TextContent("Transition Duration (s) |Transition duration in seconds"));

			public GUIContent transitionDurationNormalized = new GUIContent(EditorGUIUtility.TextContent("Transition Duration (%) |Transition duration in normalized time from current state"));

			public GUIContent orderedInterruption = new GUIContent(EditorGUIUtility.TextContent("Ordered Interruption|Can only be interrupted by higher priority transitions"));
		}

		private static AnimatorStateTransitionInspector.Styles styles;

		protected TransitionPreview m_TransitionPreview;

		protected SerializedProperty m_InterruptionSource;

		protected SerializedProperty m_OrderedInterruption;

		protected SerializedProperty m_ExitTime;

		protected SerializedProperty m_HasExitTime;

		protected SerializedProperty m_HasFixedDuration;

		protected SerializedProperty m_CanTransitionToSelf;

		protected SerializedProperty m_Duration;

		protected SerializedProperty m_Offset;

		protected int m_AnyStateSourceIndex;

		protected int m_DstStateIndex;

		protected static bool m_ShowSettings;

		protected string m_InvalidTransitionMessage;

		protected List<AnimatorState> m_DstStates;

		private void Init()
		{
			if (AnimatorStateTransitionInspector.styles == null)
			{
				AnimatorStateTransitionInspector.styles = new AnimatorStateTransitionInspector.Styles();
			}
		}

		public override void OnEnable()
		{
			base.OnEnable();
			this.m_TransitionPreview = new TransitionPreview();
		}

		public override void OnDisable()
		{
			base.OnDisable();
			if (this.m_TransitionPreview != null)
			{
				this.m_TransitionPreview.OnDisable();
			}
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			if (this.m_TransitionPreview != null)
			{
				this.m_TransitionPreview.OnDestroy();
			}
		}

		protected override void ControllerDirty()
		{
			if (this.m_TransitionPreview != null)
			{
				this.m_TransitionPreview.mustResample = true;
			}
		}

		protected override void InitSerializedProperties()
		{
			base.InitSerializedProperties();
			this.m_InterruptionSource = this.m_SerializedTransition.FindProperty("m_InterruptionSource");
			this.m_OrderedInterruption = this.m_SerializedTransition.FindProperty("m_OrderedInterruption");
			this.m_Duration = this.m_SerializedTransition.FindProperty("m_TransitionDuration");
			this.m_Offset = this.m_SerializedTransition.FindProperty("m_TransitionOffset");
			this.m_ExitTime = this.m_SerializedTransition.FindProperty("m_ExitTime");
			this.m_HasExitTime = this.m_SerializedTransition.FindProperty("m_HasExitTime");
			this.m_HasFixedDuration = this.m_SerializedTransition.FindProperty("m_HasFixedDuration");
			this.m_CanTransitionToSelf = this.m_SerializedTransition.FindProperty("m_CanTransitionToSelf");
		}

		protected override void SetTransitionToInspect(AnimatorTransitionBase transition)
		{
			base.SetTransitionToInspect(transition);
			if (this.m_Controller != null)
			{
				AnimatorStateMachine stateMachine = this.m_Controller.layers[this.m_LayerIndex].stateMachine;
				AnimatorState sourceState = this.GetSourceState(this.m_TransitionList.index);
				if (sourceState)
				{
					this.m_DstStates = new List<AnimatorState>();
					this.BuildDestinationStatesRecursive(transition, stateMachine, sourceState.FindParent(stateMachine));
				}
			}
		}

		private void BuildDestinationStatesRecursive(AnimatorTransitionBase transition, AnimatorStateMachine rootStateMachine, AnimatorStateMachine currentStateMachine)
		{
			if (transition.destinationState)
			{
				this.m_DstStates.Add(transition.destinationState);
			}
			else if (transition.isExit)
			{
				AnimatorStateMachine animatorStateMachine = rootStateMachine.FindParent(currentStateMachine);
				if (animatorStateMachine != null)
				{
					AnimatorTransition[] stateMachineTransitions = animatorStateMachine.GetStateMachineTransitions(currentStateMachine);
					AnimatorTransition[] array = stateMachineTransitions;
					for (int i = 0; i < array.Length; i++)
					{
						AnimatorTransition transition2 = array[i];
						this.BuildDestinationStatesRecursive(transition2, rootStateMachine, animatorStateMachine);
					}
				}
			}
			else if (transition.destinationStateMachine)
			{
				if (transition.destinationStateMachine.defaultState != null)
				{
					this.m_DstStates.Add(transition.destinationStateMachine.defaultState);
				}
				AnimatorTransition[] entryTransitions = transition.destinationStateMachine.entryTransitions;
				AnimatorTransition[] array2 = entryTransitions;
				for (int j = 0; j < array2.Length; j++)
				{
					AnimatorTransition transition3 = array2[j];
					this.BuildDestinationStatesRecursive(transition3, rootStateMachine, transition.destinationStateMachine);
				}
			}
		}

		protected AnimatorState GetSourceState(int index)
		{
			return (this.m_TransitionContexts[index] == null) ? null : this.m_TransitionContexts[index].sourceState;
		}

		protected bool IsPreviewable()
		{
			this.m_InvalidTransitionMessage = string.Empty;
			AnimatorStateTransition animatorStateTransition = this.m_SerializedTransition.targetObject as AnimatorStateTransition;
			if (!this.m_Controller)
			{
				this.m_InvalidTransitionMessage = "Cannot preview transition: need an AnimatorController";
				return false;
			}
			AnimatorState sourceState = this.GetSourceState(0);
			Motion exists = (!sourceState) ? null : this.m_Controller.GetStateEffectiveMotion(sourceState, this.m_LayerIndex);
			Motion exists2 = (!animatorStateTransition.destinationState) ? null : this.m_Controller.GetStateEffectiveMotion(animatorStateTransition.destinationState, this.m_LayerIndex);
			bool flag = sourceState == null;
			if (sourceState && sourceState.speed > -0.01f && sourceState.speed < 0.01f)
			{
				this.m_InvalidTransitionMessage = "Cannot preview transition: source state has a speed between -0.01 and 0.01";
				return false;
			}
			if (animatorStateTransition.destinationState && animatorStateTransition.destinationState.speed > -0.01f && animatorStateTransition.destinationState.speed < 0.01f)
			{
				this.m_InvalidTransitionMessage = "Cannot preview transition: destination state has a speed between -0.01 and 0.01";
				return false;
			}
			if (this.m_LayerIndex == 0)
			{
				if (!flag)
				{
					if (!exists)
					{
						this.m_InvalidTransitionMessage = "Cannot preview transition: source state does not have motion";
						return false;
					}
				}
				else if (!exists2)
				{
					this.m_InvalidTransitionMessage = "Cannot preview AnyState transition:  destination state does not have motion";
					return false;
				}
			}
			else if (!exists && !exists2)
			{
				this.m_InvalidTransitionMessage = "Cannot preview transition, must at least have a motion on either source or destination state";
				return false;
			}
			return true;
		}

		protected override void DoPreview()
		{
			this.Init();
			AnimatorStateTransition animatorStateTransition = this.m_SerializedTransition.targetObject as AnimatorStateTransition;
			AnimatorState animatorState = this.GetSourceState((this.m_TransitionList == null) ? 0 : this.m_TransitionList.index);
			AnimatorState animatorState2 = animatorStateTransition.destinationState;
			EditorGUILayout.PropertyField(this.m_HasExitTime, AnimatorStateTransitionInspector.styles.hasExitTime, new GUILayoutOption[0]);
			AnimatorStateTransitionInspector.m_ShowSettings = EditorGUILayout.Foldout(AnimatorStateTransitionInspector.m_ShowSettings, "Settings");
			if (AnimatorStateTransitionInspector.m_ShowSettings)
			{
				EditorGUI.indentLevel++;
				bool enabled = GUI.enabled;
				GUI.enabled = this.m_HasExitTime.boolValue;
				EditorGUILayout.PropertyField(this.m_ExitTime, AnimatorStateTransitionInspector.styles.exitTime, new GUILayoutOption[0]);
				GUI.enabled = enabled;
				EditorGUILayout.PropertyField(this.m_HasFixedDuration, AnimatorStateTransitionInspector.styles.hasFixedDuration, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_Duration, (!this.m_HasFixedDuration.boolValue) ? AnimatorStateTransitionInspector.styles.transitionDurationNormalized : AnimatorStateTransitionInspector.styles.transitionDurationFixed, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_Offset, AnimatorStateTransitionInspector.styles.transitionOffset, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_InterruptionSource, AnimatorStateTransitionInspector.styles.interruptionSource, new GUILayoutOption[0]);
				TransitionInterruptionSource enumValueIndex = (TransitionInterruptionSource)this.m_InterruptionSource.enumValueIndex;
				GUI.enabled = (enumValueIndex == TransitionInterruptionSource.Source || enumValueIndex == TransitionInterruptionSource.SourceThenDestination || enumValueIndex == TransitionInterruptionSource.DestinationThenSource);
				EditorGUILayout.PropertyField(this.m_OrderedInterruption, AnimatorStateTransitionInspector.styles.orderedInterruption, new GUILayoutOption[0]);
				GUI.enabled = enabled;
				if (animatorState == null && animatorState2)
				{
					EditorGUILayout.PropertyField(this.m_CanTransitionToSelf, new GUILayoutOption[0]);
				}
				EditorGUI.indentLevel--;
			}
			if (this.IsPreviewable())
			{
				AnimatorStateMachine stateMachine = this.m_Controller.layers[this.m_LayerIndex].stateMachine;
				if (animatorState == null)
				{
					List<ChildAnimatorState> statesRecursive = stateMachine.statesRecursive;
					string[] array = new string[statesRecursive.Count];
					int num = 0;
					foreach (ChildAnimatorState current in statesRecursive)
					{
						array[num++] = current.state.name;
					}
					EditorGUILayout.Space();
					this.m_AnyStateSourceIndex = EditorGUILayout.Popup("Preview source state", this.m_AnyStateSourceIndex, array, new GUILayoutOption[0]);
					EditorGUILayout.Space();
					animatorState = statesRecursive[this.m_AnyStateSourceIndex].state;
				}
				if (animatorState2 == null)
				{
					if (this.m_DstStates.Count > 0)
					{
						string[] array2 = new string[this.m_DstStates.Count];
						int num2 = 0;
						foreach (AnimatorState current2 in this.m_DstStates)
						{
							array2[num2++] = current2.name;
						}
						EditorGUILayout.Space();
						this.m_DstStateIndex = EditorGUILayout.Popup("Preview destination state", this.m_DstStateIndex, array2, new GUILayoutOption[0]);
						if (this.m_DstStates.Count > this.m_DstStateIndex)
						{
							animatorState2 = this.m_DstStates[this.m_DstStateIndex];
						}
						else
						{
							animatorState2 = null;
						}
						EditorGUILayout.Space();
					}
					else
					{
						EditorGUILayout.HelpBox("Cannot preview transition, there is no destination state", MessageType.Warning, true);
					}
				}
				if (animatorState != null && animatorState2 != null)
				{
					this.m_TransitionPreview.SetTransition(animatorStateTransition, animatorState, animatorState2, this.m_Controller.layers[this.m_LayerIndex], this.m_PreviewObject);
					this.m_TransitionPreview.DoTransitionPreview();
				}
			}
			else
			{
				EditorGUILayout.HelpBox(this.m_InvalidTransitionMessage, MessageType.Warning, true);
			}
		}

		protected override void DoErrorAndWarning()
		{
			if (!this.m_HasExitTime.boolValue && this.m_Conditions.arraySize == 0)
			{
				EditorGUILayout.HelpBox("Transition needs at least one condition or an Exit Time to be valid, otherwise it will be ignored.", MessageType.Warning, true);
			}
		}

		public override bool HasPreviewGUI()
		{
			return this.m_TransitionPreview != null && this.m_TransitionPreview.HasPreviewGUI();
		}

		public override void OnPreviewSettings()
		{
			if (this.m_TransitionPreview != null)
			{
				this.m_TransitionPreview.OnPreviewSettings();
			}
		}

		public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
		{
			if (this.m_TransitionPreview != null)
			{
				this.m_TransitionPreview.OnInteractivePreviewGUI(r, background);
			}
		}
	}
}
