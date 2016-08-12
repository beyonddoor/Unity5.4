using System;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityEditor.Graphs.AnimationStateMachine
{
	[CanEditMultipleObjects, CustomEditor(typeof(AnimatorStateMachine))]
	internal class StateMachineInspector : Editor
	{
		private SerializedProperty m_Name;

		private AnimatorStateMachine m_RootStateMachine;

		private AnimatorStateMachine m_ActiveStateMachine;

		private SourceNodeTransitionEditor m_TransitionEditor;

		private StateMachineBehaviorsEditor m_BehavioursEditor;

		private bool ShouldShowTransitionEditor()
		{
			return this.m_TransitionEditor != null && !base.serializedObject.isEditingMultipleObjects && (AnimatorControllerTool.tool != null && AnimatorControllerTool.tool.stateMachineGraph != null) && AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine != this.target as AnimatorStateMachine;
		}

		private void Init()
		{
			if (this.m_TransitionEditor == null && this.m_ActiveStateMachine != this.target as AnimatorStateMachine)
			{
				this.m_TransitionEditor = new SourceNodeTransitionEditor(this.target as AnimatorStateMachine, TransitionType.eStateMachine, this);
			}
			if (this.m_BehavioursEditor == null)
			{
				this.m_BehavioursEditor = new StateMachineBehaviorsEditor(this.target as AnimatorStateMachine, this);
			}
		}

		public void OnEnable()
		{
			this.m_Name = base.serializedObject.FindProperty("m_Name");
			this.m_RootStateMachine = ((!(AnimatorControllerTool.tool != null) || !(AnimatorControllerTool.tool.animatorController != null)) ? null : AnimatorControllerTool.tool.animatorController.layers[AnimatorControllerTool.tool.selectedLayerIndex].stateMachine);
			this.m_ActiveStateMachine = ((!(AnimatorControllerTool.tool != null) || !(AnimatorControllerTool.tool.stateMachineGraph != null)) ? null : AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine);
			this.Init();
			if (this.m_TransitionEditor != null)
			{
				this.m_TransitionEditor.OnEnable();
			}
			this.m_BehavioursEditor.OnEnable();
		}

		public void OnDisable()
		{
			if (this.m_TransitionEditor != null)
			{
				this.m_TransitionEditor.OnDisable();
			}
			this.m_BehavioursEditor.OnDisable();
		}

		public void OnDestroy()
		{
			if (this.m_TransitionEditor != null)
			{
				this.m_TransitionEditor.OnDestroy();
			}
			this.m_BehavioursEditor.OnDestroy();
		}

		internal override void OnHeaderTitleGUI(Rect titleRect, string header)
		{
			base.serializedObject.Update();
			Rect position = titleRect;
			position.height = 16f;
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = this.m_Name.hasMultipleDifferentValues;
			string text = EditorGUI.DelayedTextField(position, this.m_Name.stringValue, EditorStyles.textField);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck() && !string.IsNullOrEmpty(text))
			{
				UnityEngine.Object[] targets = base.targets;
				for (int i = 0; i < targets.Length; i++)
				{
					UnityEngine.Object obj = targets[i];
					ObjectNames.SetNameSmart(obj, (!this.m_RootStateMachine) ? text : this.m_RootStateMachine.MakeUniqueStateMachineName(text));
				}
			}
			base.serializedObject.ApplyModifiedProperties();
		}

		public override void OnInspectorGUI()
		{
			if (this.ShouldShowTransitionEditor())
			{
				this.m_TransitionEditor.OnInspectorGUI();
			}
			this.m_BehavioursEditor.OnInspectorGUI();
		}

		public override bool HasPreviewGUI()
		{
			return this.m_TransitionEditor != null && this.m_TransitionEditor.HasPreviewGUI();
		}

		public override void OnPreviewSettings()
		{
			if (this.m_TransitionEditor != null)
			{
				this.m_TransitionEditor.OnPreviewSettings();
			}
		}

		public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
		{
			if (this.m_TransitionEditor != null)
			{
				this.m_TransitionEditor.OnInteractivePreviewGUI(r, background);
			}
		}
	}
}
