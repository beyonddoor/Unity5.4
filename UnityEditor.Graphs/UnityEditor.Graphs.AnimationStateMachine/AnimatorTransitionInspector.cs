using System;
using UnityEditor.Animations;

namespace UnityEditor.Graphs.AnimationStateMachine
{
	[CanEditMultipleObjects, CustomEditor(typeof(AnimatorTransition))]
	internal class AnimatorTransitionInspector : AnimatorTransitionInspectorBase
	{
		protected override void DoPreview()
		{
			EditorGUILayout.HelpBox("StateMachine Transitions (displayed in grey) are not previewable. To preview a transition please select a State Transition (displayed in white)", MessageType.Info);
		}
	}
}
