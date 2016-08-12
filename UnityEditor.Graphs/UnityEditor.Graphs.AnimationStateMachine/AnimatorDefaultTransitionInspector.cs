using System;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityEditor.Graphs.AnimationStateMachine
{
	[CanEditMultipleObjects, CustomEditor(typeof(AnimatorDefaultTransition))]
	internal class AnimatorDefaultTransitionInspector : AnimatorTransitionInspectorBase
	{
		private AnimatorDefaultTransitionInspector()
		{
			this.showTransitionList = false;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUILayout.HelpBox("StateMachine Transitions (displayed in grey) are not previewable. To preview a transition please select a State Transition (displayed in white)", MessageType.Info);
		}

		internal override void OnHeaderTitleGUI(Rect titleRect, string header)
		{
			if (this.m_SerializedTransition == null)
			{
				return;
			}
			Rect position = titleRect;
			position.height = 16f;
			string label = "Entry -> Default State ( " + this.m_TransitionContexts[this.m_TransitionList.index].ownerStateMachine.defaultState.name + " )";
			EditorGUI.LabelField(position, label);
			position.y += 18f;
			string arg = (base.targets.Length != 1) ? "Transitions" : "AnimatorTransitionBase";
			EditorGUI.LabelField(position, base.targets.Length + " " + arg);
		}
	}
}
